using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace CalendarBirthdayGiftHelper
{
    public class Calendar
    {
        public const int NUM_DAYS = 28;
        private const string calendarDaysFieldName = "calendarDays";

        public struct BirthdayEventInfo
        {
            public int day;
            public string npcName;
        }

        private Billboard billboard;
        private List<ClickableTextureComponent> calendarDays;
        private bool isOpen = false;
        private Rectangle bounds;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Calendar Init(Billboard baseClass)
        {
            Clear();

            billboard = baseClass;
            bounds = new Rectangle(billboard.xPositionOnScreen, billboard.yPositionOnScreen, billboard.width, billboard.height);

            FieldInfo calendarDaysInfo = typeof(Billboard).GetField(calendarDaysFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            calendarDays = (List<ClickableTextureComponent>)calendarDaysInfo.GetValue(billboard);

            return this;
        }

        public void Clear()
        {
            billboard = null;
            if (calendarDays != null)
            {
                calendarDays.Clear();
                calendarDays = null;
            }
            isOpen = false;
        }

        public List<BirthdayEventInfo> GetNPCBirthdayEventInfo()
        {
            List<BirthdayEventInfo> eventInfoList = new List<BirthdayEventInfo>();
            int dayNumber = 1;
            foreach (ClickableTextureComponent day in calendarDays)
            {
                string hoverText = day.hoverText;
                if (hoverText.Length != 0 && hoverText.Contains("Birthday"))
                {
                    BirthdayEventInfo eventInfo = new BirthdayEventInfo();
                    eventInfo.day = dayNumber;
                    eventInfo.npcName = hoverText.Split(new char[] { '\'', ' ' })[0];
                    eventInfoList.Add(eventInfo);
                }
            }
            return eventInfoList;
        }

        public Rectangle GetDayBounds(int dayNumber)
        {
            Debug.Assert(dayNumber > 0 && dayNumber <= NUM_DAYS, "Day number out of range");
            return calendarDays[dayNumber].bounds;
        }
    }
}
