using System;
using System.Diagnostics;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GiftTasteHelper
{
    public class GiftTasteHelper : Mod
    {
        private Dictionary<Type, IGiftHelper> giftHelpers;
        private IGiftHelper currentGiftHelper = null;

        public override void Entry(IModHelper helper)
        {
            // Set the monitor ref so we can have a cheeky global log function
            Utils.InitLog(this.Monitor);

            ModConfig config = helper.ReadConfig<ModConfig>();

            // Add the helpers if they're enabled in config
            giftHelpers = new Dictionary<Type, IGiftHelper>();
            if (config.ShowOnCalendar)
            {
                giftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(config.MaxGiftsToDisplay));
            }
            if (config.ShowOnSocialPage)
            {
                giftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(config.MaxGiftsToDisplay));
            }

            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
        }

        private void OnClickableMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Utils.DebugLog(e.PriorMenu.GetType().ToString() + " menu closed.");

            if (currentGiftHelper != null)
            {
                Utils.DebugLog("Closing current helper: " + currentGiftHelper.GetType().ToString());

                UnsubscribeEvents();

                currentGiftHelper.OnClose();
            }
        }

        private void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            //DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);

            Type newMenuType = e.NewMenu.GetType();

            if (currentGiftHelper != null && currentGiftHelper.IsOpen() && 
                e.PriorMenu != null && e.PriorMenu.GetType() == newMenuType)
            {
                // resize event
                Utils.DebugLog("[OnClickableMenuChanged] Invoking resize event on helper: " + currentGiftHelper.GetType().ToString());

                currentGiftHelper.OnResize(e.NewMenu);
                return;
            }

            if (giftHelpers.ContainsKey(newMenuType))
            {
                // Close the current gift helper
                if (currentGiftHelper != null)
                {
                    Utils.DebugLog("[OnClickableMenuChanged] Closing current helper: " + currentGiftHelper.GetType().ToString());

                    UnsubscribeEvents();

                    currentGiftHelper.OnClose();
                }

                currentGiftHelper = giftHelpers[newMenuType];
                if (!currentGiftHelper.IsInitialized())
                {
                    Utils.DebugLog("[OnClickableMenuChanged initialized helper: " + currentGiftHelper.GetType().ToString());

                    currentGiftHelper.Init(e.NewMenu);
                }

                if (currentGiftHelper.OnOpen(e.NewMenu))
                {
                    Utils.DebugLog("[OnClickableMenuChanged Successfully opened helper: " + currentGiftHelper.GetType().ToString());

                    // Only subscribe to the events if it opened successfully
                    SubscribeEvents();
                }
            }
        }

        private void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Debug.Assert(currentGiftHelper != null, "OnMouseStateChange listener invoked when currentGiftHelper is null.");

            if (currentGiftHelper.CanTick())
            {
                currentGiftHelper.OnMouseStateChange(e);
            }
        }

        private void OnDraw(object sender, EventArgs e)
        {
            Debug.Assert(currentGiftHelper != null, "OnPostRenderEvent listener invoked when currentGiftHelper is null.");

            if (currentGiftHelper.CanDraw())
            {
                currentGiftHelper.OnDraw();
            }
        }

        private void UnsubscribeEvents()
        {
            ControlEvents.MouseChanged -= OnMouseStateChange;
            GraphicsEvents.OnPostRenderEvent -= OnDraw;
        }

        private void SubscribeEvents()
        {
            ControlEvents.MouseChanged += OnMouseStateChange;
            GraphicsEvents.OnPostRenderEvent += OnDraw;
        }

        private void DebugPrintMenuInfo(IClickableMenu priorMenu, IClickableMenu newMenu)
        {
        #if DEBUG
            try
            {
                string priorName = "None";
                if (priorMenu != null)
                {
                    priorName = priorMenu.GetType().Name;
                }
                string newName = newMenu.GetType().Name;
                Utils.DebugLog("Menu changed from: " + priorName + " to " + newName);
            }
            catch (Exception ex)
            {
                Utils.DebugLog("Error getting menu name: " + ex);
            }
        #endif
        }
    }
}
