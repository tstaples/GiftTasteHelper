using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Diagnostics;

namespace GiftTasteHelper
{
    public class NPCGiftInfo
    {
        public struct ItemData
        {
            public string name;
            public int ID;
            public Rectangle tileSheetSourceRect;

            public override string ToString()
            {
                return "{ID: " + ID.ToString() + ", Name: " + name + "}";
            }
        }

        public SVector2 MaxGiftNameSize { get; private set; }
        public string Name { get { return npcName; } }
        public ItemData[] FavouriteGifts { get { return favouriteGifts; } }

        private string npcName;
        private ItemData[] favouriteGifts;

        public NPCGiftInfo(string name, string[] favourite, int maxGiftsToDisplay)
        {
            npcName = name;
            MaxGiftNameSize = SVector2.Zero;

            int[] favouriteGiftIDs = Utils.StringToIntArray(favourite);
            int numGiftsToDisplay = CalculateNumberOfGiftsToDisplay(favouriteGiftIDs.Length, maxGiftsToDisplay);

            favouriteGifts = ParseGifts(favouriteGiftIDs, numGiftsToDisplay);
        }

        private ItemData[] ParseGifts(int[] ids, int numToDisplay)
        {
            Debug.Assert(numToDisplay <= ids.Length);

            List<ItemData> itemList = new List<ItemData>(numToDisplay);
            for (int i = 0; i < numToDisplay; ++i)
            {
                if (!Game1.objectInformation.ContainsKey(ids[i]))
                {
                    Utils.DebugLog("Could not find item information for ID: " + ids[i]);
                    continue;
                }

                string objectInfo = Game1.objectInformation[ids[i]];
                string[] parts = objectInfo.Split(new char[] { '/' });

                ItemData itemData = new ItemData();
                itemData.name = parts[0];
                itemData.ID = ids[i];
                itemData.tileSheetSourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ids[i], 16, 16);
                itemList.Add(itemData);

                SVector2 nameLength = SVector2.MeasureString(itemData.name, Game1.smallFont);
                if (nameLength.xi > MaxGiftNameSize.xi)
                {
                    MaxGiftNameSize = nameLength;
                }
            }
            return itemList.ToArray();
        }

        private int CalculateNumberOfGiftsToDisplay(int numGifts, int maxGiftsToDisplay)
        {
            // 0 or less means no limit
            if (maxGiftsToDisplay <= 0)
            {
                return numGifts;
            }
            return System.Math.Min(numGifts, maxGiftsToDisplay);
        }
    }
}
