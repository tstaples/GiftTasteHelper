using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CalendarBirthdayGiftHelper
{
    public abstract class GiftHelper : IGiftHelper
    {
        protected Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        protected NPCGiftInfo currentGiftInfo = null;
        protected bool drawCurrentFrame = false;
        protected bool isInitialized = false;

        public bool IsInitialized()
        {
            return isInitialized;
        }

        public virtual void Init(IClickableMenu menu)
        {
            if (isInitialized)
            {
                Log.Debug("BaseGiftHelper already initialized; skipping");
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
        }

        public virtual void OnMouseStateChange(EventArgsMouseStateChanged e)
        {
            // Empty
        }

        public virtual void OnDraw()
        {
            // Empty
        }
    }
}
