using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftTasteHelper.Framework
{
    internal abstract class BaseGiftDataProvider : IGiftDataProvider
    {
        public event DataSourceChangedDelegate DataSourceChanged;

        protected IGiftDatabase Database;

        public BaseGiftDataProvider(IGiftDatabase database)
        {
            this.Database = database;
            this.Database.DatabaseChanged += () => DataSourceChanged();
        }

        public int[] GetFavouriteGifts(string npcName)
        {
            return Database.GetGiftsForTaste(npcName, GiftTaste.Love);
        }
    }

    internal class ProgressionGiftDataProvider : BaseGiftDataProvider
    {
        public ProgressionGiftDataProvider(IGiftDatabase database)
            : base(database)
        {
        }
    }

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
                if (npcName.IndexOf('_') != -1)
                    continue;

                string[] giftTastes = giftTaste.Value.Split('/');
                if (giftTastes.Length > 0)
                {
                    //Utils.DebugLog($"Adding favourite gifts for {npcName}");
                    var favourite = Utils.StringToIntArray(giftTastes[1].Split(' '));
                    var liked = giftTastes[3].Length > 0 ? Utils.StringToIntArray(giftTastes[3].Split(' ')) : new int[] { };
                    var disliked = giftTastes[5].Length > 0 ? Utils.StringToIntArray(giftTastes[5].Split(' ')) : new int[] { };
                    var hated = giftTastes[7].Length > 0 ? Utils.StringToIntArray(giftTastes[7].Split(' ')) : new int[] { };
                    var neutral = giftTastes[9].Length > 0 ? Utils.StringToIntArray(giftTastes[9].Split(' ')) : new int[] { };

                    Database.AddGifts(npcName, GiftTaste.Love, favourite);
                    Database.AddGifts(npcName, GiftTaste.Like, liked);
                    Database.AddGifts(npcName, GiftTaste.Dislike, disliked);
                    Database.AddGifts(npcName, GiftTaste.Hate, hated);
                    Database.AddGifts(npcName, GiftTaste.Neutral, neutral);
                }
            }
        }
    }
}
