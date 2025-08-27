using framework.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace framework
{
    public class Tipo<T>
    {
        internal T InnerValue { get; }

        [JsonConstructor]
        public Tipo(int id)
        {
            var enumValues = Enum.GetValues(typeof(T));
            bool exsite = false;
            Tipo<T> value = id;
            foreach (T tipo in enumValues)
            {
                if (tipo.GetHashCode() == value.GetHashCode())
                {
                    exsite = true;
                    break;
                }
            }
            if (!exsite)
            {
                throw new InvalidCastException($"Valor inválido para o Enum {typeof(T).Name}.");
            }
            InnerValue = value;
        }

        public Tipo(T value)
        {
            var enumValues = Enum.GetValues(typeof(T));
            bool exsite = false;
            foreach (T tipo in enumValues)
            {
                if (tipo.GetHashCode() == value.GetHashCode())
                {
                    exsite = true;
                    break;
                }
            }
            if (!exsite)
            {
                throw new InvalidCastException($"Valor inválido para o Enum {typeof(T).Name}.");
            }
            InnerValue = value;
        }

        public static implicit operator Tipo<T>(T value)
        {
            var result = new Tipo<T>(value);
            return result;
        }

        public static implicit operator Tipo<T>(int value)
        {
            var enumValues = Enum.GetValues(typeof(T));
            T enumValue = default(T);
            foreach (var tipo in enumValues)
            {
                if ((int)tipo == value)
                {
                    enumValue = (T)tipo;
                    break;
                }
            }
            return new Tipo<T>(enumValue);
        }

        public static implicit operator Tipo<T>(long value)
        {
            var enumValues = Enum.GetValues(typeof(T));
            T enumValue = default(T);
            foreach (var tipo in enumValues)
            {
                if ((int)tipo == value)
                {
                    enumValue = (T)tipo;
                    break;
                }
            }
            return new Tipo<T>(enumValue);
        }

        public static implicit operator int(Tipo<T> value)
        {
            var enumValues = Enum.GetValues(typeof(T));
            int intValue = -1;
            foreach (T tipo in enumValues)
            {
                var x = new Tipo<T>(tipo);
                if (x.GetHashCode() == value.GetHashCode())
                {
                    intValue = Convert.ToInt32(tipo);
                    break;
                }
            }
            return intValue;
        }

        public static implicit operator long(Tipo<T> value)
        {
            var enumValues = Enum.GetValues(typeof(T));
            int intValue = -1;
            foreach (T tipo in enumValues)
            {
                var x = new Tipo<T>(tipo);
                if (x.GetHashCode() == value.GetHashCode())
                {
                    intValue = Convert.ToInt32(tipo);
                    break;
                }
            }
            return intValue;
        }

        public override int GetHashCode()
        {
            return InnerValue.GetHashCode();
        }

        public static implicit operator T(Tipo<T> value)
        {
            T result = value.InnerValue;
            return result;
        }

        public int Id
        {
            get
            {
                int result = new Tipo<T>(InnerValue);
                return result;
            }
        }

        public T Value
        {
            get
            {
                return InnerValue;
            }
        }

        public string Descricao
        {
            get
            {
                return DescriptionAttributeExtensions.Description<T>(InnerValue);
            }
        }

        public bool Is(T tipo)
        {
            return this.InnerValue.GetHashCode() == tipo.GetHashCode();
        }

        public bool Is(params T[] tipos)
        {
            return tipos.Select(i => i.GetHashCode()).Contains(this.InnerValue.GetHashCode());
        }

        public bool IsNot(T tipo)
        {
            return this.InnerValue.GetHashCode() != tipo.GetHashCode();
        }
    }
}
