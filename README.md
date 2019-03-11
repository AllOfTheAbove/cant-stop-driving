# Can't Stop Driving

![screenshot](https://raw.githubusercontent.com/AllOfTheAbove/CantStopDriving/master/screenshot.png)

## Play

Please consider downloading lastest version on [our website]( http://cant-stop-driving.com/play.html).
You can build the game from Unity to your desired platform but the multiplayer won't work except on local networks.

## Roadmap

### 4.0.1
- Splash Tile : transparent + darker
- Prevent doing pause when gameover
- Explosions (No explosion in solo ?)
- Projectiles tile not working ?
- Improve scoreboard API
- Fix score Van
- Remove R key (or add debug only for devs) + B key
- Night mode more often
- Ignore solo scores
- Fix Pathfinding/Architect
- Skybox cloud
- Keys for klaxon+light, SFX (drift, splash)
- Particles animation for score update + SFX

### 5.0
- Input manager in settings
- Fix ocean generation
- Play same computer
- Change role GameOver

## Regarding the team

##### I encourage you to work on an empty Unity project but if you need to work on the project follow these steps:
1. Install Unity and Git on your computer (obviously).
2. Go inside an empty directory and open a terminal there.
3. Type this command:
~~~~
git clone https://github.com/AllOfTheAbove/CantStopDriving.git
~~~~
4. Open Unity and select your workspace.
*(If you build from your clone of this repo the multiplayer won't work except on local networks. Furthermore, in order to have the scoreboard system to work you need to have a proper APISettings.cs. The one in this repo is blank for obvious security concerns.)*
5. When you have finished implementing your feature send me your zipped project (with a detail of modifications, if possible).

