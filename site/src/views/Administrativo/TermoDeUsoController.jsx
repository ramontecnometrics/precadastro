import { showError } from '../../components/Messages';

export default function TermoDeUsoController() {
   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'Id', className: 'codigo' },
         { titulo: 'Nome', width: '100%', orderby: 'nome' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [item.id, item.nome];
   };

   const getObjetoDeDados = async (formState) => {
      try {
         const item = formState?.itemSelecionado || {};

         if (!item.nome) {
            showError('Informe o termo de uso');
            return Promise.reject();
         }
         var input = {
            nome: item.nome,
            termo: item.termo,
         };

         if (formState.alterando) {
            input.id = parseInt(item.id);
         }

         return input;
      } catch (e) {
         showError(e.toString());
         console.error(e);
         return Promise.reject(e);
      }
   };

   const itemVazio = {
      situacao: { id: 1 },
      nivel: { id: 0 },
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      itemVazio,
   };
}
