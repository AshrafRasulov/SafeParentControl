namespace SafeParentControl.Methods
{
    using System;

    public class UrlChecker
    {
        public static bool IsUrlSafe(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // Prostoy primer: zapreshchaem sayty, soderzhatsya vspomogatelnye slova ili, naprimer, "badsite"
            string lowerUrl = url.ToLower();
            if (lowerUrl.Contains("badsite") || lowerUrl.Contains("malware") || lowerUrl.Contains("adult"))
            {
                return false;
            }

            return true;
        }
    }
}