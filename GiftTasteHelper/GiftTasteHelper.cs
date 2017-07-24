using System;
using System.Collections.Generic;
using System.Diagnostics;
using GiftTasteHelper.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace GiftTasteHelper
{
    internal class GiftTasteHelper : Mod
    {
        /*********
        ** Properties
        *********/
        private ModConfig Config;
        private Dictionary<Type, IGiftHelper> GiftHelpers;
        private IGiftHelper CurrentGiftHelper;
        private bool WasResized;
        private IGiftDatabase GiftDatabase;

        private uint PriorGiftsGiven;
        private StardewValley.Object HeldGift = null;
        private Dictionary<string, bool> GiftsGivenToday;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Set the monitor ref so we can have a cheeky global log function
            Utils.InitLog(this.Monitor);

            Config = helper.ReadConfig<ModConfig>();

            IGiftDataProvider dataProvider = null;
            if (Config.ShowOnlyKnownGifts)
            {
                GiftDatabase = new StoredGiftDatabase(helper);
                dataProvider = new ProgressionGiftDataProvider(GiftDatabase);
                ControlEvents.MouseChanged += CheckGiftGiven;
            }
            else
            {
                GiftDatabase = new GiftDatabase(helper);
                dataProvider = new AllGiftDataProvider(GiftDatabase);
            }

            // Add the helpers if they're enabled in config
            this.GiftHelpers = new Dictionary<Type, IGiftHelper>();
            if (Config.ShowOnCalendar)
                this.GiftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(dataProvider, Config.MaxGiftsToDisplay, helper.Reflection));
            if (Config.ShowOnSocialPage)
                this.GiftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(dataProvider, Config.MaxGiftsToDisplay, helper.Reflection));

            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
            GraphicsEvents.Resize += (sender, e) => this.WasResized = true;
            ContentEvents.AfterLocaleChanged += (sender, e) => GiftHelper.ReloadGiftInfo(dataProvider, Config.MaxGiftsToDisplay);            
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            TimeEvents.AfterDayStarted += (sender, e) => RebuildGiftsGiven();

            InitDebugCommands(helper);
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            PriorGiftsGiven = Game1.stats.GiftsGiven;
            RebuildGiftsGiven();
        }

        private void RebuildGiftsGiven()
        {
            GiftsGivenToday = new Dictionary<string, bool>();
            foreach (var friendpair in Game1.player.friendships)
            {
                // Third element is whether a gift has been given today.
                GiftsGivenToday.Add(friendpair.Key, friendpair.Value[3] > 0);
            }
        }

        private void CheckGiftGiven(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton != e.PriorState.RightButton && e.NewState.RightButton == ButtonState.Pressed)
            {
                if (Game1.player.ActiveObject != null && Game1.player.ActiveObject.canBeGivenAsGift())
                {
                    HeldGift = Game1.player.ActiveObject;
                }
            }
            else if (e.NewState.RightButton != e.PriorState.RightButton && e.NewState.RightButton == ButtonState.Released)
            {
                if (HeldGift == null)
                    return;

                Utils.DebugLog("Clicked with gift in hand");
                if (Game1.stats.GiftsGiven != PriorGiftsGiven)
                {
                    Utils.DebugLog($"GiftsGiven changed from {PriorGiftsGiven} to {Game1.stats.GiftsGiven}");
                    Utils.DebugLog($"Given item: {HeldGift.DisplayName}");

                    string npcGivenTo = null;
                    foreach (var friendpair in Game1.player.friendships)
                    {
                        bool givenToday = friendpair.Value[3] > 0;
                        if (GiftsGivenToday[friendpair.Key] != givenToday)
                        {
                            GiftsGivenToday[friendpair.Key] = true;
                            npcGivenTo = friendpair.Key;
                            this.GiftDatabase.AddGift(npcGivenTo, this.HeldGift.ParentSheetIndex);
                            break;
                        }
                    }

                    Utils.DebugLog($"Gift given to {npcGivenTo}");
                    PriorGiftsGiven = Game1.stats.GiftsGiven;
                    HeldGift = null;
                }
            }
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

        #region Debug
        void InitDebugCommands(IModHelper helper)
        {
#if DEBUG
            helper.ConsoleCommands.Add("resetgifts", "Reset gifts", (name, args) =>
            {
                foreach (var friendship in Game1.player.friendships)
                {
                    friendship.Value[1] = 0;
                    friendship.Value[3] = 0;
                    RebuildGiftsGiven();
                }
            });

            helper.ConsoleCommands.Add("printcoords", "asdf", (name, args) =>
            {
                Utils.DebugLog($"Player coords: {Game1.player.position} | location: {Game1.player.currentLocation.name}");
            });

            helper.ConsoleCommands.Add("teleport", "", (name, args) =>
            {
                string location = args.Length > 0 ? args[0] : "Town";
                int x = 635, y = 5506;
                if (args.Length == 3)
                {
                    x = int.Parse(args[1]);
                    y = int.Parse(args[2]);
                }
                Game1.warpFarmer(location, x / Game1.tileSize, y / Game1.tileSize, false);
            });

            helper.ConsoleCommands.Add("setup", "", (name, args) =>
            {
                helper.ConsoleCommands.Trigger("world_settime", new string[] { "1000" });
                helper.ConsoleCommands.Trigger("teleport", new string[] { "SamHouse", "306", "339" });
            });
#endif
        }
        #endregion
    }
}
