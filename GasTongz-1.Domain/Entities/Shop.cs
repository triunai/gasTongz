using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    public class Shop
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Location { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? UpdatedBy { get; private set; }

        private Shop() { }

        public Shop(string name, string? location, int? createdBy)
        {
            Name = name;
            Location = location;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            // UpdatedBy can remain null until something changes
        }

        public void UpdateShop(string newName, string? newLocation, int? userId)
        {
            Name = newName;
            Location = newLocation;
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
