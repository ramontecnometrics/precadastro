using data;
using framework;
using System.Linq;

namespace model.Repositories
{
    public class LeadRepository : Repository<Lead>
    {
        public LeadRepository(IUnitOfWork<Lead> unitOfWork) : base(unitOfWork)
        {
        }

        public virtual IQueryable<Lead> GetAll()
        {
            return UnitOfWork.GetAll();
        }

        public virtual Lead Get(long id, bool useCache = true)
        {
            if (useCache)
            {
                return base.Get(id);
            }
            return UnitOfWork.Get(id);
        }

        public virtual void Delete(Lead entity)
        {
            UnitOfWork.Delete(entity);
        }
    }
}
