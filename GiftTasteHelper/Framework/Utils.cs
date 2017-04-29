using System;
using System.Diagnostics;
using System.Reflection;
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

        public static string ArrayToString<T>(T[] array)
        {
            string s = "";
            int i = 0;
            foreach (T item in array)
            {
                s += item + ((++i < array.Length) ? ", " : "");
            }
            return s;
        }

        public static T[] ConcatArrays<T>(T[] a, T[] b)
        {
            T[] c = new T[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, c, b.Length);
            return c;
        }

        public static int[] StringToIntArray(string[] array, int defaultVal = 0)
        {
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == null || !Utils.IsInt32(array[i]))
                    continue;

                try
                {
                    output[i] = Int32.Parse(array[i]);
                }
                catch (Exception ex)
                {
                    Utils.DebugLog("failed to convert " + array[i] + "to int32: " + ex, LogLevel.Warn);
                    output[i] = defaultVal;
                }
            }
            return output;
        }

        public static T GetNativeField<T, Instance>(Instance instance, string fieldName)
        {
            FieldInfo fieldInfo = typeof(Instance).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)fieldInfo.GetValue(instance);
        }

        public static bool IsType<T>(object o)
        {
            if (o != null)
                return o.GetType() == typeof(T);
            return false;
        }

        public static int GetTileSheetIndexFromID(int id)
        {
            if (id == 0)
                return 0;

            const int spriteSize = 16; // each sprite on this sheet is 16x16
            int x = (int)Math.Floor((float)(id / 24.0f));
            int y = id % spriteSize;
            return (y * spriteSize) + x;
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

        public static bool IsInt32(string s)
        {
            int i;
            return int.TryParse(s, out i);
        }

        public static Rectangle MakeRect(float x, float y, float width, float height)
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }
    }
}
