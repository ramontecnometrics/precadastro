using data;
using System;
using System.Collections.Generic;
using System.Text;

namespace model
{
    public class Formulario : ISearchableEntity
    {
        public long Id { get; set; }
        public Genero GeneroDaEntidade => Genero.Masculino;
        public string NomeDaEntidade => "Formulário";
        public string Thumbprint { get; set; }
        public static string SearchableScope = "Formulario";
        public string Searchable { get; set; }
        public string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }

        public string Nome { get; set; }
        public string Descricao { get; set; }
        public IList<GrupoDeFormulario> Grupos { get; set; }
    }

    public class GrupoDeFormulario : IEntity
    {
        public long Id { get; set; }
        public Genero GeneroDaEntidade => Genero.Masculino;
        public string NomeDaEntidade => "Grupo de formulário";
        public string Thumbprint { get; set; }
        public Formulario Formulario { get; set; }

        public string Titulo { get; set; }

        public IList<CampoDeGrupoDeFormulario> Campos { get; set; }
        public int Ordem { get; set; }
    }

    public class CampoDeGrupoDeFormulario : IEntity
    {
        public long Id { get; set; }
        public Genero GeneroDaEntidade => Genero.Masculino;
        public string NomeDaEntidade => "Campo de formulário";
        public string Thumbprint { get; set; }
        public GrupoDeFormulario GrupoDeFormulario { get; set; }

        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public bool Obrigatorio { get; set; }
        public int Ordem { get; set; }
    }

    public class FormularioFast : IEntity
    {
        public long Id { get; set; }
        public Genero GeneroDaEntidade => Genero.Masculino;
        public string NomeDaEntidade => "Formulário";
        public string Thumbprint { get; set; }
        
        public string Searchable { get; set; }

        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
