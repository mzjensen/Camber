#region references
using System.Text;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Utilities
{
    [IsVisibleInDynamoLibrary(false)]
    public static class StringUtilities
    {
        /// <summary>
        /// Adds spaces between capital letters in a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preserveAcronyms"></param>
        /// <param name="dropLastCharacter"></param>
        /// <returns></returns>
        public static string AddSpacesBetweenCapitals(string text, bool preserveAcronyms, bool dropLastCharacter = false)
        {
            if (string.IsNullOrWhiteSpace(text)) { return string.Empty; }
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            if (dropLastCharacter) { newText = newText.Remove(newText.Length - 1, 1); }
            return newText.ToString();
        }
    }
}
