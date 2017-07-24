using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftTasteHelper.Framework
{
    internal class GiftDatabase : IGiftDatabase
    {
        public event DataSourceChangedDelegate DatabaseChanged;

        public GiftDatabaseModel Database { get; protected set; }
        protected readonly IModHelper Helper;

        public GiftDatabase(IModHelper helper)
        {
            this.Helper = helper;
            this.Database = new GiftDatabaseModel();
        }

        public GiftDatabase(IModHelper helper, GiftDatabaseModel database)
        {
            this.Helper = helper;
            this.Database = database;
        }

        public bool ContainsGift(string npcName, int itemId, GiftTaste taste)
        {
            var entryForTaste = Database.Entries[npcName][taste];
            return entryForTaste.Any(model => model.ItemId == itemId);
        }

        // TODO: we should probably remove this overload so the caller is reponsible for getting the taste.
        public bool AddGift(string npcName, int itemId)
        {
            GiftTaste taste = GetTasteForGift(npcName, itemId);
            return AddGift(npcName, itemId, taste);            
        }

        public virtual bool AddGift(string npcName, int itemId, GiftTaste taste)
        {
            bool check = true;
            if (!Database.Entries.ContainsKey(npcName))
            {
                Database.Entries.Add(npcName, new CharacterTasteModel());
                check = false;
            }

            var entryForTaste = Database.Entries[npcName][taste];
            if (!check || !ContainsGift(npcName, itemId, taste))
            {
                Utils.DebugLog($"Adding {itemId} to {npcName}'s {taste} tastes.");
                entryForTaste.Add(new GiftModel() { ItemId = itemId });

                DatabaseChanged();
                return true;
            }
            return false;
        }

        public virtual bool AddGifts(string npcName, GiftTaste taste, int[] itemIds)
        {
            if (!Database.Entries.ContainsKey(npcName))
            {
                Database.Entries.Add(npcName, new CharacterTasteModel());
            }

            var unique = itemIds.Where(id => !ContainsGift(npcName, id, taste)).Select(id => id);
            if (unique.Count() > 0)
            {
                Database.Entries[npcName][taste].AddRange(itemIds.Select(id => new GiftModel() { ItemId = id }));
                DatabaseChanged();
                return true;
            }
            return false;
        }

        public int[] GetGiftsForTaste(string npcName, GiftTaste taste)
        {
            if (Database.Entries.ContainsKey(npcName))
            {
                var entryForTaste = Database.Entries[npcName][taste];
                return entryForTaste.Select(model => model.ItemId).ToArray();
            }
            return new int[] { };
        }

        protected GiftTaste GetTasteForGift(string npcName, int itemId)
        {
            // TODO
            // TODO: debug option to ignore actual taste so we can test with any item.
            return GiftTaste.Love;
        }
    }

    internal class StoredGiftDatabase : GiftDatabase
    {
        private static string DBPath => "GiftDatabase.json";

        public StoredGiftDatabase(IModHelper helper)
            : base(helper, helper.ReadJsonFile<GiftDatabaseModel>(DBPath) ?? new GiftDatabaseModel())
        {
        }

        public override bool AddGift(string npcName, int itemId, GiftTaste taste)
        {
            if (base.AddGift(npcName, itemId, taste))
            {
                Write();
                return true;
            }
            return false;
        }

        public override bool AddGifts(string npcName, GiftTaste taste, int[] itemIds)
        {
            if (base.AddGifts(npcName, taste, itemIds))
            {
                Write();
                return true;
            }
            return false;
        }

        private void Write()
        {
            Utils.DebugLog("Writing gift database");
            Helper.WriteJsonFile(DBPath, Database);
        }
    }
}
