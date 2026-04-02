# KNOWN ISSUES

- Player hovers slightly above the floor, at a height of 0.01 units, which can cause some issues with collision detection, and state changes.

- Player can fall between blocks that are close together occasionally.

- Player can fall between stacks of blocks and won't be able to climb out. More of a gameplay issue than bug. 

- Sometimes the arc of the players jump places them directly on top of a block, which causes them to stay in the jumping state, instead of changing to the riding faller state. 

- Game doesn't reset when the player loses all lives.

- Punching while running into a faller and continuing to run into the faller makes the arm go farther and farther out.

- Player can walk through the water.

- Water shader may slow the game down.

- Water shader collisions don't work with the player.

- Animations don't line up well with states yet.

- Faller can freeze ontop of the trapdoor.

- level 1 is with rectangles that freeze on collision, level 2 is boulders that freeze after 30 frames of collision or at rest for half a second or so.

- Player can get stuck between two bolders.