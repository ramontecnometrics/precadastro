using System.Linq;

namespace log.Data
{
    public interface IUnitOfWork<T>
    {
        void Save(T entity);
        T Get(object id);
        void Delete(T entity);
        IQueryable<T> GetAll();
    }
}
