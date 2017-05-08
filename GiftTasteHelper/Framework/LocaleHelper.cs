using System;
using System.Collections.Generic;
using StardewValley;
using LanguageCode = StardewValley.LocalizedContentManager.LanguageCode;

namespace GiftTasteHelper.Framework
{
    internal class LocaleHelper
    {
        public static LanguageCode CurrentLocale => LocalizedContentManager.CurrentLanguageCode;

        // This could be an array indexed by the enum, but I'm afraid of the enum order changing.
        private static Dictionary<LanguageCode, string> TooltipTitles = new Dictionary<LanguageCode, string>()
        {
            { LanguageCode.en, "Favorite Gifts" },          // English
            { LanguageCode.ja, "好きな贈り物" },             // Japanese
            { LanguageCode.ru, "Любимые подарки" },         // Russian
            { LanguageCode.zh, "最喜爱的礼物" },             // Chinese
            { LanguageCode.pt, "Presentes favoritos" },     // Portugese
            { LanguageCode.es, "Regalos del favorito" },    // Spanish
            { LanguageCode.de, "Lieblingsgeschenke" },      // German
            { LanguageCode.th, "ของขวัญที่ชอบ" },              // Thai
        };

        public static string GetTooltipTitle()
        {
            return TooltipTitles[CurrentLocale];
        }
    }
}
