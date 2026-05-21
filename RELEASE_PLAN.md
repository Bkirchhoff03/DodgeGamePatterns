# Release Plan for Dodge Game

**Project Type:** Solo PC Game
**Timeline:** 3-6 Months
**Monetization:** Open to options (see recommendations below)
**Last Updated:** 2026-05-13

---

## Current Game Status

### Working Features
- **Player movement & state machine** — run, jump, fall, ride-faller, crushed states fully wired
- **Faller spawning** — timed spawn (1.5s), random X/size/speed, safe gap enforcement above existing fallers
- **Faller stacking** — fallers land and freeze on floor/each other; player can jump between them to climb
- **Three faller types implemented:**
  - **Block** — freezes solid (Static), tile sprites swap grass/dirt on freeze/unfreeze
  - **Boulder** — procedural polygon mesh, settles on its own  - **BombBlock** (Level 3) — flashes red while falling, speeds up after landing, explodes after 10 frozen flashes; explosion force scales with height (`force = max(1, round(y/10))`)
- **Punch mechanic** — push unfrozen blocks horizontally, frozen blocks absorb the punch
- **_Ride mechanic_** — land on top of a moving faller; ride it upward
- **Collision system** — top/bottom/left/right contact handling; bottom hit from moving faller = lose life + crush
- **Lives system** — 3 lives shown as tally marks ("III"); lives persist across levels via PlayerPrefs
- **EMT system (rescue)** — player-triggered: costs a life, blasts all fallers outward from player position
- **Auto-rescue** — detects when player is stuck (50-frame height history + reachable-faller ellipse check), spawns thin rescue faller after 5s
- **Height tracker** — UI countdown showing distance remaining to trapdoor
- **Level progression** — Level1 → Level2 → Level3, trapdoor contact loads next scene
- **Save/load system** — JSON split into 3 files (index + faller data + player data), session autosave + named saves + quick save
- **Pause / game over screens** — pause panel (Time.timeScale=0), game over panel with reason text
- **Main menu** — MainMenuController scene
- **Camera follow** — follows player above starting y, locks below it

### Not Yet Implemented
- Scoring / points system
- Escalating difficulty (spawn rate, faller ratios)
- Crumbler / bouncer / chain-destroyer faller types (from original plan)
- Audio (music, SFX)
- Particle effects / screen shake
- In-game settings menu (volume, controls, window mode)
- Tutorial / how-to-play
- High score tracking
- Game over stats screen (max height, fallers punched, time survived)
- Store / distribution page

---

## Phase 1: Core Feature Completion

### Faller Type System — Status & Remaining Work

**Implemented**
- [x] Basic block faller — standard stackable platform, freezes solid
- [x] Boulder faller — rolls, settles, punch-to-unfreeze interaction
- [x] BombBlock faller — flashes, explodes, force scales with height (Level 3)

**Still To Build**
- [ ] **Crumbler block** — freezes normally but dissolves after 3–5s, leaving a gap. Visual: hairline cracks appear after landing; crumble animation before disappearing.
- [ ] **Bouncer faller** — when the player lands on it, launches them upward with extra velocity. Could be a trampoline-style visual.
- [ ] **Chain-bomb faller** — when it explodes, it detonates any other BombBlock within radius (chain reactions)

### Player Interaction
- [x] Punch mechanic — horizontal push of unfrozen blocks
- [ ] **Punch cooldown / visual indicator** — currently no feedback on cooldown state; add arm recharge animation
- [ ] **Punch upgrade path** — e.g., double-tap to charge a stronger punch in a later level

### Scoring & Progression
- [ ] Score system:
  - Height climbed is the primary score (already tracked; just needs a points value attached)
  - Bonus for punching destructive fallers off-screen before they land
  - Bonus for riding a faller upward
  - Combo multiplier for chaining faller-to-faller jumps quickly
- [ ] Game over screen with full stats: max height, score, fallers punched, time survived
- [ ] Local high score table (top 5 runs)

### Difficulty Scaling
- [ ] Increase spawn rate as player climbs (e.g., reduce `currentTimeBetweenSpawns` from 1.5s → 0.8s by trapdoor height)
- [ ] Increase BombBlock spawn frequency at higher heights (currently fixed at every 10th)
- [ ] Gradually increase faller speed range (`minFallerSpeed` / `maxFallerSpeed`) as height grows

### Technical Foundations
- [x] Save/load system
- [x] Pause functionality
- [x] Scene management & level progression
- [ ] In-game settings menu (master volume, music/SFX sliders, key rebinding)
- [ ] Window mode toggle (fullscreen / windowed)

---

## Phase 2: Content & Polish

### Visual Polish
- [ ] Particle effects:
  - Dust puff on faller land
  - Explosion particles on BombBlock detonation
  - Debris / rubble on crumbler dissolve
  - Player footstep dust while running
- [ ] Screen shake on explosion and crush events
- [ ] Background visuals — parallax layers that shift as player climbs (see Creative Ideas)
- [ ] Visual warning indicators for incoming destructive fallers (skull icon, colored spawn marker)
- [ ] Better game over / level complete transition animations

### Audio
- [ ] Background music (2–3 tracks; one per level feel)
- [ ] SFX for: jump, land, punch, faller freeze, faller unfreeze, explosion, EMT trigger, level complete, game over
- [ ] Sound cue when a BombBlock is spawning (ticking sound before it appears?)
- [ ] Audio mixer with master / music / SFX sliders

### UI/UX
- [ ] In-game HUD pass — cleaner layout for height tracker, lives, score
- [ ] Tutorial popup or "how to play" on first launch
- [ ] Smooth screen transitions between levels
- [ ] Highlight fallers within punch range (subtle outline or color shift)

---

## Phase 3: Balance & Testing

### Game Balance
- [ ] Tune difficulty curve so average run is 8–12 minutes
- [ ] Ensure BombBlock frequency feels threatening but not overwhelming
- [ ] Test that rescue faller spawns reliably (known edge case: rescue can fail if all columns blocked)
- [ ] Validate that punch cooldown doesn't make the mechanic frustrating

### Bug Fixes Before Release
- [ ] Fix `FallerCollisionHandler.cs:68` — `||` should be `&&` (null dereference risk on `otherRb`)
- [ ] Fix `FallerManager.cs:261` — `SpawnFallerAtData` uses `_fallerType` instead of `data.fallerType` (breaks save/load for BombBlock)
- [ ] Remove `using Unity.VisualScripting` from `GameManager.cs` — may break non-VS builds

### Testing
- [ ] Playtest all 3 levels end-to-end
- [ ] Verify save/load round-trip works in all levels including BombBlocks
- [ ] Stress test with 50+ fallers on screen (O(n²) stuck detection concern)
- [ ] Get 5–10 friends/family to playtest
- [ ] Test on a low-spec machine (iGPU laptop if possible)

---

## Phase 4: Pre-Launch

### Technical
- [ ] Windows build pipeline
- [ ] Mac/Linux builds (optional)
- [ ] Crash reporting or error logging to file
- [ ] Final performance pass (cache `GetComponent` calls in states, replace LINQ max in `CheckIfPlayerStuck`)

### Marketing Materials
- [ ] 30–60 second gameplay trailer
- [ ] 4–6 screenshots (show bomb explosion, stack climbing, game over)
- [ ] Write short game description (elevator pitch + feature list)
- [ ] Logo / key art
- [ ] Social media setup (Twitter/X, Reddit)

### Distribution
- [ ] **Itch.io first** — free or $2–3, low friction, good for indie audience
- [ ] Steam later if traction builds ($100 fee, higher polish bar)
- [ ] Store page live 2–4 weeks before launch

---

## Phase 5: Launch & Post-Launch

- [ ] Soft launch for initial feedback
- [ ] Monitor for critical bugs; patch within 24–48 hours
- [ ] Post in r/IndieGaming, r/indiegames, r/Unity3D
- [ ] Engage with player community; collect feedback on feel/difficulty

---

## Creative Ideas (Future Fun Improvements)

These are not committed — prioritize based on what makes the game more fun to play.

### Faller Types
- **Magnetic faller** — when it lands, pulls all nearby unfrozen fallers toward it, creating dense clumps (or pushes them outward in a repel variant)
- **Fragile faller** — visually cracked; splits into two halves when the player lands on it, each half becoming its own mini faller
- **Ghost faller** — passes through other fallers and the floor, only collides with the player; can't be stacked on
- **Shield faller** — golden color; protects the 2 fallers adjacent to it from explosion/crumble effects for its lifetime

### Player Feel
- **Combo jump system** — landing on a new faller within 1.5s of the last jump triggers a "combo" visual and grants a small score multiplier (up to 3x). Resets on landing on the ground.
- **Jump-boost off explosions** — if a BombBlock explodes while the player is in the air nearby, they get an upward velocity kick (dangerous but potentially useful — skilled players can exploit bombs)
- **Dash ability** — short horizontal burst with a 3s cooldown; helps escape tight spots without using EMT

### Progression & Replayability
- **Per-level perks** — at the start of Level 2 and 3, pick one of three passive upgrades: e.g., "Extra Life", "Punch launches fallers further", "Bombs give you a height boost instead of damaging you"
- **Challenge modes** — unlockable after first clear: "No Punch" run, "Bombs Only" level, "Speed Run" (trapdoor descends toward you)
- **Daily seed** — a daily fixed-seed run so players can compare scores against each other

### Environment / Atmosphere
- **Parallax background that reacts to height** — underground rock walls near the bottom → cave opening → sky → clouds → space as the player climbs; makes the height feel meaningful
- **Rising water / lava** — a slowly rising kill plane from below adds urgency and prevents camping on low fallers; speed can scale with level
- **Ambient events** — periodic "wind burst" that nudges all airborne fallers to one side, forcing the player to adapt

### Quality of Life
- **Faller spawn preview** — a faint shadow or icon at the top of the screen 1 second before a faller appears, showing its X position and type (especially useful for bombs)
- **Height milestone markers** — visual waypoints on the side of the screen at 25%, 50%, 75% of the trapdoor distance (e.g., "Halfway there!")
- **Replay system** — record the last run and let players watch it back (simple position-key recording)

---

## Monetization Recommendations

1. **Itch.io — Pay What You Want**: Start free, let players tip. Low risk, builds audience.
2. **Itch.io — Paid ($2–5)**: Simple, keeps ~90–100% revenue.
3. **Steam — Paid ($5–10)**: Larger audience, requires $100 fee and higher polish bar.
4. **Free with optional donations**: Build audience first, monetize later.

**Recommendation:** Launch on Itch.io ($2–3), build an audience, then consider Steam if it gains traction.

---

*This is a living document — update as plans evolve. Check off items as completed, add tasks as discovered, adjust timeline based on actual progress.*
