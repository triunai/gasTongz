using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    public class User
    {
        //todo: add authentication
        public int Id { get; private set; }
        public string Username { get; private set; } = default!;
        public string? Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private User() { }  

        public User(string username, string? email)
        {
            Username = username;
            Email = email;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
            UpdatedAt = DateTime.Now;
        }
    }
}
