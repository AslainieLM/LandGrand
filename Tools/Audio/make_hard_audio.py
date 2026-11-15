"""Makes the sounds for Mountains Hard 1 to 7 and the Landform Description Screen (D-065).

Run from the Unity project folder:  python Tools/Audio/make_hard_audio.py   (needs ffmpeg for the OGG file)
It writes six files to Assets/Audio/:
  amb_wind_hard.ogg      Gusty, near-storm wind for every Hard level (D-042), with a far-off rumble, 24 s loop.
  sfx_boulder_rumble.wav A 0.8 s low rumble that warns a rolling boulder is coming.
  sfx_boulder_roll.wav   A 1.0 s rolling crunch that loops while a boulder rolls near the explorer.
  sfx_ice_slide.wav      A 0.4 s icy hiss when the explorer starts to slide on ice.
  sfx_fanfare.wav        A 2.6 s happy fanfare when the Landform Description Screen opens.
  sfx_unlock.wav         A 0.7 s padlock click and sparkle when a new landform opens on the map.
Every sound uses a fixed random seed, so each run makes the same files.
"""
import math  # math gives sin, exp and pi
import os  # os joins paths and removes the temporary file
import random  # random makes the noise (with fixed seeds)
import subprocess  # subprocess runs ffmpeg
import make_mountains_audio as ma  # Rate, folders and the WAV writer from the Easy audio script
import make_normal_audio as na  # save_wav from the Normal audio script

RATE = ma.RATE  # Samples per second


def make_wind():
    """Makes 24 seconds of gusty, near-storm wind that loops without a gap and saves amb_wind_hard.ogg."""
    seconds, fade = 24.0, 3.0  # Loop length and the part folded from the end onto the start
    total = int((seconds + fade) * RATE)  # Samples rendered
    loop_n = int(seconds * RATE)  # Samples in the loop
    channels = []  # Left and right
    for seed in (131, 149):  # One noise source per speaker, for width
        rng = random.Random(seed)  # Fixed seed
        low = mid = hiss = thunder = 0.0  # Filter memory: rumble, body, whistle and far thunder
        phase = rng.random() * 6.28  # Gust timing differs between the speakers
        data = []  # This channel
        for i in range(total):  # Each sample
            t = i / RATE  # Seconds
            white = rng.uniform(-1.0, 1.0)  # Raw noise
            low += 0.015 * (white - low)  # Deep rumble of storm wind
            mid += 0.08 * (white - mid)  # Body of the wind
            hiss += 0.35 * (white - hiss)  # Whistle over rocks
            thunder += 0.003 * (white - thunder)  # Very low far-off rumble
            gust = 0.5 + 0.3 * math.sin(2 * math.pi * t / 4.0 + phase) + 0.25 * math.sin(2 * math.pi * t / 1.6 + 2 * phase)  # Faster, wilder gusts than Normal
            gust = max(0.2, gust)  # Never quite calm
            roll = max(0.0, math.sin(2 * math.pi * t / 12.0 + phase)) ** 4  # A far rumble twice per loop
            data.append(low * 6.5 * gust + mid * 1.8 * gust * gust + (hiss - mid) * 0.45 * gust ** 3 + thunder * 30.0 * roll)  # Mix
        looped = data[:loop_n]  # The loop part
        for i in range(int(fade * RATE)):  # Fold the end onto the start with constant-power weights
            w = i / (fade * RATE)  # 0 to 1 across the fold
            looped[i] = data[i] * math.sin(w * math.pi / 2) + data[loop_n + i] * math.cos(w * math.pi / 2)  # Smooth join
        channels.append(looped)  # Keep this channel
    peak = max(max(abs(v) for v in c) for c in channels)  # Loudest sample
    gain = 0.72 / peak  # A touch louder than the Normal wind
    os.makedirs(ma.TEMP, exist_ok=True)  # The temporary folder
    temp = os.path.join(ma.TEMP, "wind_hard.wav")  # Temporary WAV
    ma.mm.write_wav(temp, [v * gain for v in channels[0]], [v * gain for v in channels[1]])  # Save the loop
    out = os.path.join(ma.AUDIO, "amb_wind_hard.ogg")  # The finished file
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", temp, "-c:a", "libvorbis", "-q:a", "4", out], check=True)  # Save as OGG
    os.remove(temp)  # Remove the temporary file
    print("Wind written to", out)  # Tell the person where the file went


def make_rumble():
    """sfx_boulder_rumble.wav: a 0.8 s low rumble that swells, warning that a boulder is coming."""
    rng = random.Random(151)  # Fixed seed
    low = 0.0  # Filter memory
    data = []  # The sound
    n = int(0.8 * RATE)  # Samples
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        low += 0.02 * (rng.uniform(-1, 1) - low)  # Deep rumble noise
        tone = math.sin(2 * math.pi * 45 * t) * 0.3  # A low hum under it
        env = min(1.0, t / 0.6) * math.exp(-max(0.0, t - 0.6) * 12)  # Swells for 0.6 s, then ends
        data.append((low * 8.0 + tone) * env)  # Mix
    na.save_wav("sfx_boulder_rumble.wav", data, 0.75)  # Clear warning


def make_roll():
    """sfx_boulder_roll.wav: 1.0 s of rolling crunch whose end meets its start, so it can loop."""
    rng = random.Random(163)  # Fixed seed
    n = int(1.0 * RATE)  # Samples
    grit = 0.0  # Filter memory
    data = []  # The sound
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        grit += 0.3 * (rng.uniform(-1, 1) - grit)  # Crunching gravel
        bump = 0.5 + 0.5 * math.sin(2 * math.pi * 6 * t)  # Six bumps a second as it turns (whole cycles, so it loops)
        thump = math.sin(2 * math.pi * 60 * t) * bump ** 4  # A low thump on each bump
        data.append(grit * 0.6 * (0.4 + 0.6 * bump) + thump * 0.5)  # Mix
    for i in range(int(0.02 * RATE)):  # Short fades so the loop point does not click
        w = i / (0.02 * RATE)  # 0 to 1
        data[i] *= w  # Fade in
        data[-1 - i] *= w  # Fade out
    na.save_wav("sfx_boulder_roll.wav", data, 0.55)  # Under the music


def make_ice():
    """sfx_ice_slide.wav: a 0.4 s bright hiss, like a boot sliding on ice."""
    rng = random.Random(173)  # Fixed seed
    n = int(0.4 * RATE)  # Samples
    low = 0.0  # Filter memory
    data = []  # The sound
    for i in range(n):  # Each sample
        t = i / RATE  # Seconds
        white = rng.uniform(-1, 1)  # Raw noise
        low += 0.5 * (white - low)  # Smooth the noise a little
        hiss = white - low  # Keep only the bright part: a thin, glassy hiss
        env = math.sin(math.pi * t / 0.4) ** 0.8  # Quick swell and fade
        ring = math.sin(2 * math.pi * 2400 * t) * 0.08 * math.exp(-t * 8)  # A tiny icy ring at the start
        data.append(hiss * env * 0.7 + ring)  # Mix
    na.save_wav("sfx_ice_slide.wav", data, 0.45)  # Soft


def note(freq, start, length, data, loud=1.0):
    """Adds one bright plucked note (a fundamental plus two overtones) into data."""
    for k in range(int(length * RATE)):  # Each sample of the note
        i = int(start * RATE) + k  # Place in the sound
        if i >= len(data):  # Past the end
            break  # Stop
        t = k / RATE  # Seconds into the note
        env = min(1.0, t / 0.01) * math.exp(-t * 3.0)  # Fast attack, gentle fade
        wave = math.sin(2 * math.pi * freq * t) + 0.4 * math.sin(4 * math.pi * freq * t) + 0.15 * math.sin(6 * math.pi * freq * t)  # Bright tone
        data[i] += wave * env * loud  # Add it


def make_fanfare():
    """sfx_fanfare.wav: a 2.6 s rising C-major fanfare that ends on a held chord."""
    data = [0.0] * int(2.6 * RATE)  # The sound
    c4, e4, g4, c5, e5, g5 = 261.6, 329.6, 392.0, 523.3, 659.3, 784.0  # Notes of C major
    for j, f in enumerate((c4, e4, g4, c5)):  # Four quick rising notes
        note(f, j * 0.14, 0.4, data, 0.8)  # One note
    note(g4, 0.62, 0.3, data, 0.7)  # A little step back
    for f in (c5, e5, g5):  # The final chord
        note(f, 0.9, 1.7, data, 0.6)  # Held chord note
    note(c4, 0.9, 1.7, data, 0.5)  # Low root under the chord
    na.save_wav("sfx_fanfare.wav", data, 0.8)  # Bright and happy


def make_unlock():
    """sfx_unlock.wav: a metal click, then a quick sparkle of high notes."""
    rng = random.Random(181)  # Fixed seed
    data = [0.0] * int(0.7 * RATE)  # The sound
    for k in range(int(0.03 * RATE)):  # The click of the padlock
        data[k] += rng.uniform(-1, 1) * math.exp(-k / (0.004 * RATE))  # Sharp metal click
        data[k] += 0.6 * math.sin(2 * math.pi * 1800 * k / RATE) * math.exp(-k / (0.01 * RATE))  # Ring of the shackle
    for j, f in enumerate((1046.5, 1318.5, 1568.0, 2093.0)):  # Sparkle: C6, E6, G6, C7
        note(f, 0.12 + j * 0.07, 0.35, data, 0.35)  # One sparkle note
    na.save_wav("sfx_unlock.wav", data, 0.7)  # Clear


def main():
    make_wind()  # Hard wind loop
    make_rumble()  # Boulder warning
    make_roll()  # Boulder rolling
    make_ice()  # Ice slide
    make_fanfare()  # Landform Description Screen
    make_unlock()  # Map unlock


if __name__ == "__main__":
    main()  # Run when the file is started directly
