using System;

namespace GiftTasteHelper.Framework.UI.Panels
{
    public class StackPanel : Panel
    {
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public override void Measure(SVector2 availableSize)
        {
            this.DesiredSize = SVector2.Zero;

            SVector2 size = this.Orientation == Orientation.Horizontal
                ? new SVector2(float.PositiveInfinity, availableSize.Y)
                : new SVector2(availableSize.X, float.PositiveInfinity);

            foreach (Element child in this.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child.Measure(size);

                if (this.Orientation == Orientation.Vertical)
                {
                    this.DesiredSize.X = Math.Max(this.DesiredSize.X, child.DesiredSize.X);
                    this.DesiredSize.Y += child.DesiredSize.Y;
                }
                else
                {
                    this.DesiredSize.Y = Math.Max(this.DesiredSize.Y, child.DesiredSize.Y);
                    this.DesiredSize.X += child.DesiredSize.X;
                }
            }

            base.Measure(availableSize);
        }

        public override void Arrange(SVector2 finalSize)
        {
            float offset = 0.0f;

            foreach (Element child in this.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                SVector2 childPosition = SVector2.Zero;
                SVector2 childSize = child.DesiredSize;

                if (this.Orientation == Orientation.Horizontal)
                {
                    childPosition.X = offset;
                    offset += childSize.X;

                    switch (child.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            childPosition.Y = (finalSize.Y - childSize.Y) * 0.5f;
                            break;
                        case VerticalAlignment.Bottom:
                            childPosition.Y = (childSize.Y - finalSize.Y);
                            break;
                        case VerticalAlignment.Stretch:
                            childSize.Y = finalSize.Y;
                            break;
                    }
                }
                else
                {
                    childPosition.Y = offset;
                    offset += childSize.Y;

                    switch (child.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Right:
                            childPosition.X = (finalSize.X - childSize.X);
                            break;
                        case HorizontalAlignment.Center:
                            childPosition.X = (finalSize.X * 0.5f) - (childSize.X * 0.5f);
                            break;
                        case HorizontalAlignment.Stretch:
                            childSize.X = finalSize.X;
                            break;
                    }
                }

                child.Arrange(childSize, childPosition + this.Position);
            }

            base.Arrange(finalSize);
        }
    }
}
