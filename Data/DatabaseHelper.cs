using System;
using System.Data.SQLite;
using System.IO;

namespace D_DCharacterCreatorAndTracker.Data
{
    /// <summary>
    /// Owns the SQLite connection string and database initialization.
    /// The database file lives alongside the executable so the roster
    /// travels with the app.
    /// </summary>
    public static class DatabaseHelper
    {
        private const string DatabaseFileName = "DnDCharacters.db";

        public static string DatabasePath
        {
            get
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(baseDirectory, DatabaseFileName);
            }
        }

        public static string ConnectionString
        {
            get { return "Data Source=" + DatabasePath + ";Version=3;Foreign Keys=True;"; }
        }

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Creates the database file (if missing) and ensures all tables
        /// exist. Safe to call on every startup. Each phase of the app adds
        /// its own CREATE TABLE IF NOT EXISTS statements here rather than
        /// replacing what came before, so upgrading the app never destroys
        /// existing character data.
        /// </summary>
        public static void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(DatabasePath);

            using (var connection = GetConnection())
            {
                // --- Phase 1: reference data + core character record ---
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Races (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL UNIQUE,
                        Speed INTEGER NOT NULL,
                        Size TEXT NOT NULL,
                        Traits TEXT NOT NULL
                    );");

                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Classes (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL UNIQUE,
                        HitDie INTEGER NOT NULL,
                        SavingThrowProficiency1 TEXT NOT NULL,
                        SavingThrowProficiency2 TEXT NOT NULL,
                        SpellcastingAbility TEXT NULL
                    );");

                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Characters (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        RaceId INTEGER NULL REFERENCES Races(Id),
                        ClassId INTEGER NULL REFERENCES Classes(Id),
                        SubclassNote TEXT NULL,
                        ExperiencePoints INTEGER NOT NULL DEFAULT 0,
                        LevelOverride INTEGER NULL,
                        Strength INTEGER NOT NULL DEFAULT 10,
                        Dexterity INTEGER NOT NULL DEFAULT 10,
                        Constitution INTEGER NOT NULL DEFAULT 10,
                        Intelligence INTEGER NOT NULL DEFAULT 10,
                        Wisdom INTEGER NOT NULL DEFAULT 10,
                        Charisma INTEGER NOT NULL DEFAULT 10,
                        MaxHitPointsOverride INTEGER NULL,
                        CurrentHitPoints INTEGER NOT NULL DEFAULT 0,
                        TemporaryHitPoints INTEGER NOT NULL DEFAULT 0,
                        ArmorClassOverride INTEGER NULL,
                        Inspiration INTEGER NOT NULL DEFAULT 0,
                        DeathSaveSuccesses INTEGER NOT NULL DEFAULT 0,
                        DeathSaveFailures INTEGER NOT NULL DEFAULT 0,
                        Conditions TEXT NULL,
                        ConcentrationSpell TEXT NULL,
                        Background TEXT NULL,
                        Alignment TEXT NULL,
                        PersonalityTraits TEXT NULL,
                        Ideals TEXT NULL,
                        Bonds TEXT NULL,
                        Flaws TEXT NULL,
                        BackstoryNotes TEXT NULL,
                        CampaignTag TEXT NULL,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL
                    );");

                // --- Phase 2: skills, saving throws, weapon/armor proficiencies ---
                // Child tables rather than columns on Characters, so this stays a
                // pure CREATE TABLE IF NOT EXISTS addition -- no ALTER TABLE
                // migration needed for a database created by an earlier version
                // of the app. Foreign Keys=True (see ConnectionString) means
                // deleting a Character cascades into both of these.
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS CharacterSkills (
                        CharacterId INTEGER NOT NULL REFERENCES Characters(Id) ON DELETE CASCADE,
                        Skill TEXT NOT NULL,
                        Proficient INTEGER NOT NULL DEFAULT 0,
                        Expertise INTEGER NOT NULL DEFAULT 0,
                        PRIMARY KEY (CharacterId, Skill)
                    );");

                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS CharacterProficiencies (
                        CharacterId INTEGER NOT NULL REFERENCES Characters(Id) ON DELETE CASCADE,
                        ProficiencyKey TEXT NOT NULL,
                        PRIMARY KEY (CharacterId, ProficiencyKey)
                    );");
            }

            if (isNewDatabase)
            {
                SeedData.Populate();
            }
            else
            {
                // Existing database file but possibly the first Phase-1 run
                // against it -- make sure reference tables are populated.
                SeedData.PopulateIfEmpty();
            }
        }

        internal static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
