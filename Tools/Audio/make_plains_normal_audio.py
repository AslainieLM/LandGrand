"""Makes the Plains Normal boat bump for LandGrand: Assets/Audio/sfx_boat_bump.wav.  # Says what this script is for

Run from the Unity project folder:  python Tools/Audio/make_plains_normal_audio.py   (needs ffmpeg and Pillow)  # How to run it
It writes one file to Assets/Audio/:  # What it makes
  sfx_boat_bump.wav    A soft, hollow wooden "thunk" with a tiny water slosh after it, when the explorer lands on a moving market boat.  # Effect
The sound is about 0.3 s long and uses fixed random seeds, so each run makes the same file.  # Repeatable
"""  # End of the description
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
from tempfile import TemporaryDirectory  # Holds temporary WAV files
from PIL import Image, ImageDraw  # Draws the review sheet
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares ffmpeg, soft noise, effect writer and measuring

RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "plains-normal" / "audio-preview.png"  # Holds the review graph
SECONDS = 0.3  # Length of the boat bump
TARGET_DB = -26.0  # Mean level: a little quieter than sfx_straw_bounce (-25 dB)


def unit(data):  # Scales a sound so its RMS is one
    """Scales a sound so its RMS is one."""  # Describes the function
    rms = math.sqrt(sum(v * v for v in data) / len(data)) or 1.0  # Measures the present level
    return [v / rms for v in data]  # Gives back the scaled copy


def boat_bump():  # Makes the soft wooden thunk and the small water slosh after it
    """Makes the soft wooden thunk and the small water slosh after it."""  # Describes the function
    count = int(SECONDS * RATE)  # Number of samples in the bump
    knock = unit(list(hn.soft_noise(SECONDS, 1901, 250, 1400)))  # Soft wood knock noise, at RMS one
    slosh = unit(list(hn.soft_noise(SECONDS, 1907, 180, 900)))  # Water noise for the slosh, at RMS one
    out = []  # Holds the bump
    for i in range(count):  # Each sample of the bump
        t = i / RATE  # Age of the bump in seconds
        attack = min(1.0, t / 0.004)  # Soft start over 4 ms, so there is no sharp click
        end = min(1.0, (SECONDS - t) / 0.04)  # Fades the last 40 ms to zero, so the sound ends quietly
        bend = math.tau * (125 * t + 25 * (1 - math.exp(-t * 30)) / 30)  # Starts near 150 Hz and eases to 125 Hz, like a hollow board
        body = math.sin(bend) * attack * math.exp(-t * 16)  # Round, hollow wood body that fades
        ring = math.sin(math.tau * 310 * t) * attack * math.exp(-t * 26) * 0.35  # A short hollow ring above the body
        tap = knock[i] * attack * math.exp(-t * 70) * 0.25  # A very short, soft tap of the wood
        age = max(0.0, t - 0.1)  # Time since the water started to move
        swell = min(1.0, age / 0.06) * math.exp(-age * 11)  # The slosh rises after 0.1 s, then fades
        wobble = 0.75 + 0.25 * math.sin(math.tau * 9 * t)  # Slow wobble, like water moving
        water = slosh[i] * swell * wobble * 0.12  # A tiny water slosh after the thunk
        out.append((body * 0.8 + ring + tap + water) * end)  # Mixes the parts and fades the end
    return out  # Gives back the bump


def preview(paths, temp):  # Shows the waveform and spectrum of the bump on one review sheet
    """Shows the waveform and spectrum of the bump on one review sheet."""  # Describes the function
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


def main():  # Writes, measures and previews the Plains boat bump
    """Writes, measures and previews the Plains boat bump."""  # Describes the function
    paths = [hn.make_effect("sfx_boat_bump.wav", boat_bump(), TARGET_DB)]  # Makes the bump at the target mean level
    with TemporaryDirectory(prefix="landgrand-plains-normal-") as folder:  # Keeps the project clear of temporary files
        preview(paths, Path(folder))  # Writes the graph from the final asset
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sound
