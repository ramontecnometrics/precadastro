using NHibernate;
using log.Data; 
using System.Linq;
using System.Collections.Generic;
using System;

namespace log.NHibernate
{
    public class UnitOfWorkForHN<T> : IUnitOfWork<T> where T : class
    {
        private readonly UnitOfWorkScope UnitOfWorkScope;

        public UnitOfWorkForHN(UnitOfWorkScope unitOfWorkScope)
        {
            UnitOfWorkScope = unitOfWorkScope;
        }

        public virtual void Save(T entity)
        {
            GetCurrentSession().Save(entity);
            GetCurrentSession().Flush();
        }

        public virtual void Delete(T entity)
        {
            GetCurrentSession().Delete(entity);
            GetCurrentSession().Flush();
        }

        public virtual T Get(object id)
        {
            return (T)GetCurrentSession().Get<T>(id);
        }

        public IQueryable<T> GetAll()
        {
            return GetCurrentSession().Query<T>();
        }

        public void StartTransaction()
        {
            GetCurrentSession().GetCurrentTransaction().Begin();
        }

        public void Commit()
        {
            GetCurrentSession().GetCurrentTransaction().Commit();
        }

        public void Rollback()
        {
            GetCurrentSession().GetCurrentTransaction().Rollback();
        }

        protected ISession GetCurrentSession() 
        {
            var result = ((UnitOfWorkScopeForNH)UnitOfWorkScope).Session();
            return result;
        }
    }

    public class CommandData
    {
        public string Sql { get; set; }
        public List<KeyValuePair<string, object>> Parametters { get; set; }

        public CommandData()
        {
            this.Parametters = new List<KeyValuePair<string, object>>();
        }

        public virtual void AddParametter(string name, object value)
        {
            this.Parametters.Add(new KeyValuePair<string, object>(name, value));
        }
    }

    public class NHCommand
    {
        private readonly UnitOfWorkScope UnitOfWorkScope;

        public NHCommand(UnitOfWorkScope unitOfWorkScope)
        {
            UnitOfWorkScope = unitOfWorkScope;
        }

        public virtual void ExecuteNonQuery(CommandData command)
        {
            using (var cmd = GetCurrentSession().Connection.CreateCommand())
            {
                cmd.CommandText = command.Sql;
                foreach (var i in command.Parametters)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = i.Key;
                    if (i.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = i.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        public virtual IEnumerable<IEnumerable<KeyValuePair<string, object>>> Execute(CommandData command)
        {
            var result = new List<List<KeyValuePair<string, object>>>();
            using (var cmd = GetCurrentSession().Connection.CreateCommand())
            {
                cmd.CommandText = command.Sql;
                foreach (var i in command.Parametters)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = i.Key;
                    parameter.Value = i.Value;
                    cmd.Parameters.Add(parameter);
                }
                using (var dr = cmd.ExecuteReader()) {
                    while (dr.Read())
                    {
                        var fields = new List<KeyValuePair<string, object>>();
                        for (var i = 0; i < dr.FieldCount; i++) {
                            var column = dr.GetName(i);
                            var fieldValue = dr.GetValue(i);
                            var value = fieldValue == DBNull.Value ? null : fieldValue;
                            fields.Add(new KeyValuePair<string, object>(column, value));
                        }
                        result.Add(fields);
                    }
                    dr.Close();
                }
                cmd.Dispose();
            };
            return result;
        }

        protected ISession GetCurrentSession()
        {
            var result = ((UnitOfWorkScopeForNH)UnitOfWorkScope).Session();
            return result;
        }
    }
}
