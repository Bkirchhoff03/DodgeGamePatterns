# Release Plan for Dodge Game

**Project Type:** Solo PC Game
**Timeline:** 3-6 Months
**Monetization:** Open to options (see recommendations below)

## Phase 1: Core Feature Completion (Weeks 1-6)

### Core Stacking Mechanic
The main gameplay loop revolves around building a tower of stacked fallers to climb higher and higher. Players must carefully manage their stack while defending against destructive fallers.

**Faller Stacking System**
- [ ] Implement faller-on-faller stacking mechanic
  - Fallers land on top of each other and remain stationary
  - Stack builds vertically as more fallers accumulate
  - Player can jump between stacked fallers to climb higher
- [ ] Height/progress tracking system
  - Track player's current height in the stack
  - Display height as score/progress indicator
  - Maximum height reached becomes the primary metric
- [ ] Stack stability and physics
  - Stacked fallers should feel stable when landed on
  - Consider if stack can become unstable/wobbly at extreme heights
  - Player loses progress if they fall down through gaps in the stack

**Faller Type System**
- [ ] Design and implement multiple faller types with distinct behaviors

  **Neutral Fallers** (Safe to stack)
  - [ ] Basic faller: Standard stackable platform
  - [ ] Potentially add variations (different sizes, speeds, point values)

  **Destructive Fallers** (Remove player progress)
  - [ ] Destroyer faller: Destroys X number of fallers below it when it lands on the stack
    - Visual indicator (different color, warning symbol, particles)
    - Sound cue when approaching
  - [ ] Crumbler faller: Breaks apart after X seconds, leaving a gap in the stack
  - [ ] Chain destroyer: Destroys all fallers in a vertical line below it

  **Helpful Fallers** (Provide benefits)
  - [ ] Bouncer faller: Launches player higher when jumped on
  - [ ] Shield faller: Protects adjacent fallers from destruction
  - [ ] Point multiplier faller: Increases points gained while standing on it

**Player Interaction Mechanics**
- [ ] Kick/push mechanic for removing fallers
  - Allow player to kick fallers horizontally off-screen
  - Must identify and kick destructive fallers before they hit the stack
  - Kicking uses stamina/cooldown to prevent spam
  - Points awarded for successfully neutralizing threats
- [ ] Visual identification system
  - Clear color coding or symbols for each faller type
  - Warning indicators for incoming destructive fallers
  - Highlight fallers within kicking range

**Point/Scoring System**
- [ ] Points for height gained (primary score metric)
- [ ] Bonus points for:
  - Successfully kicking destructive fallers off-screen
  - Riding helpful fallers
  - Maintaining combo (consecutive good decisions)
  - Building tall stacks without destruction
- [ ] Penalties for:
  - Letting destructive fallers hit the stack
  - Falling down through the stack (lose height progress)

**Difficulty Progression**
- [ ] Increase spawn rate of fallers as game progresses
- [ ] Gradually introduce higher ratio of destructive fallers
- [ ] Increase faller speed at higher heights
- [ ] More complex patterns of faller types as player climbs

**Game Over Conditions**
- [ ] Game over screen showing:
  - Maximum height reached
  - Final score
  - Fallers kicked
  - Time survived
- [ ] High score tracking (local)

### Technical Foundations
- [ ] Implement save system for high scores/settings
- [ ] Add settings menu (volume, controls, window mode)
- [ ] Implement pause functionality
- [ ] Add proper scene management

## Phase 2: Content & Polish (Weeks 7-10)

### Visual Polish
- [ ] Replace placeholder sprites with final art
- [ ] Add particle effects (impacts, player trails, etc.)
- [ ] Background visuals and parallax layers
- [ ] Visual feedback for all interactions
- [ ] Screen shake, hit effects

### Audio
- [ ] Background music (2-3 tracks minimum)
- [ ] Sound effects for all actions
- [ ] Impact sounds, jump sounds, UI sounds

### UI/UX
- [ ] Main menu (Play, Settings, Credits, Quit)
- [ ] In-game HUD improvements
- [ ] Smooth transitions between screens
- [ ] Tutorial or how-to-play screen

## Phase 3: Balance & Testing (Weeks 11-14)

### Game Balance
- [ ] Tune difficulty curve
- [ ] Test and adjust point values
- [ ] Ensure 10-15 minute average gameplay sessions feel rewarding

### Testing
- [ ] Playtest extensively yourself
- [ ] Get 5-10 friends/family to playtest
- [ ] Fix all critical bugs
- [ ] Performance optimization
- [ ] Test on different PC specs (if possible)

## Phase 4: Pre-Launch Preparation (Weeks 15-20)

### Technical
- [ ] Build pipeline for Windows (and Mac/Linux if desired)
- [ ] Add analytics (optional, for understanding player behavior)
- [ ] Implement crash reporting
- [ ] Final performance pass

### Marketing Materials
- [ ] Create game trailer (30-60 seconds)
- [ ] Take compelling screenshots
- [ ] Write game description
- [ ] Create logo/key art
- [ ] Set up social media presence (Twitter/X, Reddit)
- [ ] Create press kit

### Distribution Setup
- [ ] Choose platform: Steam (requires $100 fee + Steamworks setup) or Itch.io (free, easier start)
- [ ] Create store page with description, screenshots, trailer
- [ ] Set price point (if paid) - research similar games ($3-10 range typical for indie arcade games)
- [ ] Prepare game page 2-4 weeks before launch

## Phase 5: Launch & Post-Launch (Weeks 21-24)

### Launch
- [ ] Soft launch to gather initial feedback
- [ ] Monitor for critical bugs
- [ ] Be responsive to player feedback
- [ ] Post on relevant subreddits (r/incremental_games, r/IndieGaming, etc.)

### Post-Launch
- [ ] Patch critical bugs within 24-48 hours
- [ ] Consider updates based on feedback
- [ ] Engage with your player community

## Monetization Recommendations

Given it's a solo PC arcade game, here are your best options:

1. **Itch.io - Pay What You Want**: Start free, let players tip. Low risk, builds audience
2. **Itch.io - Paid ($2-5)**: Simple, keeps 100% revenue (or 90% if using their cut system)
3. **Steam - Paid ($5-10)**: Larger audience, but requires $100 fee + more polish expectations
4. **Free with optional donations**: Build audience first, monetize later

**Recommendation:** Start on Itch.io (free or $2-3), build an audience, then consider Steam if it gains traction.

## Current Game Status

### Working Features
- Player movement and dodging
- Falling objects spawning system
- Collision detection (top, bottom, left, right)
- Lives system (3 lives)
- Riding on top of fallers
- Crush state when hit from bottom
- Random faller sizes and speeds

### Planned Features
- Push mechanic for faller objects
- Point/scoring system
- More to be determined as development progresses

## Notes
- This is a living document - update as plans evolve
- Check off items as they're completed
- Add new tasks as they're discovered
- Adjust timeline based on actual progress
