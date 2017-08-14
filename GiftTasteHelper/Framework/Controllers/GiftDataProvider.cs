using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftTasteHelper.Framework
{
    #region BaseGiftDataProvider
    internal abstract class BaseGiftDataProvider : IGiftDataProvider
    {
        public event DataSourceChangedDelegate DataSourceChanged;

        protected IGiftDatabase Database;

        public BaseGiftDataProvider(IGiftDatabase database)
        {
            this.Database = database;
            this.Database.DatabaseChanged += () => DataSourceChanged?.Invoke();
        }

        public IEnumerable<int> GetFavouriteGifts(string npcName, bool includeUniversal)
        {
            if (includeUniversal)
            {
                return Database.GetGiftsForTaste(Utils.UniversalTasteNames[GiftTaste.Love], GiftTaste.Love)
                    .Concat(Database.GetGiftsForTaste(npcName, GiftTaste.Love));
            }
            return Database.GetGiftsForTaste(npcName, GiftTaste.Love);
        }
    }
    #endregion BaseGiftDataProvider

    #region ProgressionGiftDataProvider
    internal class ProgressionGiftDataProvider : BaseGiftDataProvider
    {
        public ProgressionGiftDataProvider(IGiftDatabase database)
            : base(database)
        {
        }
    }
    #endregion ProgressionGiftDataProvider

    #region AllGiftDataProvider 
    internal class AllGiftDataProvider : BaseGiftDataProvider
    {
        public AllGiftDataProvider(IGiftDatabase database)
            : base(database)
        {
            // TODO: filter out names that will never be used
            foreach (var giftTaste in Game1.NPCGiftTastes)
            {
                // The first few elements are universal_tastes and we only want names.
                // None of the names contain an underscore so we can check that way.
                string npcName = giftTaste.Key;
                Database.AddGifts(npcName, GiftTaste.Love, Utils.GetItemsForTaste(npcName, GiftTaste.Love));
                Database.AddGifts(npcName, GiftTaste.Like, Utils.GetItemsForTaste(npcName, GiftTaste.Like));
                Database.AddGifts(npcName, GiftTaste.Dislike, Utils.GetItemsForTaste(npcName, GiftTaste.Dislike));
                Database.AddGifts(npcName, GiftTaste.Hate, Utils.GetItemsForTaste(npcName, GiftTaste.Hate));
                Database.AddGifts(npcName, GiftTaste.Neutral, Utils.GetItemsForTaste(npcName, GiftTaste.Neutral));
            }
        }
    }
    #endregion AllGiftDataProvider
}
