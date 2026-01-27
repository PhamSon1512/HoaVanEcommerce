using System.Collections.Generic;

namespace HoaVanEcommerce.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

