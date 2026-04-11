using System;
using System.Collections.Generic;
using System.Text;
using MainApplication.Models;
using System.Linq;

namespace MainApplication.DAL
{
    class InventoryRepository : IRepository<InventoryItem>
    {
        private readonly MESDbContext _context;
        
        public InventoryRepository(MESDbContext context)
        {
            _context = context;
        }

        public InventoryItem GetById(int id)
        {
            return _context.InventoryItems.Find(id);
        }

        public IEnumerable<InventoryItem> GetAll()
        {
            return _context.InventoryItems;
        }

        public void Add(InventoryItem entity)
        {
            _context.InventoryItems.Add(entity);
            _context.SaveChanges();
        }

        public void Update(InventoryItem entity)
        {
            _context.InventoryItems.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.InventoryItems.Find(id);
            if (entity != null)
            {
                _context.InventoryItems.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}