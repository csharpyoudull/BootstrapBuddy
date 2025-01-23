using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BootstrapBuddy.Agent
{
    public class HtmlExtractor
    {
        // Define the regex pattern as a constant
        private const string HtmlPattern = @"```html\s*([\s\S]*?)\s*```";

        public static string ExtractHtml(string input)
        {
            try
            {
                // Create regex with options for multiline support
                var regex = new Regex(HtmlPattern, RegexOptions.Multiline);

                // Try to find a match
                Match match = regex.Match(input);

                if (match.Success)
                {
                    // Return the contents of the first capturing group
                    return match.Groups[1].Value;
                }

                return string.Empty; // Return empty string if no match found
            }
            catch (RegexMatchTimeoutException)
            {
                throw new TimeoutException("Regex matching timed out");
            }
        }
    }

}
