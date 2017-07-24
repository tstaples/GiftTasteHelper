using System;
namespace GiftTasteHelper.Framework
{
    internal interface IGiftDataProvider
    {
        int[] GetFavouriteGifts(string npcName);
    }
}
