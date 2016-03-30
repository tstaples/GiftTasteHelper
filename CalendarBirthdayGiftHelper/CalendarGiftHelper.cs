using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CalendarBirthdayGiftHelper
{
    public class CalendarGiftHelper : GiftHelper
    {
        private Calendar calendar = new Calendar();
        private string previousHoverText = "";

        public override void Init()
        {
            Debug.Assert(!calendar.IsInitialized, "Calendar is already initialized");

            base.Init();
        }

        public override void OnOpen(IClickableMenu menu)
        {
            // The daily quest board logic is also in the billboard, so check for that
            bool isDailyQuestBoard = Utils.GetNativeField<bool, Billboard>((Billboard)menu, "dailyQuestBoard");
            if (isDailyQuestBoard)
            {
                Log.Debug("Daily quest board was opened");
                return;
            }

            Debug.Assert(!calendar.IsOpen);

            previousHoverText = "";
            base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (calendar.IsOpen && calendar.IsInitialized)
            {
                calendar.OnResize((Billboard)menu);
            }
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {

        }
    }
}
