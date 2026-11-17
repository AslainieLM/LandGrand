# 04: Audio — cave drips, splash, bat flap and burner

**What to build** (spec section 8, `make_hills_normal_audio.py`):

1. Write `Tools/Audio/make_hills_normal_audio.py` in the style of `Tools/Audio/make_hills_audio.py`:
   - The same helpers, `write_wav`, and ffmpeg ogg encoding.
   - A low-pass and high-pass on every loop.
   - D-033 comments on every line.
2. **Files** in `Assets/Audio/`:
   - `amb_cave_drips.ogg`: a 24 s loop with its end folded onto its start. It has slow, uneven drips with a short echo tail, a very soft far rumble and air tone. No birds, insects or wind. Mean about −21 dB.
   - `sfx_splash.wav`: about 0.3 s, a splash with a small bubble tail. About −24 dB.
   - `sfx_bat_flap.wav`: about 0.25 s, three soft, quick wing flaps. Not scary. About −26 dB.
   - `sfx_burner.wav`: about 0.5 s, a gas-burner whoosh. About −24 dB.
3. Measure each file with ffmpeg `volumedetect` and report the mean dB and the length.
4. Write the preview `.scratch/hills-normal/audio-preview.png`: waveforms and spectra. Check that there is no broadband hiss.

**Checklist**
- [ ] Four files exist with the exact names
- [ ] Lengths and mean dB are reported and near the targets
- [ ] The cave loop loops without a click; existing audio is unchanged

**Blocked by:** none

**Status:** not started

**Builder to run:** none (ticket 06)
