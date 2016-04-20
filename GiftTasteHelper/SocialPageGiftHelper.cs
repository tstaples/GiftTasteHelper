using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    // NOTE: Not implemented yet
    public class SocialPageGiftHelper : GiftHelper
    {
        private List<ClickableTextureComponent> friendSlots;

        public override void Init(IClickableMenu menu)
        {
            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            friendSlots = Utils.GetNativeField<List<ClickableTextureComponent>, SocialPage>((SocialPage)menu, "friendNames");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
        }

        private void DrawGiftTooltip(NPCGiftInfo giftInfo, string title)
        {

        }
    }
}
