using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
public class EmailService
{
    IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmail(string name, string login,string password)
    {
        var apiKey = _configuration.GetSection("MailApiKey").Value;
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("helpdeskreg27@gmail.com", "HelpDeskMail");
        var to = new EmailAddress("domalewski.p@wp.pl", $"{name}");
        var templateId = "d-5da83691206a4e788dcfafe7af910429";
        var templateData = new {Name = name, Login =  login, Password = password};
        var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateData);
        var response = await client.SendEmailAsync(msg);
        Console.WriteLine(response.StatusCode);
    }
}