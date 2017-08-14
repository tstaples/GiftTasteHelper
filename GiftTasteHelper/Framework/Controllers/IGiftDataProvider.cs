using System;
using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal interface IGiftDataProvider
    {
        event DataSourceChangedDelegate DataSourceChanged;

        IEnumerable<int> GetGifts(string npcName, GiftTaste taste, bool includeUniversal);
    }
}
