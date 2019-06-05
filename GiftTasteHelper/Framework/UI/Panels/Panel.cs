using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework.UI.Panels
{
    //public class Container
    //{
    //    public Visibility Visibility { get; set; } = Visibility.Visible;
    //    public SVector2 DesiredSize { get; protected set; }
    //    public SVector2 FinalSize { get; protected set; }
    //    public SVector2 Position { get; protected set; }
    //    public Margin Margin { get; set; } = new Margin();
    //    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    //    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

    //    public Element Content { get; private set; }

    //    public Container(Element content)
    //    {
    //        this.Content = content;
    //    }

    //    public override void Measure(SVector2 availableSize)
    //    {
    //        float marginWidth = this.Margin.Left + this.Margin.Right;
    //        float marginHeight = this.Margin.Top + this.Margin.Bottom;
    //        SVector2 marginOffset = new SVector2(marginWidth, marginHeight);

    //        SVector2 size = availableSize;
    //        size -= marginOffset;

    //        this.Content.Measure(size);

    //        this.DesiredSize = this.Content.DesiredSize + marginOffset;
    //    }

    //    public virtual void Arrange(SVector2 finalSize)
    //    {
    //        this.FinalSize = finalSize;
    //    }

    //    public void Arrange(SVector2 finalSize, SVector2 position)
    //    {
    //        this.Position = new SVector2(position.X + (this.Margin.Left - this.Margin.Right) * 0.5f, position.Y + (this.Margin.Bottom - this.Margin.Top) * 0.5f);
    //        this.Arrange(new SVector2(finalSize.X - this.Margin.Left - this.Margin.Right, finalSize.Y - this.Margin.Top - this.Margin.Bottom));
    //    }

    //    public virtual void Update(SVector2 position)
    //    {
    //    }

    //    public virtual void Render(SpriteBatch spriteBatch, float zoomLevel)
    //    {
    //        this.Content.Render(spriteBatch, zoomLevel);
    //    }
    //}

    public class Panel : Element
    {
        public IList<Element> Children { get; } = new List<Element>();

        // TODO: pack these into a single class an abstract it
        public Texture2D BackgroundTexture { get; set; } = null;
        public Rectangle BackgroundSourceRect { get; set; }

        public void AddChild(Element child)
        {
            //this.Children.Add(new Container(child));
            this.Children.Add(child);
        }

        public override void Render(SpriteBatch spriteBatch, float zoomLevel)
        {
            base.Render(spriteBatch, zoomLevel);

            if (this.BackgroundTexture != null)
            {
                IClickableMenu.drawTextureBox(
                    spriteBatch,
                    this.BackgroundTexture,
                    this.BackgroundSourceRect,
                    this.Position.XInt,
                    this.Position.YInt,
                    this.FinalSize.XInt,
                    this.FinalSize.YInt,
                    Color.White,
                    zoomLevel);
            }

            foreach (Element child in this.Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    child.Render(spriteBatch, zoomLevel);
                }
            }
        }
    }
}
