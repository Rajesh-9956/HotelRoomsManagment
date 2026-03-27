using System.Linq.Expressions;
using hotel_room_api.Data;
using hotel_room_api.Models;
using hotel_room_api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace hotel_room_api.Repository;

public class Repository<T> : IRepository<T> where T: class
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext db)
    {
        _db = db;
        this._dbSet = _db.Set<T>();
    }


    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> Filter = null, Expression<Func<T, object>>[] Includes = null,
        int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<T> Query = _dbSet;

        if (Includes != null)
            foreach (var Include in Includes)
                Query = Query.Include(Include);
       
        if (Filter != null)
            Query = Query.Where(Filter);

        if (pageSize > 0)
        {
            if (pageSize > 100) pageSize = 100;
            Query = Query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
        
        
        return await Query.ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> Filter = null, Expression<Func<T, object>>[] Includes = null, bool tracked = true)
    {
        IQueryable<T> Query = _dbSet;

        if (!tracked)
            Query = Query.AsNoTracking();
        
        if (Includes != null)
            foreach (var Include in Includes)
                Query = Query.Include(Include);
       
        if (Filter != null)
            Query = Query.Where(Filter);
        

        return await Query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(T Entity)
    {
        await _dbSet.AddAsync(Entity);
        await SaveAsync();
    }

    public async Task RemoveAsync(T Entity)
    {
         _dbSet.Remove(Entity);
         await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}