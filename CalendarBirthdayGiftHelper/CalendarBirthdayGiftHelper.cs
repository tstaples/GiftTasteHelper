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
using StardewModdingAPI.Inheritance;
using StardewModdingAPI.Inheritance.Menus;

namespace CalendarBirthdayGiftHelper
{
    public class CalendarBirthdayGiftHelper : Mod
    {

        private Dictionary<string, NPCGiftInfo> npcGiftInfo; // Indexed by name
        private Calendar calendar = new Calendar();
        private string previousHoverText;
        private NPCGiftInfo currentGiftInfo = null; // Info for current day being hovered over

        private bool debug_mouseEvent = false;

        public override void Entry(params object[] objects)
        {
            MenuEvents.MenuClosed += OnClickableMenuClosed;
            MenuEvents.MenuChanged += OnClickableMenuChanged;
            GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent; // TODO: subscribe when a valid day is hovered over
            TimeEvents.SeasonOfYearChanged += OnSeasonChanged;
        }

        public void OnSeasonChanged(object sender, EventArgsStringChanged e)
        {
            if (calendar.IsInitialized)
            {
                // Force the calendar to reload the data for the new season
                calendar.Clear();
            }
        }

        public void OnClickableMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Log.Debug(e.PriorMenu.GetType().ToString() + " menu closed.");
            if (calendar.IsOpen)
            {
                Log.Debug("Calender was open; closing.");
                ControlEvents.MouseChanged -= OnMouseStateChange;
                debug_mouseEvent = false;
                calendar.IsOpen = false;
            }
        }

        public void OnClickableMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            DebugPrintMenuInfo(e.PriorMenu, e.NewMenu);
            
            if (!Utils.IsType<Billboard>(e.NewMenu) ||
                calendar.IsOpen && calendar.IsInitialized)
            {
                Log.Debug("New menu isn't a billboard or the calendar is already open and initialized.");
                return;
            }

            Log.Debug("Calender was opened");

            // Create our map and note when we did it. Reset everything else
            npcGiftInfo = new Dictionary<string, NPCGiftInfo>();
            calendar.Init((Billboard)e.NewMenu);
            previousHoverText = ""; // reset
            currentGiftInfo = null;

            calendar.IsOpen = true;
            Debug.Assert(!debug_mouseEvent, "Mouse change event is already subscribed");
            ControlEvents.MouseChanged += OnMouseStateChange;
            debug_mouseEvent = true;
            //GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent;

            Dictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
            List<Calendar.BirthdayEventInfo> birthdayEventInfo = calendar.GetNPCBirthdayEventInfo();
            foreach (Calendar.BirthdayEventInfo eventInfo in birthdayEventInfo)
            {
                if (npcGiftTastes.ContainsKey(eventInfo.npcName))
                {
                    string[] giftTastes = npcGiftTastes[eventInfo.npcName].Split(new char[] { '/' });
                    string[] favouriteGifts = giftTastes[1].Split(new char[] { ' ' });
                    //string[] goodGifts = giftTastes[3].Split(new char[] { ' ' });

                    npcGiftInfo[eventInfo.npcName] = new NPCGiftInfo(eventInfo.npcName, favouriteGifts/*, goodGifts*/);

                    //Log.Verbose("Favourite gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(favouriteGifts));
                    //Log.Verbose("Good gifts for {0}: {1}", eventInfo.npcName, Utils.ArrayToString(goodGifts));
                }
            }
        }

        public void OnMouseStateChange(object sender, EventArgsMouseStateChanged e)
        {
            Point newMouse = new Point(e.NewState.X, e.NewState.Y);
            Point oldMouse = new Point(e.PriorState.X, e.PriorState.Y);
            
            // TODO: move creation of regions etc into separate class so it only has to be done once
            // First pass to see if the user's mouse is even within the calendar
            if (!calendar.Bounds.Contains(newMouse))
                return;

            // Check if we're hovering over a day that has a birthday
            string hoverText = calendar.GetCurrentHoverText();
            if (hoverText.Length > 0 && hoverText.Contains("Birthday"))
            {
                // Check if it's the same as before
                if (hoverText != previousHoverText)
                {
                    Log.Debug("hover text: " + hoverText);

                    string npcName = Calendar.ParseNameFromHoverText(hoverText);
                    Debug.Assert(npcGiftInfo.ContainsKey(npcName));

                    currentGiftInfo = npcGiftInfo[npcName];
                    Log.Debug(npcName + " favourite gifts: " + Utils.ArrayToString(currentGiftInfo.FavouriteGifts));

                    previousHoverText = hoverText;
                }

                // TODO: draw the tooltip with the gift info
            }
            else
            {
                currentGiftInfo = null;
            }
        }

        private void OnPostRenderEvent(object sender, EventArgs e)
        {
            if (currentGiftInfo != null)
            {
                CreateGiftTooltip(currentGiftInfo);
            }
        }

        private void CreateGiftTooltip(NPCGiftInfo giftInfo)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;
            drawSimpleTooltipZoomAware(spriteBatch, giftInfo.FavouriteGifts[0].name, Game1.smallFont);
        }

        public static void drawSimpleTooltipZoomAware(SpriteBatch b, string hoverText, SpriteFont font)
        {
            int width = (int)((font.MeasureString(hoverText).X + Game1.tileSize / 2) * Game1.options.zoomLevel);
            int height = (int)(Math.Max(60, font.MeasureString(hoverText).Y + Game1.tileSize / 2) * Game1.options.zoomLevel); //60 is "cornerSize" * 3 on SDV source
            int x = (int)((Game1.getOldMouseX() + Game1.tileSize / 2) * Game1.options.zoomLevel);
            int y = (int)((Game1.getOldMouseY() + Game1.tileSize / 2) * Game1.options.zoomLevel);
            if (x + width > Game1.viewport.Width / Game1.options.zoomLevel)
            {
                x = (int)(Game1.viewport.Width / Game1.options.zoomLevel - width);
                y += (int)((Game1.tileSize / 4) * Game1.options.zoomLevel);
            }
            if (y + height > Game1.viewport.Height / Game1.options.zoomLevel)
            {
                x += (int)((Game1.tileSize / 4) * Game1.options.zoomLevel);
                y = (int)(Game1.viewport.Height / Game1.options.zoomLevel - height);
            }
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White, 1f, true);
            if (hoverText.Length > 1)
            {
                Vector2 tPosVector = new Vector2(x + (Game1.tileSize / 4) * Game1.options.zoomLevel, y + (Game1.tileSize / 4 + 4) * Game1.options.zoomLevel);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
            }
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
