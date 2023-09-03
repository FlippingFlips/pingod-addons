---
title: "Database - Sql"
date: 2022-12-26T15:26:15Z
draft: false
weight: 11
---

The nuget package `NetProc.Data` will provide you with `.sql` files to seed database data.

 This creates a `sqlite` database and saves everything here from the game, from machine items to game audits and adjustments.

 - Adjustments
 - Switches
 - Coils
 - LEDS
 - Lamps
 - Audits
 - GamesPlayed
 - BallsPlayed
 - Players
 - Scores

---

You will edit one of the files depending on the machine type, but before you edit, copy the file and create your own into the `sql` directory. So usually you edit this file outside of Visual Studio.

*If you try and edit the file from the package it will never save it, so create your own in same directory by copying the sql directory from the package*

![image](../../images/p-roc/database-sql.jpg)

If you're using a p3-roc or intend to just for simulating then the sql file we've used in this project is a good starter. (subject to change)

You can adjust everything in this template then create the database. This can easily be done from the `PinGodGame.tscn / PinGodGamePROC` by enabling delete on init and running the game.

### Development settings
---

To change the display settings in the database use the tool like `sqlitestudio` to edit rows in the `Adjustments` table under SubMenuName `DISPLAY` by loading the `netproc.db` into the application. Helpful when you want to run in a window and other settings.

### Quick overiew of tables
---

Besides the common `Switches, Coils, Leds, Lamps` some need a bit more explanation.

## Machine
---

Settings for the machine and the type of machine it is where the default is set to `7` for a PDB machine. Balls per machine for the trough and display monitor game runs on.

#### Adjustments
---

Game adjustments table with means to be shown in a UI.

Rows should have unique keys with their `MenuName` and `SubMenuName` to be used for service menus and the like. A default example would be a parent / child `STANDARD_ADJ` / `GENERAL`.

Value must be integer. `OptionType` is the type of options display in a UI.

* 0 = A range of ints like `8,30` or `0,15`
* 1 = List ints like `0,1,2,3` or `10,20,75`
* 2 = Enum like `NO,YES` which would be `0,1` to display text for option

#### Players
---

There is a player named `default` with initials (display name) set to `NETPROC`. Players should have a unique login name but display name (initials) could be the same as another player.

Players are used in the `BallsPlayed` and `Scores` tables.

#### Scores
---

Scores stored here against he `GamePlayedId` and `PlayerId` are storing the total points scored in that game.

#### Games Played
---

Games played just stored the start and end time of the game with the duration and the adjustments that were set.

#### Balls Played
---

Saves every balls score for each player and the duration.