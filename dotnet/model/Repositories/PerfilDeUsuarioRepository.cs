using data;
using System;
using System.Linq;

namespace model.Repositories
{
    public class PerfilDeUsuarioRepository : Repository<PerfilDeUsuario>
    {
        private readonly Repository<AcessoDePerfilDeUsuario> AcessoDePerfilDeUsuarioRepository;
        private readonly Repository<RotinaDoSistema> RotinaDoSistemaRepository;

        public PerfilDeUsuarioRepository(IUnitOfWork<PerfilDeUsuario> unitOfWork,
            Repository<AcessoDePerfilDeUsuario> acessoDePerfilDeUsuarioRepository,
             Repository<RotinaDoSistema> rotinaDoSistemaRepository) : base(unitOfWork)
        {
            AcessoDePerfilDeUsuarioRepository = acessoDePerfilDeUsuarioRepository;
            RotinaDoSistemaRepository = rotinaDoSistemaRepository;
        }

        public virtual PerfilDeUsuario GetPerfilByNome(string nome)
        {
            var result = UnitOfWork.GetAll()
                .Where(i => i.Nome.ToLower().Trim() == nome.ToLower().Trim())
                .FirstOrDefault();
            return result;
        }

        public override void Insert(PerfilDeUsuario entity)
        {
            Validate(entity);
            ValidarDuplicidade(entity);
            base.Insert(entity);
        }

        public override void Update(PerfilDeUsuario entity)
        {
            var itemSalvo = Get(entity.Id);
            if ((itemSalvo != null) && (itemSalvo.Nome.ToLower().Trim() == "master"))
            {
                throw new System.Exception("O perfil MASTER não pode ser alterado.");
            }
            Validate(entity);
            ValidarDuplicidade(entity);
            base.Update(entity);           
        }

        private void ValidarDuplicidade(PerfilDeUsuario entity)
        {
            var duplicado = GetAll()
                .Where(i => i.Id != entity.Id)
                .Where(i => i.Nome.ToLower() == entity.Nome.ToLower())
                .Where(i => i.Situacao == SituacaoDePerfilDeUsuario.Ativo)
                .Where(i => i.TipoDePerfil == TipoDePerfilDeUsuario.NaoDefinido)
                .Any();
            if (duplicado)
            {
                throw new Exception("Já existe um perfil ativo cadastrado com esse nome.");
            }
        }

        public virtual RotinaDoSistema GetRotina(long id, bool raiseErrorIfNotFound)
        {
            var result = RotinaDoSistemaRepository.Get(id, raiseErrorIfNotFound);
            return result;
        }

        public virtual void Delete(AcessoDePerfilDeUsuario acesso)
        {
            AcessoDePerfilDeUsuarioRepository.Delete(acesso);
        }
                
        public override void Delete(PerfilDeUsuario entity)
        {
            entity.Situacao = SituacaoDePerfilDeUsuario.Excluido;
            Update(entity);
        }
    }
}
