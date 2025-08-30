using data;
using framework;
using framework.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace model
{
    public class Auditoria : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Auditoria";
        public virtual Genero GeneroDaEntidade => Genero.Feminino;

        public virtual long IdDoUsuario { get; set; } 
        public virtual DateTime Data { get; set; }
        public virtual EncryptedText Descricao { get; set; }
        public virtual Tipo<UserAction> Acao { get; set; }

    }

    public class AuditoriaFast : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Auditoria";
        public virtual Genero GeneroDaEntidade => Genero.Feminino;

        public virtual long IdDoUsuario { get; set; } 
        public virtual string NomeDoUsuario { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual EncryptedText Descricao { get; set; }
        public virtual Tipo<UserAction> Acao { get; set; }
    }

    public static class AuditoriaHelper
    {
        public static string BuildDescriptionForInsert(IEntity entity)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var result = JsonConvert.SerializeObject(entity, serializerSettings);
            return result;
        }

        public static string BuildDescriptionForUpdate(IEntity entityBefore, IEntity entityNow)
        {
            var result = $"{entityBefore.NomeDaEntidade} ID = {entityBefore.Id}\n";

            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            //var entityBeforeJson = JsonConvert.SerializeObject(entityBefore, serializerSettings);
            //var entityNowJson = JsonConvert.SerializeObject(entityNow, serializerSettings);

            var diferencas = Comparador.CompararEntity(entityBefore, entityNow, null);

            if (diferencas.Any())
            {
                foreach (var diferenca in diferencas)
                {
                    result += $"\n{diferenca}";
                }
            }
            else
            {
                result += "\nNenhuma alteração foi realizada.";
            }

            return result;
        }

        public static string BuildDescriptionForDelete(IEntity item)
        {
            var result = $"{item.NomeDaEntidade} ID = {item.Id}";
            return result;
        }
    }

    public static class Comparador
    {
        public static List<string> CompararEntity(IEntity entity1, IEntity entity2, Type parentType)
        {
            var diferencas = new List<string>();

            if (entity1 == null || entity2 == null)
            {
                diferencas.Add("Uma ou ambas as instâncias são nulas.");
                return diferencas;
            }

            // Comparar propriedades simples e complexas
            var propriedades = entity1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propriedadesAIgnorar = new string[] { "Searchable", "Thumbprint", "NomeDaEntidade" };

            foreach (var propriedade in propriedades)
            {

                if (propriedadesAIgnorar.Contains(propriedade.Name))
                {
                    continue;
                }

                if (parentType != null && propriedade.PropertyType == parentType)
                {
                    continue;
                }

                var valor1 = propriedade.GetValue(entity1);
                var valor2 = propriedade.GetValue(entity2);

                if (propriedade.PropertyType.IsGenericType &&
                    propriedade.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) &&
                    propriedade.PropertyType.GetGenericArguments()[0].GetInterfaces().Contains(typeof(IEntity)))
                {
                    diferencas.AddRange(CompararListas(propriedade.Name, valor1, valor2, entity1.GetType()));
                }
                else
                if (propriedade.PropertyType.GetInterfaces().Contains(typeof(IEntity)))
                {
                    diferencas.AddRange(CompararIds(propriedade.Name, (IEntity)valor1, (IEntity)valor2));
                }
                else
                if (propriedade.PropertyType == typeof(EncryptedText))
                {
                    var v1 = valor1 as EncryptedText;
                    var v2 = valor2 as EncryptedText;

                    var texto1 = v1.GetPlainText();
                    var texto2 = v2.GetPlainText();

                    if (!Equals(texto1, texto2))
                    {
                        diferencas.Add($"Campo \"{propriedade.Name}\" alterado: valor antes = {texto1}, valor depois = {texto2}");
                    }
                }
                else
                if (propriedade.PropertyType.IsGenericType && propriedade.PropertyType.GetGenericTypeDefinition() == typeof(Tipo<>))
                {
                    diferencas.AddRange(CompararTipos(propriedade.Name, propriedade.PropertyType, valor1, valor2));
                }
                else
                {
                    if (!Equals(valor1, valor2))
                    {
                        diferencas.Add($"Campo \"{propriedade.Name}\" alterado: valor antes = {valor1}, valor depois = {valor2}");
                    }
                }
            }

            return diferencas;
        }

        public static string GetCamposDeItemDeLista(IEntity entity)
        {
            var propriedades = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propriedadesAIgnorar = new string[] { "Searchable", "Thumbprint", "NomeDaEntidade" };
            var novo = new StringBuilder();

            foreach (var propriedade in propriedades)
            {
                var valor = propriedade.GetValue(entity);

                if (propriedadesAIgnorar.Contains(propriedade.Name) || valor == null)
                {
                    continue;
                }

                if (propriedade.PropertyType.IsGenericType &&
                    propriedade.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) &&
                    propriedade.PropertyType.GetGenericArguments()[0].GetInterfaces().Contains(typeof(IEntity)))
                {
                    // Vamos ignorar listas dentro de itens de lista
                }
                else
                if (propriedade.PropertyType.GetInterfaces().Contains(typeof(IEntity)))
                {
                    novo.AppendLine($"{propriedade.Name}: ID {((IEntity)valor).Id}");
                }
                else
                if (propriedade.PropertyType.IsGenericType && propriedade.PropertyType.GetGenericTypeDefinition() == typeof(Tipo<>))
                {
                    var propDescricao = propriedade.PropertyType.GetProperty("Descricao");
                    novo.AppendLine($"{propriedade.Name}: {propDescricao}");
                }
                else
                {
                    novo.AppendLine($"{propriedade.Name}: {valor}");
                }
            }

            var result = novo.ToString();
            return result;
        }

        private static List<string> CompararListas(string nomeDaPropriedade, object plista1, object plista2,
            Type parentType)
        {
            var diferencas = new List<string>();

            var lista1 = (IEnumerable<IEntity>)plista1;
            var lista2 = (IEnumerable<IEntity>)plista2;

            if (lista1 == null)
            {
                lista1 = new IEntity[] { };
            }

            if (lista2 == null)
            {
                lista2 = new IEntity[] { };
            }

            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };


            lista1.Merge(lista2,
                (e1, e2) => e1.Id == e2.Id,
                (e) =>
                {
                    var excluido = GetCamposDeItemDeLista(e);
                    diferencas.Add($"Lista \"{nomeDaPropriedade}\" alterada:\n\nItem removido \n\n{excluido}");
                },
                (e1, e2) =>
                {
                    var alteracoes = "";

                    var diferencasDeAlteracaoDoItem = CompararEntity(e1, e2, parentType);

                    if (diferencasDeAlteracaoDoItem.Any())
                    {
                        foreach (var diferenca in diferencasDeAlteracaoDoItem)
                        {
                            alteracoes += $"{diferenca}\n";
                        }
                        diferencas.Add($"Lista \"{nomeDaPropriedade}\" alterada:\n\nItem {e1.Id}\n\n{alteracoes} ");
                    }
                },
                (e) =>
                {
                    var novo = GetCamposDeItemDeLista(e);
                    diferencas.Add($"Lista \"{nomeDaPropriedade}\" alterada: \n\nNovo item adicionado\n\n{novo.ToString()}");
                }
            );

            return diferencas;
        }

        private static List<string> CompararIds(string nomeDaPropriedade, IEntity entity1, IEntity entity2)
        {
            var diferencas = new List<string>();

            if (entity1 == null && entity2 == null)
            {
                // Se ambos estiverem nulos vamos ignorar
            }
            else
            if (entity1 != null && entity2 == null)
            {
                diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = {entity1.Id}, valor depois = null");
            }
            else
            if (entity1 == null && entity2 != null)
            {
                diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = null, valor depois = {entity2.Id}");
            }
            else
            if (entity1.Id != entity2.Id)
            {
                diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = {entity1.Id}, valor depois = {entity2.Id}");
            }

            return diferencas;
        }

        private static List<string> CompararTipos(string nomeDaPropriedade, Type tipoType, object tipo1, object tipo2)
        {
            var diferencas = new List<string>();

            long? id1 = null;
            string descricao1 = null;
            long? id2 = null;
            string descricao2 = null;

            if (tipo1 == null && tipo2 == null)
            {
                return diferencas;
            }

            var propId = tipoType.GetProperty("Id");
            var propDescricao = tipoType.GetProperty("Descricao");

            if (tipo1 != null && tipo2 == null)
            {
                descricao1 = propDescricao.GetValue(tipo1).ToString();
                diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = {descricao1}, valor depois = null");
            }
            else
            if (tipo1 == null && tipo2 != null)
            {
                descricao2 = propDescricao.GetValue(tipo2).ToString();
                diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = null, valor depois = {descricao2}");
            }
            else
            if (tipo1 != null && tipo2 != null)
            {
                id1 = int.Parse(propId.GetValue(tipo1).ToString());
                descricao1 = propDescricao.GetValue(tipo1).ToString();
                id2 = int.Parse(propId.GetValue(tipo2).ToString());
                descricao2 = propDescricao.GetValue(tipo2).ToString();

                if (id1 != id2)
                {
                    diferencas.Add($"Campo \"{nomeDaPropriedade}\" alterado: valor antes = {descricao1}, valor depois = {descricao2}");
                }

                return diferencas;
            }

            return diferencas;
        }
    }
}