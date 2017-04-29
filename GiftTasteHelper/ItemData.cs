using Microsoft.Xna.Framework;

namespace GiftTasteHelper
{
    internal struct ItemData
    {
        /*********
        ** Accessors
        *********/
        public string Name;
        public int ID;
        public Rectangle TileSheetSourceRect;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return "{ID: " + this.ID + ", Name: " + this.Name + "}";
        }
    }
}
