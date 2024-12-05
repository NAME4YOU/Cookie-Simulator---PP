# Cookie Simulator Game

This is a Cookie Simulator Game developed in C# for the console. It includes features like upgrades, achievements, events, power-ups, a currency system, and a filesystem-based "database" for saving and loading game progress.



## Features

- Upgrades

Upgrades improve your cookie production rate (Cookies Per Second - CPS). Each upgrade increases your CPS by a fixed amount and has a cost that must be paid in cookies. The cost of upgrades increases incrementally after each purchase.

- Menu

The main menu allows players to interact with different parts of the game. Type menu in the console to open it.

- Cookie Every Second System

Cookies are automatically baked every second based on the formula:

The baked cookies are added to your total cookies, and the elapsed time counter is incremented. This system also checks for achievements and triggers random events.

- Currency System

The game uses cookies as its currency. Cookies can be earned by:

Automatic baking (Cookies Per Second system).

Purchasing upgrades and shop items.

Power-ups that temporarily boost cookie production.

Cookies are deducted when making purchases or during certain events.

- Shop

The shop offers items that provide persistent boosts to CPS. Items increase in cost after each purchase by a multiplier of 1.15.

- Achievements

Achievements are unlocked when certain milestones are reached. They provide no direct benefits but reward players for progress.

- Events

Events are triggered randomly during gameplay. They can either benefit or penalize the player.

- Filesystem "Database"

Game data is saved to a JSON file (cookie_clicker_save.json) in the local filesystem. The game saves automatically every 5 seconds and loads progress on startup.

If no save file is found, a new one is created with default values.

- Power-Ups

Power-ups are temporary boosts that multiply cookie production for a limited time.

## Authors

- [@NAME4YOU](https://github.com/NAME4YOU)
