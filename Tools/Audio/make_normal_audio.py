"""Makes the sounds for Mountains Normal 1 to 5 (D-062).

Run from the Unity project folder:  python Tools/Audio/make_normal_audio.py   (needs ffmpeg for the OGG file)
It writes four files to Assets/Audio/:
  amb_wind_normal.ogg  Stronger wind for every Normal level (D-042): deeper rumble and bigger gusts, 24 s loop.
  sfx_gust.wav         A whoosh when a wind gust pushes the explorer (Mount Everest).
  sfx_crumble.wav      Cracking stone when a crumbling ledge breaks (Mount Pulag).
  sfx_rockfall.wav     A thud with gravel when a falling rock lands (Mount Pinatubo).
Every sound uses a fixed random seed, so each run makes the same files.
"""
import math  # math gives sin, exp and pi
import os  # os joins paths and removes the temporary file
import random  # random makes the noise (with fixed seeds)
import subprocess  # subprocess runs ffmpeg
import make_mountains_audio as ma  # Rate, folders and the WAV writer from the Easy audio script

RATE = ma.RATE  # Samples per second


def save_wav(name, data, loudness):
    """Normalises a mono sound to the given peak and saves it as a WAV file in Assets/Audio/."""
    peak = max(abs(v) for v in data) or 1.0  # Loudest sample
    data = [v * loudness / peak for v in data]  # Set the loudness
    path = os.path.join(ma.AUDIO, name)  # Where the file goes
    ma.mm.write_wav(path, data, data)  # Same sound in both speakers
    print("Effect written to", path)  # Tell the person where the file went


def make_wind():
    """Makes 24 seconds of strong mountain wind that loops without a gap and saves amb_wind_normal.ogg."""
    seconds, fade = 24.0, 3.0  # Loop length and the part folded from the end onto the start
    total = int((seconds + fade) * RATE)  # Samples rendered
    loop_n = int(seconds * RATE)  # Samples in the loop
    channels = []  # Left and right
    for seed in (53, 71):  # One noise source per speaker, for width
        rng = random.Random(seed)  # Fixed seed
        low = mid = hiss = 0.0  # Filter memory: rumble, body and whistle
        phase = rng.random() * 6.28  # Gust timing differs between the speakers
        data = []  # This channel
        for i in range(total):  # Each sample
            t = i / RATE  # Seconds
            white = rng.uniform(-1.0, 1.0)  # Raw noise
            low += 0.012 * (white - low)  # Deep rumble of strong wind
            mid += 0.06 * (white - mid)  # Body of the wind
            hiss += 0.3 * (white - hiss)  # Whistle over rocks
            gust = 0.45 + 0.35 * math.sin(2 * math.pi * t / 6.0 + phase) + 0.2 * math.sin(2 * math.pi * t / 2.4 + 2 * phase)  # Bigger, faster gusts than Easy
            gust = max(0.15, gust)  # Never quite silent
            data.append(low * 6.0 * gust + mid * 1.6 * gust * gust + (hiss - mid) * 0.35 * gust ** 3)  # Strong gusts whistle more
        looped = data[:loop_n]  # The loop part
        for i in range(int(fade * RATE)):  # Fold the end onto the start with constant-power weights
            w = i / (fade * RATE)  # 0 to 1 across the fold
            looped[i] = data[i] * math.sin(w * math.pi / 2) + data[loop_n + i] * math.cos(w * math.pi / 2)  # Smooth join
        channels.append(looped)  # Keep this channel
    peak = max(max(abs(v) for v in c) for c in channels)  # Loudest sample
    gain = 0.7 / peak  # A little louder than the Easy breeze
    os.makedirs(ma.TEMP, exist_ok=True)  # The temporary folder
    temp = os.path.join(ma.TEMP, "wind_normal.wav")  # Temporary WAV
    ma.mm.write_wav(temp, [v * gain for v in channels[0]], [v * gain for v in channels[1]])  # Save the loop
    out = os.path.join(ma.AUDIO, "amb_wind_normal.ogg")  # The finished file
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", temp, "-c:a", "libvorbis", "-q:a", "4", out], check=True)  # Save as OGG
    os.remove(temp)  # Remove the temporary file
    print("Wind written to", out)  # Tell the person where the file went


def make_gust():
    """sfx_gust.wav: a 0.9 s whoosh that rises and falls, for a wind gust push."""
    rng = random.Random(83)  # Fixed seed
    low = high = 0.0  # Filter memory
    data = []  # The sound
    n = int(0.9 * RATE)  # Samples
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        white = rng.uniform(-1.0, 1.0)  # Raw noise
        bright = 0.05 + 0.25 * math.sin(math.pi * t / 0.9)  # The filter opens in the middle, so it sounds like air rushing past
        low += 0.04 * (white - low)  # Soft body
        high += bright * (white - high)  # Rushing part
        env = math.sin(math.pi * t / 0.9) ** 1.5  # Swell and fade
        data.append((low * 2.0 + high * 0.8) * env)  # Mix
    save_wav("sfx_gust.wav", data, 0.6)  # Soft


def make_crumble():
    """sfx_crumble.wav: a 0.6 s burst of small cracks and a soft low thud, for a crumbling ledge."""
    rng = random.Random(97)  # Fixed seed
    n = int(0.6 * RATE)  # Samples
    data = [0.0] * n  # The sound
    for _ in range(26):  # Small cracks of stone
        start = int(rng.uniform(0.0, 0.4) * RATE)  # When this crack happens
        strength = rng.uniform(0.4, 1.0)  # How loud this crack is
        for k in range(int(0.012 * RATE)):  # Each crack lasts 12 ms
            if start + k < n:  # Inside the sound
                data[start + k] += rng.uniform(-1, 1) * math.exp(-k / (0.002 * RATE)) * strength  # A sharp click that dies fast
    for i in range(n):  # The low thud of the slab dropping
        t = i / RATE  # Seconds
        if t > 0.25:  # After the cracks start
            d = t - 0.25  # Time since the thud
            data[i] += 0.8 * math.sin(2 * math.pi * 70 * d) * math.exp(-d * 12)  # A soft 70 Hz bump
    save_wav("sfx_crumble.wav", data, 0.7)  # Clear but not harsh


def make_rockfall():
    """sfx_rockfall.wav: a 0.7 s heavy thud with rattling gravel, for a falling rock landing."""
    rng = random.Random(101)  # Fixed seed
    n = int(0.7 * RATE)  # Samples
    gravel = 0.0  # Filter memory for the gravel
    data = []  # The sound
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        thud = math.sin(2 * math.pi * (55 + 40 * math.exp(-t * 20)) * t) * math.exp(-t * 9)  # A falling-pitch thump
        gravel += 0.5 * (rng.uniform(-1, 1) - gravel)  # Bright rattling noise
        rattle = gravel * math.exp(-t * 6) * (0.5 + 0.5 * math.sin(2 * math.pi * 23 * t))  # Gravel that bounces and settles
        data.append(thud * 1.2 + rattle * 0.5)  # Mix
    save_wav("sfx_rockfall.wav", data, 0.8)  # Firm, so it reads as a warning


def main():
    make_wind()  # Stronger wind loop
    make_gust()  # Gust whoosh
    make_crumble()  # Crumbling ledge
    make_rockfall()  # Falling rock


if __name__ == "__main__":
    main()  # Run when the file is started directly
