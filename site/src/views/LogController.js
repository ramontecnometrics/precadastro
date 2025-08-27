import Text from '../components/Text';
import { dateTimeToString } from '../utils/Functions';

export default function LogController (view) {     

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'Id', className: 'codigo' },
         { titulo: 'Categoria', width: '5%', orderby: 'SubCategory' },
         { titulo: 'Data', width: '5%', orderby: 'Date' , className: "" },
         { titulo: 'Ação', width: '25%', orderby: 'Action' },
         { titulo: 'Método', width: '5%', orderby: 'HttpMethod' },
         { titulo: 'HTTP', width: '5%', orderby: 'HttpResponseCode' },
         { titulo: 'Usuário', width: '5%', orderby: 'UserId' },
         { titulo: 'Mensagem', width: '50%', orderby: 'HttpResponseCode', className: 'multilineTableData' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [
         item.id,
         item.subCategory.description,
         <Text style={{wordBreak: "break-all"}}>{ dateTimeToString(item.date)}</Text>,
         <Text style={{wordBreak: "break-all"}}>{item.action}</Text>,
         item.httpMethod,
         item.httpResponseCode,
         item.userId,
         item.message,
      ];
   };

   const getObjetoDeDados = () => {};

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
   };
}
