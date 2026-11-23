"""Makes the Plateaus Normal sounds for LandGrand (D-073).  # Says what this script is for

Run from the Unity project folder:  python Tools/Audio/make_plateaus_normal_audio.py   (needs ffmpeg and Pillow)  # How to run it
It writes two files to Assets/Audio/:  # What it makes
  sfx_cable_car.wav   A soft metal-and-wood clunk with a short cable creak (about 0.5 s), when the explorer lands on a cable car.  # Effect one
  sfx_llama.wav       A soft, gentle llama hum that rises a little, with two light hoof taps (about 0.6 s), when a llama nudges the explorer.  # Effect two
Both use fixed random seeds, so each run makes the same files.  # Repeatable
"""  # End of the description
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
from tempfile import TemporaryDirectory  # Holds temporary WAV files
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares soft noise, effect writer and measuring
import make_plains_normal_audio as pn  # Shares the unit scaler and the review sheet

RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "plateaus-normal" / "audio-preview.png"  # Holds the review graph
CAR_SECONDS = 0.5  # Length of the cable car clunk
LLAMA_SECONDS = 0.6  # Length of the llama hum
CAR_DB = -26.0  # Mean level: the same as sfx_boat_bump (-26 dB)
LLAMA_DB = -26.0  # Mean level: the same as sfx_herd_hooves (-26 dB, measured)


def cable_car():  # Makes a soft metal-and-wood clunk and a short cable creak after it
    """Makes a soft metal-and-wood clunk and a short cable creak after it."""  # Describes the function
    count = int(CAR_SECONDS * RATE)  # Number of samples in the sound
    knock = pn.unit(list(hn.soft_noise(CAR_SECONDS, 2001, 200, 1800)))  # Soft knock noise, at RMS one
    scrape = pn.unit(list(hn.soft_noise(CAR_SECONDS, 2003, 700, 2600)))  # Rough noise for the cable creak, at RMS one
    phase = 0.0  # Creak wave turn, added up so the pitch stays smooth
    out = []  # Holds the sound
    for i in range(count):  # Each sample of the sound
        t = i / RATE  # Age of the sound in seconds
        attack = min(1.0, t / 0.004)  # Soft start over 4 ms, so there is no sharp click
        end = min(1.0, (CAR_SECONDS - t) / 0.06)  # Fades the last 60 ms to zero, so the sound ends quietly
        body = math.sin(math.tau * (95 + 30 * math.exp(-t * 40)) * t) * attack * math.exp(-t * 14)  # Low wood body that falls a little in pitch
        clang = (math.sin(math.tau * 540 * t) + 0.6 * math.sin(math.tau * 877 * t) + 0.35 * math.sin(math.tau * 1330 * t)) * attack * math.exp(-t * 22) * 0.16  # Short, soft metal ring with uneven tones
        tap = knock[i] * attack * math.exp(-t * 60) * 0.3  # A very short, soft tap of the wood
        age = t - 0.13  # Time since the creak started
        swell = math.sin(math.pi * min(1.0, max(0.0, age / 0.3))) ** 2 if age > 0 else 0.0  # The creak rises and falls over 0.3 s
        f = 260 + 70 * math.sin(math.tau * 7 * max(age, 0.0)) + 60 * age  # Creak pitch wobbles and drifts up a little
        phase += math.tau * f / RATE  # Turns the creak wave smoothly
        saw = (phase / math.pi) % 2 - 1  # A rough saw wave, like a rubbing rope
        creak = (saw * 0.5 + scrape[i] * 0.5) * swell * (0.6 + 0.4 * math.sin(math.tau * 23 * max(age, 0.0))) * 0.07  # Rough, flickering creak
        out.append((body * 0.8 + clang + tap + creak) * end)  # Mixes the parts and fades the end
    return out  # Gives back the sound


def llama():  # Makes a low nasal hum that rises a little, with two light hoof taps
    """Makes a low nasal hum that rises a little, with two light hoof taps."""  # Describes the function
    count = int(LLAMA_SECONDS * RATE)  # Number of samples in the sound
    tapnoise = [pn.unit(list(hn.soft_noise(0.06, seed, 150, 1200))) for seed in (2011, 2017)]  # Two soft grains for the hoof taps
    phase = 0.0  # Hum wave turn, added up so the pitch stays smooth
    out = []  # Holds the sound
    for i in range(count):  # Each sample of the sound
        t = i / RATE  # Age of the sound in seconds
        start = min(1.0, max(0.0, (t - 0.05) / 0.09))  # The hum fades in from 0.05 s over 90 ms
        end = min(1.0, (LLAMA_SECONDS - t) / 0.15)  # Fades the last 150 ms to zero, so the hum ends softly
        rise = min(1.0, max(0.0, (t - 0.05) / 0.45))  # How far the hum has risen, 0 to 1
        f0 = 170 + 25 * rise + 3 * math.sin(math.tau * 5.5 * t)  # A low hum that rises a little, with a tiny shake
        phase += math.tau * f0 / RATE  # Turns the hum wave smoothly
        voice = 0.0  # Holds the hum voice
        for h in range(1, 13):  # Adds twelve overtones of the hum
            hz = f0 * h  # Pitch of this overtone
            nasal = math.exp(-((hz - 1000) / 350) ** 2) + 0.7 * math.exp(-((hz - 2300) / 500) ** 2) + 0.35 * math.exp(-((hz - 400) / 200) ** 2)  # Boosts nasal bands near 400, 1000 and 2300 Hz
            voice += math.sin(h * phase) * nasal / h ** 0.5  # Adds the overtone, with a nasal tone shape
        hum = voice * start * end * 0.16  # The finished hum
        taps = 0.0  # Holds the hoof taps at this moment
        for k, at in enumerate((0.06, 0.26)):  # Two light taps
            a = t - at  # Time since this tap
            if 0 <= a < 0.06:  # Only while the tap sounds
                grain = tapnoise[k][int(a * RATE)] * 0.5 + math.sin(math.tau * 190 * a) * 0.8  # Dull thud with a little noise
                taps += grain * min(1.0, a / 0.003) * math.exp(-a * 70) * 0.4  # Soft start and quick fade
        out.append(hum + taps)  # Mixes the hum and the taps
    return out  # Gives back the sound


def main():  # Writes, measures and previews both sounds
    """Writes, measures and previews both sounds."""  # Describes the function
    paths = [hn.make_effect("sfx_cable_car.wav", cable_car(), CAR_DB), hn.make_effect("sfx_llama.wav", llama(), LLAMA_DB)]  # Makes both sounds at their target levels
    pn.PREVIEW = PREVIEW  # Points the shared review sheet at the Plateaus Normal folder
    with TemporaryDirectory(prefix="landgrand-plateaus-normal-") as folder:  # Keeps the project clear of temporary files
        pn.preview(paths, Path(folder))  # Writes the graph from the final assets
    for path in paths:  # Reports the mean of every file
        print(f"{path.name}: mean {hn.measure(path)[1]:.1f} dB")  # Prints the mean again as a check
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
