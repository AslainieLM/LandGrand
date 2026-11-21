"""Makes the Plains audio for LandGrand (D-069).  # Says what this script is for

Run from the Unity project folder:  python Tools/Audio/make_plains_audio.py   (needs ffmpeg and Pillow)  # How to run it
It writes five files to Assets/Audio/:  # What it makes
  bgm_plains.ogg          Plains music: a calm, sunny piece in D major, 4/4, 96 BPM, 16 bars (40 s), no drums.  # Music
                          It reuses the voices of make_menu_music.py, like bgm_hills, but has its own tune and chords.  # Same family
  amb_grass_easy.ogg      Easy: soft wind over grass and rice, with crickets chirping, 24 s loop.  # Easy ambience
  amb_wetland_normal.ogg  Normal: the grass wind, plus frogs croaking and far water-bird calls, 24 s loop.  # Normal ambience
  amb_dry_hard.ogg        Hard: a dry, stronger wind over the plain, plus far thunder now and then, 24 s loop.  # Hard ambience
  sfx_straw_bounce.wav    A soft, springy "fwump" with a light straw rustle when the explorer bounces on straw.  # Effect
Every loop joins without a gap, and every sound uses a fixed random seed, so each run makes the same files.  # Repeatable
"""  # End of the description
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
import random  # Gives repeatable noise and event times with fixed seeds
from tempfile import TemporaryDirectory  # Holds temporary WAV files
from PIL import Image, ImageDraw  # Draws the review sheet
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares ffmpeg, soft noise, levels, effect writer and measuring

mm = ma.mm  # The menu music script: its notes, voices and WAV helpers
RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "plains-easy" / "audio-preview.png"  # Holds the review graph
SECONDS = 24.0  # The ambience loop length
LOOP_SAMPLES = int(SECONDS * RATE)  # The number of samples in an ambience loop
FOLD_SAMPLES = RATE  # Blends one extra second into the start

BPM = 96  # Beats per minute: the same pace as the menu music
BEAT = 60.0 / BPM  # Seconds per beat
BAR = 4 * BEAT  # Four beats to a bar, so the music is in 4/4
BARS = 16  # Bars in the loop
LENGTH = BARS * BAR  # Loop length in seconds (40)
TAIL = 4.0  # Extra seconds rendered after the loop for echoes and note endings
N = int(round((LENGTH + TAIL) * RATE))  # Total samples rendered
LOOP_N = int(round(LENGTH * RATE))  # Samples in the finished music loop

# Chords for each bar as (bass note, pad notes), MIDI numbers: D, A, Bm, G, four times.
CHORDS = [  # Each bar and its chord
    (38, [50, 54, 57]),  # D: bass D2; pad D3 F#3 A3
    (45, [52, 57, 61]),  # A: bass A2; pad E3 A3 C#4
    (35, [54, 59, 62]),  # Bm: bass B1; pad F#3 B3 D4
    (43, [55, 59, 62]),  # G: bass G2; pad G3 B3 D4
] * 4  # The four chords repeat four times

ARP_PATTERN = [0, 2, 1, 3, 2, 1, 3, 2]  # Which chord tone each eighth note plays: a guitar-like roll

# Melody as (MIDI note, start beat, length in beats). Bars 1 to 4 have no melody, only pad, pluck and bass.
MELODY = {  # Bar number (from 0): melody notes in that bar
    4: [(78, 0, 2), (76, 2, 1), (74, 3, 1)],  # Bar 5 (D): F# E D
    5: [(73, 0, 1), (76, 1, 1), (81, 2, 2)],  # Bar 6 (A): C# E A
    6: [(71, 0, 3), (69, 3, 1)],  # Bar 7 (Bm): B, then A
    7: [(74, 0, 2), (71, 2, 2)],  # Bar 8 (G): D, then B
    8: [(74, 0, 1), (78, 1, 1), (81, 2, 2)],  # Bar 9 (D): D F# A
    9: [(76, 0, 2), (73, 2, 2)],  # Bar 10 (A): E, then C#
    10: [(74, 0, 1), (71, 1, 1), (69, 2, 2)],  # Bar 11 (Bm): D B A
    11: [(71, 0, 2), (67, 2, 2)],  # Bar 12 (G): B, then G, leading back to D
    12: [(78, 0, 2), (76, 2, 1), (74, 3, 1)],  # Bar 13 (D): F# E D
    13: [(73, 0, 1), (76, 1, 1), (81, 2, 2)],  # Bar 14 (A): C# E A
    14: [(71, 0, 3), (69, 3, 1)],  # Bar 15 (Bm): B, then A
    15: [(67, 0, 2), (74, 2, 2)],  # Bar 16 (G): G, then D, leading back to the start
}  # End of the melody


def compose():  # Places every note of the 16 bars, using the voices of the menu music script
    """Places every note of the 16 bars, using the voices of the menu music script."""  # Describes the function
    mm.N = N  # The shared note code writes into arrays of this length
    mm.left = [0.0] * N  # Left speaker samples
    mm.right = [0.0] * N  # Right speaker samples
    for bar, (bass, pad) in enumerate(CHORDS):  # Each bar and its chord
        t0 = bar * BAR  # Bar start time in seconds
        mm.add_note(t0, BAR, mm.freq(bass), mm.BASS, 0.30, 0.0)  # Soft bass note for the whole bar, in the middle
        for k, note in enumerate(pad):  # Each pad note
            pan = (-0.5, 0.0, 0.5)[k]  # Spread the three notes left, middle, right
            mm.add_note(t0, BAR, mm.freq(note) * 0.9985, mm.PAD, 0.055, pan)  # Pad note, a hair flat
            mm.add_note(t0, BAR, mm.freq(note) * 1.0015, mm.PAD, 0.055, -pan)  # Same note a hair sharp, other side, for width
        tones = sorted(pad) + [pad[0] + 12]  # Four chord tones for the plucked roll, low to high
        for step, index in enumerate(ARP_PATTERN):  # Eight eighth notes per bar
            note = tones[index] + 12  # One octave above the pad
            gain = 0.14 if step % 2 == 0 else 0.10  # On-beats a little louder
            pan = 0.3 if step % 2 == 0 else -0.3  # Bounce between left and right
            mm.add_note(t0 + step * BEAT / 2, BEAT / 2, mm.freq(note), mm.PLUCK, gain, pan)  # One plucked note
        for note, beat, beats in MELODY.get(bar, []):  # Melody notes in this bar, if any
            mm.add_note(t0 + beat * BEAT, beats * BEAT * 0.92, mm.freq(note), mm.FLUTE, 0.20, 0.1)  # Slightly short, so the notes breathe


def make_music(temp):  # Composes the music, adds an echo, folds the tail into a seamless loop and saves bgm_plains.ogg
    """Composes the music, adds an echo, folds the tail into a seamless loop and saves bgm_plains.ogg."""  # Describes the function
    compose()  # Place every note
    left, right = mm.left, mm.right  # The mix
    peak = max(max(abs(v) for v in left), max(abs(v) for v in right))  # Loudest sample so far
    scale = 0.5 / peak  # Leave room for the echo
    dry, wet, loop = temp / "plains_dry.wav", temp / "plains_wet.wav", temp / "plains_loop.wav"  # Temporary files
    mm.write_wav(str(dry), [v * scale for v in left], [v * scale for v in right])  # Save the dry mix
    echo = "aecho=0.8:0.55:59|101|157:0.3|0.22|0.14,lowpass=f=7600,highpass=f=40"  # A soft, open echo with a gentle top
    hn.ffmpeg("-i", dry, "-af", echo, wet)  # Add the echo
    l, r = mm.read_wav(str(wet))  # Read the echoed mix back
    loop_l, loop_r = l[:LOOP_N], r[:LOOP_N]  # The loop part
    for i in range(LOOP_N, min(len(l), N)):  # Each sample after the loop end
        loop_l[i - LOOP_N] += l[i]  # Fold the left tail onto the start, so the loop joins smoothly
        loop_r[i - LOOP_N] += r[i]  # Fold the right tail onto the start
    peak = max(max(abs(v) for v in loop_l), max(abs(v) for v in loop_r))  # Loudest sample of the loop
    gain = 0.8 / peak  # Same peak level as bgm_hills
    mm.write_wav(str(loop), [v * gain for v in loop_l], [v * gain for v in loop_r])  # Save the finished loop
    path = AUDIO / "bgm_plains.ogg"  # The finished music file
    hn.ffmpeg("-i", loop, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "5", path)  # Save as OGG
    return path  # Give back the saved music file


# ---------- Ambience parts. Each part adds into a list of LOOP_SAMPLES samples, or SECONDS + 1 and then folds. ----------

def unit(data):  # Scales a sound so its RMS is one
    """Scales a sound so its RMS is one."""  # Describes the function
    rms = math.sqrt(sum(v * v for v in data) / len(data)) or 1.0  # Measures the present level
    return [v / rms for v in data]  # Gives back the scaled copy


def noise(seconds, seed, low, high):  # Makes band noise with an RMS of one
    """Makes band noise with an RMS of one."""  # Describes the function
    return unit(list(hn.soft_noise(seconds, seed, low, high)))  # Takes noise between the two cutoffs and sets its level


def fold(data):  # Folds the extra second onto the start so the loop joins
    """Folds the extra second onto the start so the loop joins."""  # Describes the function
    for i in range(FOLD_SAMPLES):  # Walks over the joined second
        weight = 0.5 - 0.5 * math.cos(math.pi * i / FOLD_SAMPLES)  # Moves smoothly from end to start
        data[i] = data[LOOP_SAMPLES + i] * (1 - weight) + data[i] * weight  # Carries the end through the boundary
    return data[:LOOP_SAMPLES]  # Keeps exactly one loop


def grass_wind(seed, loud):  # Makes one speaker of wind over grass and rice: a soft body and a rustle
    """Makes one speaker of wind over grass and rice: a soft body and a rustle."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = noise(SECONDS + 1, seed, 90, 700)  # Low, soft body of the wind
    rustle = noise(SECONDS + 1, seed + 1, 1100, 4200)  # Rustle of grass and rice stems
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        gust = 0.62 + 0.2 * math.sin(math.tau * 2 * t / SECONDS) + 0.1 * math.sin(math.tau * 5 * t / SECONDS + 1.1)  # Gusts that fit whole loop cycles
        out.append((body[i] * 0.5 * gust + rustle[i] * 0.3 * gust * gust) * loud)  # Lets gusts lift the rustle more
    return fold(out)  # Joins the loop


def crickets(channel, rng, count, loud):  # Adds crickets: short bursts of quick high chirps
    """Adds crickets: short bursts of quick high chirps."""  # Describes the function
    for _ in range(count):  # Each chirping cricket
        at = rng.uniform(0.2, SECONDS - 1.5)  # When the chirp starts, anywhere in the loop
        length = rng.uniform(0.6, 1.2)  # How long it chirps
        f = rng.uniform(4300.0, 4900.0)  # The cricket's pitch
        pulses = int(length * 25)  # Twenty-five tone pulses a second
        for p in range(pulses):  # Each pulse
            if (p // 4) % 2 == 1 and p % 4 == 3:  # Leave a short gap now and then, so the chirp breathes
                continue  # Skip this pulse
            start = round((at + p / 25.0) * RATE)  # Pulse start sample
            plen = int(0.012 * RATE)  # Each pulse lasts 12 ms
            for k in range(plen):  # Each sample of the pulse
                channel[(start + k) % LOOP_SAMPLES] += math.sin(math.tau * f * k / RATE) * math.sin(math.pi * k / plen) ** 2 * 0.05 * loud  # Soft tone pulse


def frog(channel, rng, at, loud):  # Adds one frog croak: a short run of pulsing low tones
    """Adds one frog croak: a short run of pulsing low tones."""  # Describes the function
    base = rng.uniform(170.0, 250.0)  # This frog's pitch
    pulse = rng.uniform(16.0, 22.0)  # Pulses per second in the croak
    length = int(rng.uniform(0.35, 0.55) * RATE)  # Croak length in samples
    start = round(at * RATE)  # Where the croak starts
    phase = 0.0  # Wave turn, added up so the falling pitch stays smooth
    for k in range(length):  # Each sample of the croak
        x = k / length  # 0 to 1 through the croak
        phase += math.tau * base * (1 - 0.12 * x) / RATE  # Pitch falls a little
        gate = (0.5 - 0.5 * math.cos(math.tau * pulse * k / RATE)) ** 2  # Pulses that open and close fast
        env = math.sin(math.pi * x) ** 0.6  # Swell and fade
        v = (math.sin(phase) + 0.4 * math.sin(2 * phase)) * gate * env  # Croak tone with a soft overtone
        channel[(start + k) % LOOP_SAMPLES] += v * loud * 0.07  # Add the croak


def water_bird(channel, rng, at, loud):  # Adds one far water-bird call: two or three low, falling honks
    """Adds one far water-bird call: two or three low, falling honks."""  # Describes the function
    notes = rng.randint(2, 3)  # How many honks in the call
    base = rng.uniform(760.0, 1100.0)  # This bird's pitch
    gap = rng.uniform(0.16, 0.22)  # Time between honks
    for n in range(notes):  # Each honk
        length = int(rng.uniform(0.14, 0.18) * RATE)  # Honk length in samples
        phase, start = 0.0, round((at + n * gap) * RATE)  # Wave turn and where this honk starts
        for k in range(length):  # Each sample of the honk
            x = k / length  # 0 to 1 through the honk
            phase += math.tau * base * (1 - 0.18 * x) / RATE  # Bends the honk down
            v = (math.sin(phase) + 0.35 * math.sin(2 * phase)) * math.sin(math.pi * x) ** 2  # Rounded honk with a soft overtone
            channel[(start + k) % LOOP_SAMPLES] += v * loud * 0.05  # Add the honk


def dry_wind(seed, loud):  # Makes one speaker of stronger, dry wind over the plain with a dusty hiss
    """Makes one speaker of stronger, dry wind over the plain with a dusty hiss."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = noise(SECONDS + 1, seed, 60, 800)  # Deep, strong body of the wind
    dust = noise(SECONDS + 1, seed + 1, 1800, 5000)  # Dry, dusty hiss over bare ground
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        gust = 0.8 + 0.3 * math.sin(math.tau * 2 * t / SECONDS) + 0.15 * math.sin(math.tau * 5 * t / SECONDS + 1.1)  # Bigger gusts that fit whole loop cycles
        out.append((body[i] * gust + dust[i] * 0.25 * gust ** 3) * loud)  # Gusts lift the dust more
    return fold(out)  # Joins the loop


def thunder(channel, seed, at, loud):  # Adds one far-off roll of thunder: low noise that swells and dies away
    """Adds one far-off roll of thunder: low noise that swells and dies away."""  # Describes the function
    rumble = unit(list(hn.soft_noise(4.0, seed, 25, 110)))  # Very low rumble, four seconds long, at RMS one
    s0 = round(at * RATE)  # Where the roll starts
    for k, v in enumerate(rumble):  # Each sample of the roll
        t = k / RATE  # Seconds into the roll
        env = min(1.0, t / 0.6) * math.exp(-t * 0.9) * (0.7 + 0.3 * math.sin(math.tau * t * 1.3))  # Swell, roll and fade
        channel[(s0 + k) % LOOP_SAMPLES] += v * env * 0.25 * loud  # Add the thunder, wrapping across the loop end


def grass_channels():  # Easy: soft wind over grass and rice, with crickets
    """Easy: soft wind over grass and rice, with crickets."""  # Describes the function
    channels = []  # Left and right
    for seed in (401, 419):  # One source per speaker, for width
        rng = random.Random(seed)  # Fixed seed for the crickets
        data = grass_wind(seed, 1.0)  # Soft wind over the grass
        crickets(data, rng, 7, 1.0)  # Seven chirping crickets in the loop
        channels.append(data)  # Keep this speaker
    return channels  # Give back both speakers


def wetland_channels():  # Normal: the grass wind, with frogs and far water-bird calls
    """Normal: the grass wind, with frogs and far water-bird calls."""  # Describes the function
    channels = []  # Left and right
    for seed in (441, 463):  # One source per speaker, for width
        rng = random.Random(seed)  # Fixed seed for the frogs and birds
        data = grass_wind(seed, 0.7)  # A softer wind than Easy, so the frogs can be heard
        for _ in range(8):  # Eight frog croaks in the loop
            frog(data, rng, rng.uniform(0.3, SECONDS - 1.0), rng.uniform(0.6, 1.0))  # One croak at a random time
        for _ in range(3):  # Three far water-bird calls in the loop
            water_bird(data, rng, rng.uniform(0.5, SECONDS - 1.0), rng.uniform(0.6, 1.0))  # One call at a random time
        channels.append(data)  # Keep this speaker
    return channels  # Give back both speakers


def dry_channels():  # Hard: a dry, stronger wind over the plain, with far thunder now and then
    """Hard: a dry, stronger wind over the plain, with far thunder now and then."""  # Describes the function
    channels = []  # Left and right
    for seed in (481, 503):  # One source per speaker, for width
        data = dry_wind(seed, 1.0)  # Dry, stronger wind
        thunder(data, 511, 6.0, 1.0)  # A far thunder roll, the same in both speakers
        thunder(data, 513, 17.5, 0.6)  # A softer second roll later in the loop
        channels.append(data)  # Keep this speaker
    return channels  # Give back both speakers


def make_loop(temp, name, channels, band, target):  # Filters, joins and encodes one seamless ambience loop
    """Filters, joins and encodes one seamless ambience loop."""  # Describes the function
    raw = temp / f"{name}-padded.wav"  # Receives the loop with a little wrap on both sides
    wet = temp / f"{name}-filtered.wav"  # Receives the trimmed, filtered loop
    pad = RATE // 2  # Lets the filters settle on real loop samples
    peaks = [max(map(abs, c)) or 1.0 for c in channels]  # The loudest sample of each speaker
    peaked = [[v / p for v in c] for c, p in zip(channels, peaks)]  # Scales each speaker to a peak of one, so 16-bit WAV does not clip
    mm.write_wav(str(raw), *(c[-pad:] + c + c[:pad] for c in peaked))  # Wraps half a second around each side
    hn.ffmpeg("-i", raw, "-af", f"{band},atrim=start_sample={pad}:end_sample={pad + LOOP_SAMPLES}", wet)  # Filters and trims after settling
    loops = mm.read_wav(str(wet))  # Reads the filtered loop back
    for channel in loops:  # Joins each speaker at the seam
        shift = (channel[-1] - channel[0]) / 2  # Shares any small edge step between the ends
        for i in range(RATE // 100):  # Blends the fix over ten milliseconds
            weight = 0.5 + 0.5 * math.cos(math.pi * i / (RATE // 100))  # Fades the fix to zero
            channel[i] += shift * weight  # Moves the start toward the end
            channel[-1 - i] -= shift * weight  # Moves the end toward the start
    gain = hn.level_gain(loops, target)  # Finds the gain for the target mean level
    mm.write_wav(str(wet), *([v * gain for v in c] for c in loops))  # Applies the gain
    path = AUDIO / f"{name}.ogg"  # Names the loop asset
    hn.ffmpeg("-i", wet, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "4", path)  # Saves as OGG with fixed settings
    return path  # Gives back the saved loop


def straw_bounce():  # Makes a short soft thump with a falling pitch and a light straw rustle
    """Makes a short soft thump with a falling pitch and a light straw rustle."""  # Describes the function
    out = []  # Holds the bounce
    for i, rustle in enumerate(hn.soft_noise(0.25, 1717, 1400, 5200)):  # Straw rustle band noise
        t = i / RATE  # Finds the age of the bounce
        bend = math.tau * (70 * t + 110 * (1 - math.exp(-t * 25)) / 25)  # Pitch starts higher and falls, like a spring giving way
        thump = math.sin(bend) * (1 - math.exp(-t * 400)) * math.exp(-t * 14)  # Soft, rounded "fwump" with a quick start
        straw = rustle * min(1.0, t / 0.005) * math.exp(-t * 18) * 0.5  # Light straw rustle that starts fast and fades
        out.append(thump * 0.9 + straw)  # Mixes the thump and the rustle
    return out  # Gives back the bounce


def preview(paths, temp):  # Shows the waveform and spectrum of every file on one review sheet
    """Shows the waveform and spectrum of every file on one review sheet."""  # Describes the function
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the preview folder if it is missing
    sheet = Image.new("RGB", (1092, 60 + 156 * len(paths)), (23, 28, 38))  # Sheet tall enough for every row
    draw = ImageDraw.Draw(sheet)  # Draws graph axes and labels
    ink, grid, text = (106, 211, 196), (52, 62, 76), (231, 237, 243)  # Keeps data and labels easy to see
    draw.text((20, 12), "Left channel waveform", fill=text)  # Names the left column
    draw.text((554, 12), "Average spectrum (dBFS, logarithmic Hz)", fill=text)  # Names the right column
    for row, path in enumerate(paths):  # Plots each sound in its own row
        decoded = temp / "decoded.wav"  # Holds the decoded asset rather than the source mix
        hn.ffmpeg("-i", path, "-c:a", "pcm_s16le", decoded)  # Reads back what Unity will hear
        left, right = mm.read_wav(str(decoded))  # Gets both decoded speakers
        seconds, mean = hn.measure(path)  # Gets the exact ffmpeg measurement
        print(f"{path.name}: {seconds:.3f} s, mean {mean:.1f} dB")  # Reports duration and loudness
        if path.suffix == ".ogg":  # Checks the playhead step across the loop seam
            for label, channel in (("left", left), ("right", right)):  # Checks the two speakers separately
                step = abs(channel[0] - channel[-1])  # Reads the single step across the join
                print(f"  {label} loop seam: {step:.6f}")  # Prints the join size for review
        top, mid = 41 + row * 156, 109 + row * 156  # Finds this panel's rows
        draw.text((20, top), f"{path.name}  |  {seconds:.3f} s  |  {mean:.1f} dB mean", fill=text)  # Names the clip and its result
        draw.line((20, mid, 519, mid), fill=grid)  # Marks the waveform center
        for x in range(500):  # Draws the tallest swing in this slice
            part = left[x * len(left) // 500:(x + 1) * len(left) // 500]  # Takes the samples under one pixel
            draw.line((20 + x, mid - int(max(part) * 48), 20 + x, mid - int(min(part) * 48)), fill=ink)  # Draws the min and max
        for db in (-20, -40, -60, -80, -100):  # Adds guide lines every twenty decibels
            y = top + 15 - db  # Places lower power toward the bottom
            draw.line((578, y, 1071, y), fill=grid)  # Marks the row
            draw.text((552, y - 5), str(db), fill=text)  # Shows the line value
        bands = hn.spectrum(left)  # Measures the power of the actual encoded audio
        points = []  # Holds the spectrum trace
        for x in range(494):  # Covers 50 to 12,000 Hz
            hz = 50 * (12000 / 50) ** (x / 493)  # Spreads the low end out
            index = round(hz / (RATE / 4096))  # Finds the nearest frequency bin
            points.append((578 + x, top + 15 - max(-100, bands[index])))  # Clips low power at the bottom edge
        draw.line(points, fill=ink)  # Draws the energy by pitch
        for hz in (50, 100, 500, 1000, 3000, 12000):  # Labels a few useful frequencies
            x = 578 + round(493 * math.log(hz / 50) / math.log(12000 / 50))  # Finds the tick on the same log scale
            draw.text((min(x, 1042), top + 121), str(hz), fill=text)  # Places the label under the trace
    sheet.save(PREVIEW)  # Writes the preview image


def main():  # Writes and measures all five Plains sounds
    """Writes and measures all five Plains sounds."""  # Describes the function
    with TemporaryDirectory(prefix="landgrand-plains-") as folder:  # Keeps the project clear of temporary WAV files
        temp = Path(folder)  # Names the temporary audio folder
        paths = [make_music(temp)]  # The Plains music loop
        paths.append(make_loop(temp, "amb_grass_easy", grass_channels(), "highpass=f=50,lowpass=f=6000", -20.8))  # Easy: same mean level as amb_stream_easy
        paths.append(make_loop(temp, "amb_wetland_normal", wetland_channels(), "highpass=f=50,lowpass=f=5500", -20.6))  # Normal: same mean level as amb_stream_normal
        paths.append(make_loop(temp, "amb_dry_hard", dry_channels(), "highpass=f=40,lowpass=f=7000", -18.8))  # Hard: same mean level as amb_rain_hard
        paths.append(hn.make_effect("sfx_straw_bounce.wav", straw_bounce(), -25.0))  # The soft straw bounce, a little above sfx_sand_step
        preview(paths, temp)  # Writes the graphs from the final assets
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
