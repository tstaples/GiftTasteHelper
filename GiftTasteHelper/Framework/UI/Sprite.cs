using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework.UI
{
    public class Sprite : Element
    {
        private Texture2D Texture;
        private Rectangle SourceRect;
        private float Scale;

        public override SVector2 DesiredSize => new SVector2(this.SourceRect.Width * this.Scale, this.SourceRect.Height * this.Scale);

        public Sprite(Texture2D texture, Rectangle sourceRect, float scale = 1.0f)
        {
            this.Texture = texture;
            this.SourceRect = sourceRect;
            this.Scale = scale;
        }

        public override void Render(SpriteBatch spriteBatch, float zoomLevel)
        {
            base.Render(spriteBatch, zoomLevel);

            if (this.Texture != null)
            {
                spriteBatch.Draw(
                    this.Texture,
                    this.Transform.ToVector2(),
                    this.SourceRect,
                    Color.White,
                    rotation: 0.0f,
                    origin: Vector2.Zero,
                    scale: this.Scale,
                    effects: SpriteEffects.None,
                    layerDepth: 0.0f);
            }
        }
    }
}
