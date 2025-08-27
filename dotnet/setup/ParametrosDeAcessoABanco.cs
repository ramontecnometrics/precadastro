using System;
using System.Collections.Generic;
using System.Text;

namespace setup
{
    public class ParametrosDeAcessoABanco
    {
        public string Servidor { get; set; }
        public int? Porta { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string NomeDoBancoDeDados { get; set; }

        public virtual string GetConnectionString(bool especificarBancoDeDados = true)
        {
            var result =
                especificarBancoDeDados ?
                    string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};SslMode=Disable;",
                        Servidor, Porta, Usuario, Senha, NomeDoBancoDeDados) :
                    string.Format("Server={0};Port={1};User Id={2};Password={3};SslMode=Disable;",
                        Servidor, Porta, Usuario, Senha);
            return result;
        }
    }
}
