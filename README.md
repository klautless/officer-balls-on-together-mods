# Mod Descriptions  

***ChatTweaks***:  
- adds a timestamp to chat
- adds outline to the font
- prevents the chat window from fading
- messages now emit a blip noise (tone from the fishing rod equip)
- players now emit a "has left the server" message
- players now emit sounds when joining and leaving the server (fishing bird on join, frog on leaving)  
  
***FishingTweaks***- Changes the fishing catch timer to 60 seconds so you have time to alt+tab back in. Also lets you fish from people's heads and desks etc.  
  
***PlayerLimitLift*** - increase maximum server size to 24 players.  

***Teleport*** - Allows you to warp to other players in the server.  
Hotkeys:  
- Shift + Q / Shift + E -> rotate through players in the server.
- Shift + X -> Teleport to selected player.

* *Known issue: If a player is too far out of range teleportation will fail; haven't yet researched whether they are unloaded or if transform updates aren't reported outside of a given range.*  
  
***Zoomies*** - simple mod that increases the maximum zoom distance.  
  
# Installation  
**Manual:**  
1. Download the latest version of BepInEx at https://github.com/BepInEx/BepInEx/releases/. If you are using Linux download the Windows version for Proton/Wine to properly load it.
2. Navigate to your game's installation by right clicking the game on Steam, clicking Manage, then Browse Local Files. Navigate one folder deeper into the On-Together subfolder, and extract BepInEx's contents there. The BepInEx folder and winhttp.dll should be in the same folder as On-Together.exe.
3. If on Linux only, right click the game on Steam and select properties, and set your launch options as follows (without the quotes): " WINEDLLOVERRIDES="winhttp=n,b" %command% "
4. Download any of the mods from releases and extract to the root directory (they should end up in /BepInEx/plugins.


*Thunderstore installation should become available in the near future.*
