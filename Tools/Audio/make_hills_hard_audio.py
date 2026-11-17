"""Make Wind Turbines, City Hills, Mud Squelch and Log Roll for Hills Hard."""  # Describes the new audio
import math  # Gives the sound envelopes and waves
from pathlib import Path  # Finds the asset and preview folders
import random  # Gives repeatable noise
from tempfile import TemporaryDirectory  # Holds temporary WAV files
import make_mountains_audio as ma  # Shares the project paths, sample rate and WAV helpers
import make_hills_normal_audio as hn  # Shares ffmpeg, soft noise, levels, effects and the preview sheet

RATE = ma.RATE  # Keeps the usual 44.1 kHz sample rate
AUDIO = Path(ma.AUDIO)  # Holds the finished sounds
PREVIEW = AUDIO.parents[1] / ".scratch" / "hills-hard" / "audio-preview.png"  # Holds the review graph
SECONDS = 24.0  # The loop length
LOOP_SAMPLES = int(SECONDS * RATE)  # The number of samples in the loop
FOLD_SAMPLES = RATE  # Blends one extra second into the start


def noise(seconds, seed, low, high):  # Makes band noise with an RMS of one
    data = list(hn.soft_noise(seconds, seed, low, high))  # Takes noise between the two cutoffs
    return unit(data)  # Sets it to a known level


def unit(data):  # Scales a sound so its RMS is one
    rms = math.sqrt(sum(v * v for v in data) / len(data)) or 1.0  # Measures the present level
    return [v / rms for v in data]  # Gives back the scaled copy


def fold(data):  # Folds the extra second onto the start so the loop joins
    for i in range(FOLD_SAMPLES):  # Walks over the joined second
        weight = 0.5 - 0.5 * math.cos(math.pi * i / FOLD_SAMPLES)  # Moves smoothly from end to start
        data[i] = data[LOOP_SAMPLES + i] * (1 - weight) + data[i] * weight  # Carries the end through the boundary
    return data[:LOOP_SAMPLES]  # Keeps exactly one loop


def resonator(data, hz, decay):  # Rings a sound through one tuned, damped body
    radius = math.exp(-decay / RATE)  # Sets how fast the ring fades
    turn = 2 * radius * math.cos(math.tau * hz / RATE)  # Sets the ring pitch
    square = radius * radius  # Holds the second filter weight
    gain = 1 - radius  # Keeps the ring near the input level
    out, one, two = [], 0.0, 0.0  # Starts the filter quietly
    for x in data:  # Filters each sample
        y = gain * x + turn * one - square * two  # Adds the input to the ringing body
        two, one = one, y  # Moves the filter memory on
        out.append(y)  # Keeps the result
    return out  # Gives back the ringing sound


def turbine_side(seed, near, far):  # Makes one speaker of wind with two turbines
    total = int((SECONDS + 1) * RATE)  # Renders one extra second for the fold
    body = noise(SECONDS + 1, seed, 70, 650)  # Makes the main wind roar
    air = noise(SECONDS + 1, seed + 1, 300, 1500)  # Adds a thin layer of moving air
    swish = noise(SECONDS + 1, seed + 2, 140, 520)  # Gives the blades their whoosh
    far_swish = noise(SECONDS + 1, seed + 3, 110, 380)  # Gives the far blades a duller whoosh
    out = []  # Holds this speaker
    for i in range(total):  # Mixes each sample
        t = i / RATE  # Finds the time in seconds
        gust = 0.66 + 0.16 * math.sin(math.tau * 3 * t / SECONDS) + 0.11 * math.sin(math.tau * 7 * t / SECONDS + 0.8) + 0.07 * math.sin(math.tau * 11 * t / SECONDS + 2.1)  # Swells the wind in whole loop cycles
        blade = (0.5 - 0.5 * math.cos(math.tau * 15 * t / SECONDS)) ** 3  # Swells the near blades 0.625 times a second
        blade2 = (0.5 - 0.5 * math.cos(math.tau * 13 * t / SECONDS + 1.3)) ** 3  # Swells the far blades 0.54 times a second
        hum = math.sin(math.tau * 46 * t) + 0.55 * math.sin(math.tau * 92 * t + 0.4) + 0.3 * math.sin(math.tau * 138 * t + 1.1)  # Makes a low machine hum with whole loop cycles
        wind = body[i] * gust + air[i] * 0.22 * gust * gust  # Lets the gusts lift the air layer more
        turn = swish[i] * blade * 1.1 * near + far_swish[i] * blade2 * 0.6 * far  # Adds both slow blade whooshes
        out.append(wind + turn + hum * 0.18 * (0.75 + 0.25 * blade) * near)  # Lets the hum rise as each blade passes
    return fold(out)  # Joins the loop


def bell(hz, loud):  # Makes one far church bell strike
    out = []  # Holds the ringing bell
    parts = ((0.5, 0.55, 0.45), (1.0, 1.0, 0.7), (1.19, 0.45, 0.95), (1.5, 0.28, 1.2), (2.0, 0.32, 1.5), (2.52, 0.12, 2.1), (3.0, 0.07, 2.7))  # Lists bell partials as ratio, level and fade
    for i in range(int(4.5 * RATE)):  # Lets the bell ring for four and a half seconds
        t = i / RATE  # Finds the age of the strike
        strike = 1 - math.exp(-t * 900)  # Softens the clapper hit
        out.append(loud * strike * sum(level * math.sin(math.tau * hz * ratio * t) * math.exp(-t * fade) for ratio, level, fade in parts))  # Adds the partials
    return out  # Gives back the strike


def murmur_voice(rng, left, right, pan, loud):  # Adds one distant talking voice
    pitch = rng.uniform(105, 230)  # Gives the voice its own pitch
    at = rng.uniform(0, SECONDS)  # Starts the voice at any point in the loop
    end = at + SECONDS * rng.uniform(0.55, 0.85)  # Lets the voice talk for most of the loop
    while at < end:  # Adds phrases until the voice stops
        words = rng.randint(3, 9)  # Chooses a phrase length
        for _ in range(words):  # Adds each syllable
            size = rng.uniform(0.11, 0.27)  # Sets the syllable length
            vowel = rng.uniform(420, 980)  # Picks a vowel colour
            phase, low = 0.0, 0.0  # Starts the voice wave and its soft filter
            sound = []  # Holds the syllable
            for k in range(int(size * RATE)):  # Makes each sample of the syllable
                x = k / (size * RATE)  # Goes from zero to one through the syllable
                phase += pitch * (1 + 0.08 * math.sin(math.pi * x) - 0.05 * x) / RATE  # Bends the pitch a little
                saw = 2 * (phase % 1) - 1  # Makes a buzzy voice wave
                low += 0.12 * (saw - low)  # Rounds off the buzz for distance
                sound.append(low * math.sin(math.pi * x) ** 2)  # Shapes a soft syllable
            sound = resonator(sound, vowel, 260)  # Colours it like a vowel
            start = round(at * RATE)  # Finds where the syllable lands
            for k, v in enumerate(sound):  # Mixes the syllable into the loop
                pos = (start + k) % LOOP_SAMPLES  # Wraps across the loop end
                left[pos] += v * loud * (1 - pan)  # Places it toward one side
                right[pos] += v * loud * pan  # Places the rest on the other side
            at += size + rng.uniform(0.02, 0.12)  # Leaves a short gap between syllables
        at += rng.uniform(0.6, 2.4)  # Pauses between phrases


def bird(rng, left, right, at):  # Adds one small bird call
    notes = rng.randint(2, 4)  # Picks how many chirps
    base = rng.uniform(2700, 3900)  # Gives this bird its pitch
    gap = rng.uniform(0.1, 0.16)  # Spaces the chirps
    pan = rng.uniform(0.25, 0.75)  # Places the bird
    for n in range(notes):  # Adds each chirp
        length = int(rng.uniform(0.05, 0.08) * RATE)  # Sets the chirp length
        sweep = rng.choice((1, -1)) * rng.uniform(0.15, 0.3)  # Bends the whistle up or down
        phase, start = 0.0, round((at + n * gap) * RATE)  # Starts the chirp
        for k in range(length):  # Makes each sample
            x = k / length  # Goes from zero to one through the chirp
            phase += math.tau * base * (1 + sweep * math.sin(math.pi * x)) / RATE  # Bends the whistle
            v = math.sin(phase) * math.sin(math.pi * x) ** 2 * 0.05  # Shapes a soft chirp
            pos = (start + k) % LOOP_SAMPLES  # Wraps across the loop end
            left[pos] += v * (1 - pan)  # Places it on the left
            right[pos] += v * pan  # Places it on the right


def city_mix():  # Mixes bells, crowd, birds and breeze for the hill town
    sides = []  # Holds the folded breeze and murmur beds
    for seed in (801, 811):  # Makes each speaker from its own noise
        breeze = noise(SECONDS + 1, seed, 60, 600)  # Makes a light breeze
        crowd = noise(SECONDS + 1, seed + 1, 220, 900)  # Makes the far blur of many voices
        bed = []  # Holds this speaker bed
        for i in range(len(breeze)):  # Mixes each sample
            t = i / RATE  # Finds the time in seconds
            sway = 0.75 + 0.25 * math.sin(math.tau * 2 * t / SECONDS + seed)  # Moves the breeze slowly
            busy = 0.8 + 0.2 * math.sin(math.tau * 5 * t / SECONDS + seed * 0.3)  # Lets the crowd come and go
            bed.append(breeze[i] * 0.05 * sway + crowd[i] * 0.022 * busy)  # Keeps both quiet under the bells
        sides.append(fold(bed))  # Joins the loop
    left, right = sides  # Names the two speakers
    rng = random.Random(821)  # Repeats the same voices and birds
    for _ in range(9):  # Adds nine far voices
        murmur_voice(rng, left, right, rng.uniform(0.2, 0.8), 0.05)  # Places each voice in the square
    for at in (2.3, 7.9, 13.4, 19.6, 22.4):  # Spaces five bird calls through the loop
        bird(rng, left, right, at)  # Adds one call
    for at, hz, loud in ((1.2, 392, 0.095), (2.9, 330, 0.085), (4.6, 392, 0.075), (14.8, 294, 0.07)):  # Lists a few gentle bell strikes
        strike = bell(hz, loud)  # Makes the strike
        for delay, gain, pan in ((0, 1, 0.38), (0.14, 0.32, 0.62), (0.31, 0.16, 0.45)):  # Adds soft town echoes
            start = round((at + delay) * RATE)  # Finds where this copy lands
            for k, v in enumerate(strike):  # Mixes the whole ring
                pos = (start + k) % LOOP_SAMPLES  # Wraps across the loop end
                left[pos] += v * gain * (1 - pan)  # Places the bell left of center
                right[pos] += v * gain * pan  # Gives the right side its share
    return [left, right]  # Gives back both speakers


def make_loop(temp, name, channels, band, target):  # Filters, joins and encodes one seamless loop
    raw = temp / f"{name}-padded.wav"  # Receives the loop with a little wrap on both sides
    wet = temp / f"{name}-filtered.wav"  # Receives the trimmed, filtered loop
    pad = RATE // 2  # Lets the filters settle on real loop samples
    ma.mm.write_wav(str(raw), *(c[-pad:] + c + c[:pad] for c in (unit(c) for c in channels)))  # Wraps half a second around each side
    hn.ffmpeg("-i", raw, "-af", f"volume=0.1,{band},atrim=start_sample={pad}:end_sample={pad + LOOP_SAMPLES}", wet)  # Filters and trims after settling
    loops = ma.mm.read_wav(str(wet))  # Reads the filtered loop back
    for channel in loops:  # Joins each speaker at the seam
        shift = (channel[-1] - channel[0]) / 2  # Shares any small edge step between the ends
        for i in range(RATE // 100):  # Blends the fix over ten milliseconds
            weight = 0.5 + 0.5 * math.cos(math.pi * i / (RATE // 100))  # Fades the fix to zero
            channel[i] += shift * weight  # Moves the start toward the end
            channel[-1 - i] -= shift * weight  # Moves the end toward the start
    gain = hn.level_gain(loops, target)  # Finds the gain for the target mean
    ma.mm.write_wav(str(wet), *([v * gain for v in c] for c in loops))  # Applies the gain
    path = AUDIO / f"{name}.ogg"  # Names the loop asset
    hn.ffmpeg("-i", wet, "-map_metadata", "-1", "-fflags", "+bitexact", "-c:a", "libvorbis", "-q:a", "4", path)  # Uses the Hills OGG settings
    return path  # Gives back the saved loop


def squelch():  # Makes a short wet slide in mud
    size = int(0.3 * RATE)  # Sets the length
    mud = noise(0.3, 831, 90, 700)  # Makes thick, low noise
    rng = random.Random(833)  # Repeats the same wet bits
    wobble = []  # Holds the sucking mud tone
    radius = math.exp(-120 / RATE)  # Gives the mud a wet ring
    one = two = 0.0  # Starts the filter memory
    for i in range(size):  # Makes each sample
        t = i / RATE  # Finds the age of the slide
        hz = 260 + 520 * math.sin(math.pi * min(1.0, t / 0.22)) ** 1.5  # Sweeps the wet tone up and back down
        turn = 2 * radius * math.cos(math.tau * hz / RATE)  # Sets the pitch of this moment
        y = (1 - radius) * mud[i] * 6 + turn * one - radius * radius * two  # Rings the mud through the moving tone
        two, one = one, y  # Moves the filter memory on
        env = (1 - math.exp(-t * 90)) * math.exp(-t * 7)  # Starts quickly and slides out
        wobble.append(y * env)  # Keeps the shaped sample
    for start in (0.06, 0.15, 0.21):  # Adds three small wet pops
        tone = rng.uniform(170, 260)  # Picks the pop pitch
        for k in range(int(0.05 * RATE)):  # Makes each pop sample
            age = k / RATE  # Finds the age of the pop
            pos = round(start * RATE) + k  # Finds where it lands
            if pos < size:  # Keeps it inside the clip
                wobble[pos] += 0.9 * math.sin(math.tau * (tone * age + 2200 * age * age)) * math.sin(math.pi * age / 0.05) ** 2  # Adds a rising bubble
    tail = int(0.02 * RATE)  # Sets a short fade at the end
    for k in range(tail):  # Fades the last samples
        wobble[-1 - k] *= k / tail  # Ends without a click
    return wobble  # Gives back the squelch


def log_roll():  # Makes a looping hollow wooden rumble with bumps
    size = int(0.8 * RATE)  # Sets the loop length
    grit = noise(0.8, 841, 40, 260)  # Makes a low rolling rumble
    knock = []  # Holds the wood excitation
    for i in range(size):  # Makes each sample
        t = i / RATE  # Finds the time in seconds
        turn = (0.5 - 0.5 * math.cos(math.tau * 5 * t / 0.8)) ** 6  # Bumps five times per loop as the log turns
        edge = (0.5 - 0.5 * math.cos(math.tau * 2 * t / 0.8 + 0.7)) ** 12  # Adds two harder knots each loop
        knock.append(grit[i] * (0.35 + 0.9 * turn + 1.4 * edge))  # Lets the bumps excite the wood more
    twice = knock + knock  # Repeats the loop so the rings settle
    body = [0.0] * (2 * size)  # Holds the hollow wooden ring
    for hz, decay, level in ((92, 28, 1.0), (176, 40, 0.8), (288, 55, 0.55), (455, 70, 0.45), (720, 95, 0.3)):  # Lists the hollow log tones
        for i, v in enumerate(resonator(twice, hz, decay)):  # Rings the bumps through each tone
            body[i] += v * level  # Adds this tone
    loop = body[size:]  # Keeps the settled second pass, which joins itself
    return [v + g * 0.08 for v, g in zip(loop, grit)]  # Adds a little rolling rumble on top


def main():  # Writes and measures all four Hills Hard sounds
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)  # Makes the preview folder
    hn.PREVIEW = PREVIEW  # Points the shared preview at the Hills Hard sheet
    with TemporaryDirectory(prefix="landgrand-hills-hard-") as folder:  # Keeps the project clear of temporary WAV files
        temp = Path(folder)  # Names the temporary audio folder
        wind = [turbine_side(851, 1.0, 0.6), turbine_side(861, 0.6, 1.0)]  # Puts the near turbine left and the far one right
        paths = [make_loop(temp, "amb_wind_turbines", wind, "highpass=f=35,lowpass=f=2200", -20)]  # Makes the windy turbine loop
        paths.append(make_loop(temp, "amb_city_hills", city_mix(), "highpass=f=55,lowpass=f=5200", -21))  # Makes the hill town loop
        paths.append(hn.make_effect("sfx_mud_squelch.wav", squelch(), -24))  # Makes the mud slide
        paths.append(hn.make_effect("sfx_log_roll.wav", log_roll(), -22))  # Makes the rolling log
        hn.preview(paths, temp)  # Writes the graphs from the final assets
    print(PREVIEW)  # Prints the preview path


if __name__ == "__main__":  # Runs only when started directly
    main()  # Makes the sounds
