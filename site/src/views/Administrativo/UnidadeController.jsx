import { showError } from '../../components/Messages';
import { create_UUID, generatePassword } from '../../utils/Functions';

export default function UnidadeController() {
   const itemVazio = {};

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
            uuid: item.uuid,
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

   const aposInserir = async (formState, setFormState) => {
      return new Promise((resolve, reject) => {
         const uuid = generatePassword(false, 6).toLocaleLowerCase();
         setFormState((prev) => ({ ...prev, itemSelecionado: { ...prev.itemSelecionado, uuid: uuid } }));
         console.log(uuid);
         resolve();
      });
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      itemVazio,
      aposInserir,
   };
}
