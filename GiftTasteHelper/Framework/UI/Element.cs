using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework.UI
{
    public interface IElement
    {
        SVector2 Transform { get; set; }

        SVector2 DesiredSize { get; }

        void Measure(SVector2 availableSize);
        void Arrange(SVector2 finalSize);
        void Update(SVector2 position);
        void Render(SpriteBatch spriteBatch, float zoomLevel);
    }

    public abstract class Element : IElement
    {
        public SVector2 Transform { get; set; }

        public abstract SVector2 DesiredSize { get; }

        public virtual void Measure(SVector2 availableSize)
        {
        }

        public virtual void Arrange(SVector2 finalSize)
        {
        }

        public virtual void Update(SVector2 position)
        {
            this.Transform = position;
        }

        public virtual void Render(SpriteBatch spriteBatch, float zoomLevel)
        {
        }
    }
}
