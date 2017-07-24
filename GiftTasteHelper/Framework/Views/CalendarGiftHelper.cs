using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
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
        /// <summary>The underlying calendar.</summary>
        private readonly Calendar Calendar = new Calendar();
        private readonly Dictionary<int, NPC> Birthdays = new Dictionary<int, NPC>();

        // The currently hovered day.
        private int HoveredDay = Calendar.InvalidDay;


        /*********
        ** Public methods
        *********/
        public CalendarGiftHelper(IGiftDataProvider dataProvider, int maxItemsToDisplay, IReflectionHelper reflection)
            : base(GiftHelperType.Calendar, dataProvider, maxItemsToDisplay, reflection) { }

        public override void Init(IClickableMenu menu)
        {
            Debug.Assert(!this.Calendar.IsInitialized, "Calendar is already initialized");

            // Store all valid npc birthdays for the current season.
            this.Birthdays.Clear();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.birthday_Season == Game1.currentSeason && Game1.player.friendships.ContainsKey(npc.name))
                {
                    this.Birthdays.Add(npc.birthday_Day, npc);
                }
            }

            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // The daily quest board logic is also in the billboard, so check for that
            bool isDailyQuestBoard = this.Reflection.GetPrivateValue<bool>(menu, "dailyQuestBoard");
            if (isDailyQuestBoard)
            {
                Utils.DebugLog("[OnOpen] Daily quest board was opened; ignoring.");
                return false;
            }

            Debug.Assert(!this.Calendar.IsOpen);

            // The calendar/billboard's internal data is re-initialized every time it's opened
            // So we need to update ours as well.
            this.Calendar.Init((Billboard)menu, this.Reflection);
            this.Calendar.IsOpen = true;
            this.HoveredDay = Calendar.InvalidDay;

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

            int hoveredDay = this.Calendar.GetHoveredDayIndex(mouse) + 1; // Days start at one
            if (hoveredDay == this.HoveredDay)
            {
                return;
            }

            this.HoveredDay = hoveredDay;
            if (this.Birthdays.ContainsKey(hoveredDay))
            {
                string npcName = this.Birthdays[hoveredDay].name;
                Debug.Assert(GiftHelper.NpcGiftInfo.ContainsKey(npcName));
                this.CurrentGiftInfo = GiftHelper.NpcGiftInfo[npcName];
                this.DrawCurrentFrame = true;
            }
            else
            {
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
