using data;
using framework;

namespace model.Repositories
{
    public class RotinaDoSistemaRepository : Repository<RotinaDoSistema>
    {

        public RotinaDoSistemaRepository(IUnitOfWork<RotinaDoSistema> unitOfWork) : base(unitOfWork)
        {
        }

        public void AtualizarRotinas()
        {
            InserirOuAtualizar(90001, "Consultar arquivo");
            InserirOuAtualizar(90002, "Cadastrar arquivo");
            InserirOuAtualizar(90003, "Aceitar termos de uso");
            InserirOuAtualizar(90004, "Alterar idioma");
            InserirOuAtualizar(90005, "Termos de uso");
            InserirOuAtualizar(90006, "Consultar cidade");
            InserirOuAtualizar(90007, "Consultar país");
            InserirOuAtualizar(90008, "Consultar rotina do sistema");
            InserirOuAtualizar(90010, "Consultar termo de uso");

            InserirOuAtualizar(1011, "Consultar perfil de usuário");
            InserirOuAtualizar(1012, "Cadastrar perfil de usuário");
            InserirOuAtualizar(1013, "Alterar perfil de usuário");
            InserirOuAtualizar(1014, "Excluir perfil de usuário");

            InserirOuAtualizar(1021, "Consultar usuário");
            InserirOuAtualizar(1022, "Cadastrar usuário");
            InserirOuAtualizar(1023, "Alterar usuário");
            InserirOuAtualizar(1024, "Excluir usuário");

            InserirOuAtualizar(1041, "Consultar parâmetro do sistema");
            InserirOuAtualizar(1042, "Cadastrar parâmetro do sistema");
            InserirOuAtualizar(1043, "Alterar parâmetro do sistema");
            InserirOuAtualizar(1044, "Excluir parâmetro do sistema");

            InserirOuAtualizar(1051, "Consultar lead");
            InserirOuAtualizar(1052, "Cadastrar lead");
            InserirOuAtualizar(1053, "Alterar lead");
            InserirOuAtualizar(1054, "Excluir lead");

            InserirOuAtualizar(1061, "Consultar formulário");
            InserirOuAtualizar(1062, "Cadastrar formulário");
            InserirOuAtualizar(1063, "Alterar formulário");
            InserirOuAtualizar(1064, "Excluir formulário");

            InserirOuAtualizar(1071, "Consultar unidade");
            InserirOuAtualizar(1072, "Cadastrar unidade");
            InserirOuAtualizar(1073, "Alterar unidade");
            InserirOuAtualizar(1074, "Excluir unidade");

            InserirOuAtualizar(1382, "Cadastrar termo de uso");

            InserirOuAtualizar(9001, "Consultar logs do sistema");
        }

        protected virtual void InserirOuAtualizar(long rotina, string descricao)
        {
            var entity = Get(rotina);
            if (entity == null)
            {
                entity = new RotinaDoSistema()
                {
                    Id = rotina,
                    Descricao = descricao,
                };
                Insert(entity);
            }
            else
            if ((entity != null) && ((entity.Descricao != descricao)))
            {
                entity.Descricao = descricao;
                Update(entity);
            }
        }
    }
}
