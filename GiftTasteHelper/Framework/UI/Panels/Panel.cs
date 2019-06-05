using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework.UI.Panels
{
    public class Panel : Element
    {
        public IList<Container> Children { get; } = new List<Container>();

        // TODO: pack these into a single class an abstract it
        public Texture2D BackgroundTexture { get; set; } = null;
        public Rectangle BackgroundSourceRect { get; set; }

        public bool Refresh = true;

        protected SVector2 desiredSize = SVector2.Zero;
        protected SVector2 finalSize = SVector2.Zero;

        public override SVector2 DesiredSize => this.desiredSize;

        public Container AddChild(IElement child)
        {
            var container = new Container(child);
            this.Children.Add(container);
            return container;
        }

        public override void Arrange(SVector2 finalSize)
        {
            this.finalSize = finalSize;
            this.Refresh = false;
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
                    this.Transform.XInt,
                    this.Transform.YInt,
                    this.finalSize.XInt,
                    this.finalSize.YInt,
                    Color.White,
                    zoomLevel);
            }

            foreach (Container child in this.Children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    child.Render(spriteBatch, zoomLevel);
                }
            }
        }

        public override void Update(SVector2 position)
        {
            if (this.Refresh)
            {
                this.Measure(this.finalSize);
                this.Arrange(this.finalSize);
            }

            this.Transform = position;

            foreach (Container child in this.Children)
            {
                child.Update(position);
            }
        }
    }
}
