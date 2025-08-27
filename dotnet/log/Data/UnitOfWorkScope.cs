using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log.Data
{
    public abstract class UnitOfWorkScope: IDisposable
    {
        protected string _connectionString;

        protected string _ID;

        public UnitOfWorkScope(IUnitOfWorkScopeParams unitOfWorkScopeParams)
        {
            _connectionString = unitOfWorkScopeParams.ConnectionString;
            _ID = Guid.NewGuid().ToString();
        }

        public abstract void StartTransaction();
        public abstract void Commit();
        public abstract void Rollback();
        public abstract void CloseSession();
        public abstract bool InTransaction();

        public void Dispose()
        {
            if (InTransaction())
            {
                Rollback();
            }
            CloseSession();
        }
    }

    public interface IUnitOfWorkScopeParams
    {
        string ConnectionString { get; set; }
    }

    public class UnitOfWorkScopeParams : IUnitOfWorkScopeParams
    {
        public string ConnectionString { get; set; }
    }
}
