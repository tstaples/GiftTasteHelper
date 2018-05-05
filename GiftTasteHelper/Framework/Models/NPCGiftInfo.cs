using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    /// <summary>A collection of gifts for a particular taste.</summary>
    internal class GiftCollection
    {
        public ItemData[] Items;
        // The size of the largest name within Items.
        public SVector2 MaxGiftNameSize;
    }

    internal class NpcGiftInfo
    {
        public Dictionary<GiftTaste, GiftCollection> Gifts = new Dictionary<GiftTaste, GiftCollection>();
        public Dictionary<GiftTaste, GiftCollection> UniversalGifts = new Dictionary<GiftTaste, GiftCollection>();

        public SVector2 GetLargestItemNameForTastes(IEnumerable<GiftTaste> tastes, bool includeUniversal)
        {
            SVector2 max = new SVector2(0, 0);
            foreach (GiftTaste taste in tastes)
            {
                max = Utils.CreateMax(max, includeUniversal
                    ? Utils.CreateMax(this.Gifts[taste].MaxGiftNameSize, this.UniversalGifts[taste].MaxGiftNameSize)
                    : this.Gifts[taste].MaxGiftNameSize);
            }
            return max;
        }
    }
}
