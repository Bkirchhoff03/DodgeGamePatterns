# KNOWN ISSUES

- Player hovers slightly above the floor, at a height of 0.01 units, which can cause some issues with collision detection, and state changes.

- Player can fall between blocks that are close together occasionally.

- Player can fall between stacks of blocks and won't be able to climb out. More of a gameplay issue than bug. 

- Sometimes the arc of the players jump places them directly on top of a block, which causes them to stay in the jumping state, instead of changing to the riding faller state. 

- Game doesn't reset when the player loses all lives.