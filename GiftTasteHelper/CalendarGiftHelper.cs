using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    public class CalendarGiftHelper : GiftHelper
    {
        private Calendar calendar = new Calendar();
        private string previousHoverText = "";

        public override void Init(IClickableMenu menu)
        {
            Debug.Assert(!calendar.IsInitialized, "Calendar is already initialized");

            base.Init(menu);
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // The daily quest board logic is also in the billboard, so check for that
            bool isDailyQuestBoard = Utils.GetNativeField<bool, Billboard>((Billboard)menu, "dailyQuestBoard");
            if (isDailyQuestBoard)
            {
                Utils.DebugLog("[OnOpen] Daily quest board was opened; ignoring.");
                return false;
            }

            Debug.Assert(!calendar.IsOpen);

            // The calendar/billboard's internal data is re-initialized every time it's opened
            // So we need to update ours as well.
            calendar.Init((Billboard)menu);
            calendar.IsOpen = true;
            previousHoverText = "";

            Utils.DebugLog("[OnOpen] Opening calendar");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (calendar.IsOpen && calendar.IsInitialized)
            {
                Utils.DebugLog("[OnResize] Re-Initializing calendar");
                calendar.OnResize((Billboard)menu);
            }
        }

        public override void OnClose()
        {
            calendar.IsOpen = false;

            base.OnClose();
        }

        public override void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            Debug.Assert(calendar.IsOpen, "OnMouseStateChange being called but the calendar isn't open");

            SVector2 newMouse = new SVector2(e.NewState.X, e.NewState.Y);
            SVector2 oldMouse = new SVector2(e.PriorState.X, e.PriorState.Y);

            // Check if we're hovering over a day that has a birthday
            string hoverText = calendar.GetCurrentHoverText();
            if (hoverText.Length > 0 && hoverText.Contains("Birthday"))
            {
                // Check if it's the same as before
                if (hoverText != previousHoverText)
                {
                    Utils.DebugLog("hover text: " + hoverText);

                    string npcName = Utils.ParseNameFromHoverText(hoverText);
                    Debug.Assert(npcGiftInfo.ContainsKey(npcName));

                    currentGiftInfo = npcGiftInfo[npcName];
                    //currentGiftInfo = npcGiftInfo["Penny"]; // Temp for testing since she has the most items
                    //Utils.DebugLog(npcName + " favourite gifts: " + Utils.ArrayToString(currentGiftInfo.FavouriteGifts));

                    previousHoverText = hoverText;
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
                DrawGiftCalTooltip(currentGiftInfo, "Favourite Gifts");
            }
        }

        private void DrawGiftCalTooltip(NPCGiftInfo giftInfo, string title)
        {
            int numItems = giftInfo.FavouriteGifts.Length;
            if (numItems == 0)
                return;

            float spriteScale = 2.0f * ZoomLevel; // 16x16 is pretty small
            Rectangle spriteRect = giftInfo.FavouriteGifts[0].tileSheetSourceRect; // We just need the dimensions which we assume are all the same
            SVector2 scaledSpriteSize = new SVector2(spriteRect.Width * spriteScale, spriteRect.Height * spriteScale);

            // The longest length of text will help us determine how wide the tooltip box should be 
            SVector2 titleSize = SVector2.MeasureString(title, Game1.smallFont);
            SVector2 maxTextSize = (titleSize.x - scaledSpriteSize.x > giftInfo.MaxGiftNameSize.x) ? titleSize : giftInfo.MaxGiftNameSize;

            SVector2 mouse = new SVector2(Game1.getOldMouseX(), Game1.getOldMouseY());

            int padding = 4;
            int rowHeight = (int)Math.Max(maxTextSize.y * ZoomLevel, scaledSpriteSize.yi) + padding;
            int width = AdjustForTileSize((maxTextSize.x * ZoomLevel) + scaledSpriteSize.xi) + padding;
            int height = AdjustForTileSize(rowHeight * (numItems + 1), 0.5f); // Add one to make room for the title
            int x = AdjustForTileSize(mouse.x, 0.5f, ZoomLevel) - width;
            int y = AdjustForTileSize(mouse.y, 0.5f, ZoomLevel);

            int viewportW = Game1.viewport.Width;
            int viewportH = Game1.viewport.Height;

            // Approximate where the original tooltip will be positioned
            SVector2 origHoverTextSize = SVector2.MeasureString(calendar.GetCurrentHoverText(), Game1.dialogueFont);
            int origTToffsetX = Math.Max(0, GiftHelper.AdjustForTileSize(origHoverTextSize.x + mouse.x, 1.0f) - viewportW);

            // Consider the position of the original tooltip and ensure we don't cover it up
            SVector2 tooltipPos = ClampToViewport(x - origTToffsetX, y, width, height, viewportW, viewportH);

            // Draw the tooltip
            DrawGiftTooltip(giftInfo, title, tooltipPos);
        }

        private SVector2 ClampToViewport(int x, int y, int w, int h, int viewportW, int viewportH)
        {
            SVector2 p = new SVector2(x, y);

            p.x = ClampToViewportAxis(p.xi, w, viewportW);
            p.y = ClampToViewportAxis(p.yi, h, viewportH);

            // This mimics the regular tooltip behaviour; moving them out of the cursor's way slightly
            int halfTileSize = AdjustForTileSize(0.0f);
            p.y -= (p.x != x) ? halfTileSize : 0;
            p.x -= (p.y != y) ? halfTileSize : 0;

            return p;
        }

        private int ClampToViewportAxis(int a, int l1, int l2)
        {
            int ca = Utils.Clamp(a, 0, a);
            if (ca + l1 > l2)
            {
                // Offset by how much it extends past the viewport
                int diff = (ca + l1) - l2;
                ca -= diff;
            }
            return ca;
        }
    }
}
