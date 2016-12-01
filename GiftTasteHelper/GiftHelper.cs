using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    public abstract class GiftHelper : IGiftHelper
    {
        public enum EGiftHelperType
        {
            GHT_Calendar,
            GHT_SocialPage
        }

        protected Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        protected NPCGiftInfo currentGiftInfo = null;
        private SVector2 origHoverTextSize;
        protected bool drawCurrentFrame = false;
        protected bool isInitialized = false;
        protected bool isOpen = false;
        public string TooltipTitle { get; protected set; } = "Favourite Gifts";

        public EGiftHelperType GiftHelperType { get; private set; }

        public GiftHelper(EGiftHelperType helperType)
        {
            GiftHelperType = helperType;
        }

        public float ZoomLevel
        {
            // SMAPI's draw call will handle zoom
            get { return 1.0f; }
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }

        public bool IsOpen()
        {
            return isOpen;
        }

        public virtual void Init(IClickableMenu menu)
        {
            if (isInitialized)
            {
                Utils.DebugLog("BaseGiftHelper already initialized; skipping");
                return;
            }

            npcGiftInfo = new Dictionary<string, NPCGiftInfo>();

            // TODO: filter out names that will never be used
            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            foreach (KeyValuePair<string, string> giftTaste in npcGiftTastes)
            {
                // The first few elements are universal_tastes and we only want names.
                // None of the names contain an underscore so we can check that way.
                string npcName = giftTaste.Key;
                if (npcName.IndexOf('_') != -1)
                    continue;

                string[] giftTastes = giftTaste.Value.Split(new char[] { '/' });
                if (giftTastes.Length > 0)
                {
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    npcGiftInfo[npcName] = new NPCGiftInfo(npcName, favouriteGifts);
                }
            }

            isInitialized = true;
        }

        public virtual bool OnOpen(IClickableMenu menu)
        {
            currentGiftInfo = null;
            isOpen = true;

            return true;
        }

        public virtual void OnResize(IClickableMenu menu)
        {
            // Empty
        }

        public virtual void OnClose()
        {
            currentGiftInfo = null;
            drawCurrentFrame = false;
            isOpen = false;
        }

        public virtual bool CanTick()
        {
            return true;
        }

        public virtual void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            // Empty
        }

        public virtual bool CanDraw()
        {
            // Double check here since we may not be unsubscribed from post render right away when the calendar closes
            return (drawCurrentFrame && currentGiftInfo != null);
        }

        public virtual void OnDraw()
        {
            DrawGiftTooltip(currentGiftInfo, TooltipTitle);
        }

        public static int AdjustForTileSize(float v, float tileSizeMod = 0.5f, float zoom = 1.0f)
        {
            float tileSize = (float)Game1.tileSize * tileSizeMod;
            return (int)((v + tileSize) * zoom);
        }

        protected virtual void AdjustTooltipPosition(ref int x, ref int y, int width, int height, int viewportW, int viewportHeight)
        {
            // Empty
        }

        protected void DrawText(string text, SVector2 pos)
        {
            Game1.spriteBatch.DrawString(Game1.smallFont, text, pos.ToXNAVector2(), Game1.textColor, 0.0f, Vector2.Zero, ZoomLevel, SpriteEffects.None, 0.0f);
        }

        protected void DrawTexture(Texture2D texture, SVector2 pos, Rectangle source, float scale = 1.0f)
        {
            Game1.spriteBatch.Draw(texture, pos.ToXNAVector2(), source, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        public void DrawGiftTooltip(NPCGiftInfo giftInfo, string title, string originalTooltipText = "")
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

            int padding = 4; // Chosen by fair dice roll
            int rowHeight = (int)Math.Max(maxTextSize.y * ZoomLevel, scaledSpriteSize.yi) + padding;
            int width = AdjustForTileSize((maxTextSize.x * ZoomLevel) + scaledSpriteSize.xi) + padding;
            int height = AdjustForTileSize(rowHeight * (numItems + 1), 0.5f); // Add one to make room for the title
            int x = AdjustForTileSize(mouse.x, 0.5f, ZoomLevel);
            int y = AdjustForTileSize(mouse.y, 0.5f, ZoomLevel);

            int viewportW = Game1.viewport.Width;
            int viewportH = Game1.viewport.Height;

            // Let derived classes adjust the positioning
            AdjustTooltipPosition(ref x, ref y, width, height, viewportW, viewportH);

            // Approximate where the original tooltip will be positioned if there is an existing one we need to account for
            int origTToffsetX = 0;
            origHoverTextSize = SVector2.MeasureString(originalTooltipText, Game1.dialogueFont);
            if (origHoverTextSize.x > 0)
            {
                origTToffsetX = Math.Max(0, AdjustForTileSize(origHoverTextSize.x + mouse.x, 1.0f) - viewportW) + width;
            }

            // Consider the position of the original tooltip and ensure we don't cover it up
            SVector2 tooltipPos = ClampToViewport(x - origTToffsetX, y, width, height, viewportW, viewportH);

            // Reduce the number items shown if it will go off screen.
            // TODO: add a scrollbar or second column
            if (height > viewportH)
            {
                numItems = (viewportH / rowHeight) - 1; // Remove an item to make space for the title
                height = AdjustForTileSize(rowHeight * numItems);
            }

            // Draw the background of the tooltip
            SpriteBatch spriteBatch = Game1.spriteBatch;

            // Part of the spritesheet containing the texture we want to draw
            Rectangle menuTextureSourceRect = new Rectangle(0, 256, 60, 60);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, menuTextureSourceRect, tooltipPos.xi, tooltipPos.yi, width, height, Color.White, ZoomLevel);
            
            // Offset the sprite from the corner of the bg, and the text to the right and centered vertically of the sprite
            SVector2 spriteOffset = new SVector2(AdjustForTileSize(tooltipPos.x, 0.25f), AdjustForTileSize(tooltipPos.y, 0.25f));
            SVector2 textOffset = new SVector2(spriteOffset.x, spriteOffset.y + (spriteRect.Height / 2));

            // Draw the title then set up the offset for the remaining text
            DrawText(title, textOffset);
            textOffset.x += scaledSpriteSize.x + padding;
            textOffset.y += rowHeight;
            spriteOffset.y += rowHeight;

            for (int i = 0; i < numItems; ++i)
            {
                NPCGiftInfo.ItemData item = giftInfo.FavouriteGifts[i];

                // Draw the sprite for the item then the item text
                DrawText(item.name, textOffset);
                DrawTexture(Game1.objectSpriteSheet, spriteOffset, item.tileSheetSourceRect, spriteScale);

                // Move to the next row
                spriteOffset.y += rowHeight;
                textOffset.y += rowHeight;
            }
        }

        public SVector2 ClampToViewport(int x, int y, int w, int h, int viewportW, int viewportH)
        {
            SVector2 p = new SVector2(x, y);

            p.x = ClampToViewportAxis(p.xi, w, viewportW);
            p.y = ClampToViewportAxis(p.yi, h, viewportH);

            // Only adjust the position if there's another tooltip that we need to adjust for.
            if (!origHoverTextSize.IsZero())
            {
                // This mimics the regular tooltip behaviour; moving them out of the cursor's way slightly
                int halfTileSize = AdjustForTileSize(0.0f);
                p.y -= (p.x != x) ? halfTileSize : 0;
                p.x -= (p.y != y) ? halfTileSize : 0;
            }
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

