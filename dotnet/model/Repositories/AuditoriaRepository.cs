using data;
using framework;

namespace model.Repositories
{
    public class AuditoriaRepository : Repository<Auditoria>
    {
        public AuditoriaRepository(IUnitOfWork<Auditoria> unitOfWork) : base(unitOfWork)
        {
        }

        public virtual void RegistrarAcao( long idDoUsuario, UserAction acao, string descricao)
        {
            var item = new Auditoria()
            {
                Data = DateTimeSync.Now,
                Acao = acao,
                Descricao = descricao,
                IdDoUsuario = idDoUsuario,
            };
            Insert(item);
        }
    }
}
