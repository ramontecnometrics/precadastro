using data;

namespace model.Repositories
{
    public class LeadRepository : Repository<Lead>
    {
        public LeadRepository(IUnitOfWork<Lead> unitOfWork) : base(unitOfWork)
        {
        }

        public override void Delete(Lead entity)
        {
            entity.Situacao = SituacaoDeLead.Excluido;
            Update(entity);
        }
    }
}
