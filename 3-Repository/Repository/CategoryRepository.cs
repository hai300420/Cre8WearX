using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Repository.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ClothesCusShopContext _context;

        public CategoryRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _dbSet.AddAsync(category);
        }

        public void Update(Category category)
        {
            _dbSet.Update(category);
        }

        public void Delete(Category category)
        {
            _dbSet.Remove(category);
        }
    }
}
