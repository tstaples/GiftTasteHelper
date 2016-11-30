using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper
{
    public class SocialPageGiftHelper : GiftHelper
    {
        private GameMenu gameMenu;
        private SocialPage soc;
        private List<ClickableTextureComponent> friendSlots;
        private string prevFriend = "";
        private bool scrolledToBottom = false;

        public SocialPageGiftHelper() 
            : base(EGiftHelperType.SocialPage)
        {

        }

        public override void Init(IClickableMenu menu)
        {
            // initialize friendSlots
            gameMenu = (GameMenu)menu;
            soc = (SocialPage)((List<IClickableMenu>)typeof(GameMenu).GetField("pages", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameMenu))[GameMenu.socialTab];
            friendSlots = Utils.GetNativeField<List<ClickableTextureComponent>, SocialPage>(soc, "friendNames");

            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            if (IsInitialized())
            {
                OnClose();
                Init(menu);
            }
            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (IsInitialized()) OnClose();
            Init(menu);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            if (!(Game1.activeClickableMenu is GameMenu))
            {
                return;
            }

            gameMenu = (GameMenu)Game1.activeClickableMenu;
            if (gameMenu.currentTab != GameMenu.socialTab)
            {
                return;
            }

            Rectangle mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            int slotPos = Utils.GetNativeField<int, SocialPage>(soc, "slotPosition");
            int minX = friendSlots[slotPos].bounds.X;
            int minY = friendSlots[slotPos].bounds.Y;
            int maxX = gameMenu.xPositionOnScreen + gameMenu.width - 9 * Game1.pixelZoom;
            int maxY = friendSlots[slotPos + 4].bounds.Y + (friendSlots[slotPos + 1].bounds.Y - friendSlots[slotPos].bounds.Y);

            bool mouseWithinSocialPage = (mouseRect.X > minX && mouseRect.Y > minY && mouseRect.X < maxX && mouseRect.Y < maxY);

            if (mouseWithinSocialPage)
            {
                foreach (ClickableTextureComponent friend in friendSlots)
                {
                    if (mouseRect.Intersects(friend.bounds) && friend.name != prevFriend)
                    {
                        if (slotPos == (friendSlots.Count - 5)) scrolledToBottom = true;
                        if (scrolledToBottom && mouseRect.Intersects(friendSlots[slotPos + 4].bounds))
                        {
                            currentGiftInfo = npcGiftInfo[friendSlots[slotPos + 4].name];
                        }
                        else
                        {
                            currentGiftInfo = npcGiftInfo[friend.name];
                        }

                        prevFriend = friend.name;
                    }

                }

                drawCurrentFrame = true;
            }
            else
            {
                drawCurrentFrame = false;
            }
        }

        public override void OnDraw()
        {
            // Double check here since we may not be unsubscribed from post render right away when the calendar closes
            if (drawCurrentFrame && currentGiftInfo != null)
            {
                DrawGiftSocTooltip(currentGiftInfo, "Favourite Gifts");
            }
        }

        private void DrawGiftSocTooltip(NPCGiftInfo giftInfo, string title)
        {
            // Approximate where the original tooltip will be positioned
            SVector2 origHoverTextSize = SVector2.MeasureString("", Game1.dialogueFont);

            // Draw the tooltip
            DrawGiftTooltip(giftInfo, title, origHoverTextSize);
        }

    }
}
