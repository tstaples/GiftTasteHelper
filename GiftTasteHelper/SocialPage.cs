using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SDVSocialPage = StardewValley.Menus.SocialPage;
using ClickableTextureComponent = StardewValley.Menus.ClickableTextureComponent;
using StardewValley;

namespace GiftTasteHelper
{
    public class SocialPage
    {
        private SDVSocialPage nativeSocialPage;
        private List<ClickableTextureComponent> friendSlots;

        private SVector2 offset;
        private float slotHeight;
        private float zoom;
        private Rectangle pageBounds;
        private int lastSlotIndex;

        public SocialPage()
        {
        }

        public void Init(SDVSocialPage nativePage)
        {
            OnResize(nativePage);
        }

        public void OnResize(SDVSocialPage nativePage)
        {
            nativeSocialPage = nativePage;
            friendSlots = Utils.GetNativeField<List<ClickableTextureComponent>, SDVSocialPage>(nativeSocialPage, "friendNames");

            // Mostly arbitrary since there's no nice way (that i know of) to get the slots positioned correctly...
            offset = new SVector2(Game1.tileSize / 4, Game1.tileSize / 8);
            zoom = Game1.options.zoomLevel;
            slotHeight = GetSlotHeight();
            lastSlotIndex = -1; // Invalidate
        }

        public string GetCurrentlyHoveredNPC(SVector2 mousePos)
        {
            int slotIndex = GetSlotIndex();
            if (slotIndex < 0 || slotIndex >= friendSlots.Count)
            {
                Utils.DebugLog("SlotIndex is invalid", StardewModdingAPI.LogLevel.Error);
                return string.Empty;
            }

            // Remake the page bounds if the slot index has changed
            // TODO: we can probably just do this once on resize with slot 0
            if (slotIndex != lastSlotIndex)
            {
                pageBounds = MakeBounds(slotIndex);
                lastSlotIndex = slotIndex;
            }

            // Early out if the mouse isn't within the page bounds
            Point mousePoint = mousePos.ToPoint();
            if (!pageBounds.Contains(mousePoint))
            {
                return string.Empty;
            }

            // Find the slot containing the cursor among the currently visible slots
            string hoveredFriendName = string.Empty;
            for (int i = slotIndex; i < slotIndex + SDVSocialPage.slotsOnPage; ++i)
            {
                var friend = friendSlots[i];
                var bounds = MakeSlotBounds(friend);

                if (bounds.Contains(mousePoint))
                {
                    hoveredFriendName = friend.name;
                    break;
                }
            }

            return hoveredFriendName;
        }

        private int GetSlotIndex()
        {
            if (nativeSocialPage != null)
            {
                return Utils.GetNativeField<int, SDVSocialPage>(nativeSocialPage, "slotPosition");
            }
            return -1;
        }

        private float GetSlotHeight()
        {
            if (friendSlots.Count > 1)
            {
                return (friendSlots[1].bounds.Y - friendSlots[0].bounds.Y);
            }
            return -1f;
        }

        private Rectangle MakeBounds(int slotIndex)
        {
            // Subtrace tilesize from the width so it's not too wide. Sucks but not easy way around it
            float x = (friendSlots[slotIndex].bounds.X - offset.x) * zoom;
            float y = (friendSlots[slotIndex].bounds.Y - offset.y) * zoom;
            float width = (friendSlots[slotIndex].bounds.Width - Game1.tileSize) * zoom;
            float height = (slotHeight * SDVSocialPage.slotsOnPage) * zoom;
            return Utils.MakeRect(x, y, width, height);
        }

        private Rectangle MakeSlotBounds(ClickableTextureComponent slot)
        {
            return Utils.MakeRect(
                (slot.bounds.X - offset.x) * zoom,
                (slot.bounds.Y - offset.y) * zoom,
                (slot.bounds.Width - Game1.tileSize) * zoom,
                slotHeight * zoom);
        }
    }
}
