using Microsoft.Xna.Framework;

namespace GiftTasteHelper.Framework
{
    internal struct ItemData
    {
        // Indices of data in yaml file: http://stardewvalleywiki.com/Modding:Object_data#Format
        public const int NameIndex = 0;
        public const int PriceIndex = 1;
        public const int EdibilityIndex = 2;
        public const int TypeIndex = 3;
        public const int DisplayNameIndex = 4;
        public const int DescriptionIndex = 5;

        /*********
        ** Accessors
        *********/
        public string Name; // Always english
        public string DisplayName; // Localized display name
        public int ID;
        public Rectangle TileSheetSourceRect;

        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"{{ID: {this.ID}, Name: {this.DisplayName}}}";
        }
    }
}
