namespace GiftTasteHelper.Framework.UI
{
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
}
