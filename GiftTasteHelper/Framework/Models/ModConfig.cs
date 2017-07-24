namespace GiftTasteHelper.Framework
{
    internal class ModConfig
    {
        /// <summary>Whether the tooltip should be displayed on the calendar.</summary>
        public bool ShowOnCalendar { get; set; } = true;

        /// <summary>Whether the tooltip should be displayed on the social page.</summary>
        public bool ShowOnSocialPage { get; set; } = true;

        /// <summary>Show only favourite gifts that you haven given to that NPC instead of all of them.</summary>
        public bool ShowOnlyKnownGifts { get; set; } = false;

        /// <summary>The maximum number of gifts to display on the tooltip (or 0 for unlimited).</summary>
        public int MaxGiftsToDisplay { get; set; } = 0;        
    }
}
