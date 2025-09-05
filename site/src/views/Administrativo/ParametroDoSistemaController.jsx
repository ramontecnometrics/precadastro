import { FormGroup } from 'react-bootstrap';
import Label from '../../components/Label';
import { showError } from '../../components/Messages';
import Select from '../../components/Select';
import { buildQueryString } from '../../utils/Functions';
import PerfilDeUsuarioView from './PerfilDeUsuarioView';
import TermoDeUsoView from './TermoDeUsoView';
import TextArea from '../../components/TextArea';
import FormularioView from './FormularioView';

export default function ParametroDoSistemaController() {
   const getTitulosDaTabela = () => {
      return [
         { titulo: 'Código', className: 'codigo' },
         { titulo: 'Descrição', width: '50%', orderby: 'Descricao' },
         { titulo: 'Valor', width: '50%', orderby: 'Valor' },
      ];
   };

   const getDadosDaTabela = (item) => {
      let parametro = comboParametros().filter((i) => i.nome === item.nome)[0];
      return parametro ? [item.id, parametro.descricao, getDescricaoDoValor(item)] : null;
   };

   const componenteTextArea = (defaultValue, setValor) => {
      return (
         <FormGroup>
            {/* <Label>Selecione</Label> */}
            <TextArea defaultValue={defaultValue} onChange={(value) => setValor(value)} />
         </FormGroup>
      );
   };

   const getDescricaoDoValor = (item) => {
      var camposObjeto = [
         'PerfilDeUsuarioParaEmpresa',
         'TermoDeUsoAtivo',
         'FichaDeAvaliacaoClinicaParaGeneroMasculino',
         'FichaDeAvaliacaoClinicaParaGeneroFeminino',
         'FichaDeAnamneseParaGeneroMasculino',
         'FichaDeAnamneseParaGeneroFeminino',
      ];
      var descricao = null;
      const getDescricaoParaObjeto = () => {
         var result = null;
         if (item.preenchido) {  
            result = JSON.parse(item.valor);
            result = result.id + ' - ' + result.nome;
         }
         return result;
      };

      const getDescricao = (valor) => {
         var result = null;
         if (valor === 'Sim') {
            result = 'Sim';
         } else if (valor === 'Não') {
            result = 'Não';
         } else {
            result = valor;
         }
         return result;
      };

      if (camposObjeto.filter((i) => i === item.nome).length > 0) {
         descricao = getDescricaoParaObjeto();
      } else {
         descricao = item.protegido && item.preenchido ? 'Protegido' : getDescricao(item.valor);
      }
      return descricao;
   };

   const getObjetoDeDados = async (formState, setFormState) => {
      console.log('getObjetoDeDados');
      try {
         let item = formState.itemSelecionado;

         if (!item.nome) {
            showError('Selecione o parâmetro.');
            return Promise.reject();
         }

         var input = {
            grupo: item.grupo,
            ordem: item.ordem ? item.ordem : 0,
            nome: item.nome,
            descricao: item.descricao,
            valor: item.valor,
            protegido: item.protegido,
         };

         if (formState.alterando) {
            input.id = parseInt(item.id);
         }

         console.log(input);

         return input;
      } catch (e) {
         showError(e.toString());
         console.error(e);
         return Promise.reject(e);
      }
   };

   const grupos = new Map([
      [1, 'Envio de E-mail'],
      [2, 'Termo de uso'],
      [3, 'Gerais'],
      [4, 'Infobip'],
      [5, 'Uno'],
      [6, 'Fichas de avaliação clínica'],
      [7, 'Fichas de anamnese'],
   ]);

   const comboParametros = () => {
      let result = [
         // ====== Envio de E-mail ======
         { nome: 'ServidorSmtp', descricao: grupos.get(1) + ' -> Servidor SMTP', grupo: 1, ordem: 1 },
         { nome: 'PortaSmtp', descricao: grupos.get(1) + ' -> Porta SMTP', grupo: 1, ordem: 2 },
         {
            nome: 'UsuarioDoServidorSmtp',
            descricao: grupos.get(1) + ' -> Usuário de acesso ao servidor SMTP',
            grupo: 1,
            ordem: 3,
            ajuda: 'Na maioria dos casos é o mesmo e-mail remetente.',
         },
         { nome: 'EmailRemetente', descricao: grupos.get(1) + ' -> E-mail remetente', grupo: 1, ordem: 4 },
         {
            nome: 'SenhaDoServidorSmtp',
            descricao: grupos.get(1) + ' -> Senha remetente',
            grupo: 1,
            ordem: 5,
            protegido: true,
         },
         { nome: 'NomeDoRemetente', descricao: grupos.get(1) + ' -> Nome do remetente', grupo: 1, ordem: 6 },
         {
            nome: 'UsarSslNoServidorSmtp',
            descricao: grupos.get(1) + ' -> Usar SSL',
            grupo: 1,
            ordem: 7,
            componente: (defaultValue, setValor) => (
               <FormGroup>
                  {/* <Label>Selecione</Label> */}
                  <Select
                     name={'UsarSslNoServidorSmtp'}
                     defaultValue={defaultValue}
                     getKeyValue={(i) => i.id}
                     getDescription={(i) => i.nome}
                     options={[
                        { id: 'Não', nome: 'Não' },
                        { id: 'Sim', nome: 'Sim' },
                     ]}
                     onSelect={(i) => setValor(i ? i.id : 'Não')}
                     allowEmpty={false}
                  />
               </FormGroup>
            ),
         },

         // ====== Objetos ======

         {
            nome: 'TermoDeUsoAtivo',
            descricao: grupos.get(2) + ' -> Termo de uso ativo',
            grupo: 2,
            ordem: 2,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     {/* <Label>Selecione</Label> */}
                     <Select
                        name={'termoDeUso'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => <TermoDeUsoView select={select} />}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) => '/termodeuso' + buildQueryString(2, null, 'id', { Searchable: text })}
                     />
                  </FormGroup>
               );
            },
         },

         // ====== Uno ======
         { nome: 'UrlDaApiDoUno', descricao: grupos.get(5) + ' -> URL base da Api do Uno', grupo: 5, ordem: 1 },
         {
            nome: 'CodigoDoColaboradorParaIntegracaoComOUno',
            descricao: grupos.get(5) + ' -> Código do colaborador para integração com o Uno',
            grupo: 5,
            ordem: 2,
         },

         // ====== Fichas de avaliação clínica ======
         {
            nome: 'FichaDeAvaliacaoClinicaParaGeneroMasculino',
            descricao: grupos.get(6) + ' -> Ficha de avaliação clínica para gênero masculino',
            grupo: 6,
            ordem: 1,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Select
                        name={'FichaDeAvaliacaoClinicaParaGeneroMasculino'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => <FormularioView select={select} />}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/formulario/fast' + buildQueryString(2, null, 'id', { Searchable: text })
                        }
                     />
                  </FormGroup>
               );
            },
         },

         {
            nome: 'FichaDeAvaliacaoClinicaParaGeneroFeminino',
            descricao: grupos.get(6) + ' -> Ficha de avaliação clínica para gênero feminino',
            grupo: 6,
            ordem: 2,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Select
                        name={'FichaDeAvaliacaoClinicaParaGeneroFeminino'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => <FormularioView select={select} />}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/formulario/fast' + buildQueryString(2, null, 'id', { Searchable: text })
                        }
                     />
                  </FormGroup>
               );
            },
         },

          // ====== Fichas de Anamnese ======
         {
            nome: 'FichaDeAnamneseParaGeneroMasculino',
            descricao: grupos.get(7) + ' -> Ficha de anamnese para gênero masculino',
            grupo: 7,
            ordem: 1,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Select
                        name={'FichaDeAnamneseParaGeneroMasculino'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => <FormularioView select={select} />}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/formulario/fast' + buildQueryString(2, null, 'id', { Searchable: text })
                        }
                     />
                  </FormGroup>
               );
            },
         },

         {
            nome: 'FichaDeAnamneseParaGeneroFeminino',
            descricao: grupos.get(7) + ' -> Ficha de anamnese para gênero femninino',
            grupo: 7,
            ordem: 1,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Select
                        name={'FichaDeAnamneseParaGeneroFeminino'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => <FormularioView select={select} />}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/formulario/fast' + buildQueryString(2, null, 'id', { Searchable: text })
                        }
                     />
                  </FormGroup>
               );
            },
         },
      ];

      return result;
   };

   return {
      getTitulosDaTabela,
      getDadosDaTabela,
      getObjetoDeDados,
      comboParametros,
   };
}
