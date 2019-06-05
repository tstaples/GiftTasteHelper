using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework.UI
{
    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Stretch
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center,
        Stretch
    }

    public class Margin
    {
        public float Left = 0.0f;
        public float Top = 0.0f;
        public float Right = 0.0f;
        public float Bottom = 0.0f;

        public float Width => this.Left + this.Right;
        public float Height => this.Top + this.Bottom;

        public Margin() { }

        public Margin(float left, float top, float right, float bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public Margin(float all)
        {
            this.Left = all;
            this.Top = all;
            this.Right = all;
            this.Bottom = all;
        }
    }

    public class Element
    {
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public SVector2 DesiredSize { get; protected set; }
        public SVector2 FinalSize { get; protected set; }
        public SVector2 Position { get; protected set; }
        public Margin Margin { get; set; } = new Margin();
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

        public virtual void Measure(SVector2 availableSize)
        {
            this.DesiredSize.X += this.Margin.Width;
            this.DesiredSize.Y += this.Margin.Height;
        }

        public virtual void Arrange(SVector2 finalSize)
        {
            this.FinalSize = finalSize;
        }

        public void Arrange(SVector2 finalSize, SVector2 position)
        {
            this.Position = new SVector2(position.X + this.Margin.Left, position.Y + this.Margin.Top);
            this.Arrange(new SVector2(finalSize.X - this.Margin.Width, finalSize.Y - this.Margin.Height));
        }

        public virtual void Render(SpriteBatch spriteBatch, float zoomLevel)
        {

        }
    }
}
