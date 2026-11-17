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
- [x] Four files exist with the exact names
- [x] Lengths and mean dB are reported and near the targets
- [x] The cave loop loops without a click; existing audio is unchanged

**Blocked by:** none

**Status:** done

**Verified:** Four assets measured with ffmpeg. Decoded seam measured and final preview opened. Unity EditMode passed 334 of 334 tests.

**Builder to run:** none (ticket 06)

## Files

- `Tools/Audio/make_hills_normal_audio.py` generates and measures the four audio files.
- `Assets/Audio/amb_cave_drips.ogg` is 24.000 s at −21.9 dB mean.
- `Assets/Audio/sfx_splash.wav` is 0.300 s at −24.0 dB mean.
- `Assets/Audio/sfx_bat_flap.wav` is 0.250 s at −26.0 dB mean.
- `Assets/Audio/sfx_burner.wav` is 0.500 s at −24.0 dB mean.
- `.scratch/hills-normal/audio-preview.png` shows waveforms and averaged spectra from decoded assets.
- Unity generated matching `.meta` files for the four new sounds.

Refinements after reviewing the preview: added a second filter stage to soften high-end noise, softened the loudest cave drips, and smoothed the last sample step. The decoded seam steps are 0.003082 and 0.003571 full scale, similar to ordinary nearby sample steps. The spectra roll off at the high end. Listening in Unity is on the play checklist. No tracked sound files changed.
