using System;
using System.Collections;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace CalendarBirthdayGiftHelper
{
    public class NPCGiftInfo
    {
        // TODO: make sure no item uses this id
        public const int INVALID_ITEM_ID = 0;

        public struct ItemData
        {
            public string name;
            public int ID;
            // TODO: add texture and other needed stuff
            public override string ToString()
            {
                return "{ID: " + ID.ToString() + ", Name: " + name + "}";
            }
        }

        private string npcName;
        private ItemData[] favouriteGifts;
        //private ItemData[] goodGifts;

        public string Name { get { return npcName; } }
        public ItemData[] FavouriteGifts { get { return favouriteGifts; } }
        //public ItemData[] GoodGifts { get { return goodGifts; } }

        public NPCGiftInfo(string name, string[] favourite/*, string[] good*/)
        {
            npcName = name;
            int[] favouriteGiftIDs = Utils.StringToIntArray(favourite);
            //int[] goodGiftIDs = Utils.StringToIntArray(good);

            favouriteGifts = ParseGifts(favouriteGiftIDs);
        }

        private ItemData[] ParseGifts(int[] ids)
        {
            ItemData[] itemList = new ItemData[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
            {
                if (!Game1.objectInformation.ContainsKey(ids[i]))
                {
                    Log.Info("Could not find item information for ID: " + ids[i]);
                    continue;
                }

                string objectInfo = Game1.objectInformation[ids[i]];
                string[] parts = objectInfo.Split(new char[] { '/' });

                ItemData itemData = new ItemData();
                itemData.name = parts[0];
                itemData.ID = ids[i];
                itemList[i] = itemData;
            }
            return itemList;
        }
    }
}
