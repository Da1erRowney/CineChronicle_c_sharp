using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;

    public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool enableSsl)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUsername = smtpUsername;
        _smtpPassword = smtpPassword;
        _enableSsl = enableSsl;
    }

    public string SendRecoveryEmail(string emailUser, string recoveryCode, bool isChange = false)
    {
        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_smtpUsername, "Cine Chronicle Support");
                message.To.Add(emailUser);
                if (!isChange)
                {
                    message.Subject = "Восстановление пароля в Cine Chronicle";
                }
                else
                {
                    message.Subject = "Изменение пользовательских данных в Cine Chronicle";
                }
                    message.IsBodyHtml = true;

                if (!isChange)
                {
                    // HTML-шаблон письма
                    message.Body = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            body {{
                font-family: 'Arial', sans-serif;
                background: linear-gradient(135deg, #1a1a2e, #16213e);
                color: #ffffff;
                padding: 20px;
                line-height: 1.6;
            }}
            .container {{
                max-width: 600px;
                margin: 0 auto;
                background: linear-gradient(135deg, #b6b6c0, #c51650);
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
            }}
            .header {{
                text-align: center;
                margin-bottom: 25px;
            }}
            .logo {{
                color: black;
                font-size: 28px;
                font-weight: bold;
                margin-bottom: 10px;
            }}
            .code {{
                background: #e50914;
                color: white;
                font-size: 24px;
                font-weight: bold;
                padding: 15px;
                text-align: center;
                border-radius: 5px;
                margin: 20px 0;
                letter-spacing: 3px;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 12px;
                color: #aaaaaa;
                text-align: center;
            }}
            a {{
                color: #e50914;
                text-decoration: none;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <div class='logo'>CINE CHRONICLE</div>
                <div>Ваш гид в мире кино</div>
            </div>
            
            <h2>Восстановление пароля</h2>
            <p>Мы получили запрос на восстановление пароля для вашей учетной записи.</p>
            <p>Используйте следующий код подтверждения:</p>
            
            <div class='code'>{recoveryCode}</div>
            
            <p>Этот код действителен в течение 15 минут. Если вы не запрашивали восстановление пароля, 
            пожалуйста, проигнорируйте это письмо или <a href='mailto:cine.chronicle.sup@gmail.com'>сообщите нам</a>.</p>
            
            <div class='footer'>
                © {DateTime.Now.Year} Cine Chronicle. Все права защищены.<br>
                Это письмо отправлено автоматически, пожалуйста, не отвечайте на него.
            </div>
        </div>
    </body>
    </html>";
                }
                else
                {
                    message.Body = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            body {{
                font-family: 'Arial', sans-serif;
                background: linear-gradient(135deg, #1a1a2e, #16213e);
                color: #ffffff;
                padding: 20px;
                line-height: 1.6;
            }}
            .container {{
                max-width: 600px;
                margin: 0 auto;
                background: linear-gradient(135deg, #b6b6c0, #c51650);
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
            }}
            .header {{
                text-align: center;
                margin-bottom: 25px;
            }}
            .logo {{
                color: black;
                font-size: 28px;
                font-weight: bold;
                margin-bottom: 10px;
            }}
            .code {{
                background: #e50914;
                color: white;
                font-size: 24px;
                font-weight: bold;
                padding: 15px;
                text-align: center;
                border-radius: 5px;
                margin: 20px 0;
                letter-spacing: 3px;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 12px;
                color: #aaaaaa;
                text-align: center;
            }}
            a {{
                color: #e50914;
                text-decoration: none;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <div class='logo'>CINE CHRONICLE</div>
                <div>Ваш гид в мире кино</div>
            </div>
            
            <h2>Изменение пользовательских данных</h2>
            <p>Мы получили запрос на подтверждение изменения пользовательских данных для вашей учетной записи.</p>
            <p>Используйте следующий код подтверждения:</p>
            
            <div class='code'>{recoveryCode}</div>
            
            <p>Этот код действителен в течение 15 минут. Если вы не совершали изменение данных, 
            срочно <a href='mailto:cine.chronicle.sup@gmail.com'>сообщите нам</a>.</p>
            
            <div class='footer'>
                © {DateTime.Now.Year} Cine Chronicle. Все права защищены.<br>
                Это письмо отправлено автоматически, пожалуйста, не отвечайте на него.
            </div>
        </div>
    </body>
    </html>";
                }
                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = _enableSsl;
                        client.Send(message);
                    }
            }

            return "Письмо с кодом восстановления успешно отправлено.";
        }
        catch (Exception ex)
        {
            return $"Ошибка при отправке письма";
            // Здесь можно добавить логирование ошибки
            throw; // или обработать ошибку по-другому
        }
    }
}