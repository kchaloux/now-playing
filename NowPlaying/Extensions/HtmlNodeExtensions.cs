using HtmlAgilityPack;
using System.Linq;

namespace NowPlaying.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="HtmlNode"/> class.
    /// </summary>
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Find the first descendant of an <see cref="HtmlNode"/> of the given tag type that has the specified class name.
        /// </summary>
        /// <param name="root">The root <see cref="HtmlNode"/> element to find descendants of.</param>
        /// <param name="tag">The type of html tag to find.</param>
        /// <param name="className">The name of the class to find.</param>
        /// <returns>The first descendant node of the specified tag and class name, or null if none is found.</returns>
        public static HtmlNode FirstDescendantWithClass(this HtmlNode root, string tag, string className)
        {
            return root
                .Descendants(tag)
                .FirstOrDefault(x => x.GetAttributeValue("class", default(string)) == className);
        }
    }
}
