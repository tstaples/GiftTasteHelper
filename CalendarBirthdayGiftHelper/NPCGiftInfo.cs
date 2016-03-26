using System;
using System.Collections;
using System.Collections.Generic;
using StardewModdingAPI;

namespace CalendarBirthdayGiftHelper
{
    public class NPCGiftInfo
    {
        // TODO: make sure no item uses this id
        public const int INVALID_ITEM_ID = 0;

        private string npcName;
        private int[] favouriteGifts;
        private int[] goodGifts;

        public string Name { get { return npcName; } }
        public int[] FavouriteGifts { get { return favouriteGifts; } }
        public int[] GoodGifts { get { return goodGifts; } }

        public NPCGiftInfo(string name, string[] favourite, string[] good)
        {
            npcName = name;
            favouriteGifts = Utils.StringToIntArray(favourite);
            goodGifts = Utils.StringToIntArray(good);
        }
    }
}
