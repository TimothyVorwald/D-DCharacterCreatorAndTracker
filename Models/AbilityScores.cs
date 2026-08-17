using System;
using System.Collections.Generic;

namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// The six ability scores and their derived modifiers. Scores are
    /// entered manually by the player (racial bonuses already baked in
    /// by the time they're typed in); modifiers are always derived, never
    /// stored.
    /// </summary>
    public class AbilityScores
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public static readonly IList<string> Abbreviations =
            new List<string> { "STR", "DEX", "CON", "INT", "WIS", "CHA" };

        public AbilityScores()
        {
            Strength = 10;
            Dexterity = 10;
            Constitution = 10;
            Intelligence = 10;
            Wisdom = 10;
            Charisma = 10;
        }

        public static int ModifierFor(int score)
        {
            return (int)Math.Floor((score - 10) / 2.0);
        }

        public int GetScore(Ability ability)
        {
            switch (ability)
            {
                case Ability.Strength: return Strength;
                case Ability.Dexterity: return Dexterity;
                case Ability.Constitution: return Constitution;
                case Ability.Intelligence: return Intelligence;
                case Ability.Wisdom: return Wisdom;
                case Ability.Charisma: return Charisma;
                default: throw new ArgumentOutOfRangeException("ability");
            }
        }

        public int GetModifier(Ability ability)
        {
            return ModifierFor(GetScore(ability));
        }

        /// <summary>
        /// Maps the three-letter abbreviations stored in the Classes table
        /// (e.g. "STR", "WIS") to the corresponding modifier.
        /// </summary>
        public int GetModifierByAbbreviation(string abbreviation)
        {
            if (string.IsNullOrEmpty(abbreviation))
                throw new ArgumentException("abbreviation is required", "abbreviation");

            switch (abbreviation.Trim().ToUpperInvariant())
            {
                case "STR": return GetModifier(Ability.Strength);
                case "DEX": return GetModifier(Ability.Dexterity);
                case "CON": return GetModifier(Ability.Constitution);
                case "INT": return GetModifier(Ability.Intelligence);
                case "WIS": return GetModifier(Ability.Wisdom);
                case "CHA": return GetModifier(Ability.Charisma);
                default:
                    throw new ArgumentOutOfRangeException("abbreviation", abbreviation, "Unrecognized ability abbreviation");
            }
        }
    }
}
