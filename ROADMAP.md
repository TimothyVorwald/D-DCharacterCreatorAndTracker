# D&D Character Creator & Tracker — Roadmap

This is the backlog of everything scoped out during planning but not yet built. It reflects the phased plan agreed on before Phase 1 started: Core character identity/stats first, everything else in later passes. Update or check items off as they land.

## Phase 1 — Done

- SQLite-backed character roster (create, open, delete, campaign tag)
- Core tab: name, race, class, subclass note, XP-driven level (with override), proficiency bonus, all six ability scores with live modifiers, max/current/temp HP (auto-calculated, overridable), armor class (unarmored formula, overridable), speed, inspiration, death saves, conditions, concentration
- Background tab: background, alignment, personality traits, ideals, bonds, flaws, backstory notes
- SRD 5.1 (2014 rules) reference data seeded for all 14 races/subraces and all 12 classes

## Phase 2 — Done

### Skills & Saving Throws
- Full 18-skill list tied to its governing ability, with proficiency and expertise checkboxes and auto-calculated bonuses (grouped by ability on the Skills & Saves tab; expertise implies proficiency, enforced in the UI)
- All 6 saving throws with proficiency (auto-derived from class) reflected in a bonus total
- Passive Perception, auto-calculated (10 + total Perception bonus)
- Weapon/armor proficiency checklist (simple weapons, martial weapons, light/medium/heavy armor, shields) — a free checklist you set yourself, no enforcement of what your class "should" get
- Tools & languages as two freeform text fields (Tool Proficiencies, Languages) on the Skills & Saves tab, no catalog or validation

## Not Yet Implemented

### Inventory & Equipment
- Structured inventory items (name, quantity, weight, value, category) pulled from a built-in SRD weapons/armor/gear catalog
- Custom/homebrew item entry for anything not in the catalog
- An "equipped" flag on weapons and armor that actually feeds the Attacks tab and Armor Class calculation
- Currency tracking (cp/sp/ep/gp/pp)
- Carrying capacity calculated from Strength, shown against total weight — informational only, no automatic penalty applied
- Full armor-based Armor Class: base AC by armor type + Dexterity modifier (capped per armor type's rules) + shield bonus. Phase 1 only has the unarmored `10 + Dex` formula since there's no equipped armor yet.

### Attacks
- Structured attack entries pulling from equipped/catalog weapons
- Auto-calculated attack bonus (ability modifier + proficiency bonus, using weapon proficiency from the Skills & Saves work above)

### Spellcasting
- Spell slots by character level and class
- Built-in SRD spell catalog (name, level, school, casting time, range, components, description) to pick spells from
- Known/prepared spells list, handling both "known" casters (Bard, Sorcerer, Warlock) and "prepared" casters (Cleric, Druid, Paladin, Ranger, Wizard) correctly
- Auto-calculated spell save DC and spell attack bonus
- (The "Concentrating On" field already exists on the Core tab from Phase 1 — it just isn't tied to a real spell list yet.)

### Abilities & Features
- Structured list for class features, racial traits, and feats
- Optional use-counters per entry (max uses, current uses remaining, recharge on short or long rest)
- Short Rest / Long Rest buttons that auto-reset spell slots, feature uses, and the relevant HP

### Subclasses & Multiclassing
- Subclass is currently a free-text note only — no mechanical features or spells are tied to it yet
- Multiclassing (holding levels in more than one class at once) isn't supported — characters are single-class only for now

### Export & Sharing
- No PDF or print export yet — everything is viewed and edited inside the app

### Other Deferred Ideas
- NPC/monster stat blocks (this app is scoped to player characters only)
- 2024 revised rules (SRD 5.2) — the app targets the 2014 rules (SRD 5.1) exclusively for now
- Character portraits/images

## Explicitly Out of Scope (by design, not just "not yet")

These were deliberate decisions during planning, not just deferred work:

- Ability score generation helpers (point buy, standard array, dice roller) — scores are always entered manually
- Enforcement of "pick N skills/proficiencies from this list" rules — proficiencies are a free checklist you set yourself
- Automatic encumbrance penalties (e.g. reduced speed when over capacity) — carrying capacity is shown informationally only
