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
        protected Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        protected NPCGiftInfo currentGiftInfo = null;
        protected bool drawCurrentFrame = false;
        protected bool isInitialized = false;
        protected bool isOpen = false;

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

        public virtual void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            // Empty
        }

        public virtual void OnDraw()
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
    }
}
