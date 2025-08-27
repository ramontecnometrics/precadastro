using framework;
using framework.Extensions;
using model.Repositories;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace model
{
    public class EnvioDeEmail
    {
        public EnvioDeEmail()
        {

        }

        public virtual void Enviar(ParametroDoSistemaRepository parametrosRepository,
            string assunto, string destinatario, string mensagem)
        {
            if (string.IsNullOrEmpty(destinatario))
            {
                return;
            }

            var servidor = parametrosRepository.GetString("ServidorSmtp");
            var usuario = parametrosRepository.GetString("UsuarioDoServidorSmtp");
            var senha = parametrosRepository.GetString("SenhaDoServidorSmtp");
            var porta = parametrosRepository.GetInt("PortaSmtp");
            var emailRemetente = parametrosRepository.GetString("EmailRemetente");
            var nomeDoRemetente = parametrosRepository.GetString("NomeDoRemetente");
            var usarSsl = parametrosRepository.GetBoolean("UsarSslNoServidorSmtp");

            Enviar(
                assunto,
                destinatario,
                mensagem,
                servidor,
                usuario,
                senha,
                porta,
                emailRemetente,
                nomeDoRemetente,
                usarSsl
            );
        }         

        public virtual void Enviar(
            string assunto,
            string destinatario,
            string mensagem,
            string Servidor,
            string Usuario,
            string Senha,
            int? Porta,
            string EmailRemetente,
            string NomeDoRemetente,
            bool UsarSsl
        )
        {
            if (string.IsNullOrEmpty(destinatario))
            {
                return;
            }

            var client = new SmtpClient(Servidor, Porta.Value)
            {
                UseDefaultCredentials = false,
                EnableSsl = UsarSsl,
                Credentials = new System.Net.NetworkCredential(!string.IsNullOrEmpty(Usuario) ? Usuario : EmailRemetente, Senha.Trim()),
                Timeout = 20000
            };

            var from = new MailAddress(
                EmailRemetente,
                NomeDoRemetente,
                System.Text.Encoding.UTF8
            );

            destinatario.Split(";").ForEach(toEmail =>
            {
                var to = new MailAddress(toEmail.Replace(";", ""));

                using var message = new MailMessage(from, to)
                {
                    IsBodyHtml = true,
                    Body = mensagem,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    Subject = assunto,
                    SubjectEncoding = System.Text.Encoding.UTF8
                };
                client.Send(message);
            });
        }

        public void EnviarAsync(ParametroDoSistemaRepository parametrosRepository, 
            string assunto, string destinatario, string mensagem)
        {
            Task.Run(() =>
            {
                this.Enviar(parametrosRepository, assunto, destinatario, mensagem);
            });
        }

        public void EnviarEmailDeNovaSenha(ParametroDoSistemaRepository parametrosRepository, 
            string destinatario, string novaSenha)
        {
            try
            {
                this.Enviar(parametrosRepository, "Nova senha", destinatario,
                    FormattedString.Build(
                    "Senha gerada para acesso ao sistema.",
                    "<br/><br/>",
                    "<b>", novaSenha, "</b>"));
            }
            catch (Exception e)
            {
                throw new Exception($"Não foi possível enviar o e-mail com nova senha.\n\n{e.Message}");
            }
        }
    }
}
