# Plateaus Hard: kinds research (checked 2026-11-24)

Method note: pages read through a summarising fetch tool. Quotes are as returned; re-check any quote before it goes in player text. Wikipedia unless noted. Pages that failed: Wikipedia "Antarctic meteorite", "Meteorites in Antarctica", "Geology of Tibet", "Yarlung Tsangpo suture zone", "Luobusa ophiolite", "Gangdese Shan" (all 404); Britannica Tibet and Antarctic meteorite pages, MDPI Minerals 15/2/126 (403); NPS geologicformations page for MEVE (404).

## Existing ItemCatalog ids (LevelData.cs, ItemCatalog.All) and their Episodes

- Plateaus shelf already: granite (Mtn, Plains, Plat), quartz (Mtn, Hills, Plains, Plat), andesite (Mtn, Hills, Plains, Plat), sandstone (Mtn, Plains, Plat), conglomerate (Mtn, Plains, Plat), shale (Mtn, Plains, Plat), basalt (Mtn, Plat), limestone (Mtn, Hills, Plains, Plat), gneiss (Mtn, Plat), tuff (Mtn, Plains, Plat), hematite (Mtn, Plat), schist (Mtn, Plat), volcanic_breccia (Hills, Plat), ignimbrite (Hills, Plat), quartzite (Hills, Plat), siltstone (Plains, Plat), halite (Plains, Plat), gypsum (Plains, Plat), laterite (Plains, Plat), halloysite (Plat), goethite (Plat), gibbsite (Plat), cristobalite (Plat), ulexite (Plat).
- Not on Plateaus shelf: feldspar (Mtn, Hills, Plains), mica (Mtn, Plains), obsidian (Mtn), pumice (Mtn, Plains), slate (Mtn), marble (Mtn), garnet (Mtn), clay (Mtn, Hills, Plains), magnetite (Mtn, Hills), sulfur (Mtn), pyrite (Mtn), scoria (Mtn), hornblende (Mtn), diorite (Mtn), gold (Mtn, Plains), serpentinite (Mtn), chromite (Mtn), calcite (Mtn, Hills, Plains), trachyte (Mtn), tanzanite (Mtn), gabbro (Mtn), olivine (Mtn), coral_stone (Hills), limonite (Hills), kaolinite (Hills), aragonite (Hills), coquina (Hills), flowstone (Hills), dripstone (Hills), zeolite (Hills), peridotite (Hills), chlorite (Hills), dacite (Hills, Plains), travertine (Hills), leucite (Hills), mudstone (Plains), peat (Plains), chalk (Plains), loess (Plains), flint (Plains).
- Using any "not on Plateaus shelf" id needs Episode.Plateaus added to that kind (precedent: D-072, D-073). Not decided here.

## Proposed kinds per level (9 pickups each)

| Level | Kinds | Review kind(s) | NEW |
|---|---|---|---|
| H1 Deccan Plateau | basalt, zeolite*, calcite*, olivine* | basalt | none |
| H2 Ethiopian Highlands | basalt, ignimbrite, tuff, clay* | basalt, ignimbrite | opal |
| H3 Tassili n'Ajjer | sandstone, granite, quartz, clay* | sandstone, granite | none |
| H4 Mesa Verde | sandstone, shale, siltstone (flag D), conglomerate? (flag D) | sandstone, shale | coal |
| H5 Antarctic Plateau | granite, gneiss, sandstone, siltstone | all four | meteorite, coal (shared) |
| H6 Loess Plateau | loess*, quartz, feldspar*, mica*, calcite* | quartz | none |
| H7 Tibetan Plateau | halite, ulexite (flag G), granite, serpentinite*/peridotite*/chromite* | halite, granite | none (borax optional) |

`*` = existing id not yet on the Plateaus shelf.

NEW total: opal, coal, meteorite = 3. For only 2, drop opal (Ethiopia then uses basalt, ignimbrite, tuff, clay).

Suggested 9-pickup splits:
- H1: basalt x3, zeolite x3, calcite x2, olivine x1.
- H2: basalt x3, ignimbrite x2, tuff x1, clay x1, opal x2.
- H3: sandstone x4, granite x2, quartz x2, clay x1.
- H4: sandstone x4, shale x2, coal x3.
- H5: granite x2, gneiss x1, sandstone x2, coal x1, meteorite x3.
- H6: loess x4, quartz x2, feldspar x1, mica x1, calcite x1.
- H7: halite x3, granite x2, serpentinite x2, chromite x1, ulexite x1.

## H1 Deccan Plateau, India
- Source https://en.wikipedia.org/wiki/Deccan_Traps:
  - Covers "about 500,000 square kilometres"; once "may have covered about 1.5 million km2".
  - Flood basalt "more than about 2 kilometres (1.2 mi) thick"; volume "about 1 million km3".
  - Eruptions "between around 66.3 to 65.6 million years ago" (end of dinosaurs' time).
  - "At least 95% of the lavas are tholeiitic basalts"; "Major mineral constituents are olivine, pyroxenes and plagioclase".
  - Caption: "Crystals of epistilbite and calcite in a vug" (Jalgaon District). The page never says "zeolite"; epistilbite is a zeolite (general knowledge, not on page).
- Zeolites (search summaries only, not opened):
  - Deccan secondary minerals: heulandite, stilbite, mordenite, apophyllite most common; calcite then zeolites then apophyllite: https://core.ac.uk/download/228401481.pdf, https://suvratk.blogspot.com/2019/08/secondary-mineralization-in-deccan.html (blog).
  - Pune: Wagholi quarries list heulandite group and cavansite with calcite, heulandite, stilbite (https://www.mindat.org/locentry-480122.html, https://mdpi.com/2075-163X/15/2/126 403 to me). Pashan quarries: apophyllite and stilbite only on a dealer page.
  - Nashik: Not confirmed. Only a dealer listing (Pandulena Hills, powellite). Sakur quarry listed under Nashik Division is in Ahmednagar District. Say "near Pune" or "Maharashtra", not Nashik.
- Hot springs (steam vents):
  - Vajreshwari page says only that the village is "famous for the Vajreshwari Temple and hot water springs" (https://en.wikipedia.org/wiki/Vajreshwari). Thane district, near the Tansa River. No temperature or cause given.
  - Temperatures about 43-52 °C, around 7 km along the Tansa riverbed, 7 to 21 springs: travel and eco sites only (https://ecoheritage.cpreec.org/hot-water-springs-of-maharashtra/, https://www.outlookindia.com/traveller/ot-getaway-guides/maharashtra_the_hot_springs_of_vajreshwari). Not confirmed by a scientific source.
  - Heat source: Not confirmed. Volcanism ended long ago; a blog suggests normal deep-earth heat, not magma. Do not say "volcano under Vajreshwari". Safe wording: "Warm springs still flow in the old lava land." Steam vents as a game hazard are a stylisation; flag for the lesson text.
- Kid facts: (1) many lava flows piled up more than 2 km thick; (2) the lava erupted about 66 million years ago, near the end of the dinosaurs; (3) the plateau covers about 500,000 km2.
- Sensitive: Vajreshwari is a temple site; show springs only, no temple.
- Overlap note: Plateaus Easy kinds (basalt, laterite, hematite, gibbsite, etc.) may already use Deccan; I did not read Easy level data.

## H2 Ethiopian Highlands
- Source https://en.wikipedia.org/wiki/Ethiopian_Highlands:
  - "with little of its surface falling below 1,500 m"; "the largest continuous area of its elevation in the continent".
  - Built by "piling layers upon layers of voluminous fissure-fed basaltic lava flows".
  - "Its summit, Ras Dashen (4,550 m), is the highest peak in Ethiopia."
- Simien, https://en.wikipedia.org/wiki/Simien_Mountains_National_Park: World Heritage Site 1978; "volcanic flood basalts dating from the Paleogene period roughly 30 million years ago"; "rich in perforated basalt"; walia ibex "found nowhere else in the world".
- Geladas, https://en.wikipedia.org/wiki/Gelada:
  - "found only in the high grasslands of the deep gorges of the central Ethiopian plateau."
  - "grass blades make up to 90% of their diet"; the only primates that mainly graze.
  - Herds of "up to 60 reproductive units" (each unit up to 12 females plus young and 1-4 males), so herds can be hundreds strong (my arithmetic, not a quoted number).
  - Live at "1,800-4,400 m".
- Opal (NEW candidate):
  - GIA https://www.gia.edu/gems-gemology/winter-2014-gemnews-new-deposit-black-opal-from-ethiopia: Wollo opal found in 2008 (Wegel Tena); Stayish black opal (2013) "at an altitude of around 3,000 meters", at "the contact zone between the volcanic rock series and the underlying clay-rich layer"; layers of volcanic ash and ignimbrite.
  - Search summary: Wegel Tena opal "occur in Oligocene rhyolitic ignimbrites" (https://figshare.com/articles/Geochemical_and_petrological_characterization_of_gem_opals_from_Wegel_Tena_Wollo_Ethiopia_opal_formation_in_an_Oligocene___soil/3453035/1, not opened).
  - Draft card idea: "A gem made of silica and water. Some flash rainbow colours. Ethiopia's plateau has a lot of it."
- Kid facts: (1) geladas live only in Ethiopia's highlands and eat mostly grass; (2) the highlands are piled-up lava flows; (3) Ras Dashen reaches 4,550 m.
- Sensitive: people have farmed Simien for a long time; resettlement of residents is a disputed topic ("green colonialism" criticism on the Simien page). Do not show villages as in the way; avoid the topic. Gelada wording: not "baboon" in a scary sense; they are grazers.
- Unconfirmed: granite not on the Highlands page. Tuff: GIA says "volcanic ash" layers, not the word tuff (inference). Clay: GIA "clay-rich layer" (fine).

## H3 Tassili n'Ajjer, Algeria
- Source https://en.wikipedia.org/wiki/Tassili_n%27Ajjer:
  - "composed largely of sandstone"; "nearly 300 natural rock arches"; rivers "dug up to 400 m deep into the sandstone and clay layers".
  - "Among the 15,000 engravings so far identified"; "the earliest pieces of art are presumed to be 12,000 years old".
  - Highest point Adrar Afao, 2,158 m; area "more than 72,000 km2"; UNESCO 1982.
  - Wikipedia does not mention wind erosion (first 100,000 chars).
- UNESCO (search summary, https://whc.unesco.org/en/list/179): "rock forests" of eroded sandstone; shaped by "the corrosive effects of water, and then wind"; "sandstone around a mass of granite dating from the Precambrian". Exact wording unverified by me.
- National Geographic (search summary): over 300 arches (counts differ: ~300 vs over 300).
- Kid facts: (1) water carved the plateau, then wind smoothed its rocks into arches and "rock forests"; (2) people painted and carved about 15,000 pictures here, some about 12,000 years old, showing a time when the Sahara was green (green-Sahara point Not confirmed on pages read).
- Sensitive: rock art is protected heritage; the player must not collect or damage it. Show it only as background.
- Unconfirmed: "metallic oxides" stain the sandstone (search summary of Wikipedia); hematite or goethite is inference, so not proposed. Quartz is inference (sandstone grains); keep or swap for conglomerate (not sourced either).

## H4 Mesa Verde, USA
- Source https://en.wikipedia.org/wiki/Mesa_Verde_National_Park:
  - "more than 5,000 sites, including 600 cliff dwellings"; elevation "about 6,000 to 8,572 feet (1,829 to 2,613 m)"; 52,485 acres (212 km2); World Heritage Site 1978.
  - Ancestral Puebloans: "by the end of the 12th century, they began to construct the massive cliff dwellings".
  - Cliff House Sandstone "the area's youngest rock layer"; shale zones "determine where alcoves formed".
- NPS https://www.nps.gov/meve/learn/nature/geology.htm:
  - Cliff House: "thick, orange-buff colored sandstone"; "The alcoves formed in this sandstone created perfect shelters for the Ancestral Pueblo people."
  - Menefee: "Thin beds of sandstones and coal seams can also be found within this formation"; "fine grained shales formed".
  - Point Lookout: "tan to buff colored sandstone, with some shale lenses".
  - Mancos Shale: "tends to be gray in color".
  - Alcoves: "Alcove formation is assisted by water that is absorbed into and percolates through pores in the sandstone"; frozen blocks "collapse, fall off in blocks".
- Coal (NEW candidate): sourced above. Draft card idea: "A black rock made from old swamp plants pressed for millions of years. It burns." Links to the existing peat card ("in time it can become coal").
- Kid facts: (1) about 600 cliff dwellings sit in natural rock alcoves; (2) seeping water and frost loosened the sandstone to make the alcoves; (3) the park rises to about 2,600 m.
- Sensitive: cliff dwellings are ancestral homes of today's Pueblo peoples. Do not have the player collect inside dwellings, climb walls, or call them "ruins of a lost people". Pick up rocks only on trail or mesa top.
- Unconfirmed (flag D): siltstone and conglomerate not on pages read. Use only sandstone, shale, coal if strict.

## H5 Antarctic Plateau
- Source https://en.wikipedia.org/wiki/Antarctic_Plateau: "average elevation of about 3,000 metres" (citation-needed tag); "diameter of about 1,000 kilometres"; temperatures "falling as low as -92 °C".
- East Antarctic Ice Sheet, https://en.wikipedia.org/wiki/East_Antarctic_Ice_Sheet: "around 2.2 km (1.4 mi) thick on average"; thickest 4,897 m; area 10,200,000 km2; enough ice "to raise global sea levels by 53.3 m"; Dome Argus about 4 km high; Dome A "nearly -100 °C"; ice sits on the East Antarctic Shield and over the buried Gamburtsev Mountains.
- Transantarctic Mountains, https://en.wikipedia.org/wiki/Transantarctic_Mountains: about 3,500 km long; Mount Kirkpatrick 4,528 m; "a basement of granites and gneisses"; "the Beacon Supergroup sandstones, siltstones, and coal"; "Ferrar Dolerite" dikes and sills. Antarctica page: "low-grade coal is known to exist across many parts of the Transantarctic Mountains."
- Meteorites (NEW candidate):
  - Why on blue ice: ice carries buried meteorites to mountain barriers; wind and sublimation strip ice and leave them on top (search summary of https://caslabs.case.edu/ansmet/?p=1929, https://discovermagazine.com/the-sciences/do-more-meteorites-fall-on-antarctica). Antarctica page: in blue-ice areas "the wind and sublimation remove more snow than is accumulated".
  - Count: Britannica (search summary, 403 to me) "more than 35,000 individual meteorites". Alaska Geophysical Institute (https://www.gi.alaska.edu/alaska-science-forum/antarctica-meteorite-storehouse, older article) gives "More than 7,000 fragments" and first find 1912. Use "tens of thousands" only after a primary check.
  - Draft card idea: "A rock that fell from space. Dark meteorites are easy to spot on blue ice."
- Kid facts: (1) the ice is about 2 km thick on average; (2) it is the coldest place on Earth, near -90 °C; (3) meteorites from space are found on its blue ice.
- Sensitive: no people live there; Antarctic Treaty protects it (not sourced here). Collecting meteorites is done by scientists; card can say "scientists collect them".
- Unconfirmed: what rock lies directly under the plateau ice (only "shield"/"craton" and buried mountains). Dolerite has no id; basalt is not the same rock, so not proposed. Gneiss and granite are in the mountains, not on the plateau surface; level should be framed at the plateau edge.

## H6 Loess Plateau, China
- Source https://en.wikipedia.org/wiki/Loess_Plateau:
  - "635,000-square-kilometre" area, about 6.6% of China; infobox elevation 1,200 m.
  - Loess "around 200 to 300 metres" thick near the Liupan Mountains.
  - "More than 90% of the sediments in the Yellow River come from the Loess Plateau."
  - Loess carried by "monsoon wind and dust storms", main source Gobi and nearby deserts.
  - "More than 90% of the loess is calcite, feldspar, mica and quartz"; about half quartz.
  - Terraces mentioned only as natural river terraces in what I read; farm terraces Not confirmed on this page.
- Yaodong, https://en.wikipedia.org/wiki/Yaodong: "earth shelter dwelling common in the Loess Plateau"; "between 30 and 40 million people" lived in them in the early 2000s; earth keeps "the interior warm in winter and cool in summer".
- Kid facts: (1) wind blew dust from the Gobi Desert and piled it up to 300 m thick; (2) the Yellow River gets its colour and most of its mud from this plateau; (3) millions of people live in cave homes dug into the loess.
- Sensitive: yaodong are people's homes; do not treat them as caves to explore or loot. Avoid "poor" framing.
- Clay: Not confirmed on page; not proposed.

## H7 Tibetan Plateau
- Source https://en.wikipedia.org/wiki/Tibetan_Plateau: "the Roof of the World"; "the world's largest and highest plateau above sea level"; "2,500,000 square kilometres"; "average elevation exceeding 4,500 metres"; "large brackish lakes"; Himalayas "mostly of uplifted sedimentary and metamorphic rock". Page does not name granite, gneiss, ophiolite, salt.
- Salt lakes (search summaries of papers, not opened):
  - Assemblages include borax and halite (https://link.springer.com/article/10.1007/s10498-008-9055-y, 2008); lakes rich in lithium and boron; Qarhan Salt Lake, Qaidam Basin, largest potash brine in China (halite).
  - Borax in Qaidam lakes (https://link.springer.com/10.1007/s00343-023-2258-7).
- Ophiolite and granite (search summaries only):
  - Luobusa ophiolite on the plateau holds "one of the largest chromite deposits in China"; mantle peridotites mostly harzburgite (https://pmc.ncbi.nlm.nih.gov/articles/pmid/38933911 and mindat refs).
  - Luobusa thrust onto the Gangdese batholith; "Gangdese granite" (same summaries).
  - Serpentinite in the ophiolite: Not confirmed (typical of ophiolites; not read).
- Kid facts: (1) average height over 4,500 m, so it is called the Roof of the World; (2) it is the largest and highest plateau, about 2.5 million km2; (3) it has many salty lakes.
- Sensitive: many sacred sites (Mount Kailash, lakes, monasteries). Avoid religious places and politics; do not name a country status. Do not show people's prayer flags or monasteries as obstacles.
- Unconfirmed (flag G): ulexite in Tibet not sourced (borax is). Gneiss not sourced for the plateau itself.

## Flags
- A. Zeolite, calcite, olivine, clay, loess, feldspar, mica, serpentinite, peridotite, chromite are not on the Plateaus shelf; each needs Episode.Plateaus added (a decision, not made here).
- B. H1 Nashik zeolites: Not confirmed. Pune (Wagholi) only from locality databases and a paper I could not open.
- C. H1 Vajreshwari spring temperatures and heat source: Not confirmed by a scientific source. Steam vents are a game stylisation.
- D. H4 siltstone and conglomerate: not sourced for Mesa Verde.
- E. H5 meteorite count: sources differ (7,000+ fragments in an older article vs 35,000+ in Britannica summary).
- F. H2 opal: GIA read directly; ignimbrite host only via search summary.
- G. H7 rocks and salts: all from search summaries of papers; ulexite unsourced (borax sourced).
- H. Quotes come from a summarising tool, not verbatim page reads. Several Wikipedia pages read only to char 100,000.
- I. Elevations vary: Antarctic Plateau "about 3,000 m" carries a citation-needed tag; Dome A/Argus about 4 km.

## Summary table

| Place | Suggested kinds (E = existing id, E+ = existing id needing Plateaus) | New kind candidates | Confidence |
|---|---|---|---|
| H1 Deccan | basalt E, zeolite E+, calcite E+, olivine E+ | none | High basalt/olivine; medium zeolite |
| H2 Ethiopia | basalt E, ignimbrite E, tuff E, clay E+ | opal | High basalt; medium opal/ignimbrite |
| H3 Tassili | sandstone E, granite E, quartz E, clay E+ | none | High sandstone; medium granite |
| H4 Mesa Verde | sandstone E, shale E | coal | High |
| H5 Antarctic | granite E, gneiss E, sandstone E, siltstone E | meteorite, coal | High rocks; medium meteorite count |
| H6 Loess | loess E+, quartz E, feldspar E+, mica E+, calcite E+ | none | High |
| H7 Tibet | halite E, granite E, chromite E+, serpentinite E+ (or peridotite E+), ulexite E? | borax (optional) | Medium-low |
