using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace data
{
    public enum UnitOfWorkScopeDefaultAction
    {
        Commit,
        Rollback,
    }

    public abstract class UnitOfWorkScope: IDisposable
    {
        protected string _connectionString;
        protected bool _useTransaction;
        protected UnitOfWorkScopeDefaultAction _defaultAction;

        protected string _ID;

        public UnitOfWorkScope(IUnitOfWorkScopeParams unitOfWorkScopeParams)
        {
            _connectionString = unitOfWorkScopeParams.ConnectionString;
            _useTransaction = unitOfWorkScopeParams.UseTransaction;
            _defaultAction = unitOfWorkScopeParams.DefaultAction;
            _ID = Guid.NewGuid().ToString();
        }

        public abstract void StartTransaction();
        public abstract void Commit();
        public abstract void Rollback();
        public abstract void CloseSession();

        public void Dispose()
        {
            CloseSession();
        }
    }

    public interface IUnitOfWorkScopeParams
    {
        string ConnectionString { get; set; }
        UnitOfWorkScopeDefaultAction DefaultAction { get; set; }
        bool UseTransaction { get; set; }
    }

    public class UnitOfWorkScopeParams : IUnitOfWorkScopeParams
    {
        public string ConnectionString { get; set; }
        public bool UseTransaction { get; set; }
        public UnitOfWorkScopeDefaultAction DefaultAction { get; set; }
    }


}
