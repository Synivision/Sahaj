using System.Globalization;

namespace Assets.Code.Extensions
{
    public static class StringExtensions
    {
        public static string ToFormalString(this string input)
        {
            var result = input.Replace('_', ' ');

            for (var i = 0; i < result.Length; i++)
                if (i == 0 || result[i - 1] == ' ')
                    result = result.Substring(0, i) + result[i].ToString(CultureInfo.InvariantCulture).ToUpper()
                             + ((i < input.Length) ? result.Substring(i + 1) : "");

            return result;
        }

        public static string ToCamelCase(this string input)
        {
            return input.Replace(" ", "").Replace("_", "");
        }

        public static string ToResourceString(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.CultureInvariant).Trim().Replace(" ", "_").ToLower();
        }
    }
}
