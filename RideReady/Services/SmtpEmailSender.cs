using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using RepositoryLibrary.Features.Email.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Users.Entities;
using System.Net;
using System.Net.Mail;

namespace RideReady.Services;

public class SmtpEmailSender : IEmailSender<EMUser>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task SendConfirmationLinkAsync(EMUser user, string email, string confirmationLink)
    {
        return SendEmailAsync(
            email,
            "Confirma o teu email — RideReady",
            $"<p>Olá {user.FirstName},</p><p>Por favor confirma o teu email clicando no link abaixo:</p><p><a href='{confirmationLink}'>Confirmar email</a></p>"
        );
    }

    public Task SendPasswordResetLinkAsync(EMUser user, string email, string resetLink)
    {
        return SendEmailAsync(
            email,
            "Recuperar password — RideReady",
            $"<p>Olá {user.FirstName},</p><p>Para recuperar a tua password, clica no link abaixo:</p><p><a href='{resetLink}'>Recuperar password</a></p>"
        );
    }

    public Task SendPasswordResetCodeAsync(EMUser user, string email, string resetCode)
    {
        return SendEmailAsync(
            email,
            "Código de recuperação — RideReady",
            $"<p>Olá {user.FirstName},</p><p>O teu código de recuperação é: <strong>{resetCode}</strong></p>"
        );
    }



    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var host = _configuration["Smtp:Host"] ?? "localhost";
        var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
        var fromEmail = _configuration["Smtp:FromEmail"] ?? "noreply@local";
        var fromName = _configuration["Smtp:FromName"] ?? "RideReady";

        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "false");
        _logger.LogInformation(
            "SMTP Config - Host={Host}, Port={Port}, User={User}, SSL={SSL}",
            host,
            port,
            username,
            enableSsl);

        using var client = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        _logger.LogWarning("ENABLE_SSL VALUE = {EnableSsl}",
    _configuration["Smtp:EnableSsl"]);
        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SMTP Error. Host={Host}, Port={Port}, User={User}",
                host,
                port,
                username);

            throw;
        }
    }
}