using HoaVanEcommerce.Domain.Entities;

namespace HoaVanEcommerce.BE.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task<OrderStatusHistory> AddStatusHistoryAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}
