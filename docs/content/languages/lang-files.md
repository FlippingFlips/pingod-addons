---
title: "Language files"
date: 2022-12-26T15:26:15Z
draft: false
weight: 40
---

Add translations for the game. There are also defaults to cover most standard pinball messages in `addons\assets\Localization`

The default example covers English and French in `pindog_default_lang.csv`.

---
## ReImport
---

If the text doesn't update when you run the game:

1. Find the CSV in Godot file explorer from `addons\assets\Localization/pingod_default_lang.csv`.
2. Select the file, then look in the `Import` tab (next to scene top left)
3. Click Reimport making sure it's set as .csv, compressed

## Files

### YourGame/local

BasicGameLang.csv

```
keys,en
MY_MSG,Add key values for lanuguages
```

## Usage

### Godot Scripts

These can be used in Godot script by using the translation method `Tr("EXTRA_BALL")`

`_defaultText = Tr("BONUS_EOB");`

#### Replacing placeholders

CSV 

`HI_SCORE_ENTRY,PLAYER %d\r\nENTER NAME,JOUEUR %d\r\nENTRER LE NOM`

SCRIPT 

`playerMessageLabel.Text = Tr("HI_SCORE_ENTRY").Replace("%d", (CurrentPlayer + 1).ToString());`

### Scene text controls

They can also be used in control scenes. See the `AudioSettings.tscn` scene where `SETT_VOL_MUSIC` and others are used.

## Project.godot default translation

```
[locale]

translations=PoolStringArray( "res://addons/assets/Localization/pingod_default_lang.en.translation" )
locale_filter=[ 0, [  ] ]
```