using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using System.Reflection;
using SDVSocialPage = StardewValley.Menus.SocialPage;

namespace GiftTasteHelper
{
    public class SocialPageGiftHelper : GiftHelper
    {
        private SocialPage socialPage = new SocialPage();
        private string lastHoveredNPC = string.Empty;

        public SocialPageGiftHelper() 
            : base(EGiftHelperType.GHT_SocialPage)
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
            return (IsCorrectMenuTab((GameMenu)Game1.activeClickableMenu) && base.CanTick());
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(IsCorrectMenuTab((GameMenu)Game1.activeClickableMenu));
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

        public override void OnDraw()
        {
            // Approximate where the original tooltip will be positioned
            SVector2 origHoverTextSize = SVector2.MeasureString("", Game1.dialogueFont);

            // Draw the tooltip
            string title = "Favourite Gifts";
            DrawGiftTooltip(currentGiftInfo, title, origHoverTextSize);
        }

        private bool IsCorrectMenuTab(IClickableMenu menu)
        {
            GameMenu gameMenu = (GameMenu)menu;
            return (gameMenu != null && gameMenu.currentTab == GameMenu.socialTab);
        }

        private SDVSocialPage GetNativeSocialPage(IClickableMenu menu)
        {
            SDVSocialPage nativeSocialPage = (SDVSocialPage)(
                (List<IClickableMenu>)typeof(GameMenu)
                .GetField("pages", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(menu))[GameMenu.socialTab];

            return nativeSocialPage;
        }

    }
}
