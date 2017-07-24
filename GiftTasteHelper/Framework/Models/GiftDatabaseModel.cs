using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal class GiftEntry
    {
        public int ItemId { get; set; }
    }

    internal class GiftDatabaseModel
    {
        public Dictionary<string, GiftEntry> Entries { get; set; }
    }
}
