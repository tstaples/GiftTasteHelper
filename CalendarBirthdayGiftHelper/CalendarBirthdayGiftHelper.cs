using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance.Menus;

namespace CalendarBirthdayGiftHelper
{
    public class CalendarBirthdayGiftHelper : Mod
    {

        private Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        private string seasonInitializedOn;
        private Calendar calendar = new Calendar();
        private string previousHoverText;
        private NPCGiftInfo currentGiftInfo; // Info for current day being hovered over

        public override void Entry(params object[] objects)
        {
            MenuEvents.MenuChanged += OnClickableMenuChanged;

            base.Entry(objects);
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);

            // If the calendar is already open then this menu event must be it closing
            if (calendar.IsOpen)
            {
                Debug.Assert(e.PriorMenu is Billboard && !(e.NewMenu is Billboard), "Calendar thinks it's open when it isn't");

                ControlEvents.MouseChanged -= OnMouseStateChange;
                calendar.IsOpen = false;
                return;
            }

            if (e.NewMenu == null || !(e.NewMenu is Billboard))
                return;

            // This has already been run this month so the info won't have changed
            if (seasonInitializedOn != null && seasonInitializedOn == Game1.currentSeason)
                return;

            // Create our map and note when we did it. Reset everything else
            npcGiftInfo = new Dictionary<string, NPCGiftInfo>();
            seasonInitializedOn = Game1.currentSeason;
            calendar.Init((Billboard)e.NewMenu);
            previousHoverText = ""; // reset
            currentGiftInfo = null;

            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            List<Calendar.BirthdayEventInfo> birthdayEventInfo = calendar.GetNPCBirthdayEventInfo();
            foreach (Calendar.BirthdayEventInfo eventInfo in birthdayEventInfo)
            {
                if (npcGiftTastes.ContainsKey(eventInfo.npcName))
                {
                    string[] giftTastes = npcGiftTastes[eventInfo.npcName].Split(new char[] { '/' });
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    string[] goodGifts = giftTastes[3].Split(new char[] { ' ' });

                    npcGiftInfo[eventInfo.npcName] = new NPCGiftInfo(eventInfo.npcName, favouriteGifts, goodGifts);

                    //Log.Verbose("Favourite gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(favouriteGifts));
                    //Log.Verbose("Good gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(goodGifts));
                }
            }

            calendar.IsOpen = true;
            ControlEvents.MouseChanged += OnMouseStateChange;
        }

        public void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Debug.Assert(calendar != null && Game1.activeClickableMenu != null, "calendar should exist if we're checking mouse state!");

            Point newMouse = new Point(e.NewState.X, e.NewState.Y);
            Point oldMouse = new Point(e.PriorState.X, e.PriorState.Y);
            
            // TODO: move creation of regions etc into separate class so it only has to be done once
            // First pass to see if the user's mouse is even within the calendar
            if (!calendar.Bounds.Contains(newMouse))
                return;

            // Check if we're hovering over a day that has a birthday
            string hoverText = calendar.GetCurrentHoverText();
            if (hoverText.Length > 0 && hoverText.Contains("Birthday"))
            {
                // Check if it's the same as before
                if (hoverText != previousHoverText)
                {
                    Log.Async("hover text: " + hoverText);

                    string npcName = Calendar.ParseNameFromHoverText(hoverText);
                    Debug.Assert(npcGiftInfo.ContainsKey(npcName));

                    currentGiftInfo = npcGiftInfo[npcName];

                    // TODO: create the tooltip with the gift info

                    previousHoverText = hoverText;
                }
                
                // TODO: draw the tooltip with the gift info
            }
            else
            {
                // TODO: hide the current birthday info tooltip if it was being drawn
            }
        }

        private void DebugPrintMenuInfo(IClickableMenu priorMenu, IClickableMenu newMenu)
        {
            try
            {
                string priorName = "None";
                if (priorMenu != null)
                {
                    priorName = priorMenu.GetType().Name;
                }
                string newName = newMenu.GetType().Name;
                Log.Verbose("Menu changed from: {0} to {1}", priorName, newName);
            }
            catch (Exception ex)
            {
                Log.Verbose("Error getting menu name: {0}", ex);
            }
        }

    }
}
