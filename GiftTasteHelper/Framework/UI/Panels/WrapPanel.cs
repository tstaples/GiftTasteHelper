using System;

namespace GiftTasteHelper.Framework.UI.Panels
{
    public class WrapPanel : Panel
    {
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public override void Measure(SVector2 availableSize)
        {
            this.DesiredSize = SVector2.Zero;
            SVector2 currentLine = SVector2.Zero;

            foreach (Element child in this.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child.Measure(availableSize);
                SVector2 childSize = child.DesiredSize;

                if (this.Orientation == Orientation.Horizontal)
                {
                    if (currentLine.X + childSize.X > availableSize.X)
                    {
                        this.DesiredSize.X = Math.Max(currentLine.X, this.DesiredSize.X);
                        this.DesiredSize.Y += currentLine.Y;
                        currentLine = childSize;

                        if (childSize.X > availableSize.X)
                        {
                            this.DesiredSize.X = Math.Max(childSize.X, this.DesiredSize.X);
                            this.DesiredSize.Y += childSize.Y;
                            currentLine = SVector2.Zero;
                        }
                    }
                    else
                    {
                        currentLine.X += childSize.X;
                        currentLine.Y = Math.Max(childSize.Y, currentLine.Y);
                    }
                }
                else
                {
                    if (currentLine.Y + childSize.Y > availableSize.Y)
                    {
                        this.DesiredSize.Y = Math.Max(currentLine.Y, this.DesiredSize.Y);
                        this.DesiredSize.X += currentLine.X;
                        currentLine = childSize;

                        if (childSize.Y > availableSize.Y)
                        {
                            this.DesiredSize.Y = Math.Max(childSize.Y, this.DesiredSize.Y);
                            this.DesiredSize.X += childSize.X;
                            currentLine = SVector2.Zero;
                        }
                    }
                    else
                    {
                        currentLine.Y += childSize.Y;
                        currentLine.X = Math.Max(childSize.X, currentLine.X);
                    }
                }
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                this.DesiredSize.X = Math.Max(currentLine.X, this.DesiredSize.X);
                this.DesiredSize.Y += currentLine.Y;
            }
            else
            {
                this.DesiredSize.Y = Math.Max(currentLine.Y, this.DesiredSize.Y);
                this.DesiredSize.X += currentLine.X;
            }
        }

        public override void Arrange(SVector2 finalSize)
        {
        }
    }
}
