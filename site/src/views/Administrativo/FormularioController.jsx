import { showError } from '../../components/Messages';
import { rsaEncrypt, concatUint8Arrays, uint8ToBase64, base64ToUint8 } from '../../utils/Functions';
import api from '../../utils/Api';

export default function FormularioController() {
   const itemVazio = {
      situacao: { id: 1 },
      grupos: [],
   };

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'id', className: 'codigo' },
         { titulo: 'Nome', width: '100%', orderby: 'nome' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [item.id, item.nome];
   };

   const getObjetoDeDados = async (formState, setFormState) => {
      try {
         const item = formState.itemSelecionado;

         // --- validações ---
         if (!item.nome) {
            showError('Informe o nome do formulário.');
            return Promise.reject();
         }

         // --- objeto final ---
         const input = {
            nome: item.nome,
            descricao: item.descricao,
            grupos: item.grupos,
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

   const alterarGrupo = (formState, setFormState, itemSelecionado, setItemSelecionado, indiceEmEdicao) => {
      return new Promise((resolve, reject) => {
         let grupoSelecionado = formState.grupoSelecionado;
         const grupos = itemSelecionado.grupos || [];
         grupos[indiceEmEdicao] = grupoSelecionado;
         console.log(grupos);
         setItemSelecionado({ grupos: grupos });
         setFormState((prev) => ({ ...prev, grupoSelecionado: null }));
         resolve();
      });
   };

   const inserirGrupo = (formState, setFormState, itemSelecionado, setItemSelecionado) => {
      return new Promise((resolve, reject) => {
         const grupo = formState.grupoSelecionado;
         if (!grupo) return;
         const grupos = itemSelecionado.grupos || [];
         grupos.push(grupo);
         setItemSelecionado({ grupos: grupos });
         setFormState((prev) => ({ ...prev, grupoSelecionado: null }));
         resolve();
      });
   };

   const inserirCampo = (formState, setFormState, grupoSelecionado, setItemSelecionado) => {
      return new Promise((resolve, reject) => {
         const campo = formState.campoSelecionado;
         if (!campo) return;
         const campos = grupoSelecionado.campos || [];
         campos.push(campo);
         setFormState((prev) => ({
            ...prev,
            campoSelecionado: null,
            grupoSelecionado: { ...prev.grupoSelecionado, campos: campos },
         }));
         resolve();
      });
   };

   const alterarCampo = (formState, setFormState, grupoSelecionado, setItemSelecionado, indiceEmEdicao) => {
      return new Promise((resolve, reject) => {
         const campo = formState.campoSelecionado;
         if (!campo) return;
         const campos = formState.grupoSelecionado.campos || [];
         campos[indiceEmEdicao] = campo;
         setFormState((prev) => ({
            ...prev,
            campoSelecionado: null,
            grupoSelecionado: { ...prev.grupoSelecionado, campos: campos },
         }));
         resolve();
      });
   };   

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      inserirGrupo,
      inserirCampo,
      itemVazio,
      alterarGrupo,
      alterarCampo
   };
}
