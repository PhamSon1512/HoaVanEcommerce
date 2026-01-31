using HoaVanEcommerce.BE.Application.DTOs.Orders;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(int userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> GetOrderByIdAsync(int orderId, int? userId = null, CancellationToken cancellationToken = default);
    Task<List<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<OrderDto> ConfirmPaymentAsync(int orderId, int userId, ConfirmPaymentRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> ConfirmReceivedAsync(int orderId, int userId, ConfirmReceivedRequest request, CancellationToken cancellationToken = default);
}
