using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingInfo)
            .Include(o => o.Payments)
            .Include(o => o.StatusHistory)
                .ThenInclude(h => h.ChangedByUser)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public Task<Order?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingInfo)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode, cancellationToken);
    }

    public Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingInfo)
            .Include(o => o.Payments)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingInfo)
            .Include(o => o.Payments)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }

    public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _dbContext.Orders.Update(order);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderStatusHistory> AddStatusHistoryAsync(OrderStatusHistory history, CancellationToken cancellationToken = default)
    {
        await _dbContext.OrderStatusHistories.AddAsync(history, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Payments.AddAsync(payment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _dbContext.Payments.Update(payment);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
