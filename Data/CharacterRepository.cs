using System;
using System.Collections.Generic;
using System.Data.SQLite;
using D_DCharacterCreatorAndTracker.Models;

namespace D_DCharacterCreatorAndTracker.Data
{
    /// <summary>
    /// CRUD access to the Characters table, plus lookups against the
    /// Races/Classes reference tables used to populate dropdowns.
    /// </summary>
    public class CharacterRepository
    {
        public List<Race> GetAllRaces()
        {
            var races = new List<Race>();
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SQLiteCommand("SELECT Id, Name, Speed, Size, Traits FROM Races ORDER BY Name", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    races.Add(new Race
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Speed = reader.GetInt32(2),
                        Size = reader.GetString(3),
                        Traits = reader.GetString(4)
                    });
                }
            }
            return races;
        }

        public List<CharacterClass> GetAllClasses()
        {
            var classes = new List<CharacterClass>();
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SQLiteCommand(
                "SELECT Id, Name, HitDie, SavingThrowProficiency1, SavingThrowProficiency2, SpellcastingAbility FROM Classes ORDER BY Name", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    classes.Add(new CharacterClass
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        HitDie = reader.GetInt32(2),
                        SavingThrowProficiency1 = reader.GetString(3),
                        SavingThrowProficiency2 = reader.GetString(4),
                        SpellcastingAbility = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }
            }
            return classes;
        }

        /// <summary>
        /// Lightweight roster projection -- avoids hydrating every
        /// character (with its Race/Class lookups) just to show a list.
        /// </summary>
        public List<CharacterSummary> GetRoster()
        {
            var roster = new List<CharacterSummary>();
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SQLiteCommand(
                @"SELECT c.Id, c.Name, r.Name, cl.Name, c.ExperiencePoints, c.LevelOverride, c.CampaignTag
                  FROM Characters c
                  LEFT JOIN Races r ON r.Id = c.RaceId
                  LEFT JOIN Classes cl ON cl.Id = c.ClassId
                  ORDER BY c.Name", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int xp = reader.GetInt32(4);
                    int? levelOverride = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);

                    roster.Add(new CharacterSummary
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        RaceName = reader.IsDBNull(2) ? "(no race)" : reader.GetString(2),
                        ClassName = reader.IsDBNull(3) ? "(no class)" : reader.GetString(3),
                        Level = levelOverride ?? LevelFromXp(xp),
                        CampaignTag = reader.IsDBNull(6) ? "" : reader.GetString(6)
                    });
                }
            }
            return roster;
        }

        private static int LevelFromXp(int xp)
        {
            // Mirrors Character.Level's logic for the summary list, without
            // hydrating a full Character just to compute one number.
            var temp = new Character { ExperiencePoints = xp };
            return temp.Level;
        }

        public Character GetById(int id)
        {
            Character character;
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Characters WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    character = ReadCharacter(reader);
                }
            }

            var races = GetAllRaces();
            var classes = GetAllClasses();
            if (character.RaceId.HasValue)
                character.Race = races.Find(r => r.Id == character.RaceId.Value);
            if (character.ClassId.HasValue)
                character.Class = classes.Find(c => c.Id == character.ClassId.Value);

            return character;
        }

        private static Character ReadCharacter(SQLiteDataReader reader)
        {
            var character = new Character();
            character.Id = Convert.ToInt32(reader["Id"]);
            character.Name = reader["Name"].ToString();
            character.RaceId = reader["RaceId"] is DBNull ? (int?)null : Convert.ToInt32(reader["RaceId"]);
            character.ClassId = reader["ClassId"] is DBNull ? (int?)null : Convert.ToInt32(reader["ClassId"]);
            character.SubclassNote = reader["SubclassNote"] is DBNull ? "" : reader["SubclassNote"].ToString();
            character.ExperiencePoints = Convert.ToInt32(reader["ExperiencePoints"]);
            character.LevelOverride = reader["LevelOverride"] is DBNull ? (int?)null : Convert.ToInt32(reader["LevelOverride"]);

            character.Abilities = new AbilityScores
            {
                Strength = Convert.ToInt32(reader["Strength"]),
                Dexterity = Convert.ToInt32(reader["Dexterity"]),
                Constitution = Convert.ToInt32(reader["Constitution"]),
                Intelligence = Convert.ToInt32(reader["Intelligence"]),
                Wisdom = Convert.ToInt32(reader["Wisdom"]),
                Charisma = Convert.ToInt32(reader["Charisma"])
            };

            character.MaxHitPointsOverride = reader["MaxHitPointsOverride"] is DBNull ? (int?)null : Convert.ToInt32(reader["MaxHitPointsOverride"]);
            character.CurrentHitPoints = Convert.ToInt32(reader["CurrentHitPoints"]);
            character.TemporaryHitPoints = Convert.ToInt32(reader["TemporaryHitPoints"]);
            character.ArmorClassOverride = reader["ArmorClassOverride"] is DBNull ? (int?)null : Convert.ToInt32(reader["ArmorClassOverride"]);
            character.Inspiration = Convert.ToInt32(reader["Inspiration"]) != 0;
            character.DeathSaveSuccesses = Convert.ToInt32(reader["DeathSaveSuccesses"]);
            character.DeathSaveFailures = Convert.ToInt32(reader["DeathSaveFailures"]);
            character.Conditions = reader["Conditions"] is DBNull ? "" : reader["Conditions"].ToString();
            character.ConcentrationSpell = reader["ConcentrationSpell"] is DBNull ? "" : reader["ConcentrationSpell"].ToString();
            character.Background = reader["Background"] is DBNull ? "" : reader["Background"].ToString();
            character.Alignment = reader["Alignment"] is DBNull ? "" : reader["Alignment"].ToString();
            character.PersonalityTraits = reader["PersonalityTraits"] is DBNull ? "" : reader["PersonalityTraits"].ToString();
            character.Ideals = reader["Ideals"] is DBNull ? "" : reader["Ideals"].ToString();
            character.Bonds = reader["Bonds"] is DBNull ? "" : reader["Bonds"].ToString();
            character.Flaws = reader["Flaws"] is DBNull ? "" : reader["Flaws"].ToString();
            character.BackstoryNotes = reader["BackstoryNotes"] is DBNull ? "" : reader["BackstoryNotes"].ToString();
            character.CampaignTag = reader["CampaignTag"] is DBNull ? "" : reader["CampaignTag"].ToString();
            character.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
            character.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

            return character;
        }

        public int Save(Character character)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                if (character.Id == 0)
                    return Insert(connection, character);

                Update(connection, character);
                return character.Id;
            }
        }

        private int Insert(SQLiteConnection connection, Character character)
        {
            DateTime now = DateTime.UtcNow;
            character.CreatedAt = now;
            character.UpdatedAt = now;

            using (var command = new SQLiteCommand(
                @"INSERT INTO Characters
                (Name, RaceId, ClassId, SubclassNote, ExperiencePoints, LevelOverride,
                 Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma,
                 MaxHitPointsOverride, CurrentHitPoints, TemporaryHitPoints, ArmorClassOverride,
                 Inspiration, DeathSaveSuccesses, DeathSaveFailures, Conditions, ConcentrationSpell,
                 Background, Alignment, PersonalityTraits, Ideals, Bonds, Flaws, BackstoryNotes,
                 CampaignTag, CreatedAt, UpdatedAt)
                VALUES
                (@Name, @RaceId, @ClassId, @SubclassNote, @ExperiencePoints, @LevelOverride,
                 @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma,
                 @MaxHitPointsOverride, @CurrentHitPoints, @TemporaryHitPoints, @ArmorClassOverride,
                 @Inspiration, @DeathSaveSuccesses, @DeathSaveFailures, @Conditions, @ConcentrationSpell,
                 @Background, @Alignment, @PersonalityTraits, @Ideals, @Bonds, @Flaws, @BackstoryNotes,
                 @CampaignTag, @CreatedAt, @UpdatedAt);
                 SELECT last_insert_rowid();", connection))
            {
                BindCharacterParameters(command, character, now.ToString("o"), now.ToString("o"));
                long newId = (long)command.ExecuteScalar();
                character.Id = (int)newId;
                return character.Id;
            }
        }

        private void Update(SQLiteConnection connection, Character character)
        {
            DateTime now = DateTime.UtcNow;
            character.UpdatedAt = now;

            using (var command = new SQLiteCommand(
                @"UPDATE Characters SET
                    Name = @Name, RaceId = @RaceId, ClassId = @ClassId, SubclassNote = @SubclassNote,
                    ExperiencePoints = @ExperiencePoints, LevelOverride = @LevelOverride,
                    Strength = @Strength, Dexterity = @Dexterity, Constitution = @Constitution,
                    Intelligence = @Intelligence, Wisdom = @Wisdom, Charisma = @Charisma,
                    MaxHitPointsOverride = @MaxHitPointsOverride, CurrentHitPoints = @CurrentHitPoints,
                    TemporaryHitPoints = @TemporaryHitPoints, ArmorClassOverride = @ArmorClassOverride,
                    Inspiration = @Inspiration, DeathSaveSuccesses = @DeathSaveSuccesses,
                    DeathSaveFailures = @DeathSaveFailures, Conditions = @Conditions,
                    ConcentrationSpell = @ConcentrationSpell, Background = @Background,
                    Alignment = @Alignment, PersonalityTraits = @PersonalityTraits, Ideals = @Ideals,
                    Bonds = @Bonds, Flaws = @Flaws, BackstoryNotes = @BackstoryNotes,
                    CampaignTag = @CampaignTag, UpdatedAt = @UpdatedAt
                  WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", character.Id);
                BindCharacterParameters(command, character, character.CreatedAt.ToString("o"), now.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        private static void BindCharacterParameters(SQLiteCommand command, Character character, string createdAt, string updatedAt)
        {
            command.Parameters.AddWithValue("@Name", character.Name ?? "");
            command.Parameters.AddWithValue("@RaceId", (object)character.RaceId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ClassId", (object)character.ClassId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubclassNote", character.SubclassNote ?? "");
            command.Parameters.AddWithValue("@ExperiencePoints", character.ExperiencePoints);
            command.Parameters.AddWithValue("@LevelOverride", (object)character.LevelOverride ?? DBNull.Value);
            command.Parameters.AddWithValue("@Strength", character.Abilities.Strength);
            command.Parameters.AddWithValue("@Dexterity", character.Abilities.Dexterity);
            command.Parameters.AddWithValue("@Constitution", character.Abilities.Constitution);
            command.Parameters.AddWithValue("@Intelligence", character.Abilities.Intelligence);
            command.Parameters.AddWithValue("@Wisdom", character.Abilities.Wisdom);
            command.Parameters.AddWithValue("@Charisma", character.Abilities.Charisma);
            command.Parameters.AddWithValue("@MaxHitPointsOverride", (object)character.MaxHitPointsOverride ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentHitPoints", character.CurrentHitPoints);
            command.Parameters.AddWithValue("@TemporaryHitPoints", character.TemporaryHitPoints);
            command.Parameters.AddWithValue("@ArmorClassOverride", (object)character.ArmorClassOverride ?? DBNull.Value);
            command.Parameters.AddWithValue("@Inspiration", character.Inspiration ? 1 : 0);
            command.Parameters.AddWithValue("@DeathSaveSuccesses", character.DeathSaveSuccesses);
            command.Parameters.AddWithValue("@DeathSaveFailures", character.DeathSaveFailures);
            command.Parameters.AddWithValue("@Conditions", character.Conditions ?? "");
            command.Parameters.AddWithValue("@ConcentrationSpell", character.ConcentrationSpell ?? "");
            command.Parameters.AddWithValue("@Background", character.Background ?? "");
            command.Parameters.AddWithValue("@Alignment", character.Alignment ?? "");
            command.Parameters.AddWithValue("@PersonalityTraits", character.PersonalityTraits ?? "");
            command.Parameters.AddWithValue("@Ideals", character.Ideals ?? "");
            command.Parameters.AddWithValue("@Bonds", character.Bonds ?? "");
            command.Parameters.AddWithValue("@Flaws", character.Flaws ?? "");
            command.Parameters.AddWithValue("@BackstoryNotes", character.BackstoryNotes ?? "");
            command.Parameters.AddWithValue("@CampaignTag", character.CampaignTag ?? "");
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            command.Parameters.AddWithValue("@UpdatedAt", updatedAt);
        }

        public void Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SQLiteCommand("DELETE FROM Characters WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Lightweight projection used by the roster list.
    /// </summary>
    public class CharacterSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RaceName { get; set; }
        public string ClassName { get; set; }
        public int Level { get; set; }
        public string CampaignTag { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
