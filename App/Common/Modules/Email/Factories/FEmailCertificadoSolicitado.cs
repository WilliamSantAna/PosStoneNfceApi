using System.Collections.Generic;

namespace PosStoneNfce.Common.Modules.Email
{
    public class FEmailCertificadoSolicitado : IEmailFactory
    {

        private EmailTemplateMessage _email;


        public FEmailCertificadoSolicitado(
            string[] destinatarios,
            string codigo
        )
        {
            var assunto = "Recuperação de Senha";

            var descricao = @"Bem vindo <br><br>
                            O Certificado será emitido no mesmo dia em que a videoconferência for realizada. <br><br>
                            Em caso de dúvidas, ligue ou chame no WhatsApp: <br> (48) 9 9853-6191 <br><br>
                            Para agendar a sua videoconferência clique no link abaixo <br><br>
                            Att Equipe Linx.";

            _email = new EmailTemplateMessage
            {
                Template = "email-card-with-button",
                Identifier = "(FEmailCertificadoSolicitado): " + assunto,
                Subject = string.Concat("[Pos Stone Nfce] - ", assunto),
                To = destinatarios,
                DataBind = new Dictionary<string, dynamic>()
                        {
                            { "Title", assunto },
                            { "Description", descricao },
                            { "ButtonLink", "http://xxxxxxxxx.com.br?code=" + codigo },
                            { "ButtonLabel", "Agendar videoconferência" },
                        }
            };
        }

        public EmailTemplateMessage GetEmail()
        {
            return _email;
        }
    }
}