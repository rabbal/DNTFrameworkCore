using DNTPersianUtils.Core;

namespace DNTFrameworkCore.TestWebApp.Helpers
{
    public static class PersianHelper
    {
        private const char RightToLeftEmbedding = (char) 0x202B;
        private const char LeftToRightEmbedding = (char) 0x202D;
        private const char PopDirectionalFormatting = (char) 0x202C;

        public static string FixWeakCharacters(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;

            var weakCharacters = new[] {@"\", "/", "+", "-", "=", ";", "$"};
            foreach (var weakCharacter in weakCharacters)
            {
                data = data.Replace(weakCharacter, RightToLeftEmbedding + weakCharacter + PopDirectionalFormatting);
            }

            return data;
        }

        public static string AppendLtrEmbeddingChar(this string text)
        {
            return LeftToRightEmbedding + text;
        }

        public static string AppendRtlEmbeddingChar(this string text)
        {
            return RightToLeftEmbedding + text;
        }

        /// <summary>
        /// Normalize persian text and remove white spaces
        /// </summary>
        /// <param name="text"></param>
        public static string NormalizePersianTitle(this string title)
        {
            const PersianNormalizers normalizers = PersianNormalizers.ApplyPersianYeKe |
                                                   PersianNormalizers.ApplyHalfSpaceRule |
                                                   PersianNormalizers.CleanupSpacingAndLineBreaks |
                                                   PersianNormalizers.CleanupExtraMarks |
                                                   PersianNormalizers.ConvertDotsToEllipsis |
                                                   PersianNormalizers.ConvertEnglishQuotes |
                                                   PersianNormalizers.FixDashes |
                                                   PersianNormalizers.RemoveAllKashida |
                                                   PersianNormalizers.RemoveDiacritics |
                                                   PersianNormalizers.RemoveOutsideInsideSpacing |
                                                   PersianNormalizers.CleanupZwnj;

            return title.NormalizePersianText(normalizers).Replace(" ", "");
        }
    }
}