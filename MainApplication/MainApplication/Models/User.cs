using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.Models
{
    class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
