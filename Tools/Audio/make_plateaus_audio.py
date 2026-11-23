"""Makes the Plateaus audio for LandGrand (D-072).  # Says what this script is for

Run from the Unity project folder:  python Tools/Audio/make_plateaus_audio.py   (needs ffmpeg and Pillow)  # How to run it
It writes five files to Assets/Audio/:  # What it makes
  bgm_plateaus.ogg        Plateaus music: an open, airy piece in A major, 6/8, 88 BPM (dotted quarter), 30 bars (about 40.9 s).  # Music
                          A flute lead over soft plucked strings and a light hand drum, with the voices of make_menu_music.py.  # Same family
  amb_highland_easy.ogg   Easy: dry wind on the high plateau, with a far hawk cry now and then, 24 s loop.  # Easy ambience
  amb_canyon_normal.ogg   Normal: hollow canyon wind, a hawk cry that echoes off the walls, and a far waterfall, 24 s loop.  # Normal ambience
  amb_highwind_hard.ogg   Hard: strong, howling high wind with blowing sand hiss, 24 s loop.  # Hard ambience
  sfx_updraft.wav         A soft whoosh that loops without a gap (1.5 s) while the explorer stands in a rising wind column.  # Effect
Every loop joins without a gap, and every sound uses a fixed random seed, so each run makes the same files.  # Repeatable
"""  # End of the description
import math  # Gives the sound envelopes and waves
import re  # Finds the measured peak in ffmpeg output
import subprocess  # Runs ffmpeg to measure the peak
from pathlib import Path  # Finds the asset and preview folders
from tempfile import TemporaryDirectory  # Holds temporary WAV files
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares ffmpeg, soft noise, levels, effect writer and measuring
import make_plains_audio as pa  # Shares the ambience loop maker, noise, fold and the review sheet

mm = ma.mm  # The menu music script: its notes, voices and WAV helpers
RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "plateaus-easy" / "audio-preview.png"  # Holds the review graph
SECONDS = pa.SECONDS  # The ambience loop length (24 s), the same as the Plains loops
LOOP_SAMPLES = pa.LOOP_SAMPLES  # The number of samples in an ambience loop

BPM = 88  # Dotted-quarter beats per minute: two beats to a 6/8 bar
BEAT = 60.0 / BPM  # Seconds per dotted-quarter beat
EIGHTH = BEAT / 3  # Seconds per eighth note: three eighths to a beat
BAR = 6 * EIGHTH  # Six eighths to a bar, so the music is in 6/8
BARS = 30  # Bars in the loop: 30 whole bars at 88 BPM come closest to 40 s
LENGTH = BARS * BAR  # Loop length in seconds (about 40.9)
TAIL = 4.0  # Extra seconds rendered after the loop for echoes and note endings
N = int(round((LENGTH + TAIL) * RATE))  # Total samples rendered
LOOP_N = int(round(LENGTH * RATE))  # Samples in the finished music loop

A, FSM, D, E, BM = (45, [57, 61, 64]), (42, [57, 61, 66]), (38, [57, 62, 66]), (40, [56, 59, 64]), (47, [59, 62, 66])  # A, F#m, D, E, Bm as (bass, pad notes)
CHORDS = [A, FSM, D, E] * 7 + [BM, E]  # Seven rounds of A F#m D E, then Bm E to lead back to the start

ARP_PATTERN = [0, 1, 2, 3, 2, 1]  # Which chord tone each eighth note plays: up and back, like fingers on soft strings

# Melody phrases as (MIDI note, start eighth, length in eighths), one list per bar over A, F#m, D, E.  # How the tune is written
P1 = [  # A calm first phrase in the middle of the flute
    [(76, 0, 3), (73, 3, 2), (76, 5, 1)],  # A: E, C#, E
    [(78, 0, 4), (76, 4, 2)],  # F#m: a long F#, then E
    [(74, 0, 2), (73, 2, 1), (71, 3, 3)],  # D: D, C#, B
    [(71, 0, 3), (68, 3, 3)],  # E: B, then G#, leading home
]  # End of phrase 1
P2 = [  # A phrase that climbs high, like looking out from the plateau
    [(81, 0, 3), (80, 3, 1), (78, 4, 2)],  # A: high A, G#, F#
    [(78, 0, 2), (76, 2, 1), (73, 3, 3)],  # F#m: F#, E, C#
    [(74, 0, 1), (76, 1, 2), (78, 3, 3)],  # D: D, E, F# rising
    [(76, 0, 6)],  # E: one long E, held open
]  # End of phrase 2
P3 = [  # The first phrase again with a new answer
    [(76, 0, 3), (73, 3, 2), (76, 5, 1)],  # A: E, C#, E
    [(78, 0, 2), (81, 2, 1), (78, 3, 3)],  # F#m: F#, high A, F#
    [(74, 0, 3), (78, 3, 2), (76, 5, 1)],  # D: D, F#, E
    [(71, 0, 3), (73, 3, 3)],  # E: B, then C#, leading home
]  # End of phrase 3
P4 = [  # The highest phrase, then a gentle way down
    [(85, 0, 3), (83, 3, 2), (81, 5, 1)],  # A: high C#, B, A
    [(78, 0, 4), (81, 4, 2)],  # F#m: a long F#, then A
    [(78, 0, 2), (76, 2, 1), (74, 3, 3)],  # D: F#, E, D
    [(73, 0, 2), (71, 2, 1), (68, 3, 3)],  # E: C#, B, G#
]  # End of phrase 4
MELODY = {}  # Bar number (from 0): melody notes in that bar
for first, phrase in ((4, P1), (8, P2), (12, P3), (16, P4), (20, P1), (24, P4)):  # Bars 1 to 4 and 29 to 30 have no melody
    for k, notes in enumerate(phrase):  # Each bar of the phrase
        MELODY[first + k] = notes  # Place it


def pan_gains(pan):  # Turns a pan place (-1 left to 1 right) into left and right loudness
    """Turns a pan place (-1 left to 1 right) into left and right loudness."""  # Describes the function
    return math.cos((pan + 1) * math.pi / 4), math.sin((pan + 1) * math.pi / 4)  # Same rule as the menu music notes


def drum(start, gain, pan):  # Adds one soft hand-drum thump: a short low tone that falls in pitch
    """Adds one soft hand-drum thump: a short low tone that falls in pitch."""  # Describes the function
    lg, rg = pan_gains(pan)  # Left and right loudness
    s0 = int(start * RATE)  # First sample of the thump
    phase = 0.0  # Wave turn, added up so the falling pitch stays smooth
    for k in range(int(0.35 * RATE)):  # Each sample of the thump
        i = s0 + k  # Place in the mix
        if i >= N:  # Stops at the end of the rendered mix
            break  # Nothing more to add
        t = k / RATE  # Seconds since the hit
        phase += math.tau * (85 + 75 * math.exp(-t * 28)) / RATE  # Pitch starts near 160 Hz and settles near 85 Hz
        v = math.sin(phase) * min(1.0, t / 0.002) * math.exp(-t * 14)  # Soft start and a quick fade
        mm.left[i] += v * gain * lg  # Add to the left speaker
        mm.right[i] += v * gain * rg  # Add to the right speaker


SHAKER = [pa.unit(list(hn.soft_noise(0.08, seed, 2500, 8000))) for seed in range(2101, 2107)]  # Six short grains of high noise for a light shaker


def shaker(start, gain, pan, grain):  # Adds one light shaker tick
    """Adds one light shaker tick."""  # Describes the function
    lg, rg = pan_gains(pan)  # Left and right loudness
    s0 = int(start * RATE)  # First sample of the tick
    for k, v in enumerate(SHAKER[grain]):  # Each sample of the grain
        i = s0 + k  # Place in the mix
        if i >= N:  # Stops at the end of the rendered mix
            break  # Nothing more to add
        t = k / RATE  # Seconds since the tick
        env = min(1.0, t / 0.004) * math.exp(-t * 60)  # Quick start and a very short fade
        mm.left[i] += v * env * gain * lg  # Add to the left speaker
        mm.right[i] += v * env * gain * rg  # Add to the right speaker


def compose():  # Places every note of the 30 bars, using the voices of the menu music script
    """Places every note of the 30 bars, using the voices of the menu music script."""  # Describes the function
    mm.N = N  # The shared note code writes into arrays of this length
    mm.left = [0.0] * N  # Left speaker samples
    mm.right = [0.0] * N  # Right speaker samples
    for bar, (bass, pad) in enumerate(CHORDS):  # Each bar and its chord
        t0 = bar * BAR  # Bar start time in seconds
        mm.add_note(t0, BAR, mm.freq(bass), mm.BASS, 0.26, 0.0)  # Soft bass note for the whole bar, in the middle
        for k, note in enumerate(pad):  # Each pad note
            pan = (-0.6, 0.0, 0.6)[k]  # Spread the three notes wide, for an open sound
            mm.add_note(t0, BAR, mm.freq(note) * 0.9985, mm.PAD, 0.045, pan)  # Pad note, a hair flat
            mm.add_note(t0, BAR, mm.freq(note) * 1.0015, mm.PAD, 0.045, -pan)  # Same note a hair sharp, other side, for width
        tones = sorted(pad) + [pad[0] + 12]  # Four chord tones for the plucked strings, low to high
        for step, index in enumerate(ARP_PATTERN):  # Six eighth notes per bar
            note = tones[index] + 12  # One octave above the pad
            gain = 0.12 if step % 3 == 0 else 0.08  # The two beats of the bar a little louder
            pan = (-0.35, 0.1, 0.35, 0.35, 0.1, -0.35)[step]  # Sweep right and back left across the bar
            mm.add_note(t0 + step * EIGHTH, EIGHTH, mm.freq(note), mm.PLUCK, gain, pan)  # One plucked note
        if bar >= 2:  # The drum joins after two bars, so each loop starts with a breath
            drum(t0, 0.20, 0.0)  # A soft thump on beat one
            drum(t0 + 3 * EIGHTH, 0.12, 0.0)  # A softer thump on beat two
            for step in range(6):  # A light shaker tick on every eighth
                shaker(t0 + step * EIGHTH, 0.030 if step % 3 == 0 else 0.018, 0.4, (bar + step) % 6)  # Placed a little right
        for note, eighth, eighths in MELODY.get(bar, []):  # Melody notes in this bar, if any
            mm.add_note(t0 + eighth * EIGHTH, eighths * EIGHTH * 0.92, mm.freq(note), mm.FLUTE, 0.20, -0.1)  # Slightly short, so the notes breathe


def make_music(temp):  # Composes the music, adds an echo, folds the tail into a seamless loop and saves bgm_plateaus.ogg
    """Composes the music, adds an echo, folds the tail into a seamless loop and saves bgm_plateaus.ogg."""  # Describes the function
    compose()  # Place every note
    left, right = mm.left, mm.right  # The mix
    peak = max(max(abs(v) for v in left), max(abs(v) for v in right))  # Loudest sample so far
    scale = 0.5 / peak  # Leave room for the echo
    dry, wet, loop = temp / "plateaus_dry.wav", temp / "plateaus_wet.wav", temp / "plateaus_loop.wav"  # Temporary files
    mm.write_wav(str(dry), [v * scale for v in left], [v * scale for v in right])  # Save the dry mix
    echo = "aecho=0.8:0.6:83|149|227:0.3|0.21|0.13,lowpass=f=8200,highpass=f=40"  # A wider, airy echo, like open high ground
    hn.ffmpeg("-i", dry, "-af", echo, wet)  # Add the echo
    l, r = mm.read_wav(str(wet))  # Read the echoed mix back
    loop_l, loop_r = l[:LOOP_N], r[:LOOP_N]  # The loop part
    for i in range(LOOP_N, min(len(l), N)):  # Each sample after the loop end
        loop_l[i - LOOP_N] += l[i]  # Fold the left tail onto the start, so the loop joins smoothly
        loop_r[i - LOOP_N] += r[i]  # Fold the right tail onto the start
    peak = max(max(abs(v) for v in loop_l), max(abs(v) for v in loop_r))  # Loudest sample of the loop
    gain = min(0.8 / peak, hn.level_gain([loop_l, loop_r], -15.0))  # No louder than bgm_plains: its peak level, or its mean level if that is lower
    mm.write_wav(str(loop), [v * gain for v in loop_l], [v * gain for v in loop_r])  # Save the finished loop
    path = AUDIO / "bgm_plateaus.ogg"  # The finished music file
    hn.ffmpeg("-i", loop, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "5", path)  # Save as OGG
    return path  # Give back the saved music file


# ---------- Ambience parts. Each wind renders SECONDS + 1 and folds; events add into the finished loop and wrap. ----------


def gust(t, base, a, b):  # Gives a slow gust shape that fits whole loop cycles
    """Gives a slow gust shape that fits whole loop cycles."""  # Describes the function
    return base + a * math.sin(math.tau * 2 * t / SECONDS) + b * math.sin(math.tau * 5 * t / SECONDS + 1.1)  # Two and five cycles in each loop


def plateau_wind(seed, loud):  # Makes one speaker of dry wind over the high plateau: a thin body and an airy hiss
    """Makes one speaker of dry wind over the high plateau: a thin body and an airy hiss."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = pa.noise(SECONDS + 1, seed, 110, 750)  # Thin body of the wind, with less low end than the Plains
    air = pa.noise(SECONDS + 1, seed + 1, 1500, 4800)  # Dry, airy hiss of thin high air
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        g = gust(i / RATE, 0.6, 0.22, 0.1)  # Gusts that fit whole loop cycles
        out.append((body[i] * 0.5 * g + air[i] * 0.22 * g * g) * loud)  # Lets gusts lift the hiss more
    return pa.fold(out)  # Joins the loop


def canyon_wind(seed, loud):  # Makes one speaker of hollow canyon wind: a body and a low moan between the walls
    """Makes one speaker of hollow canyon wind: a body and a low moan between the walls."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = pa.noise(SECONDS + 1, seed, 80, 650)  # Soft body of the wind
    moan = pa.noise(SECONDS + 1, seed + 1, 230, 320)  # Narrow band that sounds like wind moaning through the canyon
    air = pa.noise(SECONDS + 1, seed + 2, 1400, 4000)  # Light hiss over the rock
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        g = gust(i / RATE, 0.62, 0.2, 0.1)  # Gusts that fit whole loop cycles
        out.append((body[i] * 0.45 * g + moan[i] * 0.22 * g * g + air[i] * 0.12 * g) * loud)  # The moan rises with the gusts
    return pa.fold(out)  # Joins the loop


def waterfall(seed, loud):  # Makes one speaker of a far waterfall: a steady, soft rush
    """Makes one speaker of a far waterfall: a steady, soft rush."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    rush = pa.noise(SECONDS + 1, seed, 180, 2400)  # The falling water, softened by distance
    roar = pa.noise(SECONDS + 1, seed + 1, 45, 180)  # The low roar where the water lands
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        sway = 0.92 + 0.08 * math.sin(math.tau * 3 * i / RATE / SECONDS)  # A small, slow change that fits whole loop cycles
        out.append((rush[i] * 0.16 + roar[i] * 0.1) * sway * loud)  # Water never quite stops
    return pa.fold(out)  # Joins the loop


def high_wind(seed, loud):  # Makes one speaker of strong high wind: a deep body, a howl and blowing sand
    """Makes one speaker of strong high wind: a deep body, a howl and blowing sand."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = pa.noise(SECONDS + 1, seed, 60, 900)  # Deep, strong body of the wind
    howl = pa.noise(SECONDS + 1, seed + 1, 520, 760)  # Narrow band that howls around the cliff edges
    sand = pa.noise(SECONDS + 1, seed + 2, 2600, 7500)  # Fine sand hissing as it blows past
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        g = gust(t, 0.82, 0.3, 0.15)  # Bigger gusts that fit whole loop cycles
        flurry = 0.7 + 0.3 * math.sin(math.tau * 11 * t / SECONDS + 0.4) * math.sin(math.tau * 17 * t / SECONDS)  # Sand comes in flurries
        out.append((body[i] * g + howl[i] * 0.3 * g * g + sand[i] * 0.32 * g ** 3 * flurry) * loud)  # Gusts lift the howl and sand more
    return pa.fold(out)  # Joins the loop


def hawk(left, right, seed, at, loud, pan, bright=1.0):  # Adds one far hawk cry: a high scream that falls in pitch, with a little rasp
    """Adds one far hawk cry: a high scream that falls in pitch, with a little rasp."""  # Describes the function
    lg, rg = pan_gains(pan)  # Left and right loudness
    length = int(1.6 * RATE)  # The cry lasts 1.6 seconds
    rasp = pa.unit(list(hn.soft_noise(1.6, seed, 40, 300)))  # Slow, rough wobble for the rasp in the voice
    s0 = round(at * RATE)  # Where the cry starts
    phase = 0.0  # Wave turn, added up so the falling pitch stays smooth
    for k in range(length):  # Each sample of the cry
        x = k / length  # 0 to 1 through the cry
        f = 1650 + 1100 * min(1.0, x / 0.1) * (1 - x) ** 0.8  # Rises fast to about 2.7 kHz, then falls back down
        phase += math.tau * f * (1 + 0.015 * rasp[k]) / RATE  # A small rough pitch wobble makes the rasp
        env = min(1.0, k / RATE / 0.06) * (1 - x) ** 1.5 * (0.85 + 0.15 * max(-1.0, min(1.0, rasp[k])))  # Quick start, long fade, a little shake
        v = (math.sin(phase) + 0.3 * bright * math.sin(2 * phase) + 0.1 * bright * math.sin(3 * phase)) * env  # Scream tone; echoes are duller
        pos = (s0 + k) % LOOP_SAMPLES  # Wraps across the loop end
        left[pos] += v * loud * lg  # Add to the left speaker
        right[pos] += v * loud * rg  # Add to the right speaker


def highland_channels():  # Easy: dry plateau wind, with a far hawk cry now and then
    """Easy: dry plateau wind, with a far hawk cry now and then."""  # Describes the function
    left, right = plateau_wind(601, 1.0), plateau_wind(619, 1.0)  # One wind source per speaker, for width
    hawk(left, right, 631, 5.2, 0.24, -0.5)  # A far hawk cry on the left
    hawk(left, right, 641, 16.8, 0.18, 0.6)  # A softer cry later, on the right
    return [left, right]  # Give back both speakers


def canyon_channels():  # Normal: canyon wind, an echoing hawk cry and a far waterfall
    """Normal: canyon wind, an echoing hawk cry and a far waterfall."""  # Describes the function
    left, right = canyon_wind(651, 0.8), canyon_wind(659, 0.8)  # A softer wind than Easy, so the waterfall can be heard
    fall_l, fall_r = waterfall(667, 1.0), waterfall(673, 0.55)  # The waterfall sits more on the left
    left = [a + b for a, b in zip(left, fall_l)]  # Mix the waterfall into the left speaker
    right = [a + b for a, b in zip(right, fall_r)]  # Mix the waterfall into the right speaker
    for seed, at, loud in ((681, 3.5, 0.26), (691, 14.0, 0.2)):  # Two hawk cries in the loop
        for delay, gain, pan, bright in ((0.0, 1.0, 0.4, 1.0), (0.42, 0.45, -0.6, 0.6), (0.95, 0.24, 0.6, 0.4), (1.6, 0.11, -0.4, 0.2)):  # The cry, then softer, duller echoes from each canyon wall
            hawk(left, right, seed, at + delay, loud * gain, pan, bright)  # One copy of the cry
    return [left, right]  # Give back both speakers


def highwind_channels():  # Hard: strong high wind with blowing sand
    """Hard: strong high wind with blowing sand."""  # Describes the function
    return [high_wind(701, 1.0), high_wind(719, 1.0)]  # One source per speaker, for width


def updraft():  # Makes a soft whoosh that loops without a gap for the rising wind column
    """Makes a soft whoosh that loops without a gap for the rising wind column."""  # Describes the function
    n, fold_n = int(1.5 * RATE), int(0.5 * RATE)  # One loop of 1.5 s, plus half a second to blend into the start
    body = pa.noise(2.0, 1901, 140, 1100)  # Soft, rounded body of rising air
    air = pa.noise(2.0, 1903, 900, 3200)  # A light airy top, kept low so it is not harsh
    out = []  # Holds the whoosh
    for i in range(n + fold_n):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        swell = 0.85 + 0.15 * math.sin(math.tau * t / 1.5)  # One gentle swell in each loop
        flutter = 0.94 + 0.06 * math.sin(math.tau * 4 * t / 1.5 + 0.7)  # A light flutter, four times in each loop
        out.append((body[i] * 0.6 + air[i] * 0.16 * swell) * swell * flutter)  # Mixes the whoosh
    for i in range(fold_n):  # Walks over the blended half second
        weight = 0.5 - 0.5 * math.cos(math.pi * i / fold_n)  # Moves smoothly from end to start
        out[i] = out[n + i] * math.sqrt(1 - weight) + out[i] * math.sqrt(weight)  # Equal-power blend, so the noise does not dip at the join
    return out[:n]  # Keeps exactly one loop


def peak_db(path):  # Measures the loudest sample of a file in dB
    """Measures the loudest sample of a file in dB."""  # Describes the function
    run = subprocess.run(["ffmpeg", "-hide_banner", "-i", str(path), "-af", "volumedetect", "-f", "null", "-"], capture_output=True, text=True, check=True)  # Runs the loudness measurement
    return float(re.search(r"max_volume:\s+(-?\d+(?:\.\d+)?) dB", run.stderr)[1])  # Reads the peak dB


def main():  # Writes and measures all five Plateaus sounds
    """Writes and measures all five Plateaus sounds."""  # Describes the function
    with TemporaryDirectory(prefix="landgrand-plateaus-") as folder:  # Keeps the project clear of temporary WAV files
        temp = Path(folder)  # Names the temporary audio folder
        paths = [make_music(temp)]  # The Plateaus music loop
        paths.append(pa.make_loop(temp, "amb_highland_easy", highland_channels(), "highpass=f=50,lowpass=f=6500", -20.8))  # Easy: same mean level as amb_grass_easy
        paths.append(pa.make_loop(temp, "amb_canyon_normal", canyon_channels(), "highpass=f=45,lowpass=f=6500", -20.6))  # Normal: same mean level as amb_wetland_normal
        paths.append(pa.make_loop(temp, "amb_highwind_hard", highwind_channels(), "highpass=f=40,lowpass=f=8000", -18.8))  # Hard: same mean level as amb_dry_hard
        loop = updraft()  # The rising wind whoosh
        paths.append(hn.make_effect("sfx_updraft.wav", loop, -27.0))  # Quieter than sfx_straw_bounce, because it plays on and on
        print(f"sfx_updraft.wav loop seam: {abs(loop[0] - loop[-1]):.6f} (before the level is set)")  # Prints the join size for review
        pa.PREVIEW = PREVIEW  # Points the shared review sheet at the Plateaus folder
        pa.preview(paths, temp)  # Writes the graphs from the final assets
        for path in paths:  # Reports the peak of every file
            print(f"{path.name}: peak {peak_db(path):.1f} dB")  # Prints the loudest sample
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
