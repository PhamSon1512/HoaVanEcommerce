using HoaVanEcommerce.BE.Application.DTOs.Orders;
using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        // Get cart items
        var cartItems = await _cartRepository.GetCartItemsByIdsAsync(request.CartItemIds, cancellationToken);
        if (cartItems == null || cartItems.Count == 0)
        {
            throw new InvalidOperationException("No cart items found.");
        }

        // Verify all items belong to user's cart
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null)
        {
            throw new InvalidOperationException("Cart not found.");
        }

        var invalidItems = cartItems.Where(i => i.CartId != cart.Id).ToList();
        if (invalidItems.Any())
        {
            throw new UnauthorizedAccessException("Some cart items do not belong to your cart.");
        }

        // Generate order code
        var orderCode = GenerateOrderCode();

        // Create order
        var order = new Order
        {
            OrderCode = orderCode,
            UserId = userId,
            TotalAmount = cartItems.Sum(i => i.UnitPrice * i.Quantity),
            PaymentStatus = "PENDING",
            ShippingStatus = "NOT_SHIPPED",
            OrderStatus = "PENDING_PAYMENT",
            PaymentMethod = "BANK_TRANSFER_QR",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create order items
        foreach (var cartItem in cartItems)
        {
            var product = cartItem.Product;
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException($"Product {cartItem.ProductId} is not available.");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {cartItem.Quantity}");
            }

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductCode = product.Code,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                LineTotal = cartItem.UnitPrice * cartItem.Quantity
            });

            // Update product stock
            product.StockQuantity -= cartItem.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        // Create shipping info if provided
        if (!string.IsNullOrWhiteSpace(request.ShippingName) || 
            !string.IsNullOrWhiteSpace(request.ShippingPhone) || 
            !string.IsNullOrWhiteSpace(request.ShippingAddress))
        {
            order.ShippingInfo = new OrderShippingInfo
            {
                ReceiverName = request.ShippingName ?? string.Empty,
                ReceiverPhone = request.ShippingPhone ?? string.Empty,
                FullAddress = request.ShippingAddress ?? string.Empty,
                Note = request.ShippingNote
            };
        }

        // Create payment
        var payment = new Payment
        {
            OrderId = 0, // Will be set after order is created
            Amount = order.TotalAmount,
            Status = "PENDING_CONFIRM",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Save order
        order = await _orderRepository.CreateOrderAsync(order, cancellationToken);

        // Set payment order ID and save
        payment.OrderId = order.Id;
        await _orderRepository.CreatePaymentAsync(payment, cancellationToken);

        // Add status history
        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            ToStatus = "PENDING_PAYMENT",
            Note = "Đơn hàng được tạo"
        }, cancellationToken);

        // Remove cart items
        foreach (var cartItem in cartItems)
        {
            await _cartRepository.DeleteCartItemAsync(cartItem, cancellationToken);
        }

        // Reload the order with all includes to ensure all navigation properties are loaded for the DTO mapping
        var createdOrder = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
        if (createdOrder == null)
        {
            throw new InvalidOperationException("Failed to retrieve the created order.");
        }

        return MapToOrderDto(createdOrder);
    }

    public async Task<OrderDto> GetOrderByIdAsync(int orderId, int? userId = null, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        // Check authorization
        if (userId.HasValue && order.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this order.");
        }

        // Reload to ensure all navigation properties are available for mapping
        var updatedOrder = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
        if (updatedOrder == null)
        {
            throw new InvalidOperationException("Failed to retrieve the order after payment confirmation.");
        }

        return MapToOrderDto(updatedOrder);
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(MapToOrderDto).ToList();
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToOrderDto).ToList();
    }

    public async Task<OrderDto> ConfirmPaymentAsync(int orderId, int userId, ConfirmPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to confirm payment for this order.");
        }

        if (order.PaymentStatus != "PENDING")
        {
            throw new InvalidOperationException($"Payment status is already {order.PaymentStatus}.");
        }

        // Update payment status
        var payment = order.Payments.FirstOrDefault();
        if (payment != null)
        {
            payment.Status = "PENDING_VERIFY";
            payment.TransactionRef = request.TransactionRef;
            payment.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdatePaymentAsync(payment, cancellationToken);
        }

        // Update order status
        var oldPaymentStatus = order.PaymentStatus;
        order.PaymentStatus = "PENDING_VERIFY";
        order.OrderStatus = "PAYMENT_PENDING_VERIFY";
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateOrderAsync(order, cancellationToken);

        // Add status history
        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            ChangedByUserId = userId,
            FromStatus = oldPaymentStatus,
            ToStatus = "PENDING_VERIFY",
            Note = "Người dùng xác nhận đã thanh toán"
        }, cancellationToken);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        var oldOrderStatus = order.OrderStatus;
        var oldPaymentStatus = order.PaymentStatus;
        var oldShippingStatus = order.ShippingStatus;

        // Update statuses
        if (!string.IsNullOrEmpty(request.OrderStatus))
        {
            order.OrderStatus = request.OrderStatus;
        }
        if (!string.IsNullOrEmpty(request.PaymentStatus))
        {
            order.PaymentStatus = request.PaymentStatus;
        }
        if (!string.IsNullOrEmpty(request.ShippingStatus))
        {
            order.ShippingStatus = request.ShippingStatus;
        }
        if (request.Note != null)
        {
            order.CurrentStatusNote = request.Note;
        }
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateOrderAsync(order, cancellationToken);

        // Add status history
        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = oldOrderStatus,
            ToStatus = request.OrderStatus ?? oldOrderStatus,
            Note = request.Note
        }, cancellationToken);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto> ConfirmReceivedAsync(int orderId, int userId, ConfirmReceivedRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to confirm received for this order.");
        }

        if (order.ShippingStatus != "SHIPPED" && order.ShippingStatus != "DELIVERING")
        {
            throw new InvalidOperationException($"Order shipping status must be SHIPPED or DELIVERING. Current: {order.ShippingStatus}");
        }

        var oldOrderStatus = order.OrderStatus;
        order.OrderStatus = "COMPLETED";
        order.ShippingStatus = "DELIVERED";
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateOrderAsync(order, cancellationToken);

        // Add status history
        await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            ChangedByUserId = userId,
            FromStatus = oldOrderStatus,
            ToStatus = "COMPLETED",
            Note = request.Note ?? "Người dùng xác nhận đã nhận hàng"
        }, cancellationToken);

        return MapToOrderDto(order);
    }

    private OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            UserId = order.UserId,
            UserName = order.User.FullName,
            UserEmail = order.User.Email,
            TotalAmount = order.TotalAmount,
            PaymentStatus = order.PaymentStatus,
            ShippingStatus = order.ShippingStatus,
            OrderStatus = order.OrderStatus,
            PaymentMethod = order.PaymentMethod,
            CurrentStatusNote = order.CurrentStatusNote,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductCode = i.ProductCode,
                ProductImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList(),
            ShippingInfo = order.ShippingInfo != null ? new OrderShippingInfoDto
            {
                ReceiverName = order.ShippingInfo.ReceiverName,
                ReceiverPhone = order.ShippingInfo.ReceiverPhone,
                FullAddress = order.ShippingInfo.FullAddress,
                Note = order.ShippingInfo.Note
            } : null
        };
    }

    private string GenerateOrderCode()
    {
        return $"HV{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
