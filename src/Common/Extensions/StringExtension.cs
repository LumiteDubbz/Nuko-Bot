namespace NukoBot.Common.Extensions
{
    public static class StringExtension
    {
        public static string WithUppercaseFirstCharacter(this string text)
        {
            return text[0].ToString().ToUpper() + text.Substring(1);
        }

        public static string WithLowercaseFirstCharacter(this string text)
        {
            return text[0].ToString().ToLower() + text.Substring(1);
        }
    }
}
