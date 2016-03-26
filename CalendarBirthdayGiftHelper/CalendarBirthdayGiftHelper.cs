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

        private Dictionary<int, NPCGiftInfo> dayGiftInfo; // Indexed by day
        private string seasonInitializedOn;
        private Calendar calendar = new Calendar();

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

                //ControlEvents.MouseChanged -= OnMouseStateChange;
                calendar.IsOpen = false;
                return;
            }

            if (e.NewMenu == null || !(e.NewMenu is Billboard))
                return;

            // This has already been run this month so the info won't have changed
            if (seasonInitializedOn != null && seasonInitializedOn == Game1.currentSeason)
                return;

            // Create our map and note when we did it
            dayGiftInfo = new Dictionary<int, NPCGiftInfo>();
            seasonInitializedOn = Game1.currentSeason;
            calendar.Init((Billboard)e.NewMenu);

            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            List<Calendar.BirthdayEventInfo> birthdayEventInfo = calendar.GetNPCBirthdayEventInfo();
            foreach (Calendar.BirthdayEventInfo eventInfo in birthdayEventInfo)
            {
                if (npcGiftTastes.ContainsKey(eventInfo.npcName))
                {
                    string[] giftTastes = npcGiftTastes[eventInfo.npcName].Split(new char[] { '/' });
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    string[] goodGifts = giftTastes[3].Split(new char[] { ' ' });

                    dayGiftInfo[eventInfo.day] = new NPCGiftInfo(eventInfo.npcName, favouriteGifts, goodGifts);

                    Log.Verbose("Favourite gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(favouriteGifts));
                    Log.Verbose("Good gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(goodGifts));
                }
            }

            //calendar.IsOpen = true;
            //ControlEvents.MouseChanged += OnMouseStateChange;
        }

        public void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Debug.Assert(calendar != null && Game1.activeClickableMenu != null, "calendar should exist if we're checking mouse state!");

            int mx = e.NewState.X;
            int my = e.NewState.Y;

            // TODO: move creation of regions etc into separate class so it only has to be done once
            // First pass to see if the user's mouse is even within the calendar
            if (!calendar.Bounds.Contains(mx, my))
                return;
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
