using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private IGiftDatabase GiftDatabase;
        private IGiftMonitor GiftMonitor;
        private bool ReloadHelpers = false;
        private bool WasResized;
        private bool CheckGiftGivenNextInput = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Set the monitor ref so we can have a cheeky global log function
            Utils.InitLog(this.Monitor);

            this.Config = helper.ReadConfig<ModConfig>();

            this.GiftMonitor = new GiftMonitor();
            this.GiftMonitor.GiftGiven += OnGiftGiven;

            // Wait until after the save is loaded before loading the helpers.
            this.ReloadHelpers = true;

            GraphicsEvents.Resize += (sender, e) => this.WasResized = true;
            ContentEvents.AfterLocaleChanged += (sender, e) => LoadGiftHelpers(helper);
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            SaveEvents.AfterReturnToTitle += (sender, e) => OnReturnToTitleScreen();
            TimeEvents.AfterDayStarted += (sender, e) =>
            {
                if (Game1.dayOfMonth == 1 && this.GiftHelpers.ContainsKey(typeof(Billboard)))
                {
                    // Reset the birthdays when season changes
                    this.GiftHelpers[typeof(Billboard)].Reset();
                }
                this.GiftMonitor.Reset();
            };

            InitDebugCommands(helper);
        }

        private void OnReturnToTitleScreen()
        {
            // A different save may be chosen which can have different progress, so we need to reload the db and helpers.
            // TODO: Maybe add a shared progress option in the future.
            UnsubscribeEvents();
            this.ReloadHelpers = true;
            this.CurrentGiftHelper = null;

            MenuEvents.MenuClosed -= OnClickableMenuClosed;
            MenuEvents.MenuChanged -= OnClickableMenuChanged;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            GiftMonitor.Load();

            if (this.ReloadHelpers)
            {
                Utils.DebugLog("Reloading gift helpers");
                LoadGiftHelpers(Helper);
                this.ReloadHelpers = false;
            }
        }

        private void LoadGiftHelpers(IModHelper helper)
        {
            Utils.DebugLog("Initializing gift helpers");

            // TODO: add a reload method to the gift helpers instead of fully re-creating them
            // Force the gift info to be rebuilt
            IGiftDataProvider dataProvider = null;
            if (Config.ShowOnlyKnownGifts)
            {
                string path = this.Config.ShareKnownGiftsWithAllSaves
                    ? Path.Combine(StoredGiftDatabase.DBRoot, StoredGiftDatabase.DBFileName)
                    : Path.Combine(StoredGiftDatabase.DBRoot, Constants.SaveFolderName, StoredGiftDatabase.DBFileName);

                GiftDatabase = new StoredGiftDatabase(helper, path);
                dataProvider = new ProgressionGiftDataProvider(GiftDatabase);
                ControlEvents.MouseChanged += CheckGiftGivenAfterMouseChanged;
                ControlEvents.ControllerButtonPressed += CheckGiftGivenAfterControllerButtonPressed;
            }
            else
            {
                GiftDatabase = new GiftDatabase(helper);
                dataProvider = new AllGiftDataProvider(GiftDatabase);
                ControlEvents.MouseChanged -= CheckGiftGivenAfterMouseChanged;
                ControlEvents.ControllerButtonPressed -= CheckGiftGivenAfterControllerButtonPressed;
            }

            // Add the helpers if they're enabled in config
            CurrentGiftHelper = null;
            this.GiftHelpers = new Dictionary<Type, IGiftHelper>();
            if (Config.ShowOnCalendar)
            {
                this.GiftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(dataProvider, Config, helper.Reflection, helper.Translation));
            }
            if (Config.ShowOnSocialPage)
            {
                this.GiftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(dataProvider, Config, helper.Reflection, helper.Translation));
            }

            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
        }

        // ===================================================================
        // Gift Monitor Handling
        // ===================================================================

        private void OnGiftGiven(string npc, int itemId)
        {
            var taste = Utils.GetTasteForGift(npc, itemId);
            if (taste != GiftTaste.MAX)
            {
                Utils.DebugLog($"Gift given to Npc: {npc} | item: {itemId} | taste: {taste}");
                this.GiftDatabase.AddGift(npc, itemId, taste);
            }

            this.CheckGiftGivenNextInput = false;
        }

        private void CheckGiftGivenAfterControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            // Giving a gift with a controller is done on button down so we can't use the same
            // trick to do the check if the gift was given. Since a dialogue is always opened after giving a gift
            // the user has to press something to proceed so it's during that input that we'll do the check (if the flag is set).
            if (e.ButtonPressed == Buttons.A)
            {
                this.GiftMonitor.UpdateHeldGift();
                this.CheckGiftGivenNextInput = this.GiftMonitor.IsHoldingValidGift;
            }

            if (this.CheckGiftGivenNextInput)
            {
                this.GiftMonitor.CheckGiftGiven();
            }
        }

        private void CheckGiftGivenAfterMouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton != e.PriorState.RightButton && e.NewState.RightButton == ButtonState.Pressed)
            {
                this.GiftMonitor.UpdateHeldGift();
            }
            else if (e.NewState.RightButton != e.PriorState.RightButton && e.NewState.RightButton == ButtonState.Released)
            {
                this.GiftMonitor.CheckGiftGiven();
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
                    this.GiftMonitor.Reset();
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
                    try
                    {
                        x = int.Parse(args[1]);
                        y = int.Parse(args[2]);
                    }                    
                    catch (Exception)
                    {
                        Utils.DebugLog("Error parsing params", LogLevel.Error);
                    }
                }
                Game1.warpFarmer(location, x / Game1.tileSize, y / Game1.tileSize, false);
            });

            helper.ConsoleCommands.Add("setup", "", (name, args) =>
            {
                helper.ConsoleCommands.Trigger("world_settime", new string[] { "1000" });
                helper.ConsoleCommands.Trigger("teleport", new string[] { "SamHouse", "306", "339" });
                var items = new int[] { 74, 773, 417, 324, 92, 220, 176, 417, 404, 22, 18 }; // Test items for all of jodi's tastes.
                foreach (var item in items)
                {
                    helper.ConsoleCommands.Trigger("player_add", new string[] { "Object", item.ToString(), "10" });
                }
            });
#endif
        }
        #endregion
    }
}
