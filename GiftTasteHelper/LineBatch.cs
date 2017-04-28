using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper
{
    internal class LineBatch
    {
        bool cares_about_begin_without_end;
        bool began;
        GraphicsDevice GraphicsDevice;
        List<VertexPositionColor> verticies = new List<VertexPositionColor>();
        BasicEffect effect;
        public LineBatch(GraphicsDevice graphics)
        {
            this.GraphicsDevice = graphics;
            this.effect = new BasicEffect(this.GraphicsDevice);
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(-this.GraphicsDevice.Viewport.Width / 2, -this.GraphicsDevice.Viewport.Height / 2, 0);
            Matrix projection = Matrix.CreateOrthographic(this.GraphicsDevice.Viewport.Width, -this.GraphicsDevice.Viewport.Height, -10, 10);
            this.effect.World = world;
            this.effect.View = view;
            this.effect.VertexColorEnabled = true;
            this.effect.Projection = projection;
            this.effect.DiffuseColor = Color.White.ToVector3();
            this.cares_about_begin_without_end = true;
        }
        public LineBatch(GraphicsDevice graphics, bool cares_about_begin_without_end)
        {
            this.cares_about_begin_without_end = cares_about_begin_without_end;
            this.GraphicsDevice = graphics;
            this.effect = new BasicEffect(this.GraphicsDevice);
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(-this.GraphicsDevice.Viewport.Width / 2, -this.GraphicsDevice.Viewport.Height / 2, 0);
            Matrix projection = Matrix.CreateOrthographic(this.GraphicsDevice.Viewport.Width, -this.GraphicsDevice.Viewport.Height, -10, 10);
            this.effect.World = world;
            this.effect.View = view;
            this.effect.VertexColorEnabled = true;
            this.effect.Projection = projection;
            this.effect.DiffuseColor = Color.White.ToVector3();
        }
        public void DrawAngledLineWithRadians(Vector2 start, float length, float radians, Color color)
        {
            Vector2 offset = new Vector2(
                (float)Math.Sin(radians) * length, //x
                -(float)Math.Cos(radians) * length //y
            );
            this.Draw(start, start + offset, color);
        }
        public void DrawOutLineOfRectangle(Rectangle rectangle, Color color)
        {
            this.Draw(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y), color);
            this.Draw(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + rectangle.Height), color);
            this.Draw(new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color);
            this.Draw(new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), color);
        }
        public void DrawOutLineOfTriangle(Vector2 point_1, Vector2 point_2, Vector2 point_3, Color color)
        {
            this.Draw(point_1, point_2, color);
            this.Draw(point_1, point_3, color);
            this.Draw(point_2, point_3, color);
        }
        float GetRadians(float angleDegrees)
        {
            return angleDegrees * ((float)Math.PI) / 180.0f;
        }
        public void DrawAngledLine(Vector2 start, float length, float angleDegrees, Color color)
        {
            this.DrawAngledLineWithRadians(start, length, this.GetRadians(angleDegrees), color);
        }
        public void Draw(Vector2 start, Vector2 end, Color color)
        {
            this.verticies.Add(new VertexPositionColor(new Vector3(start, 0f), color));
            this.verticies.Add(new VertexPositionColor(new Vector3(end, 0f), color));
        }
        public void Draw(Vector3 start, Vector3 end, Color color)
        {
            this.verticies.Add(new VertexPositionColor(start, color));
            this.verticies.Add(new VertexPositionColor(end, color));
        }
        public void End()
        {
            if (!this.began)
                if (this.cares_about_begin_without_end)
                    throw new ArgumentException("Please add begin before end!");
                else
                    this.Begin();
            if (this.verticies.Count > 0)
            {
                VertexBuffer vb = new VertexBuffer(this.GraphicsDevice, typeof(VertexPositionColor), this.verticies.Count, BufferUsage.WriteOnly);
                vb.SetData<VertexPositionColor>(this.verticies.ToArray());
                this.GraphicsDevice.SetVertexBuffer(vb);

                foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, this.verticies.Count / 2);
                }
            }
            this.began = false;
        }
        public void Begin()
        {
            if (this.began)
                if (this.cares_about_begin_without_end)
                    throw new ArgumentException("You forgot end.");
                else
                    this.End();
            this.verticies.Clear();
            this.began = true;
        }
    }
}