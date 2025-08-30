import { showError } from "../components/Messages";

export default function RotinaDoSistemaController(view) {
   const getTitulosDaTabela = () => [
      { titulo: "Código", orderby: "Id", className: "codigo" },
      { titulo: "Descrição", width: "100%", orderby: "Descricao" },
   ];

   const getDadosDaTabela = (item) => [item.id, item.descricao];

   const getObjetoDeDados = () => {
      return new Promise((resolve, reject) => {
         const state = view.getFormState(); // ✅ corrigido
         const item = state.itemSelecionado;

         if (!item.descricao) {
            showError("Informe a descrição da rotina");
            return reject(new Error("Descrição obrigatória"));
         }

         const input = { descricao: item.descricao };
         if (state.alterando && item.id) {
            input.id = parseInt(item.id, 10);
         }
         resolve(input);
      });
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
   };
}
