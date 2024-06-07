using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace sicf_BusinessHandlers.BusinessHandlers.Compartido
{
    public class SendgridNotificaciones : ISendgridNotificaciones
    {
        private readonly IConfiguration Configuration;

        public SendgridNotificaciones(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<bool> enviarNotificacionCorreoElectronico()
        {
            bool response = true;
            await sendgridSendMail();

            return response;
        }

        public async Task<bool> EnviarCambioContrasena(string email, string temporalPass)
        {
            bool response = true;
            await RestaurarContrasena(email, temporalPass);

            return response;
        }

        public async Task<bool> EnviarContrasena(string email, string pass) {

            bool response = true;
            await EntregarContrasena(email, pass);

            return response;
        }





        private async Task sendgridSendMail()
        {
            var apiKey = Configuration["Sendgrid:apiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(Configuration["Sendgrid:senderMail"], "Remitente Comisaria");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("miguelegion@gmail.com", "Usuario Comisaria");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }

        private async Task RestaurarContrasena(string email, string temporalPass)
        {
            var apiKey = Configuration["Sendgrid:apiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(Configuration["Sendgrid:senderMail"], "Remitente Comisaria");
            var subject = "Recuperación de contraseña Sistema SICOFA";
            var to = new EmailAddress(email, "Recuperación de contraseña Sistema SICOFA");
            var plainTextContent = $"Buen día" +
                $"Le ha sido asignada la siguiente contraseña provisional {temporalPass}";
            
            var htmlContent = $"Buen día" +
                $"<br> Le ha sido asignada la siguiente contraseña provisional <strong>{temporalPass}</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }


        private async Task EntregarContrasena(string email, string pass)
        {
            var apiKey = Configuration["Sendgrid:apiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(Configuration["Sendgrid:senderMail"], "Remitente Comisaria");
            var subject = "Datos de Ingreso al Sistema SICOFA";
            var to = new EmailAddress(email, "Datos de Ingreso al Sistema SICOFA");
            var plainTextContent = $"Bienvenido al Sistema SICOFA" +
                $"Su usuario es {email}" +
                $"Su contraseña es {pass}" +
                $"Saludos ";
            var htmlContent = $"Bienvenido al Sistema SICOFA" +
                $"<br>Su usuario es {email}" +
                $"<br>Su contraseña es {pass}" +
                $"<br>Saludos ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
