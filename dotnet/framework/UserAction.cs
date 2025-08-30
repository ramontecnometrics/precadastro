namespace framework
{
    public enum UserAction
    {
        // Nunca altere o número de uma rotina!!!
        [Descricao("Desconhecido")]
        NaoInformado = 0,
        [Descricao("Login")]
        Login = 1,
        [Descricao("Solicitou recuperação de senha")]
        SolicitouCodigoParaRecuperacaoDeSenha = 2,
        [Descricao("Alterou a senha")]
        AlterouASenha = 3,
        [Descricao("Aceitou termos de uso")]
        AceitouTermosDeUso = 4,
        [Descricao("Inseriu usuário")]
        InseriuUsuarioAdministrador = 5,
        [Descricao("Alterou usuário")]
        AlterouUsuarioAdministrador = 6,
        [Descricao("Excluiu usuário")]
        ExcluiuUsuarioAdministrador = 7,
        [Descricao("Inseriu termo de uso")]
        InseriuTermoDeUso = 8,
        [Descricao("Excluiu termo de uso")]
        ExcluiuTermoDeUso = 9,        
        [Descricao("Inseriu rotina do sistema")]
        InseriuRotinaDoSistema = 10,
        [Descricao("Alterou rotina do sistema")]
        AlterouRotinaDoSistema = 11,
        [Descricao("Alterou rotina do sistema")]
        ExcluiuRotinaDoSistema = 12,
        [Descricao("Inseriu parâmetro do sistema")]
        InseriuParametroDoSistema = 13,
        [Descricao("Alterou parâmetro do sistema")]
        AlterouParametroDoSistema = 14,
        [Descricao("Excluiu parâmetro do sistema")]
        ExcluiuParametroDoSistema = 15,
        [Descricao("Inseriu perfil de usuário")]
        InseriuPerfilDeUsuario = 16,
        [Descricao("Alterou perfil de usuário")]
        AlterouPerfilDeUsuario = 17,
        [Descricao("Excluiu perfil de usuário")]
        ExcluiuPerfilDeUsuario = 18,
        [Descricao("Inseriu lead")]
        InseriuLead = 19,
        [Descricao("Alterou lead")]
        AlterouLead = 20,
        [Descricao("Excluiu lead")]
        ExcluiuLead = 21,
        FormularioInsert = 22,
        FormularioUpdate = 23,
        FormularioDelete = 24,
    }
}
