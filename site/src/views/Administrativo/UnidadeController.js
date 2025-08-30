import { showError } from '../../components/Messages';

export default function UnidadeController() {
   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'Id', className: 'codigo' },
         { titulo: 'Nome', width: '100%', orderby: 'nome' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [item.id, item.nome];
   };

   const getObjetoDeDados = (formState) => {
      return new Promise((resolve, reject) => {
         const item = formState?.itemSelecionado || {};

         if (!item.nome) {
            showError('Informe o nome da unidade.');

            
            reject();
            return;
         }
         var input = {
            nome: item.nome,
            unoAccessToken: item.unoAccessToken,
            unoSecretKey: item.unoSecretKey,
         };

         if (formState.alterando) {
            input.id = parseInt(item.id);
         }

         resolve(input);
      });
   };

   const itemVazio = {
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      itemVazio,
   };
}
