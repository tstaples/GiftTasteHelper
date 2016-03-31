using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CalendarBirthdayGiftHelper
{
    public class CalendarBirthdayGiftHelper : Mod
    {
        private Dictionary<Type, IGiftHelper> giftHelpers;
        private IGiftHelper currentGiftHelper = null;

        public override void Entry(params object[] objects)
        {
            giftHelpers = new Dictionary<Type, IGiftHelper>(1)
            {
                {typeof(Billboard), new CalendarGiftHelper() }
            };

            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
        }

        public void OnClickableMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Log.Debug(e.PriorMenu.GetType().ToString() + " menu closed.");

            if (currentGiftHelper != null)
            {
                Log.Debug("[OnClickableMenuClosed] Closing current helper: " + currentGiftHelper.GetType().ToString());

                ControlEvents.MouseChanged -= OnMouseStateChange;
                GraphicsEvents.OnPostRenderEvent -= OnPostRenderEvent;

                currentGiftHelper.OnClose();
            }
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);

            Type newMenuType = e.NewMenu.GetType();
            if (currentGiftHelper != null && currentGiftHelper.IsOpen() && 
                e.PriorMenu != null && e.PriorMenu.GetType() == newMenuType)
            {
                // resize event
                Log.Debug("[OnClickableMenuChanged] Invoking resize event on helper: " + currentGiftHelper.GetType().ToString());

                currentGiftHelper.OnResize(e.NewMenu);
                return;
            }

            if (giftHelpers.ContainsKey(newMenuType))
            {
                // Close the current gift helper
                if (currentGiftHelper != null)
                {
                    Log.Debug("[OnClickableMenuChanged] Closing current helper: " + currentGiftHelper.GetType().ToString());

                    ControlEvents.MouseChanged -= OnMouseStateChange;
                    GraphicsEvents.OnPostRenderEvent -= OnPostRenderEvent;

                    currentGiftHelper.OnClose();
                }

                currentGiftHelper = giftHelpers[newMenuType];
                if (!currentGiftHelper.IsInitialized())
                {
                    Log.Debug("[OnClickableMenuChanged initialized helper: " + currentGiftHelper.GetType().ToString());

                    currentGiftHelper.Init(e.NewMenu);
                }

                if (currentGiftHelper.OnOpen(e.NewMenu))
                {
                    Log.Debug("[OnClickableMenuChanged Successfully opened helper: " + currentGiftHelper.GetType().ToString());

                    // Only subscribe to the events if it opened successfully
                    ControlEvents.MouseChanged += OnMouseStateChange;
                    GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent;
                }
            }
        }

        public void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Debug.Assert(currentGiftHelper != null, "OnMouseStateChange listener invoked when currentGiftHelper is null.");

            currentGiftHelper.OnMouseStateChange(e);
        }

        private void OnPostRenderEvent(object sender, EventArgs e)
        {
            Debug.Assert(currentGiftHelper != null, "OnPostRenderEvent listener invoked when currentGiftHelper is null.");

            currentGiftHelper.OnDraw();
        }

        private void DebugPrintMenuInfo(IClickableMenu priorMenu, IClickableMenu newMenu)
        {
            try
            {
                string priorName = "None";
                if (priorMenu != null)
                {
                    priorName = priorMenu.GetType().Name;
                }
                string newName = newMenu.GetType().Name;
                Log.Info("Menu changed from: " + priorName + " to " + newName);
            }
            catch (Exception ex)
            {
                Log.Debug("Error getting menu name: " + ex);
            }
        }

    }
}
