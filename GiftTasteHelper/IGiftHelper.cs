using System;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    public interface IGiftHelper
    {
        bool IsInitialized();
        bool IsOpen();
        void Init(IClickableMenu menu);
        bool OnOpen(IClickableMenu menu);
        void OnResize(IClickableMenu menu);
        void OnClose();
        void OnDraw();
        void OnMouseStateChange(EventArgsMouseStateChanged e);
    }
}
