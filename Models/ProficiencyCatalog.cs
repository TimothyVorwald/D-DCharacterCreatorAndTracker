using System.Collections.Generic;

namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// Display names for the fixed 6-item weapon/armor proficiency
    /// checklist. Like SkillCatalog, this is fixed SRD data, not something
    /// stored in the database or edited by the player.
    /// </summary>
    public static class ProficiencyCatalog
    {
        private static readonly Dictionary<WeaponArmorProficiency, string> DisplayNames = new Dictionary<WeaponArmorProficiency, string>
        {
            { WeaponArmorProficiency.SimpleWeapons, "Simple Weapons" },
            { WeaponArmorProficiency.MartialWeapons, "Martial Weapons" },
            { WeaponArmorProficiency.LightArmor, "Light Armor" },
            { WeaponArmorProficiency.MediumArmor, "Medium Armor" },
            { WeaponArmorProficiency.HeavyArmor, "Heavy Armor" },
            { WeaponArmorProficiency.Shields, "Shields" }
        };

        /// <summary>
        /// All 6 proficiency categories, in the order the ROADMAP lists
        /// them. Used to lay out the checklist and to iterate when saving.
        /// </summary>
        public static WeaponArmorProficiency[] InDisplayOrder()
        {
            return new[]
            {
                WeaponArmorProficiency.SimpleWeapons,
                WeaponArmorProficiency.MartialWeapons,
                WeaponArmorProficiency.LightArmor,
                WeaponArmorProficiency.MediumArmor,
                WeaponArmorProficiency.HeavyArmor,
                WeaponArmorProficiency.Shields
            };
        }

        public static string GetDisplayName(WeaponArmorProficiency proficiency)
        {
            return DisplayNames[proficiency];
        }
    }
}
