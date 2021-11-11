# Titan_Returning
Player Controller: This script allows the player to use there guns and stores gun data such as ammo, firerate, damage and FX. The script allows for aim assist, firing, reloading, weapon swapping and ammo regenration.


Player Ability Controller: This script allows the player to use and swap abilities during gameplay, each ability creates an object or effect for the player and the dodge and grenade abilites cannot be swapped out of the button placements. When an ability is used it enters cooldown which is shown by the circle in game.


Checkpoint Saving: This script controls the checkpoint system in game and saves data when the player reaches a checkpoint, this allows the game to be closed and opened at the same point of progress along with restarting when the player dies, this also causes the game to save stats such as kills and player data such as weapons unlocked and ammo counts.


Enemy Knight: This script is one of the enemy variant scripts, it controls the Knight type enemy which is a melee unit that uses a slam/wave attack and a regular melee attack, this and all other enemy scripts control movement based on an aggro range that can increase if they are attacked. The script also allows enemies to attack based on line of sight with the player and allows the enemy to be affected by the Cryo debuff to reduce speed values.


Boss Health: This script keeps track of the various target points on the boss and allows it to shield portions based on damage taken, when a certain part of the boss is broken it will shield select areas and increase boss difficulty by adding new attacks or empowering current ones by passing data to Boss Attacks.


Boss Attacks: This script will play various attacks that the boss uses and can scale the power based on data from Boss Health, some attacks include fire pits and turret spawning.
