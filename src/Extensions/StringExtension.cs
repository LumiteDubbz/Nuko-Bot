namespace NukoBot.Extensions
{
    public sealed class StringExtension
    {
        public static string FirstCharToUpper(string input)
        {
            char[] chars = input.ToCharArray();

            chars[0] = char.ToUpper(chars[0]);

            return new string(chars);
        }
    }
}
