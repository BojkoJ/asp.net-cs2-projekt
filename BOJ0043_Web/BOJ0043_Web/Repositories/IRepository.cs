using System.Linq.Expressions;

namespace BOJ0043_Web.Repositories
{
    /// <summary>
    /// Obecné rozhraní pro repozitář, který pracuje s entitami typu T
    /// </summary>
    /// <typeparam name="T">Typ entity</typeparam>
    public interface IRepository<T> where T : class
    {
        // Získání entity podle ID
        Task<T?> GetByIdAsync(int id);
        
        // Získání všech entit
        Task<IEnumerable<T>> GetAllAsync();
        
        // Získání entit podle podmínky
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        // Přidání entity
        Task AddAsync(T entity);
        
        // Aktualizace entity
        Task UpdateAsync(T entity);
        
        // Odstranění entity
        Task DeleteAsync(T entity);
        
        // Uložení změn
        Task SaveChangesAsync();
    }
}
