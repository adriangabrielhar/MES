using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks; // Adăugat pentru Task
// IMPORTANT: Adaugă librăria corectă pentru funcțiile Async ale bazei de date.
// Dacă folosești Entity Framework Core, decomentează linia de mai jos:
using Microsoft.EntityFrameworkCore;
// Dacă folosești Entity Framework vechi (EF6), folosește: using System.Data.Entity;

using MainApplication.Models;

namespace MainApplication.BLL.Services
{
    class AuthService
    {
        private readonly MESDbContext _context;

        public AuthService(MESDbContext context)
        {
            _context = context;
        }

        // Am transformat metoda în una asincronă
        public async Task<User?> AuthenticateAsync(string username, string passwordHash)
        {
            // Folosim FirstOrDefaultAsync în loc de FirstOrDefault pentru a nu bloca UI-ul
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public bool CheckPermission(User user, string requiredRole)
        {
            if (user == null) return false;

            return user.Role == "Admin" || user.Role == requiredRole;
        }
    }
}