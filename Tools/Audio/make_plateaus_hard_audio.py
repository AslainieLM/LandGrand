"""Makes the Plateaus Hard sounds for LandGrand (D-074).  # Says what this script is for

Run from the Unity project folder:  python Tools/Audio/make_plateaus_hard_audio.py   (needs ffmpeg and Pillow)  # How to run it
It writes two new files to Assets/Audio/:  # What it makes
  sfx_gelada.wav       A short, friendly gelada monkey chatter of soft grunt-chirps (about 0.7 s), when a gelada troop nudges the explorer.  # Effect one
  sfx_gust_strong.wav  A stronger wind gust warning with a low roar and a high whistle (about 0.9 s), for snow and sand gusts.  # Effect two
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
PREVIEW = AUDIO.parents[1] / ".scratch" / "plateaus-hard" / "audio-preview.png"  # Holds the review graph
GELADA_SECONDS = 0.7  # Length of the gelada chatter
GUST_SECONDS = 0.9  # Length of the strong gust: the same as sfx_gust (0.9 s)
GELADA_DB = -26.0  # Mean level: the same as sfx_llama and sfx_herd_hooves (-26 dB, measured)
GUST_DB = -17.0  # Mean level: about 2 dB fuller than sfx_gust (-18.8 dB, measured)
GUST_PEAK_DB = -2.6  # Peak cap: sfx_gust peaks at -4.4 dB, so this stays within about 2 dB of it
NEW_FILES = ("sfx_gelada.wav", "sfx_gust_strong.wav")  # The only files this script may write


def gelada():  # Makes a few soft rising-falling grunt-chirps, like a calm monkey chatter
    """Makes a few soft rising-falling grunt-chirps, like a calm monkey chatter."""  # Describes the function
    count = int(GELADA_SECONDS * RATE)  # Number of samples in the sound
    breath = pn.unit(list(hn.soft_noise(GELADA_SECONDS, 3001, 300, 2500)))  # Soft breath noise, at RMS one
    calls = ((0.02, 0.13, 230, 1.0), (0.17, 0.11, 290, 0.8), (0.31, 0.15, 250, 0.95), (0.50, 0.12, 320, 0.7))  # Start, length, base pitch and loudness of each chirp
    out = [0.0] * count  # Holds the sound
    for start, length, base, loud in calls:  # Each grunt-chirp in turn
        phase = 0.0  # Voice wave turn, added up so the pitch stays smooth
        for j in range(int(length * RATE)):  # Each sample of this chirp
            i = int(start * RATE) + j  # Place of this sample in the whole sound
            if i >= count:  # Stops at the end of the sound
                break  # Leaves this chirp
            u = j / (length * RATE)  # How far through the chirp, 0 to 1
            env = math.sin(math.pi * u) ** 1.5  # Soft rise and fall, with no click at either end
            f0 = base * (1 + 0.45 * math.sin(math.pi * u))  # Pitch goes up then down again, like a friendly chirp
            phase += math.tau * f0 / RATE  # Turns the voice wave smoothly
            vowel = 0.5 + 0.5 * math.sin(math.pi * u)  # Mouth opens in the middle of the chirp
            f1, f2 = 500 + 350 * vowel, 1300 + 500 * vowel  # Two mouth bands that move from "oo" to "ah"
            voice = 0.0  # Holds the voice at this moment
            for h in range(1, 11):  # Adds ten overtones of the voice
                hz = f0 * h  # Pitch of this overtone
                shape = math.exp(-((hz - f1) / 220) ** 2) + 0.6 * math.exp(-((hz - f2) / 380) ** 2) + 0.25 / h  # Boosts the two mouth bands, keeps a soft base
                voice += math.sin(h * phase) * shape  # Adds the overtone with its mouth shape
            out[i] += (voice + breath[i] * 0.12) * env * loud  # Mixes the voice and a little breath for this chirp
    end = int(0.05 * RATE)  # Last 50 ms fade to zero
    for k in range(end):  # Each sample of the end fade
        out[count - 1 - k] *= k / end  # Makes the very end silent
    return out  # Gives back the sound


def gust_strong():  # Makes a strong gust: a full low roar and a whistling high band, with a quick swell and fade
    """Makes a strong gust: a full low roar and a whistling high band, with a quick swell and fade."""  # Describes the function
    count = int(GUST_SECONDS * RATE)  # Number of samples in the sound
    roar = pn.unit(list(hn.soft_noise(GUST_SECONDS, 3011, 45, 420)))  # Low rumbling roar, at RMS one
    rush = pn.unit(list(hn.soft_noise(GUST_SECONDS, 3013, 400, 2400)))  # Middle air rush, at RMS one
    hiss = pn.unit(list(hn.soft_noise(GUST_SECONDS, 3017, 1800, 5200)))  # High hiss under the whistle, at RMS one
    phase = 0.0  # Whistle wave turn, added up so the pitch stays smooth
    out = []  # Holds the sound
    for i in range(count):  # Each sample of the sound
        t = i / RATE  # Age of the sound in seconds
        if t < 0.28:  # The quick swell part
            env = math.sin(0.5 * math.pi * t / 0.28) ** 2  # Swells up fast, in 0.28 s
        else:  # The fade part
            env = math.cos(0.5 * math.pi * (t - 0.28) / (GUST_SECONDS - 0.28)) ** 2  # Fades smoothly to zero at the end
        flutter = 0.85 + 0.15 * math.sin(math.tau * 9 * t)  # The wind shakes a little
        f = 1150 + 350 * env + 60 * math.sin(math.tau * 5 * t)  # Whistle pitch rises with the gust and wobbles a little
        phase += math.tau * f / RATE  # Turns the whistle wave smoothly
        whistle = math.sin(phase) * (0.6 + 0.4 * hiss[i] * 0.5) * env ** 1.5 * 0.8  # A breathy whistle that is loudest at the top of the gust
        body = roar[i] * 1.0 + rush[i] * 0.55 * env + hiss[i] * 0.12 * env  # Full roar with the rush and hiss rising in the gust
        out.append((body * flutter + whistle) * env)  # Mixes the parts and shapes them with the gust
    return out  # Gives back the sound


def capped_target(samples, mean_db, peak_db):  # Lowers the mean target if needed so the peak stays under the cap
    """Lowers the mean target if needed so the peak stays under the cap."""  # Describes the function
    rms = math.sqrt(sum(v * v for v in samples) / len(samples))  # Measures the present RMS
    peak = max(map(abs, samples))  # Measures the present peak
    return min(mean_db, peak_db - 20 * math.log10(peak / rms))  # The mean that puts the peak right at the cap


def main():  # Writes, measures and previews both sounds
    """Writes, measures and previews both sounds."""  # Describes the function
    for name in NEW_FILES:  # Checks every name before writing anything
        if (AUDIO / name).exists():  # A file with this name is already there
            raise SystemExit(f"{name} already exists in Assets/Audio; remove it first if you mean to remake it.")  # Stops so no asset is overwritten by mistake
    gust = gust_strong()  # Makes the strong gust
    paths = [hn.make_effect("sfx_gelada.wav", gelada(), GELADA_DB), hn.make_effect("sfx_gust_strong.wav", gust, capped_target(gust, GUST_DB, GUST_PEAK_DB))]  # Makes both sounds at their target levels
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the preview folder if it is missing
    pn.PREVIEW = PREVIEW  # Points the shared review sheet at the Plateaus Hard folder
    with TemporaryDirectory(prefix="landgrand-plateaus-hard-") as folder:  # Keeps the project clear of temporary files
        pn.preview(paths, Path(folder))  # Writes the graph from the final assets
    for path in paths:  # Reports the mean of every file
        print(f"{path.name}: mean {hn.measure(path)[1]:.1f} dB")  # Prints the mean again as a check
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
