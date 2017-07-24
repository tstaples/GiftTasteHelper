using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SDVSocialPage = StardewValley.Menus.SocialPage;

namespace GiftTasteHelper.Framework
{
    internal class SocialPageGiftHelper : GiftHelper
    {
        /*********
        ** Properties
        *********/
        private readonly SocialPage SocialPage = new SocialPage();
        private string LastHoveredNpc = string.Empty;


        /*********
        ** Public methods
        *********/
        public SocialPageGiftHelper(IGiftDataProvider dataProvider, int maxItemsToDisplay, IReflectionHelper reflection)
            : base(GiftHelperType.SocialPage, dataProvider, maxItemsToDisplay, reflection) { }

        public override bool OnOpen(IClickableMenu menu)
        {
            // reset
            this.LastHoveredNpc = string.Empty;

            SDVSocialPage nativeSocialPage = this.GetNativeSocialPage(menu);
            if (nativeSocialPage != null)
                this.SocialPage.Init(nativeSocialPage, this.Reflection);
            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            base.OnResize(menu);
            this.SocialPage.OnResize(this.GetNativeSocialPage(menu));
        }

        public override bool CanTick()
        {
            // we don't have a tab-changed event so don't tick when the social tab isn't open
            return this.IsCorrectMenuTab(Game1.activeClickableMenu) && base.CanTick();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(this.IsCorrectMenuTab(Game1.activeClickableMenu));
            Debug.Assert(this.SocialPage != null);

            SVector2 mousePos = new SVector2(e.NewState.X, e.NewState.Y);
            string hoveredNpc = this.SocialPage.GetCurrentlyHoveredNpc(mousePos);
            if (hoveredNpc == string.Empty)
            {
                this.DrawCurrentFrame = false;
                return;
            }

            if (hoveredNpc != this.LastHoveredNpc)
            {
                Debug.Assert(GiftHelper.NpcGiftInfo.ContainsKey(hoveredNpc));
                this.CurrentGiftInfo = GiftHelper.NpcGiftInfo[hoveredNpc];

                this.DrawCurrentFrame = true;
                this.LastHoveredNpc = hoveredNpc;
            }
            else
            {
                this.LastHoveredNpc = string.Empty;
            }
        }


        /*********
        ** Protected methods
        *********/
        protected override void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Prevent the tooltip from going off screen if we're at the edge
            if (x + width > viewportW)
                x = viewportW - width;
        }

        private bool IsCorrectMenuTab(IClickableMenu menu)
        {
            if (menu is GameMenu)
            {
                GameMenu gameMenu = (GameMenu)menu;
                return gameMenu.currentTab == GameMenu.socialTab;
            }
            return false;
        }

        private SDVSocialPage GetNativeSocialPage(IClickableMenu menu)
        {
            try
            {
                IClickableMenu tab = this.Reflection.GetPrivateValue<List<IClickableMenu>>(menu, "pages")[GameMenu.socialTab];
                return (SDVSocialPage)tab;
            }
            catch (Exception ex)
            {
                Utils.DebugLog("Failed to get native social page: " + ex, LogLevel.Warn);
                return null;
            }
        }

    }
}
