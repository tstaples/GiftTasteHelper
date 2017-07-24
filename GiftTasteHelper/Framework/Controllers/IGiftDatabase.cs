namespace GiftTasteHelper.Framework
{
    internal interface IGiftDatabase
    {
        bool AddGift(string npcName, int itemId);
        bool AddGift(string npcName, int itemId, GiftTaste taste);
        bool AddGifts(string npcName, GiftTaste taste, int[] itemIds);

        int[] GetGiftsForTaste(string npcName, GiftTaste taste);
    }
}
