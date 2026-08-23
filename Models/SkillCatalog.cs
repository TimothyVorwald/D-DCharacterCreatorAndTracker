using System.Collections.Generic;

namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// Fixed SRD (2014 rules) skill reference data: each skill's governing
    /// ability and display name. Unlike Races/Classes, skills are never
    /// seeded into the database or edited by the player -- there are always
    /// exactly these 18 -- so this is plain code rather than a reference
    /// table.
    /// </summary>
    public static class SkillCatalog
    {
        private class SkillInfo
        {
            public Skill Skill;
            public Ability GoverningAbility;
            public string DisplayName;
        }

        // Ordered to match the standard character sheet layout: skills
        // grouped by governing ability, in PHB order within each group.
        // CharacterSheetForm relies on this order for the Skills tab and
        // for iterating skills when saving, so it doubles as the
        // canonical skill order for the whole app.
        private static readonly SkillInfo[] AllSkillInfo =
        {
            new SkillInfo { Skill = Skill.Athletics, GoverningAbility = Ability.Strength, DisplayName = "Athletics" },

            new SkillInfo { Skill = Skill.Acrobatics, GoverningAbility = Ability.Dexterity, DisplayName = "Acrobatics" },
            new SkillInfo { Skill = Skill.SleightOfHand, GoverningAbility = Ability.Dexterity, DisplayName = "Sleight of Hand" },
            new SkillInfo { Skill = Skill.Stealth, GoverningAbility = Ability.Dexterity, DisplayName = "Stealth" },

            new SkillInfo { Skill = Skill.Arcana, GoverningAbility = Ability.Intelligence, DisplayName = "Arcana" },
            new SkillInfo { Skill = Skill.History, GoverningAbility = Ability.Intelligence, DisplayName = "History" },
            new SkillInfo { Skill = Skill.Investigation, GoverningAbility = Ability.Intelligence, DisplayName = "Investigation" },
            new SkillInfo { Skill = Skill.Nature, GoverningAbility = Ability.Intelligence, DisplayName = "Nature" },
            new SkillInfo { Skill = Skill.Religion, GoverningAbility = Ability.Intelligence, DisplayName = "Religion" },

            new SkillInfo { Skill = Skill.AnimalHandling, GoverningAbility = Ability.Wisdom, DisplayName = "Animal Handling" },
            new SkillInfo { Skill = Skill.Insight, GoverningAbility = Ability.Wisdom, DisplayName = "Insight" },
            new SkillInfo { Skill = Skill.Medicine, GoverningAbility = Ability.Wisdom, DisplayName = "Medicine" },
            new SkillInfo { Skill = Skill.Perception, GoverningAbility = Ability.Wisdom, DisplayName = "Perception" },
            new SkillInfo { Skill = Skill.Survival, GoverningAbility = Ability.Wisdom, DisplayName = "Survival" },

            new SkillInfo { Skill = Skill.Deception, GoverningAbility = Ability.Charisma, DisplayName = "Deception" },
            new SkillInfo { Skill = Skill.Intimidation, GoverningAbility = Ability.Charisma, DisplayName = "Intimidation" },
            new SkillInfo { Skill = Skill.Performance, GoverningAbility = Ability.Charisma, DisplayName = "Performance" },
            new SkillInfo { Skill = Skill.Persuasion, GoverningAbility = Ability.Charisma, DisplayName = "Persuasion" },
        };

        private static readonly Dictionary<Skill, SkillInfo> Lookup = BuildLookup();

        private static Dictionary<Skill, SkillInfo> BuildLookup()
        {
            var lookup = new Dictionary<Skill, SkillInfo>();
            foreach (var info in AllSkillInfo)
                lookup[info.Skill] = info;
            return lookup;
        }

        /// <summary>
        /// All 18 skills, grouped by governing ability and in PHB order
        /// within each group. Safe to iterate for both UI layout and
        /// persistence -- returns a fresh array each call.
        /// </summary>
        public static Skill[] InDisplayOrder()
        {
            var skills = new Skill[AllSkillInfo.Length];
            for (int i = 0; i < AllSkillInfo.Length; i++)
                skills[i] = AllSkillInfo[i].Skill;
            return skills;
        }

        public static Ability GetGoverningAbility(Skill skill)
        {
            return Lookup[skill].GoverningAbility;
        }

        public static string GetDisplayName(Skill skill)
        {
            return Lookup[skill].DisplayName;
        }
    }
}
