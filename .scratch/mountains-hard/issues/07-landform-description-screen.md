# 07: Landform Description Screen

**What to build:** The summary screen shown when a landform is finished (manuscript P069, LG-14, LG-32).

1. **Rules** (no Unity types), with tests:
   - `LandformSummary.For(Episode)`: title, five sentences and picture name. Mountains uses the spec's five draft sentences; the other landforms return the title and one placeholder line, "Coming soon".
   - The badge count for the "Your trip" row reuses the existing achievement rules (earned / total).
2. **Scene and builder.**
   - A new editor builder `LandformDescriptionBuilder` with the menu item **LandGrand > Build Landform Description Scene**. It builds `Assets/Scenes/LandformDescription.unity` and adds it to the build list after `Lesson`.
   - Use the same canvas and kit as the other screens (`PixelSceneKit`).
3. **Controller** `LandformDescriptionController : PixelScreen`, laid out as in the spec:
   - the landform's menu picture behind (`LandformBackdrop`), with snow for Mountains;
   - `scroll_panel` centred at (240, 128);
   - the title in Micro 5;
   - five sentences in Andika 15, one per line, marked draft;
   - the "Your trip" row: stars n/45, kinds n/34, badges n/15, with the existing star, gem and medal icons;
   - the unlock banner: the next landform's small picture, the padlock opening frames, sparkles, "Hills is now open!" and, when `HasLevels(next)` is false, "New levels coming soon". It is hidden when there is no next landform;
   - **TO THE MAP** (`btn_to_map`): sets `GameSession.Episode` to the next landform (or keeps it for Plateaus) and opens the Landform Map;
   - `sfx_fanfare.wav` once on open;
   - Android Back works like To the Map, or like the Back icon when the screen was opened from the book.
4. **Result Screen.** When `NextAfter` gives `NextEpisode`, the Next Episode button keeps `GameSession.Episode` on the **finished** landform and opens `LandformDescription` (instead of the map).
5. **Book button on the difficulty selection.**
   - Once `IsEpisodeComplete(GameSession.Episode)`, show `btn_book_icon` at the top right, mirroring the Back icon (`SelectionBuilder.BuildDifficulty`).
   - It sets a session flag (for example `GameSession.DescriptionFromBook = true`) and opens `LandformDescription`.
   - With that flag, the Description Screen shows the Back icon at the top left, which returns to the difficulty selection.

**Blocked by:** 06

**Status:** done

- [x] `LandformSummary` tests; all Mountains sentences ≤ 44 characters
- [x] After clearing Hard 7, Next Episode opens the Description Screen; To the Map opens the map, where Hills unlocks with its animation
- [x] The book button appears only when Mountains is complete, and Back returns to the difficulty screen
- [x] The text fits the panel at 480×270 (screenshot, or ask the user)
- [x] Basic-English comments

**Verified:** Unity EditMode 223 passed. The 480×270 render fits. Live checks passed for Hard 7 → Description → Hills map, the saved animation flag, and book Back.

**Builder to run:** `python Tools/Art/make_hard_art.py`, then **LandGrand > Build Landform Description Scene**, **LandGrand > Build Selection Scenes** and **LandGrand > Build Mountains Levels** (the Result Screen lives in the level scenes).

Spec: `../spec.md` ("Landform Description Screen").
