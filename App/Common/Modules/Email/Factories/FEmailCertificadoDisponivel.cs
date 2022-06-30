using System.Collections.Generic;

namespace PosStoneNfce.Common.Modules.Email
{
    public class FEmailCertificadoDisponivel : IEmailFactory
    {

        private EmailTemplateMessage _email;


        public FEmailCertificadoDisponivel(
            string[] destinatarios
        )
        {
            var assunto = "Certificado Disponível";

            var descricao = @"Olá! <br><br>
                            O seu certificado está pronto e disponível para download. Clique no botão (Completar emissão) abaixo para completar a emissão do seu certificado. <br><br>
                            Em caso de dúvidas, ligue ou chame no WhatsApp: <br> (48) 9 9853-6191 <br><br>
                            Att Equipe Linx.";

            _email = new EmailTemplateMessage
            {
                Template = "email-card-with-button",
                Identifier = "(FEmailCertificadoDisponivel): " + assunto,
                Subject = string.Concat("[Pos Stone Nfce] - ", assunto),
                To = destinatarios,
                DataBind = new Dictionary<string, dynamic>()
                        {
                            { "Title", assunto },
                            { "Description", descricao },
                            { "ButtonLink", "http://xxxxxxxxx.com.br" },
                            { "ButtonLabel", "Completar emissão" },
                        }
            };
        }

        public EmailTemplateMessage GetEmail()
        {
            return _email;
        }
    }
}
