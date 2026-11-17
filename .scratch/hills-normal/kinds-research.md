# Hills Normal: rock and mineral kinds research

Date: 2026-11-17. Scope: proposed `ItemCatalog` kinds for Hills Normal N1 to N5, 2 per level.
Status: research draft. Not adviser-approved. Kid texts are drafts and need teacher review (Q-04, D-043).

Existing kinds checked in `Assets/Scripts/Rules/LevelData.cs` `ItemCatalog`: granite, quartz, feldspar, mica,
andesite, obsidian, pumice, slate, marble, garnet, sandstone, clay, conglomerate, shale, basalt, magnetite,
sulfur, pyrite, limestone, gneiss, scoria, hornblende, diorite, gold, tuff, hematite, serpentinite, chromite,
schist, calcite, trachyte, tanzanite, gabbro, olivine, coral stone, limonite, kaolinite.
Hills Easy already uses limestone, calcite, clay, coral stone, andesite, limonite, kaolinite, quartz, magnetite, feldspar.

Confidence scale: strong = primary or authoritative source ties the kind to the place; medium = good source for the
kind and its general link, but site tie is secondary or regional; weak = only travel or unsourced pages, or inference.

---

## Background checks (the five questions)

### 1. Pagsanjan Gorge geology
- No site-specific geology found. Wikipedia pages for Pagsanjan Falls, Pagsanjan Gorge National Park and Bumbungan
  River give no rock types. The only rock claim ("limestone and volcanic rock") is on an uncited mirror site.
  - https://en.wikipedia.org/wiki/Pagsanjan_Falls
  - https://en.wikipedia.org/wiki/Pagsanjan_Gorge_National_Park
  - https://en.wikipedia.org/wiki/Bumbungan_River (river rises in Cavinti highlands, drains to Laguna de Bay)
- Regional setting is volcanic. Laguna de Bay sits in the Laguna Caldera. Its eruptions (about 1 Ma and
  27,000 to 29,000 years ago) left thick ignimbrite sheets in Rizal, Metro Manila, Laguna and Bulacan.
  - https://en.wikipedia.org/wiki/Laguna_Caldera (cites PHIVOLCS and Catane et al.; check primary before final)
  - PHIVOLCS volcano list: https://www.phivolcs.dost.gov.ph/ (Laguna Caldera listed as potentially active)
- Gap: no source says the gorge walls are Laguna Caldera ignimbrite. Next step: MGB 1:50,000 geologic sheet
  for Pagsanjan or Laguna, or MGB *Geology of the Philippines* (2nd ed., 2010).
- Verdict: "volcanic rock" is likely but unproven at the gorge. Do not claim ignimbrite at the gorge as fact.

### 2. Banaue terraces bedrock and soil
- Bedrock: no named rock unit found for Banaue. MGB Cordillera chief geologist said the terraces are prone to
  landslides "owing to the composition of the mountain rocks", without naming them.
  - Inquirer, "Rice terraces vulnerable to landslides, says geologist": https://newsinfo.inquirer.net/88363/rice-terraces-vulnerable-to-landslides-says-geologist/amp (news, medium)
- ASCE landmark page: terraces are a rearrangement of the Cordillera "from bedrock to topsoil".
  - https://www.asce.org/about-civil-engineering/history-and-heritage/historic-landmarks/ifugao-rice-terraces
- Soil: clay-rich. Local soil series include Mayoyao Clay Loam, Longa silty clay loam, Santa Maria series and
  undifferentiated mountain soil (IFSU biophysical synthesis report, Ifugao pilot barangays). Clay subsoil
  limits seepage (Water, 2020, Bangaan/Hapao/Poblacion terraces).
  - IFSU Synthesis Report: https://issuu.com/mdgf1656/docs/ifsu_synthesis_report_final (medium)
  - Soil fertility limitations in the rice terraces of Ifugao (FAO AGRIS): https://agris.fao.org/search/ru/records/647246bc2c1d629bc97985a3
  - Britannica, Banaue rice terraces: https://www.britannica.com/place/Banaue-rice-terraces
- A 2024 Hapao study mentions stone walls from the "Sagada rock formation" (core.ac.uk/download/363398821.pdf, not verified in full).
- Verdict: soil (clay, clay loam) is documented. Bedrock type is not documented in sources found.

### 3. Callao Cave formations
- Cave is in limestone; Peñablanca means "white rocks" after the white limestone. About 300 limestone caves in the town.
  Speleothems listed: stalactites, stalagmites, flowstones, dripstones, curtains, helictites, columns.
  - https://en.wikipedia.org/wiki/Callao_Cave (cites Guillermo 2012, Scarecrow Press; speleothem list cites a travel site)
- Host rock: Callao Limestone (Callao Formation), Cagayan Valley Basin, Miocene (clastic part dated Middle Miocene, NN7).
  Main karst areas are in the Callao Limestone on the east margin of the Cagayan Valley.
  - National Museum of Nature and Science (Tokyo) bulletin: https://www.kahaku.go.jp/research/publication/geology/download/45/BNMNS_C45_37.pdf
  - Showcaves.com Cagayan region: https://showcaves.com/english/ph/region/Cagayan.html (weak)
- Stalactites = calcite: true in general for limestone caves (speleothems are mostly calcite, some aragonite).
  No mineral analysis of Callao speleothems found. Source for the general fact:
  - Britannica, "Speleothem": https://www.britannica.com/science/speleothem
  - USGS, "Karst and caves": https://www.usgs.gov/ (general karst explainer)
- Verdict: calcite stalactites/flowstone is safe as a general statement. Callao-specific chemistry is unproven.

### 4. Hundred Islands rock age and type
- Type: karst limestone from old coral reefs, uplifted, then wave-cut into mushroom shapes.
  - Malabrigo Jr. et al. 2022, "Plant diversity assessment of karst forest formations in the Hundred Islands
    National Park", Sylvatrop 32(2): https://www.academia.edu/116702171/ (peer-reviewed; supports "karst/limestone")
  - Tourism Promotions Board: https://phworldexpo.tpb.gov.ph/hundred-islands-national-park
- Age: conflict. "About two million years" (Pleistocene) is widely repeated but unsourced (Wikipedia tags it
  "citation needed"). One parks site says Miocene reef limestone, 15 to 20 Ma, also unsourced.
  - https://en.wikipedia.org/wiki/Hundred_Islands_National_Park
  - https://www.internationalparks.org/philippines/Hundred%20Islands (weak)
- Verdict: say "old coral reef limestone". Avoid a number, or say "very old" until an MGB source is found.

### 5. Cappadocia fairy chimney cap rock
- Peer-reviewed / academic answer: chimneys are carved in ignimbrite sheets (mainly Kavak, Zelve, Cemilköy;
  some on Kızılkaya and Gördeles). Caps are harder layers: mostly consolidated pumice fall deposits (Zelve basal
  fall, locally "Esbelli stone") or more welded ignimbrite, and near Cemil–Şahinefendi, blocks fallen from cornices.
  Caps are not basalt in this account. Ignimbrite ages about 10 to 5 Ma; Kumtepe 0.2 Ma.
  - Aydar et al., "Volcanism and evolution of the landscapes in Cappadocia", OpenEdition Books (IFEA):
    https://books.openedition.org/ifeagd/3212 (strong)
  - "First Paleo-Fairy Chimney Findings in the Cappadocia Region, Turkey: a Possible Geomorphosite" (Univ. Bern repo):
    https://boris.unibe.ch/122199
  - Sarıkaya, Çiner, Zreda 2011, "Fairy chimney erosion rates on Cappadocia ignimbrites", Geomorphology (cosmogenic nuclides)
  - METU thesis on fairy chimney development (welding degree, thickness, slope): https://open.metu.edu.tr/bitstream/handle/11511/17659/index.pdf
- Popular sources (Live Science, Smithsonian, travel) say "basalt cap". That is a simplification; some caps elsewhere
  in the region are lava, but the main Göreme chimneys follow the ignimbrite/pumice-fall account.
  - https://www.livescience.com/planet-earth/geology/fairy-chimneys-the-stone-spires-in-turkey-that-form-the-worlds-most-unusual-high-rise-neighborhood (medium, popular)
- UNESCO site 357 (Göreme National Park and the Rock Sites of Cappadocia, 1985): https://whc.unesco.org/en/list/357/ (page returned 403 to the fetch tool; not read)
- Minerals named in the IFEA text: plagioclase, biotite, quartz, amphibole, pyroxenes, oxides; zeolites replacing
  volcanic glass (Sarıhıdır, Tuzköy); obsidian lithics in Kumtepe.
- Verdict: cap = harder welded ignimbrite or pumice layer. Teach "a hard hat of harder rock on soft tuff".

---

## Per-level proposals

### N1 Hundred Islands, Alaminos, Pangasinan (old coral reefs)

| Candidate | Type | Kid text (draft) | Source | Confidence |
|---|---|---|---|---|
| Aragonite | mineral | The mineral corals use to build their hard homes. It can turn into calcite. | Britannica "Aragonite" https://www.britannica.com/science/aragonite ; living reefs at Hundred Islands (Wikipedia above, Sagip Lingayen Gulf project) | medium (general fact; no site sample) |
| Coquina / shell limestone | rock | A rock made of broken shells and coral pieces stuck together. | Britannica "Coquina" https://www.britannica.com/science/coquina ; site tie only by inference | weak |
| Dolomite | rock/mineral | Like limestone, but with magnesium. It fizzes only when powdered. | No Hundred Islands source found | weak (do not use) |

Review pickup: coral stone (best fit, story is old reefs), limestone (Malabrigo 2022 says karst limestone). Calcite also fits.
Note: coral stone and limestone are already Easy kinds, so N1 is a natural "review" level.

### N2 Callao Cave, Peñablanca, Cagayan (caves in limestone hills)

| Candidate | Type | Kid text (draft) | Source | Confidence |
|---|---|---|---|---|
| Flowstone (travertine family) | rock | Cave rock made when water flows over walls and leaves calcite behind. | Wikipedia Callao Cave (flowstones listed; travel citation); Britannica "Speleothem" | strong for general fact, medium for site |
| Dripstone (stalactite) | rock | Rock icicles. Drops of water leave calcite as they drip from the roof. | Wikipedia Callao Cave (stalactites, dripstones); Britannica "Speleothem", "Stalactite" https://www.britannica.com/science/stalactite | strong |
| Aragonite | mineral | A cousin of calcite. It can grow as tiny needles in caves. | Britannica "Speleothem" (some speleothems are aragonite); no Callao analysis | weak at site |

Review pickup: limestone (host rock: Callao Limestone, kahaku.go.jp bulletin) and calcite (stalactite mineral).
Design note: flowstone and dripstone overlap. Pick one, or call the dripstone item "stalactite" for clarity.

### N3 Pagsanjan Gorge, Laguna (river wearing a valley)

| Candidate | Type | Kid text (draft) | Source | Confidence |
|---|---|---|---|---|
| Ignimbrite | rock | Rock from a hot cloud of ash and stones from a big volcano blast. | Laguna Caldera ignimbrite sheets in Laguna (Wikipedia Laguna Caldera, citing PHIVOLCS/Catane); not proven at the gorge | weak at gorge, medium regionally |
| River pebble / gravel (alluvium) | sediment | Rocks the river rolled and rubbed until they turned smooth and round. | Gorge "lined with huge boulders, rocks" (Wikipedia Pagsanjan Falls); Britannica "Alluvium" https://www.britannica.com/science/alluvium | medium (process strong, rock type unknown) |
| Silt | sediment | Very fine mud the river carries and drops where it slows down. | Bumbungan delta at Lumban/Laguna de Bay (Wikipedia Bumbungan River); Britannica "Silt" | medium |

Review pickup: tuff (Laguna Caldera ash rock; safest volcanic fit), basalt or andesite (generic volcanic; not site-proven),
conglomerate (rounded river stones, matches the erosion lesson).
Open issue to surface: the gorge's real bedrock is not documented in any source found. Ask before claiming a rock type.

### N4 Banaue Rice Terraces, Ifugao (farming on hill and mountain slopes)

| Candidate | Type | Kid text (draft) | Source | Confidence |
|---|---|---|---|---|
| Loam (clay loam) | soil | Soft, rich soil of clay, silt and sand. Good for growing rice. | IFSU Synthesis Report (Mayoyao Clay Loam, Longa silty clay loam); FAO AGRIS Ifugao soil fertility study | medium |
| Humus / topsoil | soil | Dark, soft top layer of soil made from old leaves and plants. | ASCE landmark ("bedrock to topsoil"); Britannica "Humus" https://www.britannica.com/science/humus | medium (general) |
| Silt | sediment | Very fine soil grains, finer than sand. They help hold pond water. | Water (2020) terrace infiltration study (fine particles fill pores) | weak to medium |

Review pickup: clay (strong; clay subsoil keeps water in the paddies), kaolinite (clay mineral; general weathering link, not site-proven).
Open issue: no bedrock unit found for Banaue. If the team wants a rock (not a soil) here, MGB sheet data is needed.
Design note: loam and humus are soils, not rocks or minerals. Check that the Collection shelves accept a soil kind.

### N5 Cappadocia, Türkiye (fairy chimneys from volcanic tuff)

| Candidate | Type | Kid text (draft) | Source | Confidence |
|---|---|---|---|---|
| Ignimbrite | rock | Rock left by a hot flow of ash and pumice. Fairy chimneys are cut from it. | IFEA "Volcanism and evolution of the landscapes in Cappadocia" https://books.openedition.org/ifeagd/3212 ; Sarıkaya et al. 2011 Geomorphology | strong |
| Zeolite | mineral | A mineral that grows when volcanic glass changes slowly in water. | IFEA text (zeolites replacing glass at Sarıhıdır, Tuzköy); Britannica "Zeolite" https://www.britannica.com/science/zeolite | medium |
| Biotite | mineral | A dark, shiny, flaky mineral. It is a kind of mica. | IFEA text (biotite phenocrysts) | medium (overlaps existing mica) |

Review pickup: tuff (the soft body of the chimneys), pumice (the hard caps are often consolidated pumice fall), obsidian
(lithics in Kumtepe ignimbrite). Basalt fits only the popular "basalt cap" story, which the academic source contradicts.
Note: ignimbrite is a welded or unwelded kind of tuff. If both tuff and ignimbrite are in the game, the kid texts must
explain the difference (for example: "ignimbrite is tuff made by a hot flow, often pressed hard").

---

## Top picks (2 per level)

| Level | Pick 1 | Pick 2 | Review pickup |
|---|---|---|---|
| N1 Hundred Islands | Aragonite (medium) | Coquina (weak) | Coral stone, limestone |
| N2 Callao Cave | Flowstone (strong general / medium site) | Stalactite / dripstone (strong) | Limestone, calcite |
| N3 Pagsanjan Gorge | River pebble / gravel (medium) | Ignimbrite (weak at site) or silt (medium) | Tuff, conglomerate |
| N4 Banaue | Loam / clay loam (medium) | Humus / topsoil (medium) | Clay, kaolinite |
| N5 Cappadocia | Ignimbrite (strong) | Zeolite (medium) | Tuff, pumice |

Conflict to settle: ignimbrite fits N3 (regional) and N5 (strong). Use it once, at N5, and use it as review at N3 only
if a gorge source is found.

## Gaps to raise as open questions
1. Pagsanjan Gorge bedrock: undocumented. Need MGB geologic sheet.
2. Banaue bedrock: undocumented. Need MGB sheet or Medina (2003) thesis.
3. Hundred Islands age: "2 million years" is unsourced; Miocene claim is also unsourced.
4. Soil kinds (loam, humus, silt): confirm the game accepts soils as collectible "kinds".
5. Cappadocia cap: academic sources say welded ignimbrite / pumice fall, not basalt. Kid text should not say basalt.

## MGB / geologic map check (N3, N4)

Checked 2026-11-17 by web search only. The MGB 1:50,000 quadrangle sheets are not online. FOI replies say the sheets
are sold by MGB (PHP 500 per map) as a frontline service: https://foi.gov.ph/requests/geologic-map-of-laguna-philippines
The MGB Banaue 2022 case study page returned HTTP 403 and was not read.

### N3 Pagsanjan Gorge / Falls

| Finding | Source | Confidence |
|---|---|---|
| No source names the rock of the gorge walls or the falls lip. Wikipedia cites only "boulders, rocks". | https://en.wikipedia.org/wiki/Pagsanjan_Falls | n/a |
| Tourism pages conflict: "volcanic rock" versus "limestone / karst walls". Do not use either. | https://www.unchartedphilippines.com/en/adventures/the-waterfalls-of-laguna-hulugan-and-pagsanjan-cavinti/ ; https://national-parks.org/philippines/pagsanjan-gorge-national-park/ | weak |
| Regional unit: the Pleistocene "Laguna Formation" is tuff, volcanic breccia and sandstone. This is a figure legend for the Kaliwa-Laiban area in Rizal/Quezon, not the gorge. | JICA report figure, https://openjicareport.jica.go.jp/pdf/11721933_05.pdf | weak (regional, OCR text) |
| Banahaw de Lucban is andesite and/or basalt lava "overlying Laguna formation". This cites MGB Lucban Quadrangle sheet 3362III (1999). The study area is Majayjay/Lucban, near but not at the gorge. | Ravina 2016, Mapua student paper, https://www.slideshare.net/slideshow/cege-article-culled-68129192/68129192 | weak to medium |

Conclusion: the gorge is probably cut in volcanic rock of the Laguna Formation, but no source states this for the
gorge itself. Need MGB sheet 3362III (Lucban) or the Pagsanjan-area sheet.
Possible new kind: volcanic breccia (a rock of sharp volcanic pieces stuck together). Confidence is weak, and it is
regional only. Review pickup: tuff (regional Laguna Formation).

### N4 Banaue / Batad

| Finding | Source | Confidence |
|---|---|---|
| No source found that names the Banaue or Batad bedrock. No source found for Klondyke, Zigzag, or diorite at Banaue. | searches of MGB, UP, and UNESCO pages | n/a |
| The MGB Cordillera geologist says the terraces are landslide-prone "due to the composition of the mountain rocks". No rock is named. | Inquirer 2011, https://newsinfo.inquirer.net/88363/rice-terraces-vulnerable-to-landslides-says-geologist | medium (MGB quote, news source) |
| The Hapao (Hungduan) terrace walls are said to be "from Sagada rock formation". This has no geologic citation, and Hapao is not Banaue. | https://en.wikipedia.org/wiki/Banaue_Rice_Terraces | weak |
| The talk "Neogene stratigraphy of south-central Ifugao" (UP class, NIGSCon 1998) is known by title only. | https://issuu.com/nigscon2016/docs/nigscon_1998 | n/a (could not read) |

Conclusion: no new rock kind is sourced for Banaue. Keep the soil picks (loam, humus). Review pickup: clay.
The open issue stands: MGB Bontoc/Banaue sheet data is needed.
