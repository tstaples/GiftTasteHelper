using System;
namespace GiftTasteHelper.Framework
{
    internal interface IGiftDataProvider
    {
        event DataSourceChangedDelegate DataSourceChanged;

        int[] GetFavouriteGifts(string npcName);
    }
}
