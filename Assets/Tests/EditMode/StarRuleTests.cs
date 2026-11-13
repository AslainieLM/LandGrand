using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests
{
    /// <summary>Checks the star rule only through its public method.</summary>
    public class StarRuleTests
    {
        [TestCase(0, 5, 1)] // Nothing collected still gives 1 star
        [TestCase(1, 5, 1)] // 20% gives 1 star
        [TestCase(2, 5, 1)] // 40% gives 1 star
        [TestCase(3, 5, 2)] // 60% gives 2 stars
        [TestCase(4, 5, 2)] // 80% gives 2 stars
        [TestCase(5, 5, 3)] // 100% gives 3 stars
        public void FiveItemLevel_GivesExpectedStars(int collected, int total, int expected) // Checks the five-item level of Mountains Easy 1
        {
            Assert.AreEqual(expected, StarRule.StarsFor(collected, total)); // The stars must match the table
        }

        [TestCase(40, 100, 1)] // Exactly 40% still gives 1 star
        [TestCase(41, 100, 2)] // 41% gives 2 stars
        [TestCase(99, 100, 2)] // 99% gives 2 stars
        [TestCase(100, 100, 3)] // 100% gives 3 stars
        [TestCase(8, 20, 1)] // 40% of 20 gives 1 star
        [TestCase(9, 20, 2)] // 45% of 20 gives 2 stars
        [TestCase(19, 20, 2)] // 95% of 20 gives 2 stars
        public void LargerTotals_UseTheSamePercentages(int collected, int total, int expected) // Checks the percent edges on bigger totals
        {
            Assert.AreEqual(expected, StarRule.StarsFor(collected, total)); // The stars must match the percent rule
        }

        [Test]
        public void JustAboveForty_GivesTwoStars() // 3 of 7 is 42.9%, which is above 40%
        {
            Assert.AreEqual(2, StarRule.StarsFor(3, 7)); // A fraction above 40% counts as 2 stars
        }

        [TestCase(0, 0)] // A level with no items
        [TestCase(-3, 5)] // A bad negative count
        public void ClearedLevel_NeverGetsLessThanOneStar(int collected, int total) // A cleared level is never blocked by stars
        {
            Assert.GreaterOrEqual(StarRule.StarsFor(collected, total), 1); // At least 1 star
        }

        [Test]
        public void CollectedAboveTotal_StillGivesThreeStars() // Extra counts must not break the rule
        {
            Assert.AreEqual(3, StarRule.StarsFor(7, 5)); // More than all items is treated as all items
        }
    }
}
