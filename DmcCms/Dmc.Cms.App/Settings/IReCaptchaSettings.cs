namespace Dmc.Cms.App
{
    public interface IReCaptchaSettings
    {
        string RecaptchaSecretKey { get; set; }
        string RecaptchaSiteKey { get; set; }
        bool UseRecaptcha { get; set; }
    }
}