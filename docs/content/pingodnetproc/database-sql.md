---
title: "Database - Sql"
date: 2022-12-26T15:26:15Z
draft: false
weight: 11
---

The nuget package `NetProc.Data` will provide you with `.sql` files to seed database data.

 This creates a `sqlite` database and saves everything here from the game, from machine items to game audits and adjustments.

 - Switches
 - Coils
 - LEDS
 - Lamps
 - Audits
 - Adjustments
 - GamesPlayed
 - BallsPlayed
 - Players
 - Scores

---

You will edit one of the files depending on the machine type, but before you edit, copy the file and create your own into the `sql` directory. So usually you edit this file outside of Visual Studio.

*If you try and edit the file from the package it will never save it, so create your own*

![image](../../images/p-roc/database-sql.jpg)

If you're using a p3-roc or intend to just for simulating then the one we've used in this project is a good starter.

---

You can adjust everything in this template then create the database. This can easily be done from the `PinGodGame.tscn / PinGodGamePROC` by enabling delete on init and running the game.