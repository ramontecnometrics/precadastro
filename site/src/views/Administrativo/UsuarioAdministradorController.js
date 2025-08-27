import { showError } from '../../components/Messages';
import { rsaEncrypt, concatUint8Arrays, uint8ToBase64, base64ToUint8 } from '../../utils/Functions';
import api from '../../utils/Api';

export default function UsuarioAdministradorController() {
   
   const itemVazio = {
      situacao: { id: 1 },
      perfis: [],
      nome: 'Ramon Pacheco',
      email: 'ramon@tecnometrics.com.br',
      nomeDeUsuario: 'Ramon'
   };

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'id', className: 'codigo' },
         { titulo: 'Nome', width: '40%', orderby: 'nome' },
         { titulo: 'Login', width: '30%', orderby: 'nomeDeUsuario' },
         { titulo: 'Email', width: '30%', orderby: 'email' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [item.id, item.nome, item.nomeDeUsuario, item.email];
   };

   const getObjetoDeDados = async (formState, setFormState) => {
      try {
         const item = formState.itemSelecionado;

         // --- validações (mantidas) ---
         if (!item.nome) {
            showError('Informe o nome completo.');
            return Promise.reject();
         }

         if (!item.perfis) {
            showError('Informe pelo menos um perfil.');
            return Promise.reject();
         }

         if (item.perfis.length === 0) {
            showError('Informe pelo menos um perfil.');
            return Promise.reject();
         }

         // --- chave pública (PEM) ---
         const publicKeyResp = await api.get('/publickey');
         const publicKeyPem =
            typeof publicKeyResp === 'string' ? publicKeyResp : publicKeyResp?.key || publicKeyResp?.pem || '';

         let novaSenha = null;

         // --- criptografia da senha, se houver ---
         if (item.senha) {
            const BLOCK_LEN = 64; // igual ao seu código original
            const blocks = item.senha.match(new RegExp(`.{1,${BLOCK_LEN}}`, 'g')) || [];

            // cifra cada bloco -> retorna BASE64 por bloco
            const b64Chunks = await Promise.all(blocks.map((b) => rsaEncrypt(b, publicKeyPem)));

            // converte base64 -> bytes, concatena, e volta para base64 único
            const u8Chunks = b64Chunks.map(base64ToUint8);
            const concatenado = concatUint8Arrays(u8Chunks);
            novaSenha = uint8ToBase64(concatenado);
         }

         // --- objeto final ---
         const input = {
            nome: item.nome,
            email: item.email,
            dataDeCadastro: item.dataDeCadastro ? item.dataDeCadastro : new Date(),
            foto: item.foto && item.foto.id ? item.foto : null,
            situacao: item.situacao ? item.situacao.id : 0,
            senhaAlterada: item.senhaAlterada,
            novaSenha,
            nomeDeUsuario: item.nomeDeUsuario,
            perfis: item.perfis,
            enviarNovaSenhaPorEmail: item.enviarNovaSenhaPorEmail,
            certificado: item.certificado,
         };

         if (formState.alterando) {
            input.id = parseInt(item.id);
            input.nomeDeUsuario = null;
         }

         return input; // async -> Promise resolvida com o objeto
      } catch (e) {
         console.error(e);
         return Promise.reject(e);
      }
   };

   const inserirPerfil = (formState, setFormState) => {
      return new Promise((resolve, reject) => {
         let perfilSelecionado = formState.perfilSelecionado;

         if (!perfilSelecionado) {
            showError('Selecione um perfil de usuário');
            reject();
            return;
         }

         if (!formState.itemSelecionado.perfis) {
            formState.itemSelecionado.perfis = [];
         }

         let duplicado = formState.itemSelecionado.perfis.some((i) => i.perfil.id === perfilSelecionado.id);

         if (duplicado) {
            showError('Perfil já cadastrado');
            reject();
            return;
         }

         setFormState({
            itemSelecionado: {
               ...formState.itemSelecionado,
               perfis: [...formState.itemSelecionado.perfis, { perfil: perfilSelecionado }],
            },
            perfilSelecionado: null,
         }).then(resolve);
      });
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      inserirPerfil,
      itemVazio,
   };
}
