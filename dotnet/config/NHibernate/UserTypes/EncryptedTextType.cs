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
    public class EncryptedTextType : IUserType
    {
        public bool IsMutable
        {
            get { return false; }
        }

        public Type ReturnedType
        {
            get { return typeof(string); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.AnsiString.SqlType }; }
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var obj = rs[names[0]];
            if (obj == null) return new EncryptedText();
            var result = new EncryptedText();
            result.SetEncryptedText(obj.ToString());
            return result;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {                
                var encryptedText = value as EncryptedText;
                if (encryptedText == null) 
                {
                    throw new Exception("Esperava propriedade do tipo \"EncryptedText\".");
                }
                ((IDataParameter)cmd.Parameters[index]).Value =  encryptedText.GetEncryptedText();
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
    }
}