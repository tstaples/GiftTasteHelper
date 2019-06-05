using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework.UI
{
    public class Text : Element
    {
        public string String { get; set; }
        public SpriteFont Font { get; set; } = null;
        public Color TextColor { get; set; } = Color.Black;

        public override SVector2 DesiredSize => SVector2.MeasureString(this.String, this.Font);

        public override void Render(SpriteBatch spriteBatch, float zoomLevel)
        {
            spriteBatch.DrawString(
                this.Font,
                this.String,
                this.Transform.ToVector2(),
                this.TextColor,
                0.0f,
                Vector2.Zero,
                zoomLevel,
                SpriteEffects.None,
                0.0f);
        }
    }
}
