using framework;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;

namespace config.NHibernate
{
    public class IntToTipoType<T> : IEnhancedUserType where T: Enum
    {
        public bool IsMutable
        {
            get { return false; }
        }

        public Type ReturnedType
        {
            get { return typeof(Tipo<T>); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.Int32.SqlType }; }
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var obj = rs[names[0]];

            if (obj == null) return null;
            var intValue = 0;
            int.TryParse(obj.ToString(), out intValue);
            var tipo = new Tipo<T>(default(T));
            tipo = intValue;
            return tipo;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                var tipo = value as Tipo<T>;
                if (tipo == null)
                {
                    tipo = new Tipo<T>((T)value);
                }
                ((IDataParameter)cmd.Parameters[index]).Value = tipo.Id;
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            if (x == null)
            {
                return 0;
            }
            return x.GetHashCode();
        }

        public object FromXMLString(string xml)
        {
            var result = new Tipo<T>(default(T));
            if(int.TryParse(xml, out var intValue)){
                result = intValue;
            }
            return result;
        }

        public string ObjectToSQLString(object value)
        {
            var result = default(string);
            var tipo = value as Tipo<T>;
            if (tipo == null)
            {
                result = null;
            } else {
                result = tipo.Id.ToString();
            }
            return result;
        }

        public string ToXMLString(object value)
        {
            throw new NotImplementedException();
        }
    }
}
