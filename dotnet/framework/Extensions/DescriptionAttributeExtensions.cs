using System.ComponentModel;
using System.Linq;

namespace framework.Extensions
{
    public static class DescriptionAttributeExtensions
    {
        public static string Description<T>(this T source)
        {
            var result = source.ToString();
            var fi = source.GetType().GetField(source.ToString());
            var descriptionAttribute = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttribute != null && descriptionAttribute.Length > 0)
            {
                result = descriptionAttribute[0].Description;
            }
            else
            {
                var descricaoAttributes = (DescricaoAttribute[])fi.GetCustomAttributes(typeof(DescricaoAttribute), false);
                if (descricaoAttributes != null && descricaoAttributes.Length > 0)
                {
                    var att = descricaoAttributes.FirstOrDefault();
                    if (att != null)
                    {
                        result = att.Descricao;
                    }                     
                }
            }

            return result;
        }
    }
}
