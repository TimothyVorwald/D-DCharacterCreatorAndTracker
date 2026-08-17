using System;
using System.Data.SQLite;

namespace D_DCharacterCreatorAndTracker.Data
{
    /// <summary>
    /// SRD 5.1 (2014 rules) reference data for races and classes. This is
    /// mechanical/informational seed data, not user data -- it's safe to
    /// re-run and never touches the Characters table.
    /// </summary>
    public static class SeedData
    {
        public static void Populate()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                InsertRaces(connection);
                InsertClasses(connection);
            }
        }

        public static void PopulateIfEmpty()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                if (CountRows(connection, "Races") == 0)
                    InsertRaces(connection);

                if (CountRows(connection, "Classes") == 0)
                    InsertClasses(connection);
            }
        }

        private static int CountRows(SQLiteConnection connection, string tableName)
        {
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM " + tableName, connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static void InsertRace(SQLiteConnection connection, string name, int speed, string size, string traits)
        {
            using (var command = new SQLiteCommand(
                "INSERT INTO Races (Name, Speed, Size, Traits) VALUES (@name, @speed, @size, @traits)", connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@speed", speed);
                command.Parameters.AddWithValue("@size", size);
                command.Parameters.AddWithValue("@traits", traits);
                command.ExecuteNonQuery();
            }
        }

        private static void InsertClass(SQLiteConnection connection, string name, int hitDie, string save1, string save2, string spellcastingAbility)
        {
            using (var command = new SQLiteCommand(
                "INSERT INTO Classes (Name, HitDie, SavingThrowProficiency1, SavingThrowProficiency2, SpellcastingAbility) " +
                "VALUES (@name, @hitDie, @save1, @save2, @ability)", connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@hitDie", hitDie);
                command.Parameters.AddWithValue("@save1", save1);
                command.Parameters.AddWithValue("@save2", save2);
                command.Parameters.AddWithValue("@ability", (object)spellcastingAbility ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }

        private static void InsertRaces(SQLiteConnection connection)
        {
            InsertRace(connection, "Human", 30, "Medium",
                "Ability scores: +1 to every ability.\nExtra Language: one of your choice.");

            InsertRace(connection, "Hill Dwarf", 25, "Medium",
                "Darkvision 60 ft.\nDwarven Resilience: advantage on saves vs. poison, resistance to poison damage.\n" +
                "Dwarven Combat Training: proficiency with battleaxe, handaxe, light hammer, warhammer.\n" +
                "Tool Proficiency: one artisan's tool of your choice.\nStonecunning: double proficiency on History checks about stonework.\n" +
                "Dwarven Toughness: +1 HP per level.");

            InsertRace(connection, "Mountain Dwarf", 25, "Medium",
                "Darkvision 60 ft.\nDwarven Resilience: advantage on saves vs. poison, resistance to poison damage.\n" +
                "Dwarven Combat Training: proficiency with battleaxe, handaxe, light hammer, warhammer.\n" +
                "Dwarven Armor Training: proficiency with light and medium armor.\n" +
                "Stonecunning: double proficiency on History checks about stonework.");

            InsertRace(connection, "High Elf", 30, "Medium",
                "Darkvision 60 ft.\nFey Ancestry: advantage on saves vs. charmed, immune to magical sleep.\n" +
                "Trance: 4 hours of trance instead of sleeping.\nCantrip: one Wizard cantrip.\n" +
                "Extra Language: one of your choice.");

            InsertRace(connection, "Wood Elf", 35, "Medium",
                "Darkvision 60 ft.\nFey Ancestry: advantage on saves vs. charmed, immune to magical sleep.\n" +
                "Trance: 4 hours of trance instead of sleeping.\nMask of the Wild: can attempt to hide when only lightly obscured.");

            InsertRace(connection, "Dark Elf (Drow)", 30, "Medium",
                "Superior Darkvision 120 ft.\nSunlight Sensitivity.\nFey Ancestry: advantage on saves vs. charmed, immune to magical sleep.\n" +
                "Trance: 4 hours of trance instead of sleeping.\nDrow Magic: dancing lights, faerie fire (higher level), darkness (higher level).\n" +
                "Drow Weapon Training: proficiency with rapier, shortsword, hand crossbow.");

            InsertRace(connection, "Lightfoot Halfling", 25, "Small",
                "Lucky: reroll a 1 on attack/ability/save d20 rolls.\nBrave: advantage on saves vs. frightened.\n" +
                "Halfling Nimbleness: can move through the space of larger creatures.\n" +
                "Naturally Stealthy: can hide behind a creature at least one size larger.");

            InsertRace(connection, "Stout Halfling", 25, "Small",
                "Lucky: reroll a 1 on attack/ability/save d20 rolls.\nBrave: advantage on saves vs. frightened.\n" +
                "Halfling Nimbleness: can move through the space of larger creatures.\n" +
                "Stout Resilience: advantage on saves vs. poison, resistance to poison damage.");

            InsertRace(connection, "Dragonborn", 30, "Medium",
                "Draconic Ancestry: determines breath weapon and damage resistance type.\n" +
                "Breath Weapon: replaces one action, damage type and area from ancestry.\n" +
                "Damage Resistance: to the damage type of your draconic ancestry.");

            InsertRace(connection, "Forest Gnome", 25, "Small",
                "Darkvision 60 ft.\nGnome Cunning: advantage on Int/Wis/Cha saves vs. magic.\n" +
                "Natural Illusionist: minor illusion cantrip.\nSpeak with Small Beasts.");

            InsertRace(connection, "Rock Gnome", 25, "Small",
                "Darkvision 60 ft.\nGnome Cunning: advantage on Int/Wis/Cha saves vs. magic.\n" +
                "Artificer's Lore: double proficiency on History checks about magic items/alchemical/technological items.\n" +
                "Tinker: proficiency with tinker's tools, can build tiny clockwork devices.");

            InsertRace(connection, "Half-Elf", 30, "Medium",
                "Darkvision 60 ft.\nFey Ancestry: advantage on saves vs. charmed, immune to magical sleep.\n" +
                "Skill Versatility: proficiency in two skills of your choice.");

            InsertRace(connection, "Half-Orc", 30, "Medium",
                "Darkvision 60 ft.\nMenacing: proficiency in Intimidation.\n" +
                "Relentless Endurance: drop to 1 HP instead of 0 once per long rest.\n" +
                "Savage Attacks: extra weapon damage die on a melee critical hit.");

            InsertRace(connection, "Tiefling", 30, "Medium",
                "Darkvision 60 ft.\nHellish Resistance: resistance to fire damage.\n" +
                "Infernal Legacy: thaumaturgy cantrip; hellish rebuke and darkness at higher levels.");
        }

        private static void InsertClasses(SQLiteConnection connection)
        {
            InsertClass(connection, "Barbarian", 12, "STR", "CON", null);
            InsertClass(connection, "Bard", 8, "DEX", "CHA", "CHA");
            InsertClass(connection, "Cleric", 8, "WIS", "CHA", "WIS");
            InsertClass(connection, "Druid", 8, "INT", "WIS", "WIS");
            InsertClass(connection, "Fighter", 10, "STR", "CON", null);
            InsertClass(connection, "Monk", 8, "STR", "DEX", null);
            InsertClass(connection, "Paladin", 10, "WIS", "CHA", "CHA");
            InsertClass(connection, "Ranger", 10, "STR", "DEX", "WIS");
            InsertClass(connection, "Rogue", 8, "DEX", "INT", null);
            InsertClass(connection, "Sorcerer", 6, "CON", "CHA", "CHA");
            InsertClass(connection, "Warlock", 8, "WIS", "CHA", "CHA");
            InsertClass(connection, "Wizard", 6, "INT", "WIS", "INT");
        }
    }
}
