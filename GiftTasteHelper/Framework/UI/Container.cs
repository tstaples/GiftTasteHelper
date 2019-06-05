using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework.UI
{
    public interface IContainer
    {
        void Measure(SVector2 availableSize);
        void Arrange(SVector2 finalSize);
        void Update(SVector2 position);
        void Render(SpriteBatch spriteBatch, float zoomLevel);
    }

    public class Container
    {
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public SVector2 DesiredSize { get; protected set; }
        public SVector2 FinalSize { get; protected set; }
        public SVector2 Position { get; protected set; }
        public Margin Margin { get; set; } = new Margin();
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

        public IElement Content { get; private set; }

        public Container(IElement content)
        {
            this.Content = content;
        }

        public void Measure(SVector2 availableSize)
        {
            float marginWidth = this.Margin.Left + this.Margin.Right;
            float marginHeight = this.Margin.Top + this.Margin.Bottom;
            SVector2 marginOffset = new SVector2(marginWidth, marginHeight);

            SVector2 size = availableSize;
            size -= marginOffset;

            this.Content.Measure(size);

            this.DesiredSize = this.Content.DesiredSize;
            this.DesiredSize += marginOffset;
        }

        public void Arrange(SVector2 finalSize)
        {
            this.FinalSize = finalSize;

            SVector2 position = SVector2.Zero;
            SVector2 size = this.DesiredSize;

            switch (this.VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    position.Y = (finalSize.Y - size.Y) * 0.5f;
                    break;
                case VerticalAlignment.Bottom:
                    position.Y = (size.Y - finalSize.Y);
                    break;
                case VerticalAlignment.Stretch:
                    size.Y = finalSize.Y;
                    break;
            }

            switch (this.HorizontalAlignment)
            {
                case HorizontalAlignment.Right:
                    position.X = (finalSize.X - size.X);
                    break;
                case HorizontalAlignment.Center:
                    position.X = (finalSize.X * 0.5f) - (size.X * 0.5f);
                    break;
                case HorizontalAlignment.Stretch:
                    size.X = finalSize.X;
                    break;
            }

            this.Arrange(size, position);
        }

        public void Arrange(SVector2 finalSize, SVector2 position)
        {
            this.Position = new SVector2(position.X + this.Margin.Left, position.Y + this.Margin.Top);

            this.Content.Arrange(new SVector2(finalSize.X - this.Margin.Width, finalSize.Y - this.Margin.Height));
        }

        public virtual void Update(SVector2 position)
        {
            this.Position += position;
            this.Content.Update(this.Position);
        }

        public virtual void Render(SpriteBatch spriteBatch, float zoomLevel)
        {
            this.Content.Render(spriteBatch, zoomLevel);
        }
    }
}
