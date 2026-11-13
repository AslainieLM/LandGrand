"""Makes the Mountains level audio for LandGrand (ticket 12, decision D-042).

Run from the Unity project folder:  python Tools/Audio/make_mountains_audio.py
Needs ffmpeg on the PATH (for the echo and the OGG files). No other packages are needed.

It writes four files to Assets/Audio/:
  bgm_mountains.ogg    Mountains music: a slow, open piece in D minor, 84 BPM, 16 bars (about 45.7 s), no drums.
                       It reuses the sound-making code of make_menu_music.py, so it belongs to the same family but is a new tune.
  amb_breeze_easy.ogg  A light breeze for Easy levels: soft filtered noise with slow gusts, 24 s.
  sfx_hit.wav          A short falling thud when the explorer touches a trap or falls into the pit.
  sfx_checkpoint.wav   A short two-note ding when a checkpoint flag is reached.
Both loops join without a gap: the end is folded back onto the start.
"""
import math  # math gives sin, exp and pi for making the sounds
import os  # os joins paths and removes the temporary files
import random  # random makes the noise of the breeze (with a fixed seed, so every run gives the same sound)
import subprocess  # subprocess runs ffmpeg

import make_menu_music as mm  # The menu music script: its note-making code is reused here

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
AUDIO = os.path.join(ROOT, "Assets", "Audio")  # Where the finished files go
TEMP = os.path.join(ROOT, "Temp")  # Temporary files (Temp is ignored by git)

RATE = mm.RATE  # Samples per second
BPM = 84  # Beats per minute: slower and more open than the menu music
BEAT = 60.0 / BPM  # Seconds per beat
BAR = 4 * BEAT  # Seconds per bar of four beats
BARS = 16  # Bars in the loop
LENGTH = BARS * BAR  # Loop length in seconds
TAIL = 4.0  # Extra seconds rendered after the loop for echoes and note endings
N = int(round((LENGTH + TAIL) * RATE))  # Total samples rendered
LOOP_N = int(round(LENGTH * RATE))  # Samples in the finished loop

# Chords for each bar as (bass note, pad notes), MIDI numbers: Dm, Bb, F, C, four times.
CHORDS = [
    (38, [50, 53, 57]),  # Dm: bass D2; pad D3 F3 A3
    (34, [50, 53, 58]),  # Bb: bass Bb1; pad D3 F3 Bb3
    (41, [48, 53, 57]),  # F: bass F2; pad C3 F3 A3
    (36, [52, 55, 60]),  # C: bass C2; pad E3 G3 C4
] * 4  # The four chords repeat four times

# Melody for bars 5 to 16 as (MIDI note, start beat, length in beats). Bars 1 to 4 are only pad, harp and bass.
MELODY = {
    4: [(74, 0, 2), (72, 2, 1), (69, 3, 1)],  # Bar 5 (Dm): D C A
    5: [(70, 0, 3), (72, 3, 1)],  # Bar 6 (Bb): Bb, then C
    6: [(72, 0, 2), (77, 2, 2)],  # Bar 7 (F): C, then F
    7: [(76, 0, 4)],  # Bar 8 (C): a long E
    8: [(74, 0, 1), (77, 1, 1), (76, 2, 1), (74, 3, 1)],  # Bar 9 (Dm): D F E D
    9: [(74, 0, 2), (70, 2, 2)],  # Bar 10 (Bb): D, then Bb
    10: [(69, 0, 1), (72, 1, 1), (77, 2, 2)],  # Bar 11 (F): A C F
    11: [(76, 0, 2), (72, 2, 2)],  # Bar 12 (C): E, then C
    12: [(74, 0, 2), (69, 2, 2)],  # Bar 13 (Dm): D, then A
    13: [(70, 0, 2), (74, 2, 2)],  # Bar 14 (Bb): Bb, then D
    14: [(72, 0, 1), (69, 1, 1), (72, 2, 2)],  # Bar 15 (F): C A C
    15: [(67, 0, 2), (71, 2, 2)],  # Bar 16 (C): G, then B, leading back to the start
}


def compose():
    """Places every note of the 16 bars, using the voices of the menu music script."""
    mm.N = N  # The shared note code writes into arrays of this length
    mm.left = [0.0] * N  # Left speaker samples
    mm.right = [0.0] * N  # Right speaker samples
    for bar, (bass, pad) in enumerate(CHORDS):  # Each bar and its chord
        t0 = bar * BAR  # Bar start time in seconds
        mm.add_note(t0, BAR, mm.freq(bass), mm.BASS, 0.30, 0.0)  # Bass note for the whole bar, in the middle
        for k, note in enumerate(pad):  # Each pad note
            pan = (-0.5, 0.0, 0.5)[k]  # Spread the three notes left, middle, right
            mm.add_note(t0, BAR, mm.freq(note) * 0.9985, mm.PAD, 0.065, pan)  # Pad note, a hair flat
            mm.add_note(t0, BAR, mm.freq(note) * 1.0015, mm.PAD, 0.065, -pan)  # Same note a hair sharp, other side, for width
        tones = sorted(pad) + [pad[0] + 12]  # Chord tones for the harp, low to high
        for step, index in enumerate((0, 2, 3, 2)):  # Four quarter notes per bar: sparse, so the music stays calm
            note = tones[index] + 12  # One octave above the pad
            pan = 0.35 if step % 2 == 0 else -0.35  # Bounce between left and right
            mm.add_note(t0 + step * BEAT, BEAT, mm.freq(note), mm.PLUCK, 0.13, pan)  # One harp note
        for note, beat, beats in MELODY.get(bar, []):  # Melody notes in this bar, if any
            mm.add_note(t0 + beat * BEAT, beats * BEAT * 0.92, mm.freq(note), mm.FLUTE, 0.20, 0.1)  # Slightly short, so notes breathe


def make_music():
    """Composes the music, adds echo, folds the tail into a seamless loop and saves bgm_mountains.ogg."""
    compose()  # Place every note
    left, right = mm.left, mm.right  # The mix
    peak = max(max(abs(v) for v in left), max(abs(v) for v in right))  # Loudest sample so far
    scale = 0.5 / peak  # Leave room for the echo
    dry = os.path.join(TEMP, "mountains_dry.wav")  # Temporary file before the echo
    wet = os.path.join(TEMP, "mountains_wet.wav")  # Temporary file after the echo
    loop = os.path.join(TEMP, "mountains_loop.wav")  # Temporary file of the finished loop
    mm.write_wav(dry, [v * scale for v in left], [v * scale for v in right])  # Save the dry mix
    echo = "aecho=0.8:0.6:61|113|171|257:0.34|0.26|0.18|0.12,lowpass=f=7000,highpass=f=40"  # A wider, airier echo than the menu: open mountain air
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", dry, "-af", echo, wet], check=True)  # Add the echo
    l, r = mm.read_wav(wet)  # Read the echoed mix back
    loop_l, loop_r = l[:LOOP_N], r[:LOOP_N]  # The loop part
    for i in range(LOOP_N, min(len(l), N)):  # Each sample after the loop end
        loop_l[i - LOOP_N] += l[i]  # Fold the left tail onto the start, so the loop joins smoothly
        loop_r[i - LOOP_N] += r[i]  # Fold the right tail onto the start
    peak = max(max(abs(v) for v in loop_l), max(abs(v) for v in loop_r))  # Loudest sample of the loop
    gain = 0.8 / peak  # A bit lower than the menu music; the game also plays it quieter
    mm.write_wav(loop, [v * gain for v in loop_l], [v * gain for v in loop_r])  # Save the finished loop
    out = os.path.join(AUDIO, "bgm_mountains.ogg")  # The finished music file
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", loop, "-c:a", "libvorbis", "-q:a", "5", out], check=True)  # Save as OGG
    for path in (dry, wet, loop):  # Remove the temporary files
        os.remove(path)  # Delete one temporary file
    print("Music written to", out)  # Tell the person where the file went


def make_breeze():
    """Makes 24 seconds of light breeze that loops without a gap and saves amb_breeze_easy.ogg."""
    seconds = 24.0  # Loop length
    fade = 3.0  # Seconds folded from the end onto the start
    total = int((seconds + fade) * RATE)  # Samples rendered
    loop_n = int(seconds * RATE)  # Samples in the loop
    out_channels = []  # Left and right samples
    for seed in (11, 23):  # One noise source per speaker, so the breeze has width
        rng = random.Random(seed)  # Fixed seed: the same sound every run
        low = hiss = 0.0  # Filter memory for the low and the high part of the wind
        data = []  # This channel
        phase = rng.random() * 6.28  # Gust timing differs a little between the speakers
        for i in range(total):  # Each sample
            t = i / RATE  # Seconds
            white = rng.uniform(-1.0, 1.0)  # Raw noise
            low += 0.02 * (white - low)  # Slow filter: the soft rumble of moving air (about 140 Hz)
            hiss += 0.25 * (white - hiss)  # Faster filter: the airy part (about 2.2 kHz)
            gust = 0.5 + 0.28 * math.sin(2 * math.pi * t / 8.0 + phase) + 0.14 * math.sin(2 * math.pi * t / 3.3 + 2 * phase)  # Slow gusts, never silent
            airy = 0.5 + 0.5 * math.sin(2 * math.pi * t / 5.0 + phase)  # The airy part comes and goes on its own
            data.append((low * 4.0 * gust + (hiss - low) * 1.2 * gust * airy * 0.6) * 0.35)  # Mix the two parts
        looped = data[:loop_n]  # The loop part
        for i in range(int(fade * RATE)):  # Fold the end onto the start, with constant-power weights
            w = i / (fade * RATE)  # 0 at the start of the fold, 1 at its end
            looped[i] = data[i] * math.sin(w * math.pi / 2) + data[loop_n + i] * math.cos(w * math.pi / 2)  # Start fades in while the end fades out
        out_channels.append(looped)  # Keep this channel
    peak = max(max(abs(v) for v in c) for c in out_channels)  # Loudest sample
    gain = 0.6 / peak  # A soft ambience
    temp = os.path.join(TEMP, "breeze.wav")  # Temporary file
    mm.write_wav(temp, [v * gain for v in out_channels[0]], [v * gain for v in out_channels[1]])  # Save the loop
    out = os.path.join(AUDIO, "amb_breeze_easy.ogg")  # The finished ambience file
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", temp, "-c:a", "libvorbis", "-q:a", "4", out], check=True)  # Save as OGG
    os.remove(temp)  # Remove the temporary file
    print("Breeze written to", out)  # Tell the person where the file went


def make_sfx(name, notes, decay, seconds, glide=0.0):
    """Makes a short mono-like effect from (start second, frequency) notes with a soft ring-out. glide drops the pitch (0.5 = half) over the sound."""
    n = int(seconds * RATE)  # Samples
    data = [0.0] * n  # The sound
    for start, f in notes:  # Each note
        s0 = int(start * RATE)  # First sample of this note
        phase = 0.0  # How far the wave has turned, added up sample by sample so a falling pitch stays smooth
        for i in range(s0, n):  # Each sample from the start of the note
            t = (i - s0) / RATE  # Seconds since the note began
            pitch = f * (1.0 - glide * min(1.0, t / seconds))  # The pitch, falling when glide is set
            phase += 2 * math.pi * pitch / RATE  # Turn the wave by this sample's share
            env = min(1.0, t / 0.004) * math.exp(-t * decay)  # Quick start, soft ring-out
            data[i] += math.sin(phase) * env * 0.5 + math.sin(2 * phase) * env * 0.12  # Tone plus one soft overtone
    peak = max(abs(v) for v in data)  # Loudest sample
    data = [v * 0.8 / peak for v in data]  # Normalise
    mm.write_wav(os.path.join(AUDIO, name), data, data)  # Save as a WAV file
    print("Effect written to", os.path.join(AUDIO, name))  # Tell the person where the file went


def main():
    """Makes all four files."""
    os.makedirs(TEMP, exist_ok=True)  # Make the temporary folder if it is missing
    make_music()  # The Mountains music
    make_breeze()  # The Easy breeze
    make_sfx("sfx_hit.wav", [(0.0, 220.0)], 7.0, 0.45, glide=0.55)  # A falling thud for a trap or pit
    make_sfx("sfx_checkpoint.wav", [(0.0, 659.0), (0.14, 880.0)], 5.0, 0.6)  # A two-note ding for the flag


if __name__ == "__main__":
    main()  # Run when the file is started directly
