using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repos
{
    public class SqlRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public SqlRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var res = await _context.Set<T>().FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var res = await _context.Set<T>().ToListAsync();
            return res;
        }

        public IEnumerable<T> GetAll()
        {
            var res = _context.Set<T>().ToList();
            return res;
        }

        public async Task<T> AddAsync(T aggregate)
        {
            var res = await _context.Set<T>().AddAsync(aggregate);
            await _context.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<T> UpdateAsync(T aggregate)
        {
            try
            {
                var res = _context.Set<T>().Update(aggregate);
                await _context.SaveChangesAsync();
                return res.Entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                await _context.SaveChangesAsync();
                return (T)ex.Entries.Single().Entity;
            }
        }

        public virtual async Task RemoveAsync(T aggregate)
        {
            _context.Set<T>().Remove(aggregate);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllAsync()
        {
            DbSet<T> dbSet = _context.Set<T>();
            dbSet.RemoveRange(await dbSet.ToListAsync());
            await _context.SaveChangesAsync();
        }

    }
}
