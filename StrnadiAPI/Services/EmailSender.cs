using System.Net;
using System.Net.Mail;

namespace StrnadiAPI.Services;

public interface IEmailSender
{
    void SendMessage(string emailAddress, string subject, string body);
}

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    
    private string _smtpServerDomain => _configuration[$"Verification:Domain"]!;
    
    private string _smtpUsername => _configuration[$"Verification:Username"]!;
    
    private string _smtpPassword => _configuration[$"Verification:Password"]!;
    
    private int _smtpPort => int.Parse(_configuration[$"Verification:Port"]!);
    
    private string _smtpEmail => _configuration[$"Verification:Email"]!;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMessage(string emailAddress, string subject, string body)
    {
        SmtpClient smtpClient = new(_smtpServerDomain)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpEmail, _smtpPassword),
            EnableSsl = true
        };
        
        MailMessage message = new()
        {
            From = new MailAddress(_smtpEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        
        message.To.Add(emailAddress);
        smtpClient.Send(message);
    }
    
    public void SendVerificationMessage(HttpContext httpContext, string emailAddress, int userId)
    {
        string link = new StrnadiLinkGenerator().GenerateLink(httpContext, userId);
        
        SendMessage(
            emailAddress,
            "Confirm your email in Navrat krale - Nareci ceskych strnadu",
            "Please confirm you email by clicking this link: " + link
            );
    }
}