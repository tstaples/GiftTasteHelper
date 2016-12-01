using System;
using System.Diagnostics;
using System.Reflection;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GiftTasteHelper
{
    public class Utils
    {
        internal static IMonitor MonitorRef = null;
        public static void InitLog(IMonitor monitor)
        {
            MonitorRef = monitor;
        }

        public static void DebugLog(string message, LogLevel level = LogLevel.Trace)
        {
        #if WITH_LOGGING
            Debug.Assert(MonitorRef != null, "Monitor ref is not set.");
            MonitorRef.Log(message, level);
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
                s += item.ToString() + ((++i < array.Length) ? ", " : "");
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
                    DebugLog("failed to convert " + array[i] + "to int32: " + ex, LogLevel.Warn);
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
            string[] parts = text.Split(new char[] { '\'', ' ' });
            if (parts.Length > 0)
            {
                name = parts[0];
            }
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


    public class LineBatch
    {
        bool cares_about_begin_without_end;
        bool began;
        GraphicsDevice GraphicsDevice;
        List<VertexPositionColor> verticies = new List<VertexPositionColor>();
        BasicEffect effect;
        public LineBatch(GraphicsDevice graphics)
        {
            GraphicsDevice = graphics;
            effect = new BasicEffect(GraphicsDevice);
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(-GraphicsDevice.Viewport.Width / 2, -GraphicsDevice.Viewport.Height / 2, 0);
            Matrix projection = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, -GraphicsDevice.Viewport.Height, -10, 10);
            effect.World = world;
            effect.View = view;
            effect.VertexColorEnabled = true;
            effect.Projection = projection;
            effect.DiffuseColor = Color.White.ToVector3();
            cares_about_begin_without_end = true;
        }
        public LineBatch(GraphicsDevice graphics, bool cares_about_begin_without_end)
        {
            this.cares_about_begin_without_end = cares_about_begin_without_end;
            GraphicsDevice = graphics;
            effect = new BasicEffect(GraphicsDevice);
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(-GraphicsDevice.Viewport.Width / 2, -GraphicsDevice.Viewport.Height / 2, 0);
            Matrix projection = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, -GraphicsDevice.Viewport.Height, -10, 10);
            effect.World = world;
            effect.View = view;
            effect.VertexColorEnabled = true;
            effect.Projection = projection;
            effect.DiffuseColor = Color.White.ToVector3();
        }
        public void DrawAngledLineWithRadians(Vector2 start, float length, float radians, Color color)
        {
            Vector2 offset = new Vector2(
                (float)Math.Sin(radians) * length, //x
                -(float)Math.Cos(radians) * length //y
                );
            Draw(start, start + offset, color);
        }
        public void DrawOutLineOfRectangle(Rectangle rectangle, Color color)
        {
            Draw(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y), color);
            Draw(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + rectangle.Height), color);
            Draw(new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color);
            Draw(new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color);
        }
        public void DrawOutLineOfTriangle(Vector2 point_1, Vector2 point_2, Vector2 point_3, Color color)
        {
            Draw(point_1, point_2, color);
            Draw(point_1, point_3, color);
            Draw(point_2, point_3, color);
        }
        float GetRadians(float angleDegrees)
        {
            return angleDegrees * ((float)Math.PI) / 180.0f;
        }
        public void DrawAngledLine(Vector2 start, float length, float angleDegrees, Color color)
        {
            DrawAngledLineWithRadians(start, length, GetRadians(angleDegrees), color);
        }
        public void Draw(Vector2 start, Vector2 end, Color color)
        {
            verticies.Add(new VertexPositionColor(new Vector3(start, 0f), color));
            verticies.Add(new VertexPositionColor(new Vector3(end, 0f), color));
        }
        public void Draw(Vector3 start, Vector3 end, Color color)
        {
            verticies.Add(new VertexPositionColor(start, color));
            verticies.Add(new VertexPositionColor(end, color));
        }
        public void End()
        {
            if (!began)
                if (cares_about_begin_without_end)
                    throw new ArgumentException("Please add begin before end!");
                else
                    Begin();
            if (verticies.Count > 0)
            {
                VertexBuffer vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), verticies.Count, BufferUsage.WriteOnly);
                vb.SetData<VertexPositionColor>(verticies.ToArray());
                GraphicsDevice.SetVertexBuffer(vb);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, verticies.Count / 2);
                }
            }
            began = false;
        }
        public void Begin()
        {
            if (began)
                if (cares_about_begin_without_end)
                    throw new ArgumentException("You forgot end.");
                else
                    End();
            verticies.Clear();
            began = true;
        }
    }

}
