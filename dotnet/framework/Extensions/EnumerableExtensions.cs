using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Merge<T, U>(
            this IEnumerable<T> existingList,
            IEnumerable<U> newList,
            Func<T, U, bool> compare,
            Action<T> delete,
            Action<T, U> update,
            Action<U> createNew)
        {
            var list = existingList.ToList();

            if (compare == null)
            {
                throw new Exception("Função de comparação não pode ser null.");
            }

            if (existingList == null)
            {
                throw new Exception("existingList não pode ser null.");
            }

            if (newList == null)
            {
                throw new Exception("newList lista não pode ser null.");
            }

            // remover os itens que não estão na nova lista
            var deleted = list.Where(i => !newList.Where(j => compare(i, j)).Any()).ToArray();
            foreach (var i in deleted)
            {
                delete?.Invoke(i);
            }

            // atualizar os itens que estão nas duas listas
            list.ForEach(i =>
            {
                newList.Where(j => compare(i, j))
                .ForEach(j =>
                {
                    update?.Invoke(i, j);
                });
            });

            // adicionar os itens que estão somente na nova lista
            newList.ForEach(i =>
            {
                if (!list.Where(j => compare(j, i)).Any())
                {
                    createNew?.Invoke(i);
                }
            });

            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
