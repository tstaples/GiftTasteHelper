namespace GiftTasteHelper.Framework.UI.Panels
{
    public class Canvas : Panel
    {
        public override void Measure(SVector2 availableSize)
        {
            this.DesiredSize = SVector2.Zero;

            foreach (Element child in this.Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    child.Measure(availableSize);

                    this.DesiredSize = SVector2.Max(this.DesiredSize, child.DesiredSize);
                }
            }

            base.Measure(availableSize);
        }

        public override void Arrange(SVector2 finalSize)
        {
            foreach (Element child in this.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                SVector2 childPosition = SVector2.Zero;
                SVector2 childSize = child.DesiredSize;

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

                child.Arrange(finalSize, childPosition + this.Position);
            }

            base.Arrange(finalSize);
        }
    }
}
