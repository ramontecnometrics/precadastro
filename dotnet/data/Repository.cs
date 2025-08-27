using framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data
{
    public class Repository<T> where T : IEntity
    {
        private static Dictionary<string, Dictionary<long, IEntity>> _cache = new Dictionary<string, Dictionary<long, IEntity>>();
        private object _lock = new Object();
        private string _entityFullName = null;

        protected IUnitOfWork<T> UnitOfWork { get; set; }

        public Repository(IUnitOfWork<T> unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _entityFullName = typeof(T).FullName;
        }

        public virtual void Insert(T entity)
        {
            Validate(entity);
            if (entity is ISearchableEntity)
            {
                var searchableEntity = entity as ISearchableEntity;
                searchableEntity.Searchable = searchableEntity.GetSearchableText();
            }
            entity.Thumbprint = Guid.NewGuid().ToString();
            UnitOfWork.Insert(entity);
        }

        public virtual void Update(T entity)
        {
            Validate(entity);
            if (entity is ISearchableEntity)
            {
                var searchableEntity = entity as ISearchableEntity;
                searchableEntity.Searchable = searchableEntity.GetSearchableText();
                if (searchableEntity.Searchable.Length > 500)
                {
                    searchableEntity.Searchable = searchableEntity.Searchable.Substring(0, 500);
                }
            }
            entity.Thumbprint = Guid.NewGuid().ToString();
            UnitOfWork.Update(entity);
        }

        public virtual T Get(long id)
        {
            var result = TryToGetFromCache(id);
            if (result == null)
            {
                result = UnitOfWork.Get(id);
            }
            SaveToCache(result);
            return result;
        }

        public virtual T Get(EntityReference entityReference)
        {
            long? id = ConvertObjectToEntityId(entityReference);
            var result = default(T);
            if (id.HasValue)
            {
                result = TryToGetFromCache(id.Value);
                if (result == null)
                {
                    result = UnitOfWork.Get(id);
                }
                SaveToCache(result);
            }
            return result;
        }

        private void SaveToCache(T result)
        {
            if (result != null && result.Thumbprint != null)
            {
                lock (_lock)
                {
                    if (!_cache.ContainsKey(_entityFullName))
                    {
                        _cache.Add(_entityFullName, new Dictionary<long, IEntity>());
                    }
                    _cache[_entityFullName][result.Id] = result;
                }
            }
        }

        protected virtual T TryToGetFromCache(long id)
        {
            var result = default(T);
            if (_cache.ContainsKey(_entityFullName))
            {
                var entityCache = _cache[_entityFullName];
                if (entityCache.ContainsKey(id))
                {
                    var entityFromCache = entityCache[id];
                    if (entityFromCache.Thumbprint != null)
                    {
                        var thumbprintInDatabase = UnitOfWork.GetAll()
                            .Where(i => i.Id == id)
                            .Select(i => i.Thumbprint)
                            .FirstOrDefault();
                        if (entityFromCache.Thumbprint == thumbprintInDatabase)
                        {
                            result = (T)entityFromCache;
                        }
                    }
                }
            }
            return result;
        }

        protected virtual long? ConvertObjectToEntityId(object id)
        {
            var result = default(long?);
            if (id != null)
            {
                if (id is IId)
                {
                    var entityReference = id as IId;
                    if (entityReference.Id.HasValue)
                    {
                        result = entityReference.Id.Value;
                    }
                }
                else
                {
                    result = long.Parse(id.ToString());
                }
            }
            return result;
        }

        public virtual T Get(object id, bool raiseErrorIfNotFound)
        {
            var result = default(T);
            if (id != null)
            {
                var oid = ConvertObjectToEntityId(id);
                if (oid.HasValue)
                {
                    result = Get(oid.Value);
                }
            }

            if ((result == null) && (id != null) && (raiseErrorIfNotFound))
            {
                var entity = Activator.CreateInstance<T>();
                throw new RegistroNaoEncontradoException(string.Format("{0}{1}{2}{3}{4}",
                    entity.NomeDaEntidade, " ", id.ToString(), " não ",
                    entity.GeneroDaEntidade.IsMasculino() ? "encontrado." : "encontrada."));
            }
            return result;
        }

        public virtual void Delete(T entity)
        {
            UnitOfWork.Delete(entity);
        }

        public virtual IQueryable<T> GetAll()
        {
            return UnitOfWork.GetAll();
        }

        public virtual AggregateException Validate(T entity, bool raiseExceptions = true)
        {
            var result = PropertyValidator.ValidateProperties(entity);
            if (raiseExceptions && (result != null))
            {
                throw result.InnerExceptions.First();
            }
            return result;
        }
    }

    public static class IEnumerableExtensions
    {
        public static T GetFieldValue<T>(this IEnumerable<KeyValuePair<string, object>> record, string columnName)
        {
            var result = default(T);
            var field = record.Where(i => i.Key.ToLower() == columnName.ToLower()).FirstOrDefault();
            if (field.Value != null)
            {
                var isNullable = typeof(T).IsGenericType &&
                    typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);
                var valueType = isNullable ? typeof(T).GetGenericArguments().First() : typeof(T);

                result = (T)Convert.ChangeType(field.Value, valueType);
            }
            return result;
        }
    }

    [System.Serializable]
    public class RegistroNaoEncontradoException : System.Exception
    {
        public RegistroNaoEncontradoException() { }
        public RegistroNaoEncontradoException(string message) : base(message) { }
        public RegistroNaoEncontradoException(string message, System.Exception inner) : base(message, inner) { }
        protected RegistroNaoEncontradoException(
            System.Runtime.Serialization.SerializationInfo info,
#pragma warning disable SYSLIB0051 // Type or member is obsolete
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051 // Type or member is obsolete
    }
}

