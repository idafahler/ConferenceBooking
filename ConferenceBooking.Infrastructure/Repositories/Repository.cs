using ConferenceBooking.Application.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class Repository<T>(ConferenceBookingContext context) : IRepository<T> where T : class
    {
        protected readonly DbSet<T> dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(int id) 
            => await dbSet.FindAsync(id);

        public async Task<List<T>> GetAllAsync() 
            => await dbSet.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> condition)
            => await dbSet.AsNoTracking().Where(condition).ToListAsync();

        public async Task AddAsync(T entity)
        {
            dbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
