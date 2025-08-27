using System;
using data;

namespace model.Repositories
{
    public class UsuarioAdministradorRepository : Repository<UsuarioAdministrador>
    {
        private readonly Repository<TelefoneDePessoa> TelefoneDePessoaRepository;
        private readonly Repository<EnderecoDePessoa> EnderecoDePessoaRepository; 
        private readonly Repository<PerfilDoUsuario>  PerfilDoUsuarioRepository;

        public UsuarioAdministradorRepository(
            IUnitOfWork<UsuarioAdministrador> unitOfWork,
            Repository<TelefoneDePessoa> telefoneDePessoaRepository,
            Repository<EnderecoDePessoa> enderecoDePessoaRepository, 
            Repository<PerfilDoUsuario> perfilDoUsuarioRepository) :
            base(unitOfWork)
        {
            TelefoneDePessoaRepository = telefoneDePessoaRepository;
            EnderecoDePessoaRepository = enderecoDePessoaRepository; 
            PerfilDoUsuarioRepository = perfilDoUsuarioRepository;
        }

        public void DeleteTelefone(TelefoneDePessoa telefoneDePessoa)
        {
            TelefoneDePessoaRepository.Delete(telefoneDePessoa);
        }

        public void Delete(EnderecoDePessoa enderecoDePessoa)
        {
            EnderecoDePessoaRepository.Delete(enderecoDePessoa);
        }               

        public void Delete(PerfilDoUsuario perfilDoUsuario)
        {
            PerfilDoUsuarioRepository.Delete(perfilDoUsuario);
        }
    }

    public class UsuarioAdministradorFastRepository : Repository<UsuarioAdministradorFast>
    {
        public UsuarioAdministradorFastRepository(IUnitOfWork<UsuarioAdministradorFast> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
