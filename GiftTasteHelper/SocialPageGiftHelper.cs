using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    public class SocialPageGiftHelper : GiftHelper
    {
        private List<ClickableTextureComponent> friendSlots;
        //private SocialPage socialPage = null;

        public override void Init(IClickableMenu menu)
        {
            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            //socialPage = (SocialPage)menu;
            friendSlots = Utils.GetNativeField<List<ClickableTextureComponent>, SocialPage>((SocialPage)menu, "friendNames");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            //socialPage = (SocialPage)menu;
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {

        }
    }
}
