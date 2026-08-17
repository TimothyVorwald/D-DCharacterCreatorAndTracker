namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// Read-only SRD (2014 rules) reference data for a race. Rows come from
    /// the Races table and are seeded once on first run -- the player picks
    /// one, but never edits the race data itself.
    /// </summary>
    public class Race
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public string Size { get; set; }

        /// <summary>
        /// Human-readable, newline-separated list of racial traits
        /// (Darkvision, Fey Ancestry, etc.) shown read-only on the sheet.
        /// </summary>
        public string Traits { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
