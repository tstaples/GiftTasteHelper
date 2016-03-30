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

        public virtual void Init()
        {
            currentGiftInfo = null;
            if (npcGiftInfo != null)
            {
                Log.Debug("BaseGiftHelper already initialized; skipping");
                return;
            }

            //Debug.Assert(npcGiftInfo == null, "npcGiftInfo shouldn't be initialized if InitGiftData is being called");
            npcGiftInfo = new Dictionary<string, NPCGiftInfo>();

            // TODO: filter out names that will never be used
            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            foreach (KeyValuePair<string, string> giftTaste in npcGiftTastes)
            {
                string[] giftTastes = giftTaste.Value.Split(new char[] { '/' });
                string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });

                npcGiftInfo[giftTaste.Key] = new NPCGiftInfo(giftTaste.Key, favouriteGifts);
            }
        }

        public virtual void OnOpen(IClickableMenu menu)
        {
            currentGiftInfo = null;
        }

        public virtual void OnResize(IClickableMenu menu)
        {
        }

        public virtual void OnClose()
        {
            currentGiftInfo = null;
            drawCurrentFrame = false;
        }

        public virtual void OnMouseStateChange(EventArgsMouseStateChanged e)
        {

        }

        public virtual void OnDraw()
        {

        }
    }
}
