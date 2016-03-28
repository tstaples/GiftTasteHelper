using System;
using System.Reflection;
using System.Collections.Generic;
using StardewModdingAPI;

namespace CalendarBirthdayGiftHelper
{
    public class Utils
    {
        public static string ArrayToString<T>(T[] array)
        {
            string s = "";
            int i = 0;
            foreach (T item in array)
            {
                s += item.ToString() + ((++i < array.Length) ? ", " : "");
            }
            return s;
        }

        public static int[] StringToIntArray(string[] array, int defaultVal=0)
        {
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == null || !array[i].IsInt32())
                    continue;

                try
                {
                    output[i] = Int32.Parse(array[i]);
                }
                catch (Exception ex)
                {
                    Log.AsyncR("[CalendarBirthdayGiftHelper] failed to convert " + array[i] + "to int32: " + ex);
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
            {
                return (o.GetType() == typeof(T));
            }
            return false;
        }
    }
}
