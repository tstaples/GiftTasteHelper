using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal class GiftModel
    {
        public int ItemId { get; set; }

        public static explicit operator int(GiftModel model)
        {
            return model.ItemId;
        }
    }

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

    internal class GiftDatabaseModel
    {
        public Dictionary<string, CharacterTasteModel> Entries { get; set; } = new Dictionary<string, CharacterTasteModel>();
    }
}
