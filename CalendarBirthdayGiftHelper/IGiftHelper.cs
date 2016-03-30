using System;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CalendarBirthdayGiftHelper
{
    public interface IGiftHelper
    {
        void Init();
        void OnOpen(IClickableMenu menu);
        void OnResize(IClickableMenu menu);
        void OnClose();
        void OnDraw();
        void OnMouseStateChange(EventArgsMouseStateChanged e);
    }
}
