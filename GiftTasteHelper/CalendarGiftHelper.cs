using System;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace GiftTasteHelper
{
    public class CalendarGiftHelper : GiftHelper
    {
        private Calendar calendar = new Calendar();
        private string previousHoverText = "";

        public CalendarGiftHelper() 
            : base(EGiftHelperType.GHT_Calendar)
        {

        }

        public override void Init(IClickableMenu menu)
        {
            Debug.Assert(!calendar.IsInitialized, "Calendar is already initialized");

            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // The daily quest board logic is also in the billboard, so check for that
            bool isDailyQuestBoard = Utils.GetNativeField<bool, Billboard>((Billboard)menu, "dailyQuestBoard");
            if (isDailyQuestBoard)
            {
                Utils.DebugLog("[OnOpen] Daily quest board was opened; ignoring.");
                return false;
            }

            Debug.Assert(!calendar.IsOpen);

            // The calendar/billboard's internal data is re-initialized every time it's opened
            // So we need to update ours as well.
            calendar.Init((Billboard)menu);
            calendar.IsOpen = true;
            previousHoverText = "";

            Utils.DebugLog("[OnOpen] Opening calendar");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (calendar.IsOpen && calendar.IsInitialized)
            {
                Utils.DebugLog("[OnResize] Re-Initializing calendar");
                calendar.OnResize((Billboard)menu);
            }
        }

        public override void OnClose()
        {
            calendar.IsOpen = false;

            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(calendar.IsOpen, "OnMouseStateChange being called but the calendar isn't open");

            // This gets the scaled mouse position
            SVector2 mouse = new SVector2(Game1.getMouseX(), Game1.getMouseY());

            // Check if we're hovering over a day that has a birthday
            string hoverText = calendar.GetHoveredBirthdayNPCName(mouse);
            if (hoverText.Length > 0)
            {
                // Check if it's the same as before
                if (hoverText != previousHoverText)
                {
                    string npcName = Utils.ParseNameFromHoverText(hoverText);
                    Debug.Assert(npcGiftInfo.ContainsKey(npcName));

                    currentGiftInfo = npcGiftInfo[npcName];
                    //currentGiftInfo = npcGiftInfo["Penny"]; // Temp for testing since she has the most items
                    //Utils.DebugLog(npcName + " favourite gifts: " + Utils.ArrayToString(currentGiftInfo.FavouriteGifts));

                    previousHoverText = hoverText;
                }

                drawCurrentFrame = true;
            }
            else
            {
                previousHoverText = string.Empty;
                drawCurrentFrame = false;
            }
        }

        public override void OnDraw()
        {
            // Draw the tooltip
            DrawGiftTooltip(currentGiftInfo, TooltipTitle, calendar.GetCurrentHoverText());
        }
    }
}
