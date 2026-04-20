# KNOWN ISSUES

- Player hovers slightly above the floor, at a height of 0.01 units, which can cause some issues with collision detection, and state changes.

- Player can fall between blocks that are close together occasionally.

- Player can fall between stacks of blocks and won't be able to climb out. More of a gameplay issue than bug. 
    - Improved with system that checks if player is stuck and spawns a rescue block, but still can happen if there is no place to spawn rescue faller.

- Sometimes the arc of the players jump places them directly on top of a block, which causes them to stay in the jumping state, instead of changing to the riding faller state. 
    - Have not seen this in a long time so I think it may be fixed, but I will keep it here just in case. 

- Animations don't line up well with states yet.
    - They line up pretty well for the most part, but it doesn't always look perfect, so I am gonna keep it here for now.  

- Faller can freeze ontop of the trapdoor.
