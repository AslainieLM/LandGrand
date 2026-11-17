"""Make Cave Drips, Splash, Bat Flap and Burner for Hills Normal."""  # Describes the new audio
import cmath  # Gives a wave turn for the spectrum
import json  # Reads the sound length from ffprobe
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
import random  # Gives repeatable noise
import re  # Finds the measured level in ffmpeg output
import subprocess  # Runs ffmpeg and ffprobe
from tempfile import TemporaryDirectory  # Holds temporary WAV files
from PIL import Image, ImageDraw  # Draws the wave and spectrum preview
import make_mountains_audio as ma  # Shares the project paths and sample rate
import make_normal_audio as na  # Shares the mono effect writer

RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "hills-normal" / "audio-preview.png"  # Holds the review graph
SECONDS = 24.0  # The loop length
LOOP_SAMPLES = int(SECONDS * RATE)  # The number of samples in the loop
FOLD_SAMPLES = RATE  # Blends one extra second into the start


def ffmpeg(*args):  # Runs the audio encoder with short logs
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", *map(str, args)], check=True)  # Stops if ffmpeg fails


def soft_noise(seconds, seed, low, high):  # Makes noise between two soft cutoffs
    rng = random.Random(seed)  # Gives this effect its own repeatable grains
    first = mid = bass = 0.0  # Starts the filters quietly
    fast = 1 - math.exp(-math.tau * high / RATE)  # Sets the upper filter speed
    slow = 1 - math.exp(-math.tau * low / RATE)  # Sets the lower filter speed
    for _ in range(int(seconds * RATE)):  # Makes the requested number of samples
        first += fast * (rng.uniform(-1, 1) - first)  # Takes the edge off the noise
        mid += fast * (first - mid)  # Rolls off the remaining high hiss
        bass += slow * (mid - bass)  # Follows the low part
        yield mid - bass  # Leaves only the band between the cutoffs


def cave_floor(seed):  # Makes the soft rumble and still air beneath the drips
    floor = []  # Holds the sound with an extra second for the fold
    low = soft_noise(SECONDS + 1, seed, 50, 150)  # Makes a low, quiet rumble
    air = soft_noise(SECONDS + 1, seed + 1, 135, 420)  # Adds a soft cavern tone
    for i, (rumble, room) in enumerate(zip(low, air)):  # Mixes the two parts
        sway = 0.81 + 0.16 * math.sin(math.tau * i / (RATE * 7.8))  # Lets the distant rumble vary slowly
        floor.append(rumble * 0.29 * sway + room * 0.055)  # Keeps dry air under the water sound
    for i in range(FOLD_SAMPLES):  # Folds the end onto the start
        weight = 0.5 - 0.5 * math.cos(math.pi * i / FOLD_SAMPLES)  # Joins the steady bed smoothly
        floor[i] = floor[LOOP_SAMPLES + i] * (1 - weight) + floor[i] * weight  # Carries the end through the boundary
    return floor[:LOOP_SAMPLES]  # Keeps exactly one loop


def drop(tone):  # Shapes the sound of one drip falling into water
    out = []  # Holds the short falling note
    for i in range(int(0.19 * RATE)):  # Gives the ripple time to die away
        t = i / RATE  # Finds the age of the drop
        pulse = 1 - math.exp(-t * 600)  # Takes the edge off the strike
        bend = math.tau * (tone * t + 29 * (1 - math.exp(-t * 37)))  # Starts bright and falls in pitch
        tick = math.sin(bend) * math.exp(-t * 41)  # Lets the main impact decay quickly
        ring = math.sin(math.tau * tone * 0.63 * t) * math.exp(-t * 24)  # Adds a short low ripple
        out.append(pulse * (tick * 0.78 + ring * 0.37))  # Combines the strike with the ring
    return out  # Gives back the drop


def cave_mix():  # Mixes staggered drops and their short stereo echoes
    left, right = cave_floor(705), cave_floor(707)  # Builds the quiet stereo cavern bed
    rng = random.Random(709)  # Repeats the same timing and loudness
    at = 0.51  # Starts with a little open space
    while at < SECONDS:  # Spaces the drips over the loop
        drops = drop(rng.uniform(310, 640))  # Varies the pitch from one drip to the next
        sway, loud = rng.uniform(0.32, 0.68), rng.uniform(0.19, 0.38)  # Places each drop in the room
        for delay, gain, left_gain in ((0, 1, sway), (0.097, 0.41, 1 - sway), (0.181, 0.27, sway), (0.303, 0.14, 1 - sway), (0.449, 0.07, 0.5)):  # Gives it several softer wall echoes
            start = round((at + delay) * RATE)  # Finds the start of this reflection
            for i, sample in enumerate(drops):  # Mixes the whole decay
                pos = (start + i) % LOOP_SAMPLES  # Folds any late echoes onto the start
                left[pos] += sample * gain * loud * left_gain  # Leaves the source slightly off center
                right[pos] += sample * gain * loud * (1 - left_gain)  # Gives the other wall its own delay balance
        at += rng.uniform(0.56, 1.77)  # Leaves uneven gaps between drips
    return left, right  # Gives back both speakers


def splash():  # Makes a small splash with a bubbly end
    out = []  # Holds the half-damped spray
    for i, fizz in enumerate(soft_noise(0.30, 713, 230, 1750)):  # Uses a soft filter on the water
        t = i / RATE  # Finds the age of the splash
        env = (1 - math.exp(-t * 250)) * math.exp(-t * 15)  # Makes a quick hit and a soft tail
        bubble = sum(math.sin(math.tau * (420 * age + 2700 * age * age)) * math.sin(math.pi * age / 0.044) ** 2 for start in (0.079, 0.131, 0.19, 0.223) if 0 < (age := t - start) < 0.044)  # Adds four small rising bubbles
        out.append(fizz * env + bubble * 0.066 * math.exp(-t * 4))  # Mixes the spray and bubbles
    return out  # Gives back the splash


def flap():  # Makes three soft, quick wing sounds
    out = []  # Holds the wing puffs
    for i, rustle in enumerate(soft_noise(0.25, 715, 120, 750)):  # Keeps the wings soft and low
        t = i / RATE  # Finds the age of the sound
        env = sum(loud * math.sin(math.pi * age / 0.064) ** 2 for start, loud in ((0.01, 1.0), (0.083, 0.8), (0.155, 0.64)) if 0 < (age := t - start) < 0.064)  # Shapes three rounded strokes
        out.append(rustle * env)  # Adds the soft stroke without a squeal
    return out  # Gives back the flaps


def burner():  # Makes the rising balloon's soft gas whoosh
    out = []  # Holds the burner sound
    for i, rush in enumerate(soft_noise(0.50, 717, 90, 980)):  # Lowers the harsh top of the rush
        t = i / RATE  # Finds the age of the rush
        env = min(1.0, t / 0.05) * min(1.0, (0.5 - t) / 0.14)  # Opens quickly and tapers off near the end
        flutter = 0.82 + 0.18 * math.sin(math.tau * 14 * t)  # Adds a low flame flutter
        out.append(rush * env * flutter)  # Shapes the gas sound
    return out  # Gives back the whoosh


def level_gain(channels, mean_db):  # Finds the gain needed for the target mean
    power = sum(sum(sample * sample for sample in channel) for channel in channels) / sum(map(len, channels))  # Measures mean-square level
    gain = 10 ** (mean_db / 20) / math.sqrt(power)  # Sets the desired RMS
    peak = max(max(map(abs, channel)) for channel in channels)  # Checks the loudest instant
    return min(gain, 0.95 / peak)  # Leaves room so no samples clip


def make_cave(temp):  # Filters and encodes the seamless cave loop
    raw = temp / "cave-padded.wav"  # Receives the sound with a little wrap on both sides
    wet = temp / "cave-filtered.wav"  # Receives the trimmed, filtered loop
    dry, post = cave_mix(), []  # Builds the unfiltered two-channel loop
    pad = RATE // 2  # Lets the filters settle on real loop samples
    for channel in dry:  # Prepares each speaker
        post.append(channel[-pad:] + channel + channel[:pad])  # Wraps half a second around each side
    ma.mm.write_wav(str(raw), *post)  # Uses the shared WAV writer
    ffmpeg("-i", raw, "-af", f"highpass=f=55,lowpass=f=2600,atrim=start_sample={pad}:end_sample={pad + LOOP_SAMPLES}", wet)  # Softens the loop after the filter settles
    channels = [[0.16 * math.tanh(v / 0.16) for v in channel] for channel in ma.mm.read_wav(str(wet))]  # Softens the loudest drop so the room can stay audible
    for channel in channels:  # Joins each filtered speaker at the seam
        shift = (channel[-1] - channel[0]) / 2  # Shares any last small edge shift between the two ends
        for i in range(RATE // 100):  # Blends the correction over ten milliseconds
            weight = 0.5 + 0.5 * math.cos(math.pi * i / (RATE // 100))  # Fades the step correction to zero
            channel[i] += shift * weight  # Moves the start gently toward the end
            channel[-1 - i] -= shift * weight  # Moves the end gently toward the start
    gain = level_gain(channels, -21)  # Targets the requested mean level
    ma.mm.write_wav(str(wet), *([v * gain for v in channel] for channel in channels))  # Applies the gain before encoding
    path = AUDIO / "amb_cave_drips.ogg"  # Names the new loop asset
    ffmpeg("-i", wet, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "4", path)  # Uses the Hills OGG settings
    return path  # Gives back the saved loop


def make_effect(name, samples, target):  # Sets the effect mean and writes it with the shared helper
    peak = max(map(abs, samples)) * level_gain([samples], target)  # Sends the required peak to the existing writer
    na.save_wav(name, samples, peak)  # Writes the stereo asset with matching channels
    return AUDIO / name  # Gives back the saved file


def measure(path):  # Measures the file as Unity receives it
    measure_run = subprocess.run(["ffmpeg", "-hide_banner", "-i", str(path), "-af", "volumedetect", "-f", "null", "-"], capture_output=True, text=True, check=True)  # Runs the required loudness measurement
    mean = float(re.search(r"mean_volume:\s+(-?\d+(?:\.\d+)?) dB", measure_run.stderr)[1])  # Reads the measured mean dB
    duration = subprocess.check_output(["ffprobe", "-v", "error", "-show_entries", "format=duration", "-of", "json", str(path)], text=True)  # Reads the encoded length
    return float(json.loads(duration)["format"]["duration"]), mean  # Gives back the exact length and measured level


def spectrum(samples):  # Measures the average power by frequency with a short FFT
    n = 4096  # Gives enough samples to show the lower pitches
    bands = [0.0] * (n // 2 + 1)  # Holds the power of each frequency
    windows = min(240, len(samples) // n)  # Includes quiet and loud parts throughout the clip
    for frame in range(windows):  # Averages many overlapping slices of sound
        start = (len(samples) - n) * frame // max(1, windows - 1)  # Spreads the slices over the file
        window = [sample * (0.5 - 0.5 * math.cos(math.tau * i / n)) for i, sample in enumerate(samples[start:start + n])]  # Softens the ends of this slice
        for i, value in enumerate(fft(window)[:len(bands)]):  # Reads the lower half of the transform
            bands[i] += abs(value) ** 2  # Adds power in this band
    return [10 * math.log10(max(power / (windows * (n / 4) ** 2), 1e-12)) for power in bands]  # Changes power to dBFS


def fft(values):  # Turns one window into frequency bins
    n = len(values)  # Records the transform size
    if n == 1:  # A one-sample window needs no combining
        return values  # Gives back its only bin
    even, odd = fft(values[::2]), fft(values[1::2])  # Splits into even and odd positions
    turn = cmath.exp(-1j * math.tau / n)  # Gives the angle step for one bin
    rotated = [value * turn ** i for i, value in enumerate(odd)]  # Lines up the odd bins
    return [a + b for a, b in zip(even, rotated)] + [a - b for a, b in zip(even, rotated)]  # Combines both halves


def preview(paths, temp):  # Shows the file waveform and spectrum on one review sheet
    sheet = Image.new("RGB", (1092, 666), (23, 28, 38))  # Gives the graphs room and a quiet background
    draw = ImageDraw.Draw(sheet)  # Draws graph axes and labels
    ink, grid, text = (106, 211, 196), (52, 62, 76), (231, 237, 243)  # Keeps data and labels easy to see
    draw.text((20, 12), "Left channel waveform", fill=text)  # Names the left column
    draw.text((554, 12), "Average spectrum (dBFS, logarithmic Hz)", fill=text)  # Names the right column
    for row, path in enumerate(paths):  # Plots each sound in its own row
        decoded = temp / "decoded.wav"  # Holds the decoded asset rather than the source mix
        ffmpeg("-i", path, "-c:a", "pcm_s16le", decoded)  # Reads back what Unity will hear
        left, right = ma.mm.read_wav(str(decoded))  # Gets both decoded speakers
        seconds, mean = measure(path)  # Gets the exact ffmpeg measurement
        print(f"{path.name}: {seconds:.3f} s, mean {mean:.1f} dB")  # Reports duration and loudness
        if path.suffix == ".ogg":  # Checks the playhead step across the cave seam
            for label, channel in (("left", left), ("right", right)):  # Checks the two speakers separately
                step = abs(channel[0] - channel[-1])  # Reads the single step across the join
                rms = math.sqrt(sum((b - a) ** 2 for a, b in zip(channel[:RATE // 10], channel[1:RATE // 10 + 1])) / (RATE // 10))  # Measures ordinary sample steps nearby
                print(f"  {label} loop seam: {step:.6f} (nearby step RMS {rms:.6f})")  # Prints the join size for review
        top, mid = 41 + row * 156, 109 + row * 156  # Finds this panel's rows
        draw.text((20, top), f"{path.name}  |  {seconds:.3f} s  |  {mean:.1f} dB mean", fill=text)  # Names the clip and its result
        draw.line((20, mid, 519, mid), fill=grid)  # Marks the waveform center
        for x in range(500):  # Draws the tallest swing in this slice
            samples = left[x * len(left) // 500:(x + 1) * len(left) // 500]  # Takes the samples under one pixel
            draw.line((20 + x, mid - int(max(samples) * 48), 20 + x, mid - int(min(samples) * 48)), fill=ink)  # Draws the min and max
        for db in (-20, -40, -60, -80, -100):  # Adds guide lines every twenty decibels
            y = top + 15 - db  # Places lower power toward the bottom
            draw.line((578, y, 1071, y), fill=grid)  # Marks the row
            draw.text((552, y - 5), str(db), fill=text)  # Shows the line value
        bands = spectrum(left)  # Measures the power of the actual encoded audio
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


def main():  # Writes and measures all four Hills Normal sounds
    with TemporaryDirectory(prefix="landgrand-hills-normal-") as folder:  # Keeps the project clear of intermediate WAV files
        temp = Path(folder)  # Names the temporary audio folder
        paths = [make_cave(temp), make_effect("sfx_splash.wav", splash(), -24), make_effect("sfx_bat_flap.wav", flap(), -26), make_effect("sfx_burner.wav", burner(), -24)]  # Makes the one loop and three effects
        preview(paths, temp)  # Writes the graphs from the final assets
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
