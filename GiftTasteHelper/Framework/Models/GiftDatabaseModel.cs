using System.Collections.Generic;

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
        public List<List<GiftModel>> Entries { get; set; }

        public CharacterTasteModel()
        {
            Entries = new List<List<GiftModel>>();
            for (int i = 0; i < (int)GiftTaste.MAX; ++i)
            {
                Entries.Add(new List<GiftModel>());
            }
        }

        public List<GiftModel> this[GiftTaste taste]
        {
            get => Entries[(int)taste];
            set => Entries[(int)taste] = value;
        }
    }

    /// <summary>Main Database model containing all NPC's and their gift tastes.</summary>
    internal class GiftDatabaseModel
    {
        public Dictionary<string, CharacterTasteModel> Entries { get; set; } = new Dictionary<string, CharacterTasteModel>();
    }
}
