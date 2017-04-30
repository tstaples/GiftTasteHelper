**Gift Taste Helper** is a [Stardew Valley](http://stardewvalley.net/) mod that shows a helpful
tooltip with a villager's favourite gifts when you point at them on the calendar or (thanks to
[dreamsicl](https://github.com/dreamsicl)) on the social page. It won't show the two universal
loved items.

![Calendar preview image](images/calendar_example.png?raw=true)

![Social page preview image](images/social_page_example.png?raw=true)

## Contents
* [Install](#install)
* [Configure](#configure)
* [Future features](#future-features)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Unzip [the mod folder](https://github.com/tstaples/GiftTasteHelper/releases) into your `Mods` folder.
3. Run the game using SMAPI.

## Configure
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file in a text editor. (The file might not appear until you've run the game once with the mod.)

Available settings:

setting           | what it affects
:---------------- | :------------------
`ShowOnCalendar` | Default `true`. Whether the tooltip should be displayed on the calendar.
`ShowOnSocialPage` | Default `true`. Whether the tooltip should be displayed on the social page.
`MaxGiftsToDisplay` | Default unlimited. The maximum number of gifts to list in the tooltip (or `0` for unlimited).

## Future features
* Display more than just loved gifts (if I can find a way to do it without cluttering the UI).
* Add config option to change tooltip to only show known gifts.

## See also
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/229)
* [Discussion thread](http://community.playstarbound.com/threads/npc-gift-taste-helper.112180/)
