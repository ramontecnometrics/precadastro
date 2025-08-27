using data;
using framework;
using System;
using System.Linq;

namespace model.Repositories
{
    public class ParametroDoSistemaRepository : Repository<ParametroDoSistema>
    {
        public ParametroDoSistemaRepository(IUnitOfWork<ParametroDoSistema> unitOfWork)
            : base(unitOfWork)
        {
        }

        public override void Insert(ParametroDoSistema entity)
        {
            ValidarDuplicidade(entity);
            base.Insert(entity);
        }

        public override void Update(ParametroDoSistema entity)
        {
            ValidarDuplicidade(entity);
            base.Update(entity);
        }

        protected virtual void ValidarDuplicidade(ParametroDoSistema entity)
        {

                if (GetAll().Where(i => i.Nome == entity.Nome && i.Id != entity.Id).Any())
                {
                    throw new Exception("Parâmetro já cadastrado.");
                }
         
        }

        protected virtual object GetValue(string nome)
        {
            var valor = GetAll()
                .Where(i => i.Nome == nome)
                .Select(i => i.Valor)
                .FirstOrDefault();

            var result = valor != null ? valor.GetPlainText() : null;
            return result;
        }

        public virtual string GetString(string nome)
        {
            var result = default(string);
            var value = GetValue(nome);
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }


        public virtual int? GetInt(string nome)
        {
            var result = default(int?);
            var value = GetValue(nome);
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int resultInt))
                {
                    result = resultInt;
                }
            }
            return result;
        }

        public virtual long? GetLong(string nome)
        {
            var result = default(long?);
            var value = GetValue(nome);
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out long resultInt))
                {
                    result = resultInt;
                }
            }
            return result;
        }

        public virtual decimal? GetDecimal(string nome)
        {
            var result = default(decimal?);
            var value = GetValue(nome);
            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out decimal resultInt))
                {
                    result = resultInt;
                }
            }
            return result;
        }

        public virtual bool GetBoolean(string nome)
        {
            var valores = new string[] { "sim", "1", "true", "t", "y" };
            var result = default(bool);
            var value = GetValue(nome);
            if (value != null)
            {
                result = valores.Contains(value.ToString().ToLower());
            }
            return result;
        }
    }
}
