import { showError } from '../../components/Messages';
import { buildQueryString } from '../../utils/Functions';
import api from '../../utils/Api';

export default function PerfilDeUsuarioController() {

   const itemVazio = {
      situacao: { id: 1 },
      acessos: [],
      tipoDePerfil: { id: 1 },
   };

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'Id', className: 'codigo' },
         { titulo: 'Nome', width: '50%', orderby: 'Nome' },
         { titulo: 'Situação', width: '25%', orderby: null },
         { titulo: 'Tipo de perfil', width: '25%', orderby: null },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [
         item.id,
         item.nome,
         item.situacao ? item.situacao.descricao : null,
         item.tipoDePerfil ? item.tipoDePerfil.descricao : null,
      ];
   };

   const getObjetoDeDados = (formState) => {
      return new Promise((resolve, reject) => {
         let state = formState;
         let item = state.itemSelecionado;

         if (!item.nome) {
            showError('Informe o nome do perfil de usuário');
            reject();
            return;
         }

         var input = {
            nome: item.nome,
            situacao: item.situacao ? item.situacao.id : 0,
            acessos: item.acessos,
            tipoDePerfil: item.tipoDePerfil ? item.tipoDePerfil.id : 0,
         };

         if (state.alterando) {
            input.id = parseInt(item.id);
         }
         resolve(input);
      });
   };

   const aposInserir = (formState, setFormState) => {
      inserirTodas(formState, setFormState);
   };

   const inserirTodas = (formState, setFormState) => {
      return new Promise((resolve, reject) => {
         let state = formState;

         api.get(
            '/rotinadosistema' +
               buildQueryString(null, null, 'id', {
                  Searchable: '',
                  DisponivelParaTecnometrics: true,
               })
         )
            .then((result) => {
               state.itemSelecionado.acessos = [];

               result.items.filter((rotina) => rotina.id < 90000).forEach((rotina) => {
                  state.itemSelecionado.acessos.push({ rotina: rotina });
               });

               setFormState((prev) => ({ ...prev, itemSelecionado: state.itemSelecionado }));

               resolve();
            })
            .catch(reject);
      });
   };

   return {
      itemVazio,
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      aposInserir,
      inserirTodas,
   };
}
