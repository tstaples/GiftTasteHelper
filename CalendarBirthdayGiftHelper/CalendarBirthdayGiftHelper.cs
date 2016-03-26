using System;
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
        public const int NUM_DAYS = 28;

        private List<NPCGiftInfo> dayGiftInfo; // Indexed by day
        private int dayInitializedOn = 0;

        public override void Entry(params object[] objects)
        {
            MenuEvents.MenuChanged += OnClickableMenuChanged;

            base.Entry(objects);
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);

            if (e.NewMenu == null || !(e.NewMenu is Billboard))
                return;

            // This has already been run today so the info won't have changed
            if (dayInitializedOn == Game1.dayOfMonth)
                return;

            // Create our map and note when we did it
            dayGiftInfo = new List<NPCGiftInfo>(NUM_DAYS);
            dayInitializedOn = Game1.dayOfMonth;

            // Get the calendar and npc gift taste info
            const string calendarDaysFieldName = "calendarDays";
            Billboard calendar = (Billboard)e.NewMenu; // Yes, the calendar is stuffed in the billboard ;-;
            FieldInfo calendarDaysInfo = typeof(Billboard).GetField(calendarDaysFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>)calendarDaysInfo.GetValue(calendar);
            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;

            int dayNumber = 1;
            foreach (ClickableTextureComponent day in calendarDays)
            {
                string hoverText = day.hoverText;
                if (hoverText.Length == 0 || !hoverText.Contains("Birthday"))
                    continue;

                string npcName = hoverText.Split(new char[] { '\'', ' ' })[0];
                if (npcGiftTastes.ContainsKey(npcName))
                {
                    string[] giftTastes = npcGiftTastes[npcName].Split(new char[] { '/' });
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    string[] goodGifts = giftTastes[3].Split(new char[] { ' ' });

                    dayGiftInfo[dayNumber] = new NPCGiftInfo(npcName, favouriteGifts, goodGifts);

                    Log.Verbose("Favourite gifts for {0}: {1}", npcName, Utils.ArrayToString(favouriteGifts));
                    Log.Verbose("Good gifts for {0}: {1}", npcName, Utils.ArrayToString(goodGifts));
                }
                ++dayNumber;
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
