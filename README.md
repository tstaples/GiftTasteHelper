Stardew Valley NPC Gift Taste Helper
====================================

This mod displays a helpful tooltip that shows an NPC's favourite gifts when hovering over their birthday on the calendar.
This works with both the calendar furniture item that you can have in your house, as well as the calendar on the billboard in town.

Note that it currently does not include the two universal loved items, but may in the future.

![Calendar preview image](images/calendar_example.png?raw=true)

**NEW**: tooltips now appear on the social page thanks to [dreamsicl](https://github.com/dreamsicl)!

![Social page preview image](images/social_page_example.png?raw=true)

## Requirements

This mod works with the most recent version of Stardew Valley (last tested with 1.1).

This mod requires [SMAPI](https://github.com/ClxS/SMAPI) __1.1 or higher to run__. The lastest version it was updated for is **1.2**.

For instructions on installing SMAPI view [this help page](http://canimod.com/guides/using-mods#installing-smapi).

# Legacy compatiblity

NOTE: SMAPI 40.x and earlier support is no longer supported. Use at your own risk!

If you are unable to update SMAPI due to existing mods breaking then you can get the SMAPI 39.2 compatible version [here](https://github.com/tstaples/GiftTasteHelper/releases/tag/0.9).
Just note that this version will be less likely to be maintained in the future. I still highly recommend updating SMAPI to the latest version if possible.

## Installation

1. Download the latest release [here](https://github.com/tstaples/GiftTasteHelper/releases).
2. Unzip the contents into your Mods folder which is located in the same directory as your Stardew Valley.exe and StardewValleyModdingAPI.exe.

To uninstall this mod you can simply delete the "GiftTasteHelper" folder from your Mods directory.

## Configuration

In this mod's `config.json` file you will find options to enable/disable displaying the tooltip on the calendar and social page. There is also an option for specifying the maximum number of gifts to display on the tooltip.

The config file is located in the `Mods/GiftTasteHelper` directory under your game install. To find your game directory click [here](http://canimod.com/guides/smapi-faq#where-is-my-game-directory).

To edit the file just open it in any text editor.

The config will look like:

```
{
	"ShowOnCalendar": true,
	"ShowOnSocialPage": true,
	"MaxGiftsToDisplay": 0 // 0 means no limit.
}
```

Just replace `true` with `false` and save the file (game must be restarted for changes to take effect) for the menus you don't want to see the tooltip on and visa versa.

## Future Features

- ~~Displaying the tooltip when hovering over the gift icon in the social tab.~~
- Display more than just loved gifts (if I can find a way to do it without cluttering the UI).
- ~~Add config options to enable/disable showing the tooltip in the calendar/social page.~~
- Add config option to change tooltip to only show known gifts.