namespace D_DCharacterCreatorAndTracker.Models
{
    /// <summary>
    /// The six D&amp;D ability scores. Used wherever code needs to refer to
    /// "which ability" rather than a raw string abbreviation.
    /// </summary>
    public enum Ability
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    /// <summary>
    /// The 18 SRD (2014 rules) skills. Fixed list -- see SkillCatalog for
    /// each skill's governing ability and display name. Stored in the
    /// database as this enum's name (e.g. "SleightOfHand"), never as the
    /// numeric ordinal, so the mapping stays stable even if members are
    /// reordered or inserted later.
    /// </summary>
    public enum Skill
    {
        Acrobatics,
        AnimalHandling,
        Arcana,
        Athletics,
        Deception,
        History,
        Insight,
        Intimidation,
        Investigation,
        Medicine,
        Nature,
        Perception,
        Performance,
        Persuasion,
        Religion,
        SleightOfHand,
        Stealth,
        Survival
    }

    /// <summary>
    /// The fixed 6-item weapon/armor proficiency checklist (see ROADMAP --
    /// this is a free checklist the player sets themselves, with no
    /// enforcement of what their class "should" get). Stored in the
    /// database as this enum's name, same reasoning as Skill above.
    /// </summary>
    public enum WeaponArmorProficiency
    {
        SimpleWeapons,
        MartialWeapons,
        LightArmor,
        MediumArmor,
        HeavyArmor,
        Shields
    }
}
