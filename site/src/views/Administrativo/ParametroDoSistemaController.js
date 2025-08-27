import { FormGroup } from 'react-bootstrap';
import Label from '../../components/Label';
import { showError } from '../../components/Messages';
import Select from '../../components/Select';
import { buildQueryString } from '../../utils/Functions';
import PerfilDeUsuarioView from './PerfilDeUsuarioView';
import TermoDeUsoView from './TermoDeUsoView';
import TextArea from '../../components/TextArea';

 
export default function ParametroDoSistemaController(view) {
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
            <Label>Selecione</Label>
            <TextArea defaultValue={defaultValue} onChange={(value) => setValor(value)} />
         </FormGroup>
      );
   };

   const getDescricaoDoValor = (item) => {
      var camposObjeto = ['PerfilDeUsuarioParaEmpresa', 'TermoDeUsoAtivo'];
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

   const getObjetoDeDados = () => {
      return new Promise((resolve, reject) => {
         let item = view.state.itemSelecionado;

         if (!item.nome) {
            showError('Selecione o parâmetro');
            reject();
            return;
         }

         var input = {
            grupo: item.grupo,
            ordem: item.ordem ? item.ordem : 0,
            nome: item.nome,
            descricao: item.descricao,
            valor: item.valor,
            protegido: item.protegido,
         };
         if (view.state.alterando) {
            input.id = parseInt(item.id);
         }
         resolve(input);
      });
   };

   const comboParametros = () => {
      let result = [
         // ====== Envio de E-mail ======
         { nome: 'ServidorSmtp', descricao: "Servidor SMTP", grupo: 1, ordem: 1 },
         { nome: 'PortaSmtp', descricao: "Porta SMTP", grupo: 1, ordem: 2 },
         {
            nome: 'UsuarioDoServidorSmtp',
            descricao: "Usuário de acesso ao servidor SMTP",
            grupo: 1,
            ordem: 3,
            ajuda: "Na maioria dos casos é o mesmo e-mail remetente."
         },
         { nome: 'EmailRemetente', descricao: "E-mail remetente", grupo: 1, ordem: 4 },
         {
            nome: 'SenhaDoServidorSmtp',
            descricao: "Senha remetente",
            grupo: 1,
            ordem: 5,
            protegido: true
         },
         { nome: 'NomeDoRemetente', descricao: "Nome do remetente", grupo: 1, ordem: 6 },
         {
            nome: 'UsarSslNoServidorSmtp',
            descricao: "Usar SSL",
            grupo: 1,
            ordem: 7,
            componente: (defaultValue, setValor) => (
               <FormGroup>
                  <Label>Selecione</Label>
                  <Select
                     name={'UsarSslNoServidorSmtp'}
                     defaultValue={defaultValue}
                     getKeyValue={(i) => i.id}
                     getDescription={(i) => i.nome}
                     options={[
                        { id: 'Não', nome: 'Não' },
                        { id: 'Sim', nome: 'Sim' }
                     ]}
                     onSelect={(i) => setValor(i ? i.id : 'Não')}
                     allowEmpty={false}
                  />
               </FormGroup>
            )
         },
   
         // ====== Objetos ======
         {
            nome: 'PerfilDeUsuarioParaEmpresa',
            descricao: "Perfil de usuário para empresas",
            grupo: 2,
            ordem: 1,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Label>Selecione</Label>
                     <Select
                        name={'perfil'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => (
                           <PerfilDeUsuarioView 
                              select={select}
                              filtroExtra={() => ({ tipoDePerfil: 2 })}
                           />
                        )}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/perfildeusuario/fast' +
                           buildQueryString(2, null, 'id', { Searchable: text, tipoDePerfil: 3 })
                        }
                     />
                  </FormGroup>
               );
            }
         },
         {
            nome: 'TermoDeUsoAtivo',
            descricao: "Termo de uso ativo",
            grupo: 2,
            ordem: 2,
            componente: (defaultValue, setValor) => {
               defaultValue = defaultValue ? JSON.parse(defaultValue) : null;
               return (
                  <FormGroup>
                     <Label>Selecione</Label>
                     <Select
                        name={'termoDeUso'}
                        defaultValue={defaultValue}
                        getKeyValue={(i) => i.id}
                        getDescription={(i) => i.nome}
                        onSelect={(i) => setValor(i ? JSON.stringify({ id: i.id, nome: i.nome }) : null)}
                        formularioPadrao={(select) => (
                           <TermoDeUsoView select={select} />
                        )}
                        noDropDown={true}
                        readOnlyColor='#ffff'
                        getFilterUrl={(text) =>
                           '/termodeuso' +
                           buildQueryString(2, null, 'id', { Searchable: text })
                        }
                     />
                  </FormGroup>
               );
            }
         },
   
         // ====== Gerais ======
         { nome: 'UrlPublica', descricao: "Url pública do site", grupo: 3, ordem: 1 },
   
         // ====== Infobip ======
         { nome: 'InfobipApiUrl', descricao: "URL da API Infobip", grupo: 4, ordem: 1 },
         { nome: 'InfobipApiKey', descricao: "Chave da API Infobip", grupo: 4, ordem: 2, protegido: true },
         { nome: 'InfobipApiRemetenteWhatsApp', descricao: "Remetente WhatsApp da API Infobip", grupo: 4, ordem: 3 },
   
         // ====== Notificações ======
         {
            nome: 'TextoParaNotificacaoDeMudancaDeTemperaturaViaEmail',
            descricao: "Texto para notificação de mudança de temperatura via E-mail",
            grupo: 5,
            ordem: 1,
            componente: componenteTextArea,
            ajuda:
               "Variáveis que você pode usar na mensagem:\n\n%Data%\n%DataPorExtenso%\n%DataEHora%\n%DataEHoraPorExtenso%\n%NomeDoEquipamento%\n%NomeDoPredio%\n%NomeDoLocal%\n%MacDaTag%\n%NumeroDeSerieDoMonitorado%\n%NumeroDePatrimonioDoMonitorado%\n%Andar%\n%Temperatura%"
         },
         {
            nome: 'TextoParaNotificacaoDeUmidadeViaEmail',
            descricao: "Texto para notificação de mudança de umidade via E-mail",
            grupo: 5,
            ordem: 2,
            componente: componenteTextArea,
            ajuda:
               "Variáveis que você pode usar na mensagem:\n\n%Data%\n%DataPorExtenso%\n%DataEHora%\n%DataEHoraPorExtenso%\n%NomeDoEquipamento%\n%NomeDoPredio%\n%NomeDoLocal%\n%MacDaTag%\n%NumeroDeSerieDoMonitorado%\n%NumeroDePatrimonioDoMonitorado%\n%Andar%\n%Umidade%"
         },
         {
            nome: 'TextoParaNotificacaoDeBateriaFracaViaEmail',
            descricao: "Texto para notificação de bateria fraca via E-mail",
            grupo: 5,
            ordem: 3,
            componente: componenteTextArea,
            ajuda:
               "Variáveis que você pode usar na mensagem:\n\n%Data%\n%DataPorExtenso%\n%DataEHora%\n%DataEHoraPorExtenso%\n%NomeDoEquipamento%\n%NomeDoPredio%\n%NomeDoLocal%\n%MacDaTag%\n%NumeroDeSerieDoMonitorado%\n%NumeroDePatrimonioDoMonitorado%\n%Andar%\n%Bateria%"
         },
         {
            nome: 'TextoParaNotificacaoDeGatewayOfflineViaEmail',
            descricao: "Texto para notificação de gateway offline via E-mail",
            grupo: 5,
            ordem: 4,
            componente: componenteTextArea,
            ajuda:
               "Variáveis que você pode usar na mensagem:\n\n%MacDoGateway%\n%NumeroDePatrimonioDoGateway%\n%NomeDoPredio%\n%NomeDoLocal%\n%Andar%"
         }
         // ... segue o mesmo padrão pros outros campos de SMS, WhatsApp e Voz
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
