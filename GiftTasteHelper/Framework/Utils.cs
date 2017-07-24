using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace GiftTasteHelper.Framework
{
    internal class Utils
    {
        /*********
        ** Properties
        *********/
        private static IMonitor MonitorRef;


        /*********
        ** Public methods
        *********/
        public static void InitLog(IMonitor monitor)
        {
            Utils.MonitorRef = monitor;
        }

        public static void DebugLog(string message, LogLevel level = LogLevel.Trace)
        {
#if WITH_LOGGING
            Debug.Assert(Utils.MonitorRef != null, "Monitor ref is not set.");
            Utils.MonitorRef.Log(message, level);
#else
            // don't spam other developer consoles
            if (level > LogLevel.Debug)
            {
                Debug.Assert(MonitorRef != null, "Monitor ref is not set.");
                MonitorRef.Log(message, level);
            }
#endif
        }

        public static int[] StringToIntArray(string[] array, int defaultVal = 0)
        {
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                int value;
                if (int.TryParse(array[i], out value))
                    output[i] = value;
            }
            return output;
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Max(Math.Min(val, max), min);
        }

        // TODO: handle more cases
        public static string ParseNameFromHoverText(string text)
        {
            string name = "";
            string[] parts = text.Split('\'', ' ');
            if (parts.Length > 0)
                name = parts[0];
            return name;
        }

        public static Rectangle MakeRect(float x, float y, float width, float height)
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        public static GiftTaste GetTasteForGift(string npcName, int itemId)
        {
            var giftTaste = Game1.NPCGiftTastes[npcName];
            string[] giftTastes = giftTaste.Split('/');
            if (giftTastes.Length == 0)
            {
                return GiftTaste.MAX;
            }

            // See http://stardewvalleywiki.com/Modding:Gift_taste_data
            GiftTaste taste = GiftTaste.Love;
            for (int i = 1; i <= 9; i += 2)
            {
                if (giftTastes[i].Length > 0)
                {
                    var items = Utils.StringToIntArray(giftTastes[i].Split(' '));
                    if (items.Contains(itemId))
                    {
                        return taste;
                    }
                }
                taste++;
            }
            return GiftTaste.MAX;
        }
    }
}
