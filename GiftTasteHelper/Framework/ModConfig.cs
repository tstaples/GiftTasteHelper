namespace GiftTasteHelper.Framework
{
    internal class ModConfig
    {
        /// <summary>Whether the tooltip should be displayed on the calendar.</summary>
        public bool ShowOnCalendar { get; set; } = true;

        /// <summary>Whether the tooltip should be displayed on the social page.</summary>
        public bool ShowOnSocialPage { get; set; } = true;

        /// <summary>The maximum number of gifts to display on the tooltip (or 0 for unlimited).</summary>
        public int MaxGiftsToDisplay { get; set; } = 0;
    }
}
