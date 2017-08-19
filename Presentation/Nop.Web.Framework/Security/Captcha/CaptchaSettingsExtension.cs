

namespace Nop.Web.Framework.Security.Captcha
{
    public static class CaptchaSettingsExtension
    {
        public static string GetWrongCaptchaMessage(this CaptchaSettings captchaSettings)
           
        {
            if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version1)
                return "Common.WrongCaptcha";
            else if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version2)
                return "Common.WrongCaptchaV2";
            return string.Empty;
        }
    }
}