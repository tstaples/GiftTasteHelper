using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

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
    }
}
