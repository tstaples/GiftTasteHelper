using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftTasteHelper
{
    class ModConfig
    {
        /// <summary>
        /// Should the tooltip be displayed on the calendar.
        /// </summary>
        public bool ShowOnCalendar { get; set; } = true;

        /// <summary>
        /// Should the tooltip be displayed on the social page.
        /// </summary>
        public bool ShowOnSocialPage { get; set; } = true;

        /// <summary>
        /// Max number of gifts to display on the tooltip.
        /// For unlimited set to 0.
        /// </summary>
        public int MaxGiftsToDisplay { get; set; } = 0;
    }
}
