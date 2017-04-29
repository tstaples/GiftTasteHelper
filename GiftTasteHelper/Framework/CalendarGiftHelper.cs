using System.Diagnostics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework
{
    internal class CalendarGiftHelper : GiftHelper
    {
        /*********
        ** Properties
        *********/
        private readonly Calendar Calendar = new Calendar();
        private string PreviousHoverText = "";


        /*********
        ** Public methods
        *********/
        public CalendarGiftHelper(int maxItemsToDisplay)
            : base(GiftHelperType.Calendar, maxItemsToDisplay) { }

        public override void Init(IClickableMenu menu)
        {
            Debug.Assert(!this.Calendar.IsInitialized, "Calendar is already initialized");

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

            Debug.Assert(!this.Calendar.IsOpen);

            // The calendar/billboard's internal data is re-initialized every time it's opened
            // So we need to update ours as well.
            this.Calendar.Init((Billboard)menu);
            this.Calendar.IsOpen = true;
            this.PreviousHoverText = "";

            Utils.DebugLog("[OnOpen] Opening calendar");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (this.Calendar.IsOpen && this.Calendar.IsInitialized)
            {
                Utils.DebugLog("[OnResize] Re-Initializing calendar");
                this.Calendar.OnResize((Billboard)menu);
            }
        }

        public override void OnClose()
        {
            this.Calendar.IsOpen = false;

            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(this.Calendar.IsOpen, "OnMouseStateChange being called but the calendar isn't open");

            // This gets the scaled mouse position
            SVector2 mouse = new SVector2(Game1.getMouseX(), Game1.getMouseY());

            // Check if we're hovering over a day that has a birthday
            string hoverText = this.Calendar.GetHoveredBirthdayNpcName(mouse);
            if (hoverText.Length > 0)
            {
                // Check if it's the same as before
                if (hoverText != this.PreviousHoverText)
                {
                    string npcName = Utils.ParseNameFromHoverText(hoverText);
                    Debug.Assert(this.NpcGiftInfo.ContainsKey(npcName));

                    this.CurrentGiftInfo = this.NpcGiftInfo[npcName];
                    //CurrentGiftInfo = NpcGiftInfo["Penny"]; // Temp for testing since she has the most items
                    //Utils.DebugLog(npcName + " favourite gifts: " + Utils.ArrayToString(CurrentGiftInfo.FavouriteGifts));

                    this.PreviousHoverText = hoverText;
                }

                this.DrawCurrentFrame = true;
            }
            else
            {
                this.PreviousHoverText = string.Empty;
                this.DrawCurrentFrame = false;
            }
        }

        public override void OnDraw()
        {
            // Draw the tooltip
            this.DrawGiftTooltip(this.CurrentGiftInfo, this.TooltipTitle, this.Calendar.GetCurrentHoverText());
        }
    }
}
