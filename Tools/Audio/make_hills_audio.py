"""Makes the Hills audio for LandGrand (D-042, D-066).

Run from the Unity project folder:  python Tools/Audio/make_hills_audio.py   (needs ffmpeg for the OGG files)
It writes five files to Assets/Audio/:
  bgm_hills.ogg          Hills music: a light, swaying piece in G major, 3 beats to a bar, 108 BPM, 24 bars (40 s), no drums.
                         It reuses the voices of make_menu_music.py, like bgm_mountains, so it sounds like the same game.
  amb_stream_easy.ogg    Easy: a gentle stream, songbirds and a far cow bell, 24 s loop.
  amb_stream_normal.ogg  Normal: the stream and birds, plus cicadas and wind in the grass, 24 s loop.
  amb_rain_hard.ogg      Hard: rain on a louder stream with far-off thunder, 24 s loop.
  sfx_sand_step.wav      A soft sand crunch for each step on soft sand (Hills Easy 3).
Every loop joins without a gap, and every sound uses a fixed random seed, so each run makes the same files.
"""
import math  # math gives sin, exp and pi
import os  # os joins paths and removes the temporary files
import random  # random makes the noise and places the birds (with fixed seeds)
import subprocess  # subprocess runs ffmpeg
import make_mountains_audio as ma  # Rate, folders and the WAV writer from the Mountains audio script
import make_normal_audio as na  # save_wav from the Normal audio script

mm = ma.mm  # The menu music script: its notes, voices and WAV helpers
RATE = ma.RATE  # Samples per second
BPM = 108  # Beats per minute: lighter and quicker than the Mountains music
BEAT = 60.0 / BPM  # Seconds per beat
BAR = 3 * BEAT  # Three beats to a bar gives the rolling, swaying feel of the hills
BARS = 24  # Bars in the loop
LENGTH = BARS * BAR  # Loop length in seconds (40)
TAIL = 4.0  # Extra seconds rendered after the loop for echoes and note endings
N = int(round((LENGTH + TAIL) * RATE))  # Total samples rendered
LOOP_N = int(round(LENGTH * RATE))  # Samples in the finished loop

# Chords for each bar as (bass note, pad notes), MIDI numbers: G, Em, C, D, six times.
CHORDS = [
    (43, [55, 59, 62]),  # G: bass G2; pad G3 B3 D4
    (40, [55, 59, 64]),  # Em: bass E2; pad G3 B3 E4
    (36, [55, 60, 64]),  # C: bass C2; pad G3 C4 E4
    (38, [54, 57, 62]),  # D: bass D2; pad F#3 A3 D4
] * 6  # The four chords repeat six times

# Melody as (MIDI note, start beat, length in beats). Bars 1 to 4 and 21 to 24 are only pad, harp and bass.
PHRASE_A = [  # Four bars that rise and fall like rolling hills
    [(74, 0, 2), (71, 2, 1)],  # G: D, B
    [(72, 0, 1), (71, 1, 1), (67, 2, 1)],  # Em: C B G
    [(69, 0, 2), (72, 2, 1)],  # C: A, C
    [(71, 0, 3)],  # D: a long B
]
PHRASE_B = [  # Four bars that climb a little higher, then come home
    [(74, 0, 1), (76, 1, 1), (79, 2, 1)],  # G: D E G
    [(78, 0, 2), (76, 2, 1)],  # Em: F#, E
    [(76, 0, 1), (74, 1, 1), (72, 2, 1)],  # C: E D C
    [(69, 0, 2), (66, 2, 1)],  # D: A, F# leading back to G
]
MELODY = {}  # Bar number: notes in that bar
for start, phrase in ((4, PHRASE_A), (8, PHRASE_B), (12, PHRASE_A), (16, PHRASE_B)):  # Where each phrase goes
    for k, notes in enumerate(phrase):  # Each bar of the phrase
        MELODY[start + k] = notes  # Place it


def compose():
    """Places every note of the 24 bars, using the voices of the menu music script."""
    mm.N = N  # The shared note code writes into arrays of this length
    mm.left = [0.0] * N  # Left speaker samples
    mm.right = [0.0] * N  # Right speaker samples
    for bar, (bass, pad) in enumerate(CHORDS):  # Each bar and its chord
        t0 = bar * BAR  # Bar start time in seconds
        mm.add_note(t0, BAR, mm.freq(bass), mm.BASS, 0.28, 0.0)  # Bass note on beat one, held for the bar
        for k, note in enumerate(pad):  # Each pad note
            pan = (-0.5, 0.0, 0.5)[k]  # Spread the three notes
            mm.add_note(t0, BAR, mm.freq(note) * 0.9985, mm.PAD, 0.055, pan)  # Pad note, a hair flat
            mm.add_note(t0, BAR, mm.freq(note) * 1.0015, mm.PAD, 0.055, -pan)  # A hair sharp on the other side, for width
        tones = sorted(pad)  # Chord tones for the harp, low to high
        for step in range(3):  # One harp note per beat, rising: the sway
            note = tones[step] + 12  # One octave above the pad
            pan = (-0.35, 0.0, 0.35)[step]  # Left, middle, right
            mm.add_note(t0 + step * BEAT, BEAT, mm.freq(note), mm.PLUCK, 0.14, pan)  # One harp note
        for note, beat, beats in MELODY.get(bar, []):  # Melody notes in this bar, if any
            mm.add_note(t0 + beat * BEAT, beats * BEAT * 0.9, mm.freq(note), mm.FLUTE, 0.19, 0.1)  # Slightly short, so the notes breathe


def make_music():
    """Composes the music, adds a short echo, folds the tail into a seamless loop and saves bgm_hills.ogg."""
    compose()  # Place every note
    left, right = mm.left, mm.right  # The mix
    peak = max(max(abs(v) for v in left), max(abs(v) for v in right))  # Loudest sample so far
    scale = 0.5 / peak  # Leave room for the echo
    dry = os.path.join(ma.TEMP, "hills_dry.wav")  # Temporary file before the echo
    wet = os.path.join(ma.TEMP, "hills_wet.wav")  # Temporary file after the echo
    loop = os.path.join(ma.TEMP, "hills_loop.wav")  # Temporary file of the finished loop
    mm.write_wav(dry, [v * scale for v in left], [v * scale for v in right])  # Save the dry mix
    echo = "aecho=0.8:0.5:47|89|131:0.28|0.18|0.1,lowpass=f=8000,highpass=f=40"  # A shorter, closer echo than the Mountains: low green hills
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", dry, "-af", echo, wet], check=True)  # Add the echo
    l, r = mm.read_wav(wet)  # Read the echoed mix back
    loop_l, loop_r = l[:LOOP_N], r[:LOOP_N]  # The loop part
    for i in range(LOOP_N, min(len(l), N)):  # Each sample after the loop end
        loop_l[i - LOOP_N] += l[i]  # Fold the left tail onto the start, so the loop joins smoothly
        loop_r[i - LOOP_N] += r[i]  # Fold the right tail onto the start
    peak = max(max(abs(v) for v in loop_l), max(abs(v) for v in loop_r))  # Loudest sample of the loop
    gain = 0.8 / peak  # Same level as the Mountains music
    mm.write_wav(loop, [v * gain for v in loop_l], [v * gain for v in loop_r])  # Save the finished loop
    out = os.path.join(ma.AUDIO, "bgm_hills.ogg")  # The finished music file
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", loop, "-c:a", "libvorbis", "-q:a", "5", out], check=True)  # Save as OGG
    for path in (dry, wet, loop):  # Remove the temporary files
        os.remove(path)  # Delete one temporary file
    print("Music written to", out)  # Tell the person where the file went


# ---------- Ambience parts. Each part adds into a list of samples that is SECONDS + FADE long. ----------

SECONDS, FADE = 24.0, 3.0  # Loop length and the part folded from the end onto the start
TOTAL = int((SECONDS + FADE) * RATE)  # Samples rendered for each ambience


def stream(data, rng, loud, rush):
    """A brook: soft rushing water plus many tiny bubble chirps. rush makes the water deeper and louder."""
    low = mid = 0.0  # Filter memory: the deep rush and the splashy middle
    for i in range(TOTAL):  # Each sample
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        low += (0.03 + 0.02 * rush) * (white - low)  # Deep rush of water
        mid += 0.12 * (white - mid)  # Splashy middle part, kept soft so the brook does not hiss
        t = i / RATE  # Seconds
        wobble = 0.75 + 0.25 * math.sin(2 * math.pi * t / 2.7) * math.sin(2 * math.pi * t / 1.1)  # Water never flows evenly
        data[i] += (low * (3.0 + 4.0 * rush) + (mid - low) * 0.35) * wobble * loud  # Mix
    for _ in range(int(SECONDS * 22)):  # About 22 bubbles a second
        start = int(rng.uniform(0, SECONDS + FADE - 0.05) * RATE)  # When the bubble pops
        f0 = rng.uniform(500.0, 1400.0)  # Its starting pitch
        length = int(rng.uniform(0.01, 0.03) * RATE)  # Very short
        phase = 0.0  # Wave turn, added up so the rising pitch stays smooth
        for k in range(length):  # Each sample of the bubble
            if start + k >= TOTAL:  # Past the end
                break  # Stop this bubble
            phase += 2 * math.pi * f0 * (1 + 1.5 * k / length) / RATE  # Pitch rises, like a real bubble
            data[start + k] += math.sin(phase) * math.exp(-6.0 * k / length) * 0.05 * loud  # Soft, quick fade


def birds(data, rng, count, loud):
    """Songbirds: short calls of two to five bending whistles, placed at fixed random times."""
    for _ in range(count):  # Each bird call
        at = rng.uniform(0.5, SECONDS - 1.0)  # When the call starts, never inside the fold, so the loop stays clean
        notes = rng.randint(2, 5)  # How many chirps in the call
        base = rng.uniform(2600.0, 4200.0)  # This bird's pitch
        gap = rng.uniform(0.09, 0.15)  # Time between its chirps
        for n in range(notes):  # Each chirp
            start = int((at + n * gap) * RATE)  # Chirp start
            length = int(rng.uniform(0.05, 0.09) * RATE)  # Chirp length
            sweep = rng.choice((1, -1)) * rng.uniform(0.15, 0.35)  # Up or down whistle
            phase = 0.0  # Wave turn
            for k in range(length):  # Each sample
                x = k / length  # 0 to 1 through the chirp
                phase += 2 * math.pi * base * (1 + sweep * math.sin(math.pi * x)) / RATE  # Whistle that bends
                env = math.sin(math.pi * x) ** 2  # Smooth swell and fade
                data[start + k] += math.sin(phase) * env * 0.08 * loud  # Add the chirp


def cow_bell(data, at, loud):
    """One soft, far-away cow bell: two metal tones that ring for a second."""
    s0 = int(at * RATE)  # First sample
    for k in range(int(1.2 * RATE)):  # Ring for 1.2 s
        t = k / RATE  # Seconds
        env = min(1.0, t / 0.003) * math.exp(-t * 4.0)  # Quick hit, soft ring
        data[s0 + k] += (math.sin(2 * math.pi * 880 * t) + 0.6 * math.sin(2 * math.pi * 1420 * t)) * env * 0.035 * loud  # Two bell tones


def cicadas(data, rng, loud):
    """Cicadas: a high buzzing that swells in and out twice per loop."""
    hi = 0.0  # Filter memory
    for i in range(TOTAL):  # Each sample
        t = i / RATE  # Seconds
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        hi += 0.6 * (white - hi)  # Bright noise
        buzz = 0.5 + 0.5 * math.sin(2 * math.pi * 55.0 * t)  # Fast pulsing, like wings
        swell = max(0.0, math.sin(2 * math.pi * t / 12.0)) ** 2  # Comes and goes, twice per loop
        data[i] += hi * buzz * swell * 0.06 * loud  # Add the buzz


def grass_wind(data, rng, loud):
    """Light wind in tall grass: a soft airy hiss with slow gusts."""
    low = hiss = 0.0  # Filter memory
    for i in range(TOTAL):  # Each sample
        t = i / RATE  # Seconds
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        low += 0.04 * (white - low)  # Soft part
        hiss += 0.3 * (white - hiss)  # Rustling part
        gust = 0.5 + 0.35 * math.sin(2 * math.pi * t / 6.0) + 0.15 * math.sin(2 * math.pi * t / 2.4)  # Slow gusts
        data[i] += (low * 1.5 + (hiss - low) * 0.4) * max(0.15, gust) * loud  # Add the wind


def rain(data, rng, loud):
    """Steady rain: dense bright hiss with drips on leaves."""
    hiss = 0.0  # Filter memory
    for i in range(TOTAL):  # Each sample
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        hiss += 0.5 * (white - hiss)  # Bright rain noise
        data[i] += hiss * 0.22 * loud  # Add the rain
    for _ in range(int(SECONDS * 30)):  # About 30 drips a second
        start = int(rng.uniform(0, SECONDS + FADE - 0.02) * RATE)  # When the drip lands
        f = rng.uniform(1800.0, 3500.0)  # Its pitch
        for k in range(int(0.012 * RATE)):  # A very short tick
            if start + k >= TOTAL:  # Past the end
                break  # Stop this drip
            data[start + k] += math.sin(2 * math.pi * f * k / RATE) * math.exp(-k / (0.003 * RATE)) * 0.06 * loud  # Tick


def thunder(data, rng, at, loud):
    """One far-off roll of thunder: very low noise that swells and dies away over four seconds."""
    low = 0.0  # Filter memory
    s0 = int(at * RATE)  # First sample
    for k in range(int(4.0 * RATE)):  # Four seconds
        t = k / RATE  # Seconds
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        low += 0.004 * (white - low)  # Very low rumble
        env = min(1.0, t / 0.6) * math.exp(-t * 0.9) * (0.7 + 0.3 * math.sin(2 * math.pi * t * 1.3))  # Swell, roll and fade
        data[s0 + k] += low * 40.0 * env * loud  # Add the thunder


def save_loop(name, channels, gain_peak, top):
    """Folds each channel's end onto its start, sets the loudness and saves the loop as OGG. top is the highest pitch kept, in Hz."""
    loop_n = int(SECONDS * RATE)  # Samples in the loop
    out = []  # Finished channels
    for data in channels:  # Left and right
        looped = data[:loop_n]  # The loop part
        for i in range(int(FADE * RATE)):  # Fold the end onto the start with constant-power weights
            w = i / (FADE * RATE)  # 0 to 1 across the fold
            looped[i] = data[i] * math.sin(w * math.pi / 2) + data[loop_n + i] * math.cos(w * math.pi / 2)  # Smooth join
        out.append(looped)  # Keep it
    peak = max(max(abs(v) for v in c) for c in out)  # Loudest sample
    gain = gain_peak / peak  # Target loudness
    os.makedirs(ma.TEMP, exist_ok=True)  # The temporary folder
    temp = os.path.join(ma.TEMP, name + ".wav")  # Temporary WAV
    mm.write_wav(temp, [v * gain for v in out[0]], [v * gain for v in out[1]])  # Save the loop
    path = os.path.join(ma.AUDIO, name + ".ogg")  # The finished file
    soft = "lowpass=f=%d,highpass=f=50" % top  # Cut the harsh top and the rumble nobody can hear on a phone
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", temp, "-af", soft, "-c:a", "libvorbis", "-q:a", "4", path], check=True)  # Soften and save as OGG
    os.remove(temp)  # Remove the temporary file
    print("Ambience written to", path)  # Tell the person where the file went


def make_easy():
    """Easy: a gentle stream, songbirds and one far cow bell."""
    channels = []  # Left and right
    for seed in (201, 223):  # One source per speaker, for width
        rng = random.Random(seed)  # Fixed seed
        data = [0.0] * TOTAL  # This channel
        stream(data, rng, 1.0, 0.0)  # Gentle brook
        birds(data, rng, 9, 1.0)  # Nine bird calls in the loop
        cow_bell(data, 9.0 if seed == 201 else 9.02, 1.0)  # The far bell, almost the same in both speakers
        channels.append(data)  # Keep it
    save_loop("amb_stream_easy", channels, 0.55, 6000)  # A soft ambience; the birds sit below 6 kHz


def make_normal():
    """Normal: the stream and birds, plus cicadas and wind in the grass."""
    channels = []  # Left and right
    for seed in (241, 263):  # One source per speaker
        rng = random.Random(seed)  # Fixed seed
        data = [0.0] * TOTAL  # This channel
        stream(data, rng, 0.9, 0.3)  # A slightly fuller brook
        birds(data, rng, 6, 0.8)  # Fewer birds
        cicadas(data, rng, 1.0)  # Warm-day cicadas
        grass_wind(data, rng, 0.6)  # Wind in the grass
        channels.append(data)  # Keep it
    save_loop("amb_stream_normal", channels, 0.6, 7000)  # A little louder than Easy; room for the cicadas


def make_hard():
    """Hard: rain on a louder stream with far-off thunder."""
    channels = []  # Left and right
    for seed in (281, 307):  # One source per speaker
        rng = random.Random(seed)  # Fixed seed
        data = [0.0] * TOTAL  # This channel
        stream(data, rng, 1.2, 1.0)  # A rushing, rain-fed stream
        rain(data, rng, 1.0)  # Steady rain
        thunder(data, random.Random(311), 5.0, 1.0)  # A far thunder roll, the same in both speakers
        thunder(data, random.Random(313), 16.0, 0.7)  # A softer second roll
        channels.append(data)  # Keep it
    save_loop("amb_rain_hard", channels, 0.7, 8000)  # Same level as the Mountains Hard wind; rain needs a brighter top


def make_sand_step():
    """sfx_sand_step.wav: a 0.14 s soft crunch of sand under a boot."""
    rng = random.Random(331)  # Fixed seed
    n = int(0.14 * RATE)  # Samples
    low = 0.0  # Filter memory
    data = []  # The sound
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        low += 0.25 * (white - low)  # Soft, muffled grains
        grains = 1.0 if rng.random() < 0.3 else 0.4  # Uneven crunch
        env = min(1.0, t / 0.008) * math.exp(-t * 28.0)  # Quick crunch that fades
        data.append(low * grains * env)  # One sample
    na.save_wav("sfx_sand_step.wav", data, 0.45)  # Quiet: it plays on every step


def main():
    """Makes all five files."""
    os.makedirs(ma.TEMP, exist_ok=True)  # Make the temporary folder if it is missing
    make_music()  # The Hills music
    make_easy()  # The Easy stream and birds
    make_normal()  # The Normal stream, birds, cicadas and grass wind
    make_hard()  # The Hard rain, stream and thunder
    make_sand_step()  # The soft sand step


if __name__ == "__main__":
    main()  # Run when the file is started directly
