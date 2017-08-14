using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;

namespace GiftTasteHelper.Framework
{
    internal class NpcGiftInfo
    {
        /*********
        ** Accessors
        *********/
        public SVector2 MaxGiftNameSize { get; private set; }
        public string Name { get; }
        public ItemData[] FavouriteGifts { get; }


        /*********
        ** Public methods
        *********/
        public NpcGiftInfo(string name, int[] favouriteGiftIDs, int maxGiftsToDisplay)
        {
            this.Name = name;
            this.MaxGiftNameSize = SVector2.Zero;
            this.FavouriteGifts = this.ParseGifts(favouriteGiftIDs, CalculateNumberOfGiftsToDisplay(favouriteGiftIDs.Length, maxGiftsToDisplay));
        }


        /*********
        ** Private methods
        *********/
        private ItemData[] ParseGifts(int[] ids, int numToDisplay)
        {
            Debug.Assert(numToDisplay <= ids.Length);

            var itemList = new List<ItemData>(numToDisplay);
            for (int i = 0; i < numToDisplay; ++i)
            {
                if (!Game1.objectInformation.ContainsKey(ids[i]))
                {
                    Utils.DebugLog("Could not find item information for ID: " + ids[i]);
                    continue;
                }

                var itemData = ItemData.MakeItem(ids[i]);
                itemList.Add(itemData);

                SVector2 nameLength = SVector2.MeasureString(itemData.DisplayName, Game1.smallFont);
                if (nameLength.XInt > this.MaxGiftNameSize.XInt)
                {
                    this.MaxGiftNameSize = nameLength;
                }
            }
            return itemList.ToArray();
        }

        private static int CalculateNumberOfGiftsToDisplay(int numGifts, int maxGiftsToDisplay)
        {
            // 0 or less means no limit
            return maxGiftsToDisplay <= 0 ? numGifts : Math.Min(numGifts, maxGiftsToDisplay);
        }
    }
}
