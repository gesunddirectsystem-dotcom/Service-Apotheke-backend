namespace Service_Apotheke.Services.Email
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(string toEmail, string confirmationCode);
        Task SendApplicationNotification(string toEmail, string pharmacistName, DateTime shiftDate);
    }
}
