using System;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    // TODO: document
    public interface IGiftHelper
    {
        bool IsInitialized();
        bool IsOpen();
        void Init(IClickableMenu menu);
        bool OnOpen(IClickableMenu menu);
        void OnResize(IClickableMenu menu);
        void OnClose();
        bool CanDraw();
        void OnDraw();
        bool CanTick();
        void OnMouseStateChange(EventArgsMouseStateChanged e);
    }
}
