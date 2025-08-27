import { showInfo } from '../../../components/Messages';
import './../../../contents/css/login-logo.css';
import { showError } from '../../../components/Messages';
import api from '../../../utils/Api';

export default function RecoverPasswordController (view) {

   const recuperarSenha = () => {
      var result = new Promise((resolve, reject) => {
         let state = view.state;
         var input = {
            nomeDeUsuario: state.nomeDeUsuario,
            codigoDeSeguranca: state.codigoDeSeguranca,
            novaSenha: state.novaSenha,
            confirmacaoDaSenha: state.confirmacaoDaSenha,
            identificacaoComplementar: state.identificacaoComplementar,
            tipoDeAcesso: view.props.tipoDeAcesso,
         };
         api
            .protectedPost('/login/resetpassword', input)
            .then((result) => {
               if (result) {
                  showInfo('Senha alterada com sucesso').then(
                     () =>
                        (window.location =
                           './#' + (view.props.tipoDeAcesso === 'EMPRESA' ? '/' : '/adm'))
                  );
               }
               resolve();
            })
            .catch(reject);
      });
      return result;
   };

   const  enviarCodigoDeSeguranca = () => {
      var result = new Promise((resolve, reject) => {
         let tipoDeAcesso = view.props.tipoDeAcesso;
         let identificacaoComplementar = view.state.identificacaoComplementar;
         let nomeDeUsuario = view.state.nomeDeUsuario;

         if (tipoDeAcesso === 'EMPRESA' && !identificacaoComplementar) {
            showError('Informe a identificação da empresa');
            reject();
            return;
         }

         if (!nomeDeUsuario) {
            showError('Informe o nome de usuário');
            reject();
            return;
         }

         let input = {
            nomeDeUsuario: nomeDeUsuario,
            identificacaoComplementar: identificacaoComplementar,
            tipoDeAcesso: tipoDeAcesso,
            recaptcha: view.state.recaptcha
         };
         api
            .post('/login/recoverpassword', input)
            .then(() => {
               view.setState({ codigoDeSegurancaEnviado: true });
               showInfo('Código enviado para o e-mail');
               resolve();
            })
            .catch(reject);
      });
      return result;
   };

   return {
      recuperarSenha,
      enviarCodigoDeSeguranca,
   };
}
