namespace log.Data
{
    public class Repository<T> where T : class
    {
        public IUnitOfWork<T> UnitOfWork { get; set; }

        public Repository(IUnitOfWork<T> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}

