using System; // Gives Enum and basic types
using System.Collections.Generic; // Gives Dictionary and HashSet
using System.Globalization; // Gives culture-free number text
using System.Linq; // Gives sorting helpers
using System.Text; // Gives StringBuilder

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>The four landforms of the game, in the order they unlock.</summary>
    public enum Episode { Mountains, Hills, Plains, Plateaus } // Landform list

    /// <summary>The three difficulties of each landform.</summary>
    public enum Difficulty { Easy, Normal, Hard } // Difficulty list

    /// <summary>
    /// Everything the game remembers about the player: name, cleared levels with best stars,
    /// and every item kind ever found. It also decides what is unlocked.
    /// </summary>
    public class PlayerProgress // Holds all saved player data
    {
        readonly Dictionary<(Episode, Difficulty, int), int> bestStars = new Dictionary<(Episode, Difficulty, int), int>(); // Best stars of each cleared level
        readonly HashSet<string> foundKinds = new HashSet<string>(); // Item kinds found so far
        string playerName = ""; // The player's name

        /// <summary>The player's name. Empty until the player types one.</summary>
        public string Name { get => playerName; set => playerName = value ?? ""; } // Never null

        /// <summary>How many levels a difficulty has: Easy 3, Normal 5, Hard 7.</summary>
        public static int LevelCount(Difficulty d) => d == Difficulty.Easy ? 3 : d == Difficulty.Normal ? 5 : 7; // Counts from the game design

        /// <summary>True when the landform can be played. Only Mountains for now (Q-03 is still open for the others).</summary>
        public bool IsEpisodeUnlocked(Episode e) => e == Episode.Mountains; // Mountains is open from the start

        /// <summary>True when the difficulty is open: Normal after all Easy levels, Hard after all Normal levels.</summary>
        public bool IsDifficultyUnlocked(Episode e, Difficulty d) // Checks the unlock chain
        {
            if (!IsEpisodeUnlocked(e)) return false; // A locked landform has nothing open
            if (d == Difficulty.Easy) return true; // Easy is always open
            return AllCleared(e, d == Difficulty.Normal ? Difficulty.Easy : Difficulty.Normal); // Needs the difficulty before it cleared
        }

        /// <summary>True when the level can be played: the first uncleared level and every earlier one.</summary>
        public bool IsLevelAvailable(Episode e, Difficulty d, int level) // Checks order inside a difficulty
        {
            if (level < 1 || level > LevelCount(d) || !IsDifficultyUnlocked(e, d)) return false; // Out of range or locked
            for (int n = 1; n < level; n++) if (!IsCleared(e, d, n)) return false; // Every earlier level must be cleared
            return true; // Nothing blocks it
        }

        /// <summary>True when the level was cleared at least once.</summary>
        public bool IsCleared(Episode e, Difficulty d, int level) => bestStars.ContainsKey((e, d, level)); // Only cleared levels are stored

        /// <summary>Best stars ever earned on the level, or 0 when it was never cleared.</summary>
        public int BestStars(Episode e, Difficulty d, int level) => bestStars.TryGetValue((e, d, level), out var s) ? s : 0; // 0 when missing

        /// <summary>Saves a clear. A worse replay never lowers the stars (D-045).</summary>
        public void RecordClear(Episode e, Difficulty d, int level, int stars) // Keeps the larger star count
        {
            stars = Math.Max(1, Math.Min(3, stars)); // A clear is always 1 to 3 stars
            bestStars[(e, d, level)] = Math.Max(stars, BestStars(e, d, level)); // Keep the best
        }

        /// <summary>Most stars any one landform can give: 3 stars for each of its 15 levels (45). Every landform has the same levels.</summary>
        public static int MaxEpisodeStars => (LevelCount(Difficulty.Easy) + LevelCount(Difficulty.Normal) + LevelCount(Difficulty.Hard)) * 3; // (3 + 5 + 7) levels, 3 stars each

        /// <summary>Most stars one difficulty can give: 3 stars per level (Easy 9, Normal 15, Hard 21).</summary>
        public static int MaxDifficultyStars(Difficulty d) => LevelCount(d) * 3; // Levels times 3 stars

        /// <summary>Best stars earned so far in one difficulty of one landform.</summary>
        public int DifficultyStars(Episode e, Difficulty d) => bestStars.Where(k => k.Key.Item1 == e && k.Key.Item2 == d).Sum(k => k.Value); // Only this card's levels

        /// <summary>Best stars earned so far in one landform, added over all its levels.</summary>
        public int EpisodeStars(Episode e) => bestStars.Where(k => k.Key.Item1 == e).Sum(k => k.Value); // Only this landform's levels

        /// <summary>True when this kind of rock or mineral was found before, in any session.</summary>
        public bool HasFoundKind(string kind) => foundKinds.Contains(kind); // Looks the kind up

        /// <summary>Remembers a found kind. Found kinds are never removed.</summary>
        public void RecordFoundKind(string kind) // Adds the kind
        {
            if (!string.IsNullOrEmpty(kind)) foundKinds.Add(kind); // Ignore empty names
        }

        bool AllCleared(Episode e, Difficulty d) // True when every level of the difficulty is cleared
        {
            for (int n = 1; n <= LevelCount(d); n++) if (!IsCleared(e, d, n)) return false; // One missing level is enough to fail
            return true; // All cleared
        }

        /// <summary>Writes the progress as JSON text. The order is fixed, so equal progress gives equal text.</summary>
        public string ToJson() // Builds the save text
        {
            var sb = new StringBuilder(); // Collects the text
            sb.Append("{\"name\":").Append(Quote(playerName)).Append(",\"levels\":["); // Name, then the level list
            sb.Append(string.Join(",", bestStars.OrderBy(k => k.Key.Item1).ThenBy(k => k.Key.Item2).ThenBy(k => k.Key.Item3) // Sort so the text is stable
                .Select(k => "{\"e\":" + (int)k.Key.Item1 + ",\"d\":" + (int)k.Key.Item2 + ",\"n\":" + k.Key.Item3.ToString(CultureInfo.InvariantCulture) + ",\"s\":" + k.Value + "}"))); // One object per level
            sb.Append("],\"found\":[").Append(string.Join(",", foundKinds.OrderBy(k => k, StringComparer.Ordinal).Select(Quote))).Append("]}"); // Found kinds, sorted
            return sb.ToString(); // Give the text back
        }

        /// <summary>Reads progress from JSON text. Returns false and a fresh profile when the text is broken.</summary>
        public static bool TryFromJson(string json, out PlayerProgress result) // Never throws
        {
            result = new PlayerProgress(); // Start fresh
            try // Run the risky part; broken text is handled below
            {
                var parsed = new PlayerProgress(); // Fill this one; only keep it when all text is valid
                var r = new JsonReader(json ?? ""); // Walks the text
                r.Expect('{'); // Object starts
                bool first = true; // True before the first key
                while (!r.TryConsume('}')) // Until the object ends
                {
                    if (!first) r.Expect(','); // Keys are separated by commas
                    first = false; // Past the first key
                    string key = r.ReadString(); // Key name
                    r.Expect(':'); // Colon after key
                    if (key == "name") parsed.Name = r.ReadString(); // Player name
                    else if (key == "found") r.ReadList(() => parsed.RecordFoundKind(r.ReadString())); // Found kinds
                    else if (key == "levels") r.ReadList(() => ReadLevel(r, parsed)); // Cleared levels
                    else return false; // Unknown key means a broken file
                }
                r.ExpectEnd(); // No extra text after the object
                result = parsed; // All good
                return true; // Tell the caller it worked
            }
            catch (FormatException) { return false; } // Broken text gives a fresh profile
        }

        static void ReadLevel(JsonReader r, PlayerProgress p) // Reads one {"e":0,"d":0,"n":1,"s":3} object
        {
            int e = -1, d = -1, n = -1, s = -1; // Values start invalid
            r.Expect('{'); // Object starts
            bool first = true; // True before the first key
            while (!r.TryConsume('}')) // Until the object ends
            {
                if (!first) r.Expect(','); // Keys are separated by commas
                first = false; // Past the first key
                string key = r.ReadString(); // Key name
                r.Expect(':'); // Colon after key
                int v = r.ReadInt(); // All level values are numbers
                if (key == "e") e = v; else if (key == "d") d = v; else if (key == "n") n = v; else if (key == "s") s = v; // Store by key
                else throw new FormatException("Unknown level key"); // Unknown key
            }
            if (!Enum.IsDefined(typeof(Episode), e) || !Enum.IsDefined(typeof(Difficulty), d)) throw new FormatException("Bad level"); // Episode and difficulty must exist
            if (n < 1 || n > LevelCount((Difficulty)d) || s < 1 || s > 3) throw new FormatException("Bad level"); // Level and stars must be in range
            p.RecordClear((Episode)e, (Difficulty)d, n, s); // Keep the level
        }

        static string Quote(string text) // Wraps text in quotes and escapes the special characters
        {
            var sb = new StringBuilder("\""); // Opening quote
            foreach (char c in text) // Go through each character
            {
                if (c == '"' || c == '\\') sb.Append('\\').Append(c); // Escape quote and backslash
                else if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4")); // Escape control characters
                else sb.Append(c); // Normal character
            }
            return sb.Append('"').ToString(); // Closing quote
        }

        /// <summary>A tiny JSON reader that only knows the shapes the save file uses.</summary>
        sealed class JsonReader // Reads the save text piece by piece
        {
            readonly string text; // The text to read
            int pos; // Current position

            public JsonReader(string text) { this.text = text; } // Remember the text

            void SkipSpace() { while (pos < text.Length && char.IsWhiteSpace(text[pos])) pos++; } // Skip spaces and new lines

            public bool TryConsume(char c) // Eats c when it is next
            {
                SkipSpace(); // Ignore spaces
                if (pos < text.Length && text[pos] == c) { pos++; return true; } // Matched
                return false; // Not there
            }

            public void Expect(char c) { if (!TryConsume(c)) throw new FormatException("Expected " + c); } // Must be next

            public void ExpectEnd() { SkipSpace(); if (pos != text.Length) throw new FormatException("Extra text"); } // Nothing may follow

            public void ReadList(Action readItem) // Reads [item,item,...]
            {
                Expect('['); // List starts
                bool first = true; // True before the first item
                while (!TryConsume(']')) // Until the list ends
                {
                    if (!first) Expect(','); // Items are separated by commas
                    first = false; // Past the first item
                    readItem(); // Read one item
                }
            }

            public int ReadInt() // Reads a whole number
            {
                SkipSpace(); // Ignore spaces
                int start = pos; // Where the number begins
                while (pos < text.Length && char.IsDigit(text[pos])) pos++; // Take digits
                if (pos == start || !int.TryParse(text.Substring(start, pos - start), NumberStyles.None, CultureInfo.InvariantCulture, out int v)) throw new FormatException("Bad number"); // Needs digits that fit
                return v; // The number
            }

            public string ReadString() // Reads "text" with escapes
            {
                Expect('"'); // String starts
                var sb = new StringBuilder(); // Collects the characters
                while (true) // Until the closing quote
                {
                    if (pos >= text.Length) throw new FormatException("Unterminated string"); // Text ended too early
                    char c = text[pos++]; // Next character
                    if (c == '"') return sb.ToString(); // End of string
                    if (c != '\\') { sb.Append(c); continue; } // Normal character
                    if (pos >= text.Length) throw new FormatException("Bad escape"); // Text ended after backslash
                    char esc = text[pos++]; // Escaped character
                    if (esc == '"' || esc == '\\' || esc == '/') sb.Append(esc); // Simple escapes
                    else if (esc == 'u' && pos + 4 <= text.Length && int.TryParse(text.Substring(pos, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code)) { sb.Append((char)code); pos += 4; } // \uXXXX escape
                    else throw new FormatException("Bad escape"); // Anything else is invalid
                }
            }
        }
    }
}
