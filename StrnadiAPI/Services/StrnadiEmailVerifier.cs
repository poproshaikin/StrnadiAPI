using System.Net;
using System.Net.Mail;

namespace StrnadiAPI.Services;

public interface IStrnadiEmailVerifier
{
    public void SendLink(string email);
}

public class StrnadiEmailVerifier : IStrnadiEmailVerifier
{
    private readonly IConfiguration _configuration;

    private const string verification_section_name = "Verification";
    
    private string _smtpServerDomain => _configuration[$"{verification_section_name}:Domain"]!;
    private string _smtpUsername => _configuration[$"{verification_section_name}:Username"]!;
    private string _smtpPassword => _configuration[$"{verification_section_name}:Password"]!;
    private int _smtpPort => int.Parse(_configuration[$"{verification_section_name}:Port"]!);
    private string _smtpEmail => _configuration[$"{verification_section_name}:Email"]!;
    private string _emailSubject => _configuration[$"{verification_section_name}:Subject"]!;
    private string _emailBody => _configuration[$"{verification_section_name}:Body"]!;

    public StrnadiEmailVerifier(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void SendLink(string email, string link)
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
            Subject = _emailSubject,
            Body = $"{_emailBody}{link}",
            IsBodyHtml = true
        };
        
        message.To.Add(email);
        
        smtpClient.Send(message);
    }
}