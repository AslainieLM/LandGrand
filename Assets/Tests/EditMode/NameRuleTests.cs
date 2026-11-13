using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the player-name rule only through its public method.</summary>
    public class NameRuleTests // A group of checks
    {
        [TestCase("Ana", "Ana")] // A short name is fine
        [TestCase("A", "A")] // One character is the smallest name
        [TestCase("Juan Carlo 1", "Juan Carlo 1")] // Exactly 12 characters with spaces and a number
        [TestCase("Noelño", "Noelño")] // Letters with accents are letters
        [TestCase("  Ana  ", "Ana")] // Spaces around the name are removed
        public void ValidNames_AreAccepted(string raw, string expected) // Checks names that must pass
        {
            Assert.IsTrue(NameRule.TryClean(raw, out var name, out var message)); // The name is accepted
            Assert.AreEqual(expected, name); // The cleaned name matches
            Assert.AreEqual("", message); // No message when it is fine
        }

        [TestCase(null)] // Nothing typed at all
        [TestCase("")] // Empty text
        [TestCase("    ")] // Only spaces
        [TestCase("Juan Carlos 12")] // 14 characters: too long
        [TestCase("1234567890123")] // 13 characters: one too many
        [TestCase("Ana!")] // A symbol
        [TestCase("Ana_B")] // An underscore
        [TestCase("Ana\tB")] // A tab is not a space
        [TestCase("Ana😀")] // An emoji
        public void BadNames_AreRefused_WithAMessage(string raw) // Checks names that must fail
        {
            Assert.IsFalse(NameRule.TryClean(raw, out var name, out var message)); // The name is refused
            Assert.AreEqual("", name); // No name is given back
            Assert.IsNotEmpty(message); // The player gets a message
        }

        [Test] // Unity runs this method as a test
        public void EachProblem_HasItsOwnMessage() // The message must say what to fix
        {
            NameRule.TryClean("", out _, out var empty); // Message for a missing name
            NameRule.TryClean("1234567890123", out _, out var tooLong); // Message for a long name
            NameRule.TryClean("Ana!", out _, out var badCharacter); // Message for a bad character
            Assert.AreNotEqual(empty, tooLong); // Empty and long differ
            Assert.AreNotEqual(tooLong, badCharacter); // Long and bad character differ
            Assert.AreNotEqual(empty, badCharacter); // Empty and bad character differ
        }
    }
}
