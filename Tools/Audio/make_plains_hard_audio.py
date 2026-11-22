"""Makes the Plains Hard sounds for LandGrand: a hay bale roll, a gentle hoof nudge and a cold plain ambience.

Run from the Unity project folder:  python Tools/Audio/make_plains_hard_audio.py   (needs ffmpeg and Pillow)
It writes three files to Assets/Audio/:
  sfx_hay_roll.wav      A soft straw crunch with low, muffled thumps, when a round hay bale rolls. Softer and brighter than sfx_log_roll.wav.
  sfx_herd_hooves.wav   Three or four soft hoof thuds on grass with a tiny snort, when a grazing animal gently nudges the explorer.
  amb_cold_plain.ogg    A cold, open winter plain: steady soft wind, a faint snow hiss and far crow calls, 24 s seamless loop.
Every loop joins without a gap, and every sound uses a fixed random seed, so each run makes the same files.
"""  # End of the description
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
import random  # Gives repeatable noise, grains and crow timings with fixed seeds
from tempfile import TemporaryDirectory  # Holds temporary WAV files
from PIL import Image, ImageDraw  # Draws the review sheet
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares ffmpeg, soft noise, level finding, effect writer and measuring

RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "plains-hard" / "audio-preview.png"  # Holds the review graph
HAY_SECONDS = 0.8  # Same length as sfx_log_roll.wav
HAY_TARGET_DB = -24.0  # Softer than sfx_log_roll.wav (-22 dB)
HOOF_SECONDS = 0.35  # A short gentle burst, inside the 0.3 to 0.4 s brief
HOOF_TARGET_DB = -26.0  # Same mean level as sfx_bat_flap.wav (-26 dB)
SECONDS = 24.0  # The ambience loop length, same as amb_dry_hard.ogg
LOOP_SAMPLES = int(SECONDS * RATE)  # The number of samples in an ambience loop
FOLD_SAMPLES = RATE  # Blends one extra second into the start
LOOP_TARGET_DB = -18.8  # Same mean target as amb_dry_hard.ogg (measures -19.0 dB after OGG encoding)


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


def hay_roll():  # Makes a soft straw crunch with three low, muffled thumps
    """Makes a soft straw crunch with three low, muffled thumps."""  # Describes the function
    size = int(HAY_SECONDS * RATE)  # Number of samples in the roll
    crunch = noise(HAY_SECONDS, 1301, 1200, 6500)  # Bright, dry straw noise, at RMS one
    rng = random.Random(1303)  # Repeats the same straw grains
    block = RATE // 200  # One grain every 5 ms
    grains = [rng.uniform(0.25, 1.0) for _ in range(size // block + 2)]  # Random loudness for each grain
    thumps = ((0.03, 0.9), (0.29, 0.75), (0.55, 0.6))  # Three low bale thumps: start time and loudness
    out = []  # Holds the roll
    for i in range(size):  # Each sample of the roll
        t = i / RATE  # Age of the roll in seconds
        pos = i / block  # Position in grain units
        k = int(pos)  # Index of this grain
        frac = pos - k  # How far between two grains
        grain = grains[k] * (1 - frac) + grains[k + 1] * frac  # Smooth grain loudness, so there are no clicks
        edge = min(1.0, t / 0.01) * min(1.0, (HAY_SECONDS - t) / 0.12)  # Quick start, soft end
        straw = crunch[i] * grain * edge * 0.55  # The straw crunch, crackling grain by grain
        thud = 0.0  # Holds the low thumps at this sample
        for start, loud in thumps:  # Each bale thump
            age = t - start  # Age of this thump
            if age >= 0:  # Only after the thump starts
                bend = math.tau * (85 * age + 18 * (1 - math.exp(-age * 35)) / 35)  # Starts near 103 Hz and falls to 85 Hz, a soft low body
                thud += math.sin(bend) * (1 - math.exp(-age * 250)) * math.exp(-age * 22) * loud  # Soft, rounded thump that fades
        out.append(straw + thud * 0.6)  # Mixes the crunch and the thumps
    return out  # Gives back the roll


def herd_hooves():  # Makes three or four soft hoof thuds on grass with a tiny snort
    """Makes three or four soft hoof thuds on grass with a tiny snort."""  # Describes the function
    size = int(HOOF_SECONDS * RATE)  # Number of samples in the burst
    rng = random.Random(1401)  # Repeats the same hoof timing
    grass = noise(HOOF_SECONDS, 1403, 180, 900)  # Soft grassy noise under each hoof
    breath = noise(HOOF_SECONDS, 1409, 250, 1100)  # Breathy noise for the snort
    out = [0.0] * size  # Holds the burst
    for base in (0.02, 0.105, 0.19, 0.265):  # Four hoof steps, a little uneven
        start = round((base + rng.uniform(-0.006, 0.006)) * RATE)  # Where this hoof lands
        loud = rng.uniform(0.8, 1.0)  # Each hoof is a little different
        for k in range(int(0.12 * RATE)):  # Each sample of the thud
            i = start + k  # Position in the burst
            if 0 <= i < size:  # Keeps the thud inside the burst
                t = k / RATE  # Age of this thud
                env = (1 - math.exp(-t * 900)) * math.exp(-t * 45)  # Soft strike that fades quickly
                bend = math.tau * (95 * t + 12 * (1 - math.exp(-t * 60)) / 60)  # Low thud falling slightly
                out[i] += (math.sin(bend) * 0.8 + grass[i] * 0.25 * math.exp(-t * 70)) * env * loud  # Low body plus grassy tap
    snort_start, snort_len = round(0.13 * RATE), round(0.09 * RATE)  # The tiny snort between the second and third hoof
    for k in range(snort_len):  # Each sample of the snort
        i = snort_start + k  # Position in the burst
        x = k / snort_len  # 0 to 1 through the snort
        out[i] += breath[i] * math.sin(math.pi * x) ** 2 * 0.3  # Soft breath, rounded at both ends
    fade = int(0.03 * RATE)  # Last 30 ms fade, so the burst ends without a click
    for k in range(fade):  # Each sample of the fade
        out[size - 1 - k] *= k / fade  # Fades the end to zero
    return out  # Gives back the burst


def cold_wind(seed, loud):  # Makes one speaker of steady, cold wind with a faint snow hiss
    """Makes one speaker of steady, cold wind with a faint snow hiss."""  # Describes the function
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = noise(SECONDS + 1, seed, 50, 600)  # Steady, low body of the cold wind
    hiss = noise(SECONDS + 1, seed + 1, 2800, 8000)  # Faint snow hiss, high and thin
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        gust = 0.72 + 0.18 * math.sin(math.tau * 2 * t / SECONDS) + 0.08 * math.sin(math.tau * 5 * t / SECONDS + 1.7)  # Gusts that fit whole loop cycles
        snow = 0.12 * gust ** 3  # The snow hiss only lifts a little in the bigger gusts
        out.append((body[i] * 0.5 * gust + hiss[i] * snow) * loud)  # Mixes the wind and the hiss
    return fold(out)  # Joins the loop


def crow(channel, rng, at, loud):  # Adds one far crow call: two or three rough, falling caws
    """Adds one far crow call: two or three rough, falling caws."""  # Describes the function
    caws = rng.randint(2, 3)  # How many caws in the call
    base = rng.uniform(340.0, 430.0)  # This crow's pitch
    gap = rng.uniform(0.26, 0.33)  # Time between caws
    for n in range(caws):  # Each caw
        length = int(rng.uniform(0.22, 0.30) * RATE)  # Caw length in samples
        phase, start = 0.0, round((at + n * gap) * RATE)  # Wave turn and where this caw starts
        for k in range(length):  # Each sample of the caw
            x = k / length  # 0 to 1 through the caw
            phase += math.tau * base * (1 - 0.22 * x) / RATE  # Bends the caw down
            rough = math.sin(phase) + 0.5 * math.sin(2 * phase) + 0.3 * math.sin(3 * phase)  # Rough, nasal tone with odd harmonics
            v = rough * math.sin(math.pi * x) ** 1.5  # Rounded caw shape
            channel[(start + k) % LOOP_SAMPLES] += v * loud * 0.035  # Add the caw, quiet because it is far away


def cold_channels():  # Cold plain: steady wind with a faint hiss and far crows
    """Cold plain: steady wind with a faint hiss and far crows."""  # Describes the function
    channels = []  # Left and right
    for seed in (521, 547):  # One source per speaker, for width
        rng = random.Random(seed)  # Fixed seed for the crows
        data = cold_wind(seed, 1.0)  # Steady cold wind
        for _ in range(3):  # Three far crow calls in the loop
            crow(data, rng, rng.uniform(0.5, SECONDS - 1.0), rng.uniform(0.7, 1.0))  # One call at a random time
        channels.append(data)  # Keep this speaker
    return channels  # Give back both speakers


def make_loop(temp, name, channels, band, target):  # Filters, joins and encodes one seamless ambience loop
    """Filters, joins and encodes one seamless ambience loop."""  # Describes the function
    raw = temp / f"{name}-padded.wav"  # Receives the loop with a little wrap on both sides
    wet = temp / f"{name}-filtered.wav"  # Receives the trimmed, filtered loop
    pad = RATE // 2  # Lets the filters settle on real loop samples
    peaks = [max(map(abs, c)) or 1.0 for c in channels]  # The loudest sample of each speaker
    peaked = [[v / p for v in c] for c, p in zip(channels, peaks)]  # Scales each speaker to a peak of one, so 16-bit WAV does not clip
    ma.mm.write_wav(str(raw), *(c[-pad:] + c + c[:pad] for c in peaked))  # Wraps half a second around each side
    hn.ffmpeg("-i", raw, "-af", f"{band},atrim=start_sample={pad}:end_sample={pad + LOOP_SAMPLES}", wet)  # Filters and trims after settling
    loops = ma.mm.read_wav(str(wet))  # Reads the filtered loop back
    for channel in loops:  # Joins each speaker at the seam
        shift = (channel[-1] - channel[0]) / 2  # Shares any small edge step between the ends
        for i in range(RATE // 100):  # Blends the fix over ten milliseconds
            weight = 0.5 + 0.5 * math.cos(math.pi * i / (RATE // 100))  # Fades the fix to zero
            channel[i] += shift * weight  # Moves the start toward the end
            channel[-1 - i] -= shift * weight  # Moves the end toward the start
    gain = hn.level_gain(loops, target)  # Finds the gain for the target mean level
    ma.mm.write_wav(str(wet), *([v * gain for v in c] for c in loops))  # Applies the gain
    path = AUDIO / f"{name}.ogg"  # Names the loop asset
    hn.ffmpeg("-i", wet, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "4", path)  # Saves as OGG with fixed settings
    return path  # Gives back the saved loop


def preview(paths, temp):  # Shows the waveform and spectrum of every new file on one review sheet
    """Shows the waveform and spectrum of every new file on one review sheet."""  # Describes the function
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the preview folder if it is missing
    sheet = Image.new("RGB", (1092, 60 + 156 * len(paths)), (23, 28, 38))  # Sheet tall enough for every row
    draw = ImageDraw.Draw(sheet)  # Draws graph axes and labels
    ink, grid, text = (106, 211, 196), (52, 62, 76), (231, 237, 243)  # Keeps data and labels easy to see
    draw.text((20, 12), "Left channel waveform", fill=text)  # Names the left column
    draw.text((554, 12), "Average spectrum (dBFS, logarithmic Hz)", fill=text)  # Names the right column
    for row, path in enumerate(paths):  # Plots each sound in its own row
        decoded = temp / "decoded.wav"  # Holds the decoded asset rather than the source sound
        hn.ffmpeg("-i", path, "-c:a", "pcm_s16le", decoded)  # Reads back what Unity will hear
        left, right = ma.mm.read_wav(str(decoded))  # Gets both decoded speakers
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


def main():  # Writes, measures and previews the three Plains Hard sounds
    """Writes, measures and previews the three Plains Hard sounds."""  # Describes the function
    with TemporaryDirectory(prefix="landgrand-plains-hard-") as folder:  # Keeps the project clear of temporary WAV files
        temp = Path(folder)  # Names the temporary audio folder
        paths = [hn.make_effect("sfx_hay_roll.wav", hay_roll(), HAY_TARGET_DB)]  # The soft hay bale roll
        paths.append(hn.make_effect("sfx_herd_hooves.wav", herd_hooves(), HOOF_TARGET_DB))  # The gentle hoof nudge
        paths.append(make_loop(temp, "amb_cold_plain", cold_channels(), "highpass=f=40,lowpass=f=6500", LOOP_TARGET_DB))  # The cold plain loop
        preview(paths, temp)  # Writes the graphs from the final assets
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
