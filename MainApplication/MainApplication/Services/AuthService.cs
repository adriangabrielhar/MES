using System;
using System.Collections.Generic;
using System.Text;
using MainApplication.Models;

namespace MainApplication.Services
{
    class AuthService
    {
        private readonly MESDbContext _context;

        public AuthService(MESDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string passwordHash)
        {
           
            return _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public bool CheckPermission(User user, string requiredRole)
        {
            if (user == null) return false;
        
            return user.Role == "Admin" || user.Role == requiredRole;
        }
    }
}

