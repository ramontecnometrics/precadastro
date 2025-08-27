import { dateTimeToString, formatDate } from '../utils/Functions'; 

export default function AuditoriaController (view) {

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'Id', className: 'codigo' },
         { titulo: 'Usuário', width: '25%', orderby: 'nomeDoUsuario' },
         { titulo: 'Ação', width: '25%', orderby: 'acao' },
         { titulo: 'Data', width: '25%', orderby: 'data' },
         { titulo: 'Detalhes', width: '25%', orderby: 'descricao' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [item.id, item.nomeDoUsuario, item.acao.descricao, dateTimeToString(item.data), item.descricao];
   };

   const getObjetoDeDados = () => {
      return new Promise((resolve, reject) => {
         resolve({});
      });
   };

   const getFiltro = () => {
      let result = {};

      result.usuario = view.state.usuario  ? 
         view.state.usuario.idDoUsuario ?  view.state.usuario.idDoUsuario :
         view.state.usuario.id : null;
      result.acao = view.state.acao ? view.state.acao.id : null;
      result.dataInicial = formatDate(view.state.dataInicial);
      result.horaInicial = view.state.horaInicial;
      result.dataFinal = formatDate(view.state.dataFinal);
      result.horaFinal = view.state.horaFinal;

      return result;
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      getFiltro,
   };
}
