namespace Dmc.Cms.App
{
    public interface IEmailSettings
    {
        string Password { get; }
        string SendFromEmail { get; }
        string SendFromName { get; }
        string SmtpHost { get; }
        int? SmtpPort { get; }
        string Username { get; }
    }
}