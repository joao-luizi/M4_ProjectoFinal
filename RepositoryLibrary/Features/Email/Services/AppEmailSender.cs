using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Email.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using System.Net;
using System.Net.Mail;

namespace RepositoryLibrary.Features.Email.Services
{
    public class AppEmailSender : IEmailSender<EMUser>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppEmailSender> _logger;

        public AppEmailSender(IConfiguration configuration, ILogger<AppEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // =========================
        // Identity required methods
        // =========================

        public Task SendConfirmationLinkAsync(EMUser user, string email, string confirmationLink)
            => SendEmailAsync(email, "Confirm email", $"<a href='{confirmationLink}'>Confirm</a>");

        public Task SendPasswordResetLinkAsync(EMUser user, string email, string resetLink)
            => SendEmailAsync(email, "Reset password", $"<a href='{resetLink}'>Reset</a>");

        public Task SendPasswordResetCodeAsync(EMUser user, string email, string resetCode)
            => SendEmailAsync(email, "Reset code", resetCode);

        // =========================
        // YOUR CUSTOM METHOD
        // =========================

        public async Task SendLessonCancelledAsync(EmailSenderDto dto)
        {
            var tasks = dto.UserEmails.Select(email =>
                SendEmailAsync(email, dto.Subject, dto.Body));

            await Task.WhenAll(tasks);
        }

        // =========================
        // Shared SMTP method
        // =========================

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _configuration["Smtp:Host"] ?? "localhost";
            var port = int.Parse(_configuration["Smtp:Port"] ?? "25");
            var fromEmail = _configuration["Smtp:FromEmail"] ?? "noreply@rideready.local";
            var fromName = _configuration["Smtp:FromName"] ?? "RideReady";

            var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "false");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];

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

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent to {Email}", toEmail);
        }
    }
}