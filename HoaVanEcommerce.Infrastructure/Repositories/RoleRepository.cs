using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Role?> GetByNameAsync(string name)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _dbContext.UserRoles.AddAsync(userRole);
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}

