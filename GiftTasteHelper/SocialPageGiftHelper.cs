using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using System.Reflection;
using SDVSocialPage = StardewValley.Menus.SocialPage;
using Microsoft.Xna.Framework;

namespace GiftTasteHelper
{
    public class SocialPageGiftHelper : GiftHelper
    {
        private SocialPage socialPage = new SocialPage();
        private string lastHoveredNPC = string.Empty;

        public SocialPageGiftHelper(int maxItemsToDisplay) 
            : base(EGiftHelperType.GHT_SocialPage, maxItemsToDisplay)
        {
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // Reset
            lastHoveredNPC = string.Empty;

            SDVSocialPage nativeSocialPage = GetNativeSocialPage(menu);
            if (nativeSocialPage != null)
            {
                socialPage.Init(nativeSocialPage);
            }
            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            base.OnResize(menu);
            socialPage.OnResize(GetNativeSocialPage(menu));
        }

        public override bool CanTick()
        {
            // We don't have a tab-changed event so don't tick when the social tab isn't open
            return (IsCorrectMenuTab(Game1.activeClickableMenu) && base.CanTick());
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(IsCorrectMenuTab(Game1.activeClickableMenu));
            Debug.Assert(socialPage != null);

            SVector2 mousePos = new SVector2(e.NewState.X, e.NewState.Y);
            string hoveredNPC = socialPage.GetCurrentlyHoveredNPC(mousePos);
            if (hoveredNPC == string.Empty)
            {
                drawCurrentFrame = false;
                return;
            }

            if (hoveredNPC != lastHoveredNPC)
            {
                Debug.Assert(npcGiftInfo.ContainsKey(hoveredNPC));
                currentGiftInfo = npcGiftInfo[hoveredNPC];

                drawCurrentFrame = true;
                lastHoveredNPC = hoveredNPC;
            }
            else
            {
                lastHoveredNPC = string.Empty;
            }
        }

        protected override void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Prevent the tooltip from going off screen if we're at the edge
            if (x + width > viewportW)
            {
                x = viewportW - width;
            }
        }

        private bool IsCorrectMenuTab(IClickableMenu menu)
        {
            if (menu != null && menu is GameMenu)
            {
                GameMenu gameMenu = (GameMenu)menu;
                return (gameMenu != null && gameMenu.currentTab == GameMenu.socialTab);
            }
            return false;
        }

        private SDVSocialPage GetNativeSocialPage(IClickableMenu menu)
        {
            SDVSocialPage nativeSocialPage = null;
            try
            {
                nativeSocialPage = (SDVSocialPage)(
                    (List<IClickableMenu>)typeof(GameMenu)
                    .GetField("pages", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(menu))[GameMenu.socialTab];
            }
            catch (Exception ex)
            {
                Utils.DebugLog("Failed to get native social page: " + ex.ToString(), StardewModdingAPI.LogLevel.Warn);
            }

            return nativeSocialPage;
        }

    }
}
