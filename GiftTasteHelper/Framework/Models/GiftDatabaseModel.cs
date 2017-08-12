using System.Collections.Generic;
using System.Linq;

namespace GiftTasteHelper.Framework
{
    /// <summary>Model for a gift item.</summary>
    internal class GiftModel
    {
        public int ItemId { get; set; }

        public static explicit operator int(GiftModel model)
        {
            return model.ItemId;
        }
    }

    /// <summary>Model for an NPC's gift tastes.</summary>
    internal class CharacterTasteModel
    {
        // Indexed by GiftTaste
        public Dictionary<GiftTaste, List<GiftModel>> Entries { get; set; } = new Dictionary<GiftTaste, List<GiftModel>>();

        public List<GiftModel> this[GiftTaste taste]
        {
            get => Entries.ContainsKey(taste) ? Entries[taste] : null;
            private set => Entries[taste] = value;
        }

        public bool Contains(GiftTaste taste, int itemId)
        {
            return Entries.ContainsKey(taste) && Entries[taste].Any(model => model.ItemId == itemId);
        }

        public void Add(GiftTaste taste, GiftModel model)
        {
            if (!Entries.ContainsKey(taste))
            {
                Entries.Add(taste, new List<GiftModel>());
            }
            Entries[taste].Add(model);
        }

        public void AddRange(GiftTaste taste, IEnumerable<GiftModel> gifts)
        {
            if (!Entries.ContainsKey(taste))
            {
                Entries.Add(taste, new List<GiftModel>());
            }
            Entries[taste].AddRange(gifts);
        }
    }

    /// <summary>Main Database model containing all NPC's and their gift tastes.</summary>
    internal class GiftDatabaseModel
    {
        public Dictionary<string, CharacterTasteModel> Entries { get; set; } = new Dictionary<string, CharacterTasteModel>();
    }
}
