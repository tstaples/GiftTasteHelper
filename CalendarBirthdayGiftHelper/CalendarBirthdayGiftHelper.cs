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
        public override void Entry(params object[] objects)
        {
            MenuEvents.MenuChanged += OnClickableMenuChanged;

            base.Entry(objects);
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            try
            {
                string priorName = "None";
                if (e.PriorMenu != null)
                {
                    priorName = e.PriorMenu.GetType().Name;
                }
                string newName = e.NewMenu.GetType().Name;
                Log.Verbose("Menu changed from: {0} to {1}", priorName, newName);
            }
            catch (Exception ex)
            {
                Log.Verbose("Error getting menu name: {0}", ex);
            }

            //MouseState state = Mouse.GetState();

            // Get all the birthdays n shit
            // then listen to mouse move event and check for hover over different day
            // if mouse on day that has a birthday, render tooltip with gift info
            const string calendarDaysFieldName = "calendarDays";
            if (e.NewMenu != null && e.NewMenu is Billboard)
            {
                Billboard calendar = (Billboard)e.NewMenu; // Yes, the calendar is stuffed in the billboard ;-;
                FieldInfo calendarDaysInfo = typeof(Billboard).GetField(calendarDaysFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>)calendarDaysInfo.GetValue(calendar);
                Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;

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

                        Log.Verbose("Favourite gifts for {0}: {1}", npcName, ArrayToString(favouriteGifts));
                        Log.Verbose("Good gifts for {0}: {1}", npcName, ArrayToString(goodGifts));
                    }
                }
            }
        }

        private string ArrayToString<T>(T[] array)
        {
            string s = "";
            foreach (T item in array)
            {
                s += item.ToString() + ", ";
            }
            return s;
        }
    }
}
