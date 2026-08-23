using System;
using System.Collections.Generic;

namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// A player character. Holds both stored fields (persisted as columns
    /// in the Characters table) and derived, read-only properties (Level,
    /// ProficiencyBonus, MaxHitPoints, ArmorClass, Speed) computed from the
    /// stored fields plus the selected Race/Class reference data.
    /// </summary>
    public class Character
    {
        // Standard 5e (2014 rules) experience-to-level thresholds.
        private static readonly int[] XpThresholds =
        {
            0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000,
            85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000
        };

        public int Id { get; set; }
        public string Name { get; set; }

        public int? RaceId { get; set; }
        public Race Race { get; set; }

        public int? ClassId { get; set; }
        public CharacterClass Class { get; set; }

        /// <summary>
        /// Free-text note for subclass (e.g. "Circle of the Moon").
        /// Subclass mechanics aren't modeled yet -- this is a placeholder
        /// until a later phase adds real subclass support.
        /// </summary>
        public string SubclassNote { get; set; }

        public int ExperiencePoints { get; set; }

        /// <summary>
        /// When set, overrides the level normally derived from XP (e.g.
        /// for tables that level up by milestone instead of tracking XP).
        /// </summary>
        public int? LevelOverride { get; set; }

        public AbilityScores Abilities { get; set; }

        /// <summary>
        /// When set, overrides the auto-calculated (average hit die) max
        /// HP -- for tables that roll HP instead of averaging it.
        /// </summary>
        public int? MaxHitPointsOverride { get; set; }

        public int CurrentHitPoints { get; set; }
        public int TemporaryHitPoints { get; set; }

        /// <summary>
        /// When set, overrides the auto-calculated armor class.
        /// </summary>
        public int? ArmorClassOverride { get; set; }

        public bool Inspiration { get; set; }
        public int DeathSaveSuccesses { get; set; }
        public int DeathSaveFailures { get; set; }

        /// <summary>Freeform list of active conditions (poisoned, prone, etc.).</summary>
        public string Conditions { get; set; }

        /// <summary>The spell currently being concentrated on, if any.</summary>
        public string ConcentrationSpell { get; set; }

        /// <summary>Skills the character is proficient in (adds ProficiencyBonus).</summary>
        public HashSet<Skill> ProficientSkills { get; set; }

        /// <summary>
        /// Skills the character has expertise in (adds ProficiencyBonus a
        /// second time). Expertise implies proficiency -- the UI enforces
        /// that a skill can't be in this set without also being in
        /// ProficientSkills, so GetSkillBonus doesn't need to guard against it.
        /// </summary>
        public HashSet<Skill> ExpertiseSkills { get; set; }

        /// <summary>
        /// Free checklist of weapon/armor categories the character is
        /// proficient with -- no enforcement of what the class "should" get.
        /// </summary>
        public HashSet<WeaponArmorProficiency> WeaponArmorProficiencies { get; set; }

        /// <summary>Freeform list of tool proficiencies (e.g. "Thieves' Tools, Herbalism Kit").</summary>
        public string ToolProficiencies { get; set; }

        /// <summary>Freeform list of known languages (e.g. "Common, Elvish").</summary>
        public string Languages { get; set; }

        public string Background { get; set; }
        public string Alignment { get; set; }
        public string PersonalityTraits { get; set; }
        public string Ideals { get; set; }
        public string Bonds { get; set; }
        public string Flaws { get; set; }
        public string BackstoryNotes { get; set; }

        /// <summary>Optional free-text tag for grouping characters by campaign.</summary>
        public string CampaignTag { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Character()
        {
            Abilities = new AbilityScores();
            CurrentHitPoints = 0;
            TemporaryHitPoints = 0;
            Conditions = string.Empty;
            ProficientSkills = new HashSet<Skill>();
            ExpertiseSkills = new HashSet<Skill>();
            WeaponArmorProficiencies = new HashSet<WeaponArmorProficiency>();
            ToolProficiencies = string.Empty;
            Languages = string.Empty;
        }

        /// <summary>
        /// Level derived from XP using the standard 5e (2014) table, unless
        /// LevelOverride has been set by the player.
        /// </summary>
        public int Level
        {
            get
            {
                if (LevelOverride.HasValue)
                    return LevelOverride.Value;

                int level = 1;
                for (int i = 0; i < XpThresholds.Length; i++)
                {
                    if (ExperiencePoints >= XpThresholds[i])
                        level = i + 1;
                    else
                        break;
                }
                return level;
            }
        }

        public int ProficiencyBonus
        {
            get { return 2 + ((Level - 1) / 4); }
        }

        /// <summary>
        /// Average-hit-die max HP: the full hit die at level 1, then
        /// (hit die / 2 + 1) plus the Constitution modifier per level after
        /// that -- the standard 5e "take the average" method. Returns
        /// MaxHitPointsOverride instead when the player has set one.
        /// </summary>
        public int MaxHitPoints
        {
            get
            {
                if (MaxHitPointsOverride.HasValue)
                    return MaxHitPointsOverride.Value;

                if (Class == null)
                    return 0;

                int conModifier = Abilities.GetModifier(Ability.Constitution);
                int hitDie = Class.HitDie;
                int perLevelAverage = (hitDie / 2) + 1;

                int total = hitDie + conModifier; // level 1
                total += (Level - 1) * (perLevelAverage + conModifier); // levels 2+
                return Math.Max(1, total);
            }
        }

        /// <summary>
        /// Phase 1 armor class: 10 + Dexterity modifier (unarmored), with a
        /// manual override. Once equipped armor exists (a later phase),
        /// this becomes armor base AC + capped Dex modifier + shield.
        /// </summary>
        public int ArmorClass
        {
            get
            {
                if (ArmorClassOverride.HasValue)
                    return ArmorClassOverride.Value;

                return 10 + Abilities.GetModifier(Ability.Dexterity);
            }
        }

        public int Speed
        {
            get { return Race != null ? Race.Speed : 30; }
        }

        public bool IsProficientInSave(Ability ability)
        {
            if (Class == null)
                return false;

            string abbreviation = AbilityAbbreviation(ability);
            return string.Equals(Class.SavingThrowProficiency1, abbreviation, StringComparison.OrdinalIgnoreCase)
                || string.Equals(Class.SavingThrowProficiency2, abbreviation, StringComparison.OrdinalIgnoreCase);
        }

        public int GetSavingThrowBonus(Ability ability)
        {
            int modifier = Abilities.GetModifier(ability);
            if (IsProficientInSave(ability))
                modifier += ProficiencyBonus;
            return modifier;
        }

        public bool IsProficientInSkill(Skill skill)
        {
            return ProficientSkills.Contains(skill);
        }

        public bool HasExpertiseInSkill(Skill skill)
        {
            return ExpertiseSkills.Contains(skill);
        }

        /// <summary>
        /// Ability modifier, plus ProficiencyBonus if proficient, plus a
        /// second ProficiencyBonus if the skill also has expertise.
        /// </summary>
        public int GetSkillBonus(Skill skill)
        {
            int modifier = Abilities.GetModifier(SkillCatalog.GetGoverningAbility(skill));

            if (HasExpertiseInSkill(skill))
                return modifier + (ProficiencyBonus * 2);

            if (IsProficientInSkill(skill))
                return modifier + ProficiencyBonus;

            return modifier;
        }

        /// <summary>10 + the character's total Perception skill bonus.</summary>
        public int PassivePerception
        {
            get { return 10 + GetSkillBonus(Skill.Perception); }
        }

        private static string AbilityAbbreviation(Ability ability)
        {
            switch (ability)
            {
                case Ability.Strength: return "STR";
                case Ability.Dexterity: return "DEX";
                case Ability.Constitution: return "CON";
                case Ability.Intelligence: return "INT";
                case Ability.Wisdom: return "WIS";
                case Ability.Charisma: return "CHA";
                default: throw new ArgumentOutOfRangeException("ability");
            }
        }
    }
}
