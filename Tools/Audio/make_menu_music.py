"""Makes the calm Main Menu music loop for LandGrand: Assets/Audio/bgm_mainmenu.ogg.

Run from the Unity project folder:  python Tools/Audio/make_menu_music.py
Needs ffmpeg on the PATH (for the echo and the OGG file). No other packages are needed.

The piece is a calmer relative of the start-screen "Quest" music: same key (A minor) and the same
chords (Am, F, C, G), but slower (96 BPM), with no drums. A soft pad, a music-box arpeggio and a
gentle flute-like melody play over a low bass. It is 12 bars long (exactly 30 seconds) and loops
without a gap: the echo tail at the end is folded back onto the start.
"""
import math  # math gives sin, cos, exp and pi for making the sounds
import os  # os joins paths and removes the temporary files
import struct  # struct turns sample numbers into bytes and back
import subprocess  # subprocess runs ffmpeg
import wave  # wave reads and writes WAV files

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))  # The Unity project folder
OUT_OGG = os.path.join(ROOT, "Assets", "Audio", "bgm_mainmenu.ogg")  # The finished music file
TEMP_DRY = os.path.join(ROOT, "Temp", "menu_dry.wav")  # Temporary file before the echo (Temp is ignored by git)
TEMP_WET = os.path.join(ROOT, "Temp", "menu_wet.wav")  # Temporary file after the echo
TEMP_LOOP = os.path.join(ROOT, "Temp", "menu_loop.wav")  # Temporary file of the finished loop

RATE = 44100  # Samples per second
BPM = 96  # Beats per minute: calmer than Quest's 132
BEAT = 60.0 / BPM  # Seconds per beat (0.625)
BAR = 4 * BEAT  # Seconds per bar of four beats (2.5)
BARS = 12  # Bars in the loop
LENGTH = BARS * BAR  # Loop length in seconds (30)
TAIL = 4.0  # Extra seconds rendered after the loop for echoes and note endings
N = int(round((LENGTH + TAIL) * RATE))  # Total samples rendered
LOOP_N = int(round(LENGTH * RATE))  # Samples in the finished loop

# Chords for each bar, as MIDI note numbers (60 = middle C). Am, F, C, G, three times.
CHORDS = [
    (45, [57, 60, 64]),  # Am: bass A2; pad A3 C4 E4
    (41, [57, 60, 65]),  # F: bass F2; pad A3 C4 F4
    (48, [55, 60, 64]),  # C: bass C3; pad G3 C4 E4
    (43, [55, 59, 62]),  # G: bass G2; pad G3 B3 D4
] * 3  # The four chords repeat three times

# Melody for bars 5 to 12, as (MIDI note, start beat in the bar, length in beats). Bars 1 to 4 have no melody.
MELODY = {
    4: [(76, 0, 2), (74, 2, 1), (72, 3, 1)],  # Bar 5 (Am): E D C
    5: [(69, 0, 3), (72, 3, 1)],  # Bar 6 (F): A, then C
    6: [(67, 0, 2), (76, 2, 2)],  # Bar 7 (C): G, then E
    7: [(74, 0, 4)],  # Bar 8 (G): a long D
    8: [(76, 0, 1), (79, 1, 1), (76, 2, 1), (74, 3, 1)],  # Bar 9 (Am): E G E D
    9: [(72, 0, 2), (69, 2, 2)],  # Bar 10 (F): C, then A
    10: [(67, 0, 1), (72, 1, 1), (76, 2, 2)],  # Bar 11 (C): G C E
    11: [(74, 0, 2), (71, 2, 2)],  # Bar 12 (G): D, then B, leading back to the start
}

ARP_PATTERN = [0, 2, 3, 4, 3, 2, 1, 2]  # Which chord tone each eighth note plays (low to high and back)

left = [0.0] * N  # Left speaker samples
right = [0.0] * N  # Right speaker samples


def freq(midi):
    """Turns a MIDI note number into a frequency in hertz."""
    return 440.0 * 2 ** ((midi - 69) / 12.0)  # A4 (69) is 440 Hz; each step is a semitone


def add_note(start, length, f, voice, gain, pan):
    """Adds one note to the mix. start and length are in seconds; pan is -1 (left) to 1 (right)."""
    lg = gain * math.cos((pan + 1) * math.pi / 4)  # Left loudness for this pan position
    rg = gain * math.sin((pan + 1) * math.pi / 4)  # Right loudness for this pan position
    s0 = int(start * RATE)  # First sample of the note
    s1 = min(N, int((start + length + voice["release"]) * RATE))  # Last sample, including the fade
    w = 2 * math.pi * f / RATE  # How far the wave turns each sample
    wobble = 0.004 * f / 5.0 if voice.get("vibrato") else 0.0  # Size of the flute's pitch wobble (0.4% at 5 times a second)
    for i in range(s0, s1):  # Each sample of the note
        t = (i - s0) / RATE  # Seconds since the note started
        env = voice["env"](t, length)  # Loudness shape at this moment
        if env <= 0.00001:  # Skip silent samples
            continue
        phase = w * (i - s0)  # Wave position
        if wobble:  # Only the flute wobbles
            phase += wobble * math.sin(2 * math.pi * 5.0 * t) * min(1.0, t / 0.3)  # Wobble grows in over 0.3 s
        v = sum(a * math.sin(phase * k) for k, a in voice["partials"]) * env  # Sum of the note's overtones
        left[i] += v * lg  # Add to the left speaker
        right[i] += v * rg  # Add to the right speaker


def pad_env(t, length):
    """Slow swell in and out, for the pad."""
    attack, release = 0.5, 0.8  # Seconds to swell in and to fade out
    if t < attack:  # Swelling in
        return t / attack
    if t < length:  # Holding
        return 1.0
    return max(0.0, 1.0 - (t - length) / release)  # Fading out


def pluck_env(t, length):
    """Quick start and a soft ring-out, for the music-box arpeggio."""
    return min(1.0, t / 0.004) * math.exp(-t * 4.0)  # 4 ms start, then a fading ring


def bass_env(t, length):
    """Gentle start and a slow fall, for the bass."""
    hold = min(1.0, t / 0.04) * (0.55 + 0.45 * math.exp(-t * 2.0))  # Soft start, settling to a lower level
    if t < length:  # While the bar lasts
        return hold
    return max(0.0, hold * (1.0 - (t - length) / 0.3))  # Fade out over 0.3 s


def flute_env(t, length):
    """Breathy start and a short fade, for the melody."""
    attack, release = 0.09, 0.25  # Seconds to start and to fade
    if t < attack:  # Starting
        return t / attack
    if t < length:  # Holding, with a slight natural drop
        return 1.0 - 0.15 * min(1.0, (t - attack) / 1.5)
    return max(0.0, 0.85 * (1.0 - (t - length) / release))  # Fading out


PAD = {"env": pad_env, "release": 0.8, "partials": [(1, 1.0), (2, 0.3), (3, 0.12), (4, 0.05)]}  # Warm, soft pad
PLUCK = {"env": pluck_env, "release": 1.2, "partials": [(1, 1.0), (2, 0.18), (4, 0.05)]}  # Music-box pluck
BASS = {"env": bass_env, "release": 0.3, "partials": [(1, 1.0), (2, 0.25)]}  # Round low bass
FLUTE = {"env": flute_env, "release": 0.25, "partials": [(1, 1.0), (2, 0.08), (3, 0.1)], "vibrato": True}  # Soft flute


def compose():
    """Places every note of the 12 bars."""
    for bar, (bass, pad) in enumerate(CHORDS):  # Each bar and its chord
        t0 = bar * BAR  # Bar start time in seconds
        add_note(t0, BAR, freq(bass), BASS, 0.30, 0.0)  # Bass note for the whole bar, in the middle
        for k, note in enumerate(pad):  # Each pad note
            pan = (-0.5, 0.0, 0.5)[k]  # Spread the three notes left, middle, right
            add_note(t0, BAR, freq(note) * 0.9985, PAD, 0.055, pan)  # Pad note, a hair flat
            add_note(t0, BAR, freq(note) * 1.0015, PAD, 0.055, -pan)  # Same note a hair sharp, other side, for width
        tones = sorted(pad) + [pad[0] + 12, pad[1] + 12]  # Chord tones for the arpeggio, low to high
        for step, index in enumerate(ARP_PATTERN):  # Eight eighth notes per bar
            note = tones[index] + 12  # One octave above the pad
            gain = 0.16 if step % 2 == 0 else 0.11  # On-beats a little louder
            pan = 0.35 if step % 2 == 0 else -0.35  # Bounce between left and right
            add_note(t0 + step * BEAT / 2, BEAT / 2, freq(note), PLUCK, gain, pan)  # One music-box note
        for note, beat, beats in MELODY.get(bar, []):  # Melody notes in this bar, if any
            add_note(t0 + beat * BEAT, beats * BEAT * 0.92, freq(note), FLUTE, 0.20, 0.1)  # Slightly short, so notes breathe


def write_wav(path, l, r):
    """Writes a 16-bit stereo WAV file."""
    os.makedirs(os.path.dirname(path), exist_ok=True)  # Make the folder if it is missing
    with wave.open(path, "wb") as w:  # Open the file
        w.setnchannels(2)  # Stereo
        w.setsampwidth(2)  # 16-bit
        w.setframerate(RATE)  # 44.1 kHz
        frames = bytearray()  # The sound data
        for a, b in zip(l, r):  # Each left and right sample
            frames += struct.pack("<hh", int(max(-1.0, min(1.0, a)) * 32767), int(max(-1.0, min(1.0, b)) * 32767))  # Clip, then 16-bit
        w.writeframes(bytes(frames))  # Write it


def read_wav(path):
    """Reads a 16-bit stereo WAV file into two lists from -1 to 1."""
    with wave.open(path, "rb") as w:  # Open the file
        data = w.readframes(w.getnframes())  # All the sound data
    values = struct.unpack("<%dh" % (len(data) // 2), data)  # Bytes to numbers
    return [v / 32767 for v in values[0::2]], [v / 32767 for v in values[1::2]]  # Split into left and right


def main():
    """Composes, adds echo, folds the tail into a seamless loop, and saves the OGG file."""
    compose()  # Place every note
    peak = max(max(abs(v) for v in left), max(abs(v) for v in right))  # Loudest sample so far
    scale = 0.5 / peak  # Leave room for the echo
    write_wav(TEMP_DRY, [v * scale for v in left], [v * scale for v in right])  # Save the dry mix

    echo = "aecho=0.8:0.55:47|89|137|211:0.32|0.24|0.17|0.11,lowpass=f=7500,highpass=f=40"  # Soft room echo, gentle top, no rumble
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", TEMP_DRY, "-af", echo, TEMP_WET], check=True)  # Add the echo

    l, r = read_wav(TEMP_WET)  # Read the echoed mix back
    loop_l = l[:LOOP_N]  # Left: the 30-second loop
    loop_r = r[:LOOP_N]  # Right: the 30-second loop
    for i in range(LOOP_N, min(len(l), N)):  # Each sample after the loop end
        loop_l[i - LOOP_N] += l[i]  # Fold the left tail onto the start, so the loop joins smoothly
        loop_r[i - LOOP_N] += r[i]  # Fold the right tail onto the start
    peak = max(max(abs(v) for v in loop_l), max(abs(v) for v in loop_r))  # Loudest sample of the loop
    gain = 0.89 / peak  # Make the loudest sample about -1 dB
    write_wav(TEMP_LOOP, [v * gain for v in loop_l], [v * gain for v in loop_r])  # Save the finished loop

    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", TEMP_LOOP, "-c:a", "libvorbis", "-q:a", "5", OUT_OGG], check=True)  # Save as OGG
    for path in (TEMP_DRY, TEMP_WET, TEMP_LOOP):  # Remove the temporary files
        os.remove(path)
    print("Menu music written to", OUT_OGG)  # Tell the person running it where the file went


if __name__ == "__main__":
    main()  # Run when the file is started directly
