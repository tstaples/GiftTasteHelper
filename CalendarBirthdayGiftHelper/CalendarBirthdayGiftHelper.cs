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
                    // NPCGiftTastes format:
                    // Love_gifts_text / Loved_items / Liked_Gift_Text / Liked_Items / Disliked_gifts_text / Disliked_items / Hated_gifts_text / Hated_Items / Neutral_Gifts_Text / Neutral_Items
                    //
                    // NPC tastes take precedence over universal; TODO: check against personal if we're going to use universal too

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
                    //currentGiftInfo = npcGiftInfo["Penny"]; // Temp for testing since she has the most items
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
            int numItems = giftInfo.FavouriteGifts.Length;
            if (numItems == 0)
                return;

            Vector2 maxTextSize = Game1.smallFont.MeasureString(giftInfo.LongestGiftName);
            Rectangle spriteRect = giftInfo.FavouriteGifts[0].tileSheetSourceRect;
            float spriteScale = 2.0f;

            int padding = 4;
            int rowHeight = Math.Max((int)maxTextSize.Y, (int)(spriteRect.Height * spriteScale)) + padding;
            int width = AdjustForZoom(maxTextSize.X + (spriteRect.Width * spriteScale) + padding);
            int height = AdjustForZoom(rowHeight * numItems);
            int x = AdjustForZoom(Game1.getOldMouseX());
            int y = AdjustForZoom(Game1.getOldMouseY());

            Rectangle tooltipBox = ClampToViewport(x, y, width, height);

            SpriteBatch spriteBatch = Game1.spriteBatch;
            DrawTooltipBackground(spriteBatch, tooltipBox);

            Vector2 spriteOffset = new Vector2(AdjustForZoom(tooltipBox.X, 0.25f), AdjustForZoom(tooltipBox.Y, 0.25f));
            Vector2 textOffset = new Vector2(spriteOffset.X + (spriteRect.Width * spriteScale) + padding, spriteOffset.Y + (spriteRect.Height / 2));

            for (int i=0; i < numItems; ++i)
            {
                NPCGiftInfo.ItemData item = giftInfo.FavouriteGifts[i];
                spriteBatch.Draw(Game1.objectSpriteSheet, spriteOffset, item.tileSheetSourceRect, Color.White, 0.0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(Game1.smallFont, item.name, textOffset, Game1.textColor);

                spriteOffset.Y += rowHeight;
                textOffset.Y += rowHeight;
            }
        }

        private void DrawTooltipBackground(SpriteBatch spriteBatch, Rectangle r)
        {
            Rectangle menuTextureSourceRect = new Rectangle(0, 256, 60, 60);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, menuTextureSourceRect, r.X, r.Y, r.Width, r.Height, Color.White);
        }

        private int AdjustForZoom(float v, float tileSizeMod=0.5f)
        {
            float tileSize = (float)Game1.tileSize * tileSizeMod;
            return (int)((v + tileSize) * Game1.options.zoomLevel);
        }

        private Rectangle ClampToViewport(int x, int y, int w, int h)
        {
            Rectangle r = new Rectangle(x, y, w, h);

            int quarterTileSize = AdjustForZoom(0.0f, 0.25f);
            int vw = (int)(((float)Game1.viewport.Width) / Game1.options.zoomLevel);
            int vh = (int)(((float)Game1.viewport.Height) / Game1.options.zoomLevel);

            r.X = ClampToViewportAxis(r.X, r.Width, vw);
            r.Y = ClampToViewportAxis(r.Y, r.Height, vh);

            return r;
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
