namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// Read-only SRD (2014 rules) reference data for a class's fixed
    /// mechanics (hit die, saving throw proficiencies, spellcasting
    /// ability). Named CharacterClass to avoid colliding with the
    /// "class" keyword and with System.Type-style naming.
    /// </summary>
    public class CharacterClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HitDie { get; set; }

        /// <summary>Ability abbreviation, e.g. "STR".</summary>
        public string SavingThrowProficiency1 { get; set; }

        /// <summary>Ability abbreviation, e.g. "CON".</summary>
        public string SavingThrowProficiency2 { get; set; }

        /// <summary>
        /// Governing spellcasting ability abbreviation (e.g. "WIS" for
        /// Druid, "CHA" for Bard), or null for classes with no base
        /// spellcasting (Barbarian, Fighter, Monk, Rogue).
        /// </summary>
        public string SpellcastingAbility { get; set; }

        public bool IsSpellcaster
        {
            get { return !string.IsNullOrEmpty(SpellcastingAbility); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
