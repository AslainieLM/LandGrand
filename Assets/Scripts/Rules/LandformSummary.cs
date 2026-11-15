using System.Collections.Generic; // Holds the summary lines

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class LandformSummary // Text and art for a finished landform
    { // Opens the summary
        public string Title { get; } // Landform name
        public IReadOnlyList<string> Sentences { get; } // The short review lines
        public string Picture { get; } // Name of the landform menu picture
        public bool IsDraft { get; } // True until a teacher reviews the summary

        LandformSummary(string title, string[] sentences, string picture, bool isDraft) // Keeps the supplied content
        { // Starts the setup
            Title = title; // Keeps the name
            Sentences = System.Array.AsReadOnly(sentences); // Protects the lines from changes
            Picture = picture; // Keeps the art name
            IsDraft = isDraft; // Keeps the review state
        } // Ends the setup

        static readonly LandformSummary Mountains = new LandformSummary("Mountains", new[] // The five approved draft lines
        { // Starts the lines
            "Mountains are high, steep landforms.", // What mountains are
            "Some form from volcanoes; others fold up.", // How mountains form
            "Weathering and erosion wear them down.", // What wears them down
            "They give us water and homes for animals.", // What mountains give us
            "Let us all take care of our mountains.", // How to care for mountains
        }, "bg_mountains", true); // Uses the Mountains picture and draft note

        public static LandformSummary For(Episode episode) // Finds the summary for this landform
        { // Starts the lookup
            if (episode == Episode.Mountains) return Mountains; // Mountains has its full review
            string title = episode.ToString(); // Keeps the name of the requested landform
            return new LandformSummary(title, new[] { "Coming soon" }, "bg_" + title.ToLowerInvariant(), false); // Other summaries wait for their levels
        } // Ends the lookup
    } // Ends the summary
} // Ends the rules
