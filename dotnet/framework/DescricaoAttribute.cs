using System;

namespace framework
{
    [AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public class DescricaoAttribute : Attribute
    {
        public string Descricao { get; set; }

        public DescricaoAttribute()
        {

        }

        public DescricaoAttribute(string descricao)
        {
            Descricao = descricao;
        }
    }
}
