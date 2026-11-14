---
status: ready-for-agent
created: 2026-11-14
sources:
  - Grilling session in chat, 2026-11-14 (Q1–Q18, all answered "go with your recommendation")
  - Vault Decisions/Decision Register.md (D-036, D-041, D-045, D-050, D-051, D-052)
  - Vault Project/Scope and Requirements.md (LG-15, LG-16, LG-19, LG-22, LG-28)
  - Vault Questions/Open Questions.md (Q-03, Q-04, Q-07)
  - CONTEXT.md (glossary: Explorer, Profile, Item kind, Found, Collection, Home landform, Shelf, Item card)
---

# Spec: Profile screen with the rock and mineral Collection

## Problem Statement

LandGrand saves which rocks and minerals the explorer has found (LG-19), but after the item pop-up closes the learner can never see them again until the Completion Screen at the very end of the game. A Grade 5 learner has no place to look back at what they found, re-read the descriptions, or see what is still missing. The name, the only part of the explorer's identity, is tucked into Settings next to the volume sliders. Popular games for children give the player a profile with their name, progress and a collection book; LandGrand has none.

## Solution

Add a **Profile** screen, opened from a round explorer portrait in the top-right corner of the Main Menu. One screen, no tabs:

- **Explorer strip** (top): the explorer's portrait, "Explorer: Ana" with a pencil icon to change the name, and three totals: stars earned, item kinds found out of all item kinds, and levels cleared.
- **Collection** (below): one **shelf** per landform, in episode order (Mountains, Hills, Plains, Plateaus). Each shelf holds the item kinds whose **home landform** it is. A found item kind shows its picture in colour and its name; an unfound one shows a dark silhouette of its picture and "?". A landform with no item kinds yet shows a "Coming soon" shelf.
- **Item card**: tapping a found item kind opens a pop-up with its picture, name, a "Rock" or "Mineral" tag and its description, in the same layout as the in-level item pop-up, with the same "Draft — needs teacher review" note (Q-04). Unfound item kinds cannot be opened.

The name pencil moves from Settings to the Profile; Settings keeps only the volume sliders, Back and Save. Badges are **not** built (D-052): they are new game rules that need the adviser's approval first.

## User Stories

### Opening the Profile
1. As a learner, I want my explorer's face in the top corner of the Main Menu, so that I know where my profile is.
2. As a learner, I want the four big menu buttons to stay the same, so that Play, Settings, Tutorial and Exit are where I expect them.
3. As a learner, I want the Profile to look and sound like the Main Menu (same background and calm music), so that it feels like part of the menu.
4. As a learner, I want a Back arrow in the top-left corner and Android's Back button to return to the Main Menu, so that leaving works like every other screen.

### Explorer strip
5. As a learner, I want to see "Explorer:" and my name, so that the profile feels like mine.
6. As a learner, I want a pencil next to my name that opens the name panel with my name already typed, so that I can fix it easily.
7. As a learner who has not played yet, I want to see "Explorer: —" and still be able to set my name with the pencil, so that the Profile works from the start.
8. As a learner, I want to see my total stars, how many rocks and minerals I have found out of all of them, and how many levels I have cleared, so that I can see my progress at a glance.

### Collection
9. As a learner, I want one shelf per landform, so that I learn which rocks and minerals go with which landform.
10. As a learner, I want found rocks and minerals in colour with their names, so that I can see what I have collected.
11. As a learner, I want unfound ones as dark shapes with "?", so that I know something is still waiting to be found without being told what it is.
12. As a learner, I want landforms without rocks and minerals yet to say "Coming soon", so that the empty shelves are not confusing.
13. As a learner, I want to tap a found rock or mineral to read its card (picture, name, rock or mineral, description), so that I can review what I learned.
14. As a learner, I want tapping an unfound one to do nothing, so that its name and description stay a surprise.
15. As a learner, I want to close a card with its button or by tapping outside it, so that it is easy to go back to the shelves.

### Settings
16. As a learner, I want Settings to hold only the sound options, so that it is simple.

### Quality
17. As a teacher, I want every description on a card to be the same text as the in-level pop-up, with the draft note until it is reviewed, so that there is only one text to check.
18. As a learner, I want the Profile to work offline and keep my data on this device only, so that nothing is sent anywhere (LG-22, LG-24).
19. As a learner, I want all sentences in the easy-to-read font, so that I can read them (D-051).

## Implementation Decisions

- **Manuscript fit (D-036, D-052):** the Profile shows only data the manuscript already saves (LG-19: name, cleared levels, stars, collected rocks and minerals). It adds no new rule and no new saved field. Badges and achievements are out of scope until the adviser approves them.
- **Entry point:** a round 32×32 explorer portrait button (Cave Spelunker face; 32 rather than 28 so small fingers hit it) in the top-right corner of the Main Menu, with the name in small Andika text under it. The four Main Menu buttons (LG-16) do not change.
- **Separate scene:** `Assets/Scenes/Profile.unity`, built by a new `ProfileBuilder` (menu: LandGrand > Build Profile Scene), added to the build list after the Main Menu. It reuses the Main Menu backdrop without the title, the menu music and `PixelSceneKit` (Back icon, guide, fonts, `FixFonts()` before saving). Back goes to the Main Menu with the usual screen fade.
- **Layout (480×270, no scrolling):** explorer strip about 56 px tall at the top; four shelves of about 48 px below it. Each shelf has the landform name (pixel font) on the left and up to 6 slots of 32 px. More than 6 item kinds on one shelf scroll sideways (not needed yet). Text sizes follow D-050 and D-051: headings and numbers in Micro 5 at 22 px, names and sentences in Andika at 15.
- **Collection rules (pure C#, rules assembly):**
  - Each `ItemKind` gains a home landform (`Episode`). Granite, Quartz, Feldspar and Mica are Mountains.
  - A small collection model answers: the item kinds of a shelf in lesson order; whether a kind is found (`PlayerProgress.HasFoundKind`); the found count and total count; whether a shelf is "Coming soon" (no item kinds).
  - Totals: stars = sum of best stars over all levels; levels cleared = number of cleared levels; items = found kinds / all kinds. No total for levels or stars (Q-03 is open on the level count).
  - The Completion Screen (LG-15, not built yet) must use the same totals and collection model, so the two screens always agree.
- **Unfound look:** the silhouette is the item's own sprite drawn in one dark colour (a tinted Image or a generated silhouette sprite), name "?".
- **Item card:** reuses the item pop-up layout from the level (picture in a slot, name, description, draft note), plus a "Rock"/"Mineral" tag. Closed by an OK button or a tap outside it. Android Back closes the card first, then the Profile.
- **Name editing moves (D-041 updated by D-052):** remove the "Explorer" row and pencil from the Settings panel; the Profile pencil opens a name panel in the Profile scene that works like the Main Menu one (same `NameRule`, prefilled, OK saves at once, Back cancels). The first-Play name prompt on the Main Menu stays.
- **No reset** of progress anywhere (Q-07 stays open for reset and reinstall).
- **No "new item" dot** on the portrait (would need a saved "seen" list).
- **Save format:** unchanged.

## Testing Decisions

- Rules tests (dotnet, NUnit, outside Unity) for the collection model: every item kind has a home landform; Mountains shelf lists Granite, Quartz, Feldspar, Mica in lesson order; Hills, Plains and Plateaus shelves are "Coming soon"; found state follows `PlayerProgress`; found and total counts; star and cleared-level totals with replays (best stars only, D-045).
- Unity screens are checked by hand in the Editor: portrait on the Main Menu, Profile layout at 480×270, found and unfound slots, item card open and close, name change from the Profile, Back and Android Back, Settings without the name row.

## Out of Scope

- Badges and achievements (await adviser approval; D-052).
- Progress reset and reinstall behaviour (Q-07).
- A "new item" dot or any other new saved field.
- Item kinds for Hills, Plains and Plateaus (Q-04).
- "First found in" information on the card.
- The Completion Screen itself (LG-15); only the shared totals and collection model are built now.

## Further Notes

- Glossary terms are in `CONTEXT.md`. Use them in code and tickets: Explorer, Profile, Item kind, Pickup, Found, Collection, Home landform, Shelf, Item card.
- Every new line of code carries a plain-English comment (D-033). The user makes all commits.
