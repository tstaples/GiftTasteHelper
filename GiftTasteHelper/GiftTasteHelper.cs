using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace GiftTasteHelper
{
    internal class GiftTasteHelper : Mod
    {
        /*********
        ** Properties
        *********/
        private Dictionary<Type, IGiftHelper> GiftHelpers;
        private IGiftHelper CurrentGiftHelper;
        private bool WasResized;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Set the monitor ref so we can have a cheeky global log function
            Utils.InitLog(this.Monitor);

            ModConfig config = helper.ReadConfig<ModConfig>();

            // Add the helpers if they're enabled in config
            this.GiftHelpers = new Dictionary<Type, IGiftHelper>();
            if (config.ShowOnCalendar)
                this.GiftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(config.MaxGiftsToDisplay));
            if (config.ShowOnSocialPage)
                this.GiftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(config.MaxGiftsToDisplay));

            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
            GraphicsEvents.Resize += (sender, e) => this.WasResized = true;
        }


        /*********
        ** Private methods
        *********/
        private void OnClickableMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Utils.DebugLog(e.PriorMenu.GetType() + " menu closed.");

            if (this.CurrentGiftHelper != null)
            {
                Utils.DebugLog("Closing current helper: " + this.CurrentGiftHelper.GetType());

                UnsubscribeEvents();

                this.CurrentGiftHelper.OnClose();
            }
        }

        private void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            //DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);

            Type newMenuType = e.NewMenu.GetType();

            if (this.WasResized && this.CurrentGiftHelper != null && this.CurrentGiftHelper.IsOpen &&
                e.PriorMenu != null && e.PriorMenu.GetType() == newMenuType)
            {
                // resize event
                Utils.DebugLog("[OnClickableMenuChanged] Invoking resize event on helper: " + this.CurrentGiftHelper.GetType());

                this.CurrentGiftHelper.OnResize(e.NewMenu);
                this.WasResized = false;
                return;
            }
            this.WasResized = false;


            if (this.GiftHelpers.ContainsKey(newMenuType))
            {
                // Close the current gift helper
                if (this.CurrentGiftHelper != null)
                {
                    Utils.DebugLog("[OnClickableMenuChanged] Closing current helper: " + this.CurrentGiftHelper.GetType());

                    UnsubscribeEvents();

                    this.CurrentGiftHelper.OnClose();
                }

                this.CurrentGiftHelper = this.GiftHelpers[newMenuType];
                if (!this.CurrentGiftHelper.IsInitialized)
                {
                    Utils.DebugLog("[OnClickableMenuChanged initialized helper: " + this.CurrentGiftHelper.GetType());

                    this.CurrentGiftHelper.Init(e.NewMenu);
                }

                if (this.CurrentGiftHelper.OnOpen(e.NewMenu))
                {
                    Utils.DebugLog("[OnClickableMenuChanged Successfully opened helper: " + this.CurrentGiftHelper.GetType());

                    // Only subscribe to the events if it opened successfully
                    SubscribeEvents();
                }
            }
        }

        private void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Debug.Assert(this.CurrentGiftHelper != null, "OnMouseStateChange listener invoked when currentGiftHelper is null.");

            if (this.CurrentGiftHelper.CanTick())
            {
                this.CurrentGiftHelper.OnMouseStateChange(e);
            }
        }

        private void OnDraw(object sender, EventArgs e)
        {
            Debug.Assert(this.CurrentGiftHelper != null, "OnPostRenderEvent listener invoked when currentGiftHelper is null.");

            if (this.CurrentGiftHelper.CanDraw())
            {
                this.CurrentGiftHelper.OnDraw();
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
    }
}
