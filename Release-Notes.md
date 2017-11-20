# Release Notes

## Upcoming
* Option to only show gifts if you're above a certain heart level with that NPC.
* Option to show more than just loved gifts in the tooltip.

### 2.6.4
* Fixed an exception when hovering over unknown NPC's in the social page.
* Fixed an exception when giving a gift to an NPC you haven't spoken to before.
* Fixed an issue where the gift database would fail to load when using the Categorize Chests mod.

### 2.6.3
* Fixed an exception when giving an NPC an invalid item.

### 2.6.2
* Fixed an issue where an exception would be thrown if you had an NPC with no gift taste data in your save.
* Fixed an issue where the calendar wouldn't update live when the season changed.

### 2.6.1
* Added missing translation files.

## 2.6
* New optional 'progression' mode where you will only see the loved gifts for gifts you have already given.
* Config option to switch between sharing known gifts between saves and storing it per-save.
* Config option to show universally loved gifts.
* Locale fixes.
* Switched to using SMAPI's locale system.
* Code refactoring and cleanup.

## 2.5
* Full localization support - Item names and the tooltip title now display in the selected language.
* Fixed an issue where the gift tooltip wouldn't appear in non-English languages.
* Minor optimization.

## 2.4
* Fixes for SDV 1.2 and SMAPI 1.10. Special thanks to Pathoschild for doing this update.

## 2.3
* Fixes the error when opening the JunimoNoteMenu from the inventory.
* Adds a config option to specify the max number of gifts to display on the tooltip.

## 2.2
This release contains various bug fixes and code improvements as well as some config options to enable/disable the tooltips on the calendar and social page.
* Added config settings via config.json to enable/disable the tooltips on the calendar and social page. For info on how to use this please view the README.
* Social page tooltips now scale/position correctly with the zoom level.
* Calendar tooltips now appear/disappear at the same time as the original tooltip.
* Calendar tooltips no longer get clipped on the left side of the viewport.

## 2.1
This release contains fixes to make it compatible with SMAPI 1.1+ (test with both 1.1 and 1.2, will not work with 1.0), as well as a new feature that displays the tooltip on the social page.

## 1.1
Backwards compatibility with previous version of SMAPI down to 39.2 are now supported in the main build. In this build I also fixed some zoom issues that were happening with the 39.2 version.

## 1.0
Initial release.
* Currently works with the most recent version of StardewValley as well as the 1.07 beta.
* Compatible with SMAPI 39.5 and a few previous versions.