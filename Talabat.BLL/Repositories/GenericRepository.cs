using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDBContext _context;

        public GenericRepository(StoreDBContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync() 
            => await _context.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(int id) 
            => await _context.Set<T>().FindAsync(id);
        public async Task<IReadOnlyList<T>> GetAllWithSpec(ISpecifications<T> specifications)
            => await ApplySpecifications(specifications).ToListAsync();
        public async Task<T> GetEntityWithSpec(ISpecifications<T> specifications)
         => await ApplySpecifications(specifications).FirstOrDefaultAsync();
        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
            => SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), specifications);

        public async Task<int> GetCountAsync(ISpecifications<T> specifications)
        => await ApplySpecifications(specifications).CountAsync();

        public async Task Create(T entity)
         => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity)
        => _context.Set<T>().Update(entity);

        public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
    }
}
