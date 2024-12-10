using System.Text.RegularExpressions;

namespace Utils.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Parse the input string to get the title and description.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static (string, string) ParseToTitleDescription(this string input)
        {
            const string titlePattern = @"title:(.*?)\s*(description:|$)";
            const string descriptionPattern = @"description:(.*)";

            var title = Regex.Match(input, titlePattern).Groups[1].Value.Trim();
            var description = Regex.Match(input, descriptionPattern).Groups[1].Value.Trim();

            return (title, description);
        }
    }
}