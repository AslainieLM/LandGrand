# Plateaus Hard 1–7 and the Completion Screen (D-074)

Planned on 2026-11-24 in a grilling session (16 questions), from the D-072 outline. The user accepted every recommendation: "Go with your recommendations for Q1 to Q16, and choose the best option", then "Yes, record and plan".

**Build split:** Claude made the art, backgrounds, sounds and bot-checked layouts. **Claude Code sessions build the C#**, one ticket per session, from `SESSION-PROMPT.md` / `BUILD-START.md` (the Plains Hard route). Then a `/code-review`, then the user plays the checklist.

This is the last episode. After Plateaus Hard 7 every landform can be complete, and the **Completion Screen** ends the playthrough (manuscript P069, Figure 4E).

## 1. Rules that are already settled (do not change)

- Hard levels: width 9600, start (40, 0), goal 9440, checkpoints 2400, 4800 and 7200, 240 s, 9 pickups with one secret on a high ledge (y ≥ 72). Pits and spikes send the explorer a short way back. Walking 84 px/s.
- One script runs every level; level differences are data in `LevelCatalog`; rules are plain C# with tests (D-061). No per-level, per-landform or per-theme `if` in play scripts.
- Every level theme has its own six-layer background (D-063). Shared kinds count once (D-066).
- **No new rules.** Steam vents, wind gusts (push left only, 40 px/s, 4 s cycle), soft sand, ice, crumbling ledges, updrafts, moving platforms (horizontal and vertical) and herds (a `Bat` on the ground: `new Bat(cx, 6f, 48f, 0f, period, phase)`) already exist. New looks go through optional `ThemeArt` fields whose null keeps the old art.
- The five Plateaus Hard badges already exist and must not change: `mesa_ranger`, `world_plateaus`, `steady_plateaus`, `plateau_master`, `land_grand_champion`. No new badges.
- Ambience for all seven: `amb_highwind_hard` (already the Plateaus Hard table entry). Music `bgm_plateaus`.
- Mountains, Hills, Plains, Plateaus Easy and Plateaus Normal must play and look exactly as before.
- All lesson, review, kind and screen text is a draft (Q-04).

## 2. New decisions (D-074)

1. **H1 hot-spring vents:** the steam-vent rule, drawn as a rocky hot pool with soft steam (the Deccan has no active volcano; the Vajreshwari hot springs are real, what heats them is not confirmed, so the lesson says only that hot springs bubble up).
2. **H2 gelada troops** and **H7 yak herds:** the herd rule. Geladas use `gelada_00–02` and the new `sfx_gelada`; yaks use `yak_00–02` and reuse `sfx_herd_hooves`.
3. **H3, H5, H7 gusts:** still left only. Sand streaks in H3, snow streaks in H5 and H7, with the new stronger `sfx_gust_strong`.
4. **H4 Mesa Verde:** sandstone-slab moving platforms (some vertical); wooden ladders are props only (no climbing rule).
5. **H5 blue ice** (`IceTop`) and **H6 loess sand** (new `SandTop`); H6 crumbles keep the old art.
6. **Kinds:** three new kinds, **coal**, **meteorite** (stored as a rock; its card says it fell from space) and **opal**; nine shared kinds gain `Episode.Plateaus`. Plateaus shelf 24 → 36; total 64 → 67.
7. **Completion Screen** (the manuscript's name; scene `CompletionScreen`), shown once, with a "See Ending" button for later visits and a champion flag on the map (section 7).
8. **Leave out:** real rock-art copies, named groups at Mesa Verde, religious signs or politics in Tibet, the Vajreshwari temple, the Simien resettlement. H3 says "water and wind wore the rock" (Wikipedia credits rivers).

## 3. Levels

| Level | Place (banner) | Topic | Theme, goal | Objects | Kinds |
|---|---|---|---|---|---|
| H1 | Deccan Plateau, India ("Deccan Plateau") | Lava plateaus | DeccanTraps, Hill Fort Gate | hot-spring vents, crumbling ledges | basalt, zeolite, calcite, olivine |
| H2 | Ethiopian Highlands ("Ethiopian Highlands") | Life on high plateaus | SimienHighlands, Highland Village | gelada troops | basalt, ignimbrite, tuff, **opal** |
| H3 | Tassili n'Ajjer, Algeria ("Tassili n'Ajjer") | Water and wind shape rock | SandstoneArches, Rock Art Shelter | sand gusts, soft sand | sandstone, granite, quartz, clay |
| H4 | Mesa Verde, USA ("Mesa Verde") | Cliff homes | CliffDwellings, Cliff Palace Ladder | high ledges, updrafts, sandstone slabs (horizontal and vertical) | sandstone, shale, **coal** |
| H5 | Antarctic Plateau ("Antarctic Plateau") | An ice plateau | IcePlateau, Research Station | blue ice, snow gusts | granite, gneiss, coal, **meteorite** |
| H6 | Loess Plateau, China ("Loess Plateau") | Soil loss and terraces | LoessTerraces, Cave House | loess sand, crumbling ledges | loess, quartz, feldspar, mica, calcite |
| H7 | Tibetan Plateau ("Tibetan Plateau") | Roof of the world (finale) | RoofOfWorld, Summit Camp | snow gusts, ice, crumbles, updrafts, yak herds, vertical slabs | halite, granite, chromite, serpentinite |

Exact numbers, pickups and bot results: `layouts.md`. Copy them exactly.

## 4. Code changes

| Where | Change |
|---|---|
| `Rules/LevelLayout.cs` `LevelTheme` | Append `DeccanTraps, SimienHighlands, SandstoneArches, CliffDwellings, IcePlateau, LoessTerraces, RoofOfWorld` at the **end**. |
| `Rules/LevelData.cs` `ItemCatalog` | Append `coal` (rock), `meteorite` (rock), `opal` (mineral) at the **end** of `All` (Plateaus only, `IsDraft`). Add `Episode.Plateaus` to zeolite, calcite, olivine, clay, loess, feldspar, mica, chromite, serpentinite (keep their other landforms and comments). |
| `Rules/LevelData.cs` `LevelCatalog` | Add Plateaus Hard 1–7 exactly as `layouts.md`, with a `PlateausHard(...)` helper like `PlainsHard(...)`; text from section 6. |
| `Rules` (new, plain C#) | `PlayerProgress.IsGameComplete` (every `Episode` complete) and `CompletionSummary.For(progress)`: stars (sum over landforms, max 180), kinds found / total (67), badges earned / total (`AchievementCatalog.All`). Tests. |
| `Editor/LevelThemes.cs` `ThemeArt` | New optional fields, defaults keep today's art: `VentRim` (null = `vent`), `VentSteamFrames` (null = `Props/steam`), `VentWarnFrames` (null = `Props/steam_warn`), `GustFrames` (null = `Props/gust`), `GustSound` (null = `sfx_gust.wav`), `SandTop` (null = `soft_top`), each with a script field for the missing-asset message (like `IceTopScript`). Seven theme entries (section 8). Extend the "old themes keep defaults" test to the new fields. |
| `Editor/LevelBuilder.cs` (vent build), `Editor/LevelObjects.cs` (gust build), `Editor/HardLevelObjects.cs` (sand strip) | Read the new fields instead of the fixed names. |
| Herds and ice | No code change: theme data (`BatFrames`, `BatFrameCount = 3`, `BatSound`, `BatArtScript`, `BatSoundScript`, `BatArtLift`, `BatArtFacesLeft = false`, `IceTop`, `IceTopScript`), as Plateaus Normal llamas and salt crust. |
| Moving platforms | `PlatformSprite = "sandstone_slab"` for CliffDwellings and RoofOfWorld (same size as `platform`, so `PlatformArtAbove = 0`). |
| Completion Screen | Section 7. |
| Tests | Update tests that expect a Plateaus shelf of 24 or a total of 64; add `PlateausHardLevelsTests.cs` like `PlainsHardLevelsTests.cs`; tests for `IsGameComplete`, `CompletionSummary`, and the new `ThemeArt` defaults. |

## 5. Objects (no new rules)

- **Hot-spring vent:** the steam-vent rule and numbers. Rim `vent_hotspring` (same size as `vent`), puff `Props/hotspring_steam_00–03`, warning `Props/hotspring_steam_warn_00–01`; same sound as today's vent. The sulfur-crust decoration stays MossyForest only.
- **Gelada troop / yak herd:** the herd rule and numbers. Frames `gelada_00–02`, `yak_00–02` (32×16, facing right, so `BatArtFacesLeft = false`); sounds `sfx_gelada.wav`, `sfx_herd_hooves.wav`.
- **Sand / snow gust:** the wind rule and numbers (left push). Frames `Props/gust_sand_00–03` (SandstoneArches) and `Props/gust_snow_00–03` (IcePlateau, RoofOfWorld); sound `sfx_gust_strong.wav` for those three themes.
- **Blue ice:** the ice rule; top tile `blue_ice_top` (IcePlateau, RoofOfWorld).
- **Loess sand:** the soft-sand rule; top tile `loess_soft_top` (LoessTerraces only; SandstoneArches keeps `soft_top`).
- **Sandstone slab:** the moving-platform rule (horizontal or `MovingPlatform.Vertical`); sprite `sandstone_slab`.
- **Updrafts, crumbling ledges, pits, spikes, boulders:** exactly as before.

## 6. Content (drafts, Q-04: needs teacher review)

### Lessons (4 sentences, each at most 44 characters)

**H1 "Lava Plateaus"** (`lesson_deccan`): "Long ago, lava flowed out again and again." / "It cooled into flat layers of basalt." / "These layers built the Deccan Plateau." / "Hot springs still bubble up in places."

**H2 "Life Up High"** (`lesson_ethiopia`): "The Ethiopian Highlands are very high." / "Grass grows on their cool, flat tops." / "Geladas live there in big groups." / "They eat grass and live only in Ethiopia."

**H3 "Rock Shaped by Water and Wind"** (`lesson_tassili`): "Tassili n'Ajjer is a sandstone plateau." / "Water and wind wore its rock away." / "Now it has arches and tall rock towers." / "People painted on its rocks long ago."

**H4 "Cliff Homes"** (`lesson_mesa_verde`): "Mesa Verde means green table." / "It is a mesa in the United States." / "Long ago, people built homes in its cliffs." / "They used sandstone blocks and ladders."

**H5 "An Ice Plateau"** (`lesson_antarctic`): "Antarctica has a high plateau of ice." / "The ice is about 2 km thick." / "It is the coldest place on Earth." / "Meteorites from space lie on its ice."

**H6 "Soil and Terraces"** (`lesson_loess`): "The Loess Plateau is made of fine soil." / "Wind carried this dust there long ago." / "Rain washes the soft soil away easily." / "Farmers build step fields called terraces."

**H7 "Roof of the World"** (`lesson_tibet`): "The Tibetan Plateau is the highest one." / "It is called the roof of the world." / "Its average height is over 4,500 m." / "Salt lakes dot its wide, flat land."

### Reviews (3 sentences)

- **H1:** "The Deccan Plateau is made of lava layers." / "Basalt formed when the lava cooled." / "Zeolite crystals grow in holes in basalt."
- **H2:** "Geladas live on Ethiopia's high plateaus." / "The highlands are made of old lava rock." / "Opal is found in volcanic layers there."
- **H3:** "Tassili n'Ajjer is a sandstone plateau." / "Water and wind wore the rock into arches." / "Sandstone is made of grains of quartz."
- **H4:** "People built homes in Mesa Verde's cliffs." / "The cliffs are sandstone above shale." / "Coal lies in some of its rock layers."
- **H5:** "The Antarctic Plateau is a plateau of ice." / "Granite and gneiss lie in nearby mountains." / "Meteorites fell from space onto the ice."
- **H6:** "Loess is fine dust that the wind piled up." / "Terraces and plants keep the soil in place." / "Loess has tiny bits of quartz and mica."
- **H7:** "The Tibetan Plateau is the highest plateau." / "Its salt lakes leave halite behind." / "Chromite and serpentinite come from deep rock."

### New kinds

| Id | Name | Rock? | Description | Picture |
|---|---|---|---|---|
| `coal` | Coal | rock | A black rock made from old plants pressed for a very long time. People burn it for heat. | `coal` |
| `meteorite` | Meteorite | rock | A rock that fell from space. Thousands are found on Antarctic ice, where they are easy to see. | `meteorite` |
| `opal` | Opal | mineral | A gem that flashes rainbow colours. Ethiopia has opal in old volcanic layers. | `opal` |

### Sources and checks (see `kinds-research.md`, 2026-11-24)

**Not confirmed (teacher check):** zeolite near Nashik and Pune (locality databases only); what heats the Vajreshwari springs; wind carving at Tassili (Wikipedia credits rivers); siltstone and conglomerate at Mesa Verde (not used); every Tibet kind (paper summaries only); the meteorite count (sources say 7,000 or 35,000, so the card says "thousands"); the rock under the Antarctic ice (H5 review says "nearby mountains"); the Antarctic Plateau elevation (citation needed, not used).

## 7. Completion Screen and the last landform

**Plateaus Description Screen** (`LandformDescriptionController`, data and builder only, no per-landform `if`):
- When `NextEpisodeFor(episode)` is null **and** `IsGameComplete`, a line "You explored all four landforms!" shows where the next-landform banner would be (new text object built by `LandformDescriptionBuilder`). Before the game is complete (a book visit), nothing shows there.
- **ToMap:** if `IsGameComplete` and the device setting `LandGrand.CompletionSeen` is not set, leave to `CompletionScreen`; otherwise to the map as today.
- **See Ending:** a button shown only on a book visit (`fromBook`) when `IsGameComplete`; it opens `CompletionScreen`.

**Completion Screen** (new scene `CompletionScreen`, new `CompletionScreenController`, new builder `CompletionBuilder.Build`, menu **LandGrand > Build Completion Scene**, added to Build Settings):
- Picture: the start-up screen background layers with `Completion/completion_plateau` in front and the explorer standing on it, looking over the landforms (reuse the start-up explorer sprite; idle frame is fine).
- Text (Micro 5, 22 px, D-047): title "Land Grand Explorer!"; "You explored mountains, hills, plains and plateaus!"; "Stars n/180"; "Rocks and minerals n/67"; "Badges n/total"; "Thank you for playing Land Grand!". Values from `CompletionSummary.For(GameProgress.Current)`; totals are computed, never typed.
- One **Back to Map** button (map with Plateaus picked). Android Back does the same.
- Music: the menu music (as the map). A short fanfare on entry is fine if it reuses the Description fanfare.
- On opening, set `LandGrand.CompletionSeen` (PlayerPrefs, like `LandGrand.UnlockSeen.`). Replays of levels stay allowed; the save file does not change.

**Landform Map** (`LandformMapBuilder`, `LandformMapController`): when `IsGameComplete`, show the golden flag (`Plateaus/champion_flag_0/1`, two-frame wave) on the Plateaus region and the ribbon `Labels/champion_ribbon` with "All landforms explored!". Hidden otherwise.

**Result Screen:** no change; the Land Grand Champion "New badge!" notice already works.

**Manuscript (D-036):** P069 names stars, collected items and a congratulatory message. The badge total, Back to Map, See Ending and the map flag are additions (Q-13 d).

## 8. Assets (made by Claude; do not edit)

- `python Tools/Art/make_plateaus_hard_art.py`: theme tiles, boulders and goals for `deccan`, `simien`, `tassili`, `mesaverde`, `iceplateau`, `loess`, `tibet`; goals `hill_fort_gate`, `highland_village`, `rock_art_shelter`, `cliff_palace_ladder`, `research_station`, `cave_house`, `summit_camp` (56×48); objects `vent_hotspring`, `Props/hotspring_steam_00–03`, `Props/hotspring_steam_warn_00–01`, `Props/gust_sand_00–03`, `Props/gust_snow_00–03`, `gelada_00–02`, `yak_00–02`, `blue_ice_top`, `loess_soft_top`, `sandstone_slab`; props (below); items `coal`, `meteorite`, `opal`; lesson pictures `lesson_deccan`, `lesson_ethiopia`, `lesson_tassili`, `lesson_mesa_verde`, `lesson_antarctic`, `lesson_loess`, `lesson_tibet`; `Completion/Sprites/completion_plateau`, `LandformMap/Sprites/Plateaus/champion_flag_0/1`, `LandformMap/Sprites/Labels/champion_ribbon`. Preview `.scratch/plateaus-hard/art-preview.png`.
- `python Tools/Art/make_plateaus_hard_backdrops.py` (folders in `Assets/Art/Level/Sprites/Backdrop/`): `Backdrop/DeccanTraps/`, `SimienHighlands/`, `SandstoneArches/`, `CliffDwellings/`, `IcePlateau/`, `LoessTerraces/`, `RoofOfWorld/`, each L0–L5. Preview `backdrop-preview.png`.
- `python Tools/Audio/make_plateaus_hard_audio.py`: `sfx_gelada.wav` (0.70 s), `sfx_gust_strong.wav` (0.90 s, peak 1.8 dB above `sfx_gust`). Preview `audio-preview.png`.

### Theme table

| Theme | Tiles (top / soil / ledge / boulder ending) | Goal (name, sprite, offset) | Backdrop | Extra |
|---|---|---|---|---|
| DeccanTraps | `deccan_top` / `deccan_soil` / `ledge_deccan` / `_deccan` | "Hill Fort Gate", `hill_fort_gate`, (24, 24) | `Backdrop/DeccanTraps/` | `VentRim = "vent_hotspring"`, `VentSteamFrames = "Props/hotspring_steam"`, `VentWarnFrames = "Props/hotspring_steam_warn"` |
| SimienHighlands | `simien_*` | "Highland Village", `highland_village`, (24, 24) | `Backdrop/SimienHighlands/` | `BatFrames = "gelada"`, `BatFrameCount = 3`, `BatSound = "sfx_gelada.wav"`, `BatArtLift = 2`, `BatArtFacesLeft = false` |
| SandstoneArches | `tassili_*` | "Rock Art Shelter", `rock_art_shelter`, (24, 24) | `Backdrop/SandstoneArches/` | `GustFrames = "Props/gust_sand"`, `GustSound = "sfx_gust_strong.wav"` |
| CliffDwellings | `mesaverde_*` | "Cliff Palace Ladder", `cliff_palace_ladder`, (24, 24) | `Backdrop/CliffDwellings/` | `PlatformSprite = "sandstone_slab"` |
| IcePlateau | `iceplateau_*` | "Research Station", `research_station`, (24, 24) | `Backdrop/IcePlateau/` | `IceTop = "blue_ice_top"`, `GustFrames = "Props/gust_snow"`, `GustSound = "sfx_gust_strong.wav"` |
| LoessTerraces | `loess_*` | "Cave House", `cave_house`, (24, 24) | `Backdrop/LoessTerraces/` | `SandTop = "loess_soft_top"` |
| RoofOfWorld | `tibet_*` | "Summit Camp", `summit_camp`, (24, 24) | `Backdrop/RoofOfWorld/` | `IceTop = "blue_ice_top"`, snow gusts as IcePlateau, `BatFrames = "yak"`, `BatFrameCount = 3`, `BatSound = "sfx_herd_hooves.wav"` (script `make_plains_hard_audio.py`), `BatArtLift = 2`, `BatArtFacesLeft = false`, `PlatformSprite = "sandstone_slab"` |

Check each goal in the Editor; a lower offset is fine when the sprite has clear rows at its base; report it.

### Living props (decoration only; keep them off pits, ledges, boulders, vents, updraft columns, platform paths and herd loops with the existing prop-fit helpers; report any prop moved more than 64 px)

- **DeccanTraps:** `dry_teak` about every 300 px from x = 200, order 3, centre y = 12; langurs (`langur_00/01`, swap every 1.5–2.5 s) at x = 1500, 4300 and 6900, order 4, centre y = 6.
- **SimienHighlands:** `giant_lobelia` about every 250 px from x = 150, order 7, centre y = 10; lammergeiers (`lammergeier_00/01`, swap every 0.4 s) at x = 2000 and 5600, centre y = 150, order 2.
- **SandstoneArches:** `saharan_cypress` at x = 900, 3600, 6100 and 8300, order 3, centre y = 12; `sand_ripple` about every 200 px from x = 120, order 7, centre y = 2 (not on sand strips).
- **CliffDwellings:** `juniper_mesa` about every 260 px from x = 180, order 3, centre y = 10; `cliff_ladder` leaning on the first three ledges, order 6; wild turkeys (`wild_turkey_00/01`, swap every 1.5–2.5 s) at x = 1300 and 5000, order 4, centre y = 6.
- **IcePlateau:** `snow_drift` about every 280 px from x = 160, order 7, centre y = 3 (not on ice strips); snow petrels (`snow_petrel_00/01`, swap every 0.4 s) at x = 2600 and 6600, centre y = 150, order 2.
- **LoessTerraces:** `millet_00/01` (1.2–1.8 frames per second) about every 140 px from x = 100, order 7, centre y = 6 (not on sand strips); `terrace_step` at x = 1100, 3900, 6400 and 8600, order 3, centre y = 8.
- **RoofOfWorld:** `alpine_cushion` about every 180 px from x = 140, order 7, centre y = 3 (not on ice); blue sheep (`bharal_00/01`, swap every 1.5–2.5 s) at x = 2100 and 5800, order 4, centre y = 6, never within 8 px of a yak loop.

Prop y values are a starting point; adjust to the final sprite heights so props stand on the ground, and report it.

## 9. Not in scope

Any change to older levels; new badges; a ladder climbing rule; a gust that pushes right; Profile changes; the Android device check.
