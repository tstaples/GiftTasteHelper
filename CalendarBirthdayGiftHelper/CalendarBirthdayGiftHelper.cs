using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance;
using StardewModdingAPI.Inheritance.Menus;

namespace CalendarBirthdayGiftHelper
{
    public class CalendarBirthdayGiftHelper : Mod
    {

        private Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        private Calendar calendar = new Calendar();
        private string previousHoverText;
        private NPCGiftInfo currentGiftInfo = null; // Info for current day being hovered over

        private bool debug_mouseEvent = false;

        public override void Entry(params object[] objects)
        {
            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
            GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent; // TODO: subscribe when a valid day is hovered over
            TimeEvents.SeasonOfYearChanged += OnSeasonChanged;
        }

        public void OnSeasonChanged(object sender, EventArgsStringChanged e)
        {
            if (calendar.IsInitialized)
            {
                // Force the calendar to reload the data for the new season
                calendar.Clear();
            }
        }

        public void OnClickableMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Log.Info(e.PriorMenu.GetType().ToString() + " menu closed.");
            if (calendar.IsOpen)
            {
                Log.Info("Calender was open; closing.");
                ControlEvents.MouseChanged -= OnMouseStateChange;
                debug_mouseEvent = false;
                calendar.IsOpen = false;
            }
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);
            
            if (!Utils.IsType<Billboard>(e.NewMenu) ||
                calendar.IsOpen && calendar.IsInitialized)
            {
                Log.Info("New menu isn't a billboard or the calendar is already open and initialized.");
                return;
            }

            Log.Debug("Calender was opened");

            // Create our map and note when we did it. Reset everything else
            npcGiftInfo = new Dictionary<string, NPCGiftInfo>();
            calendar.Init((Billboard)e.NewMenu);
            previousHoverText = ""; // reset
            currentGiftInfo = null;

            calendar.IsOpen = true;
            Debug.Assert(!debug_mouseEvent, "Mouse change event is already subscribed");
            ControlEvents.MouseChanged += OnMouseStateChange;
            debug_mouseEvent = true;
            //GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent;

            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            List<Calendar.BirthdayEventInfo> birthdayEventInfo = calendar.GetNPCBirthdayEventInfo();
            foreach (Calendar.BirthdayEventInfo eventInfo in birthdayEventInfo)
            {
                if (npcGiftTastes.ContainsKey(eventInfo.npcName))
                {
                    string[] giftTastes = npcGiftTastes[eventInfo.npcName].Split(new char[] { '/' });
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    //string[] goodGifts = giftTastes[3].Split(new char[] { ' ' });

                    npcGiftInfo[eventInfo.npcName] = new NPCGiftInfo(eventInfo.npcName, favouriteGifts/*, goodGifts*/);

                    //Log.Verbose("Favourite gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(favouriteGifts));
                    //Log.Verbose("Good gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(goodGifts));
                }
            }
        }

        public void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            //Debug.Assert(calendar != null && Game1.activeClickableMenu != null && (Game1.activeClickableMenu is Billboard), "calendar should exist if we're checking mouse state!");
            //Debug.Assert(calendar != null && calendar.IsOpen, "calendar should exist if we're checking mouse state!");

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
                    Log.Info(npcName + " favourite gifts: " + Utils.ArrayToString(currentGiftInfo.FavouriteGifts));

                    previousHoverText = hoverText;
                }

                // TODO: draw the tooltip with the gift info
            }
            else
            {
                currentGiftInfo = null;
            }
        }

        private void OnPostRenderEvent(object sender, EventArgs e)
        {
            if (currentGiftInfo != null)
            {
                //CreateGiftTooltip(currentGiftInfo);
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
                Log.Info("Menu changed from: " + priorName + " to " + newName);
            }
            catch (Exception ex)
            {
                Log.Debug("Error getting menu name: " + ex);
            }
        }

    }
}
