import { showError } from '../../components/Messages';
import { dateToString } from '../../utils/Functions';

export default function LeadController() {
   const itemVazio = {
      situacao: { id: 1, descricao: 'Ativo' },
   };

   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', orderby: 'id', className: 'codigo' },
         { titulo: 'Nome', width: '30%', orderby: 'nomeCompleto' },
         { titulo: 'Unidade', width: '15%', orderby: 'unidade' },
         { titulo: 'Email', width: '20%', orderby: 'email' },
         { titulo: 'Celular', width: '15%', orderby: 'celular' },
         { titulo: 'Data de cadastro', width: '15%', orderby: 'dataDeCadastro' },
         { titulo: 'Situação', width: '10%', orderby: 'situacao' },
      ];
   };

   const getDadosDaTabela = (item) => {
      return [
         item.id,
         item.nomeCompleto,
         item.nomeDaUnidade,
         item.email,
         item.celular,
         dateToString(item.dataDeCadastro),
         item.situacao?.descricao,
      ];
   };

   const getObjetoDeDados = async (formState, setFormState) => {
      try {
         const item = formState.itemSelecionado;

         // --- validações ---
         if (!item.nomeCompleto) {
            showError('Informe o nome.');
            return Promise.reject();
         }

         if (!item.dataDeNascimento) {
            showError('Informe a data de nascimento.');
            return Promise.reject();
         }

         if (!item.cpf) {
            showError('Informe o CPF.');
            return Promise.reject();
         }

         // if (!item.documentoDeIdentidade) {
         //    showError('Informe o RG.');
         //    return Promise.reject();
         // }

         // if (!item.cnh) {
         //    showError('Informe a CNH.');
         //    return Promise.reject();
         // }

         // if (!item.estadoCivil) {
         //    showError('Informe o estado civil.');
         //    return Promise.reject();
         // }

         if (!item.sexo) {
            showError('Informe o sexo.');
            return Promise.reject();
         }

         // if (!item.email) {
         //    showError('Informe o email.');
         //    return Promise.reject();
         // }

         // if (!item.telefone || !item.telefone.numero) {
         //    showError('Informe o telefone.');
         //    return Promise.reject();
         // }

         if (!item.celular || !item.celular.numero) {
            showError('Informe o celular.');
            return Promise.reject();
         }         


         // --- objeto final ---
         const input = {
            nomeCompleto: item.nomeCompleto,
            cpf: item.cpf,
            documentoDeIdentidade: item.documentoDeIdentidade,
            email: item.email,
            cnh: item.cnh,
            observacao: item.observacao,
            alertaDeSaude: item.alertaDeSaude,
            estadoCivil: item.estadoCivil,
            situacao: item.situacao,
            dataDeNascimento: item.dataDeNascimento,
            sexo: item.sexo,
            telefone: item.telefone && item.telefone.numero
               ? {
                    ddd: item.telefone.ddd,
                    numero: item.telefone.numero,                   
                    tipo: { id: 2, descricao: 'Residencial' },
                 }
               : null,
            celular: item.celular && item.celular.numero
               ? {
                    ddd: item.celular.ddd,
                    numero: item.celular.numero,
                    tipo: { id: 1, descricao: 'Celular' },
                 }
               : null,
            endereco: item.endereco
               ? {
                    tipo: item.endereco.tipo,
                    endereco: {
                       logradouro: item.endereco.endereco?.logradouro,
                       numero: item.endereco.endereco?.numero,
                       complemento: item.endereco.endereco?.complemento,
                       bairro: item.endereco.endereco?.bairro,
                       cep: item.endereco.endereco?.cep,
                       cidade: item.endereco.endereco?.cidade,
                    },
                 }
               : null,
            profissao: item.profissao,
            foto: item.foto && item.foto.id ? { id: item.foto.id } : null,
         };

         if (formState.alterando) {
            input.id = parseInt(item.id);
         }

         return input;
      } catch (e) {
         console.error(e);
         return Promise.reject(e);
      }
   };

   const aposInserir = async (formState, setFormState) => {
      return new Promise((resolve, reject) => {      
         formState.itemSelecionado.dataDeCadastro = new Date();         
         setFormState((prev) => ({ ...prev, itemSelecionado: formState.itemSelecionado }));
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
