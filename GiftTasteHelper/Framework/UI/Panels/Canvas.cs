namespace GiftTasteHelper.Framework.UI.Panels
{
    public class Canvas : Panel
    {
        public override void Measure(SVector2 availableSize)
        {
            this.desiredSize = SVector2.Zero;

            foreach (Container child in this.Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    child.Measure(availableSize);

                    this.desiredSize = SVector2.Max(this.DesiredSize, child.DesiredSize);
                }
            }

            base.Measure(availableSize);
        }

        public override void Arrange(SVector2 finalSize)
        {
            foreach (Container child in this.Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                   child.Arrange(finalSize);
                }

            }

            base.Arrange(finalSize);
        }
    }
}
