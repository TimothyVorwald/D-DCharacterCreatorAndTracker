using System;
using System.Collections.Generic;
using System.Windows.Forms;
using D_DCharacterCreatorAndTracker.Data;
using D_DCharacterCreatorAndTracker.Models;

namespace D_DCharacterCreatorAndTracker.Forms
{
    /// <summary>
    /// The tabbed character sheet. The Core tab (identity, progression,
    /// ability scores, combat stats, live-play status) plus a Background
    /// tab are fully functional in Phase 1; the remaining tabs are
    /// placeholders for later phases (skills/saves, inventory, attacks,
    /// spellcasting, abilities &amp; features).
    /// </summary>
    public partial class CharacterSheetForm : Form
    {
        private readonly CharacterRepository _repository;
        private readonly Character _character;
        private List<Race> _races;
        private List<CharacterClass> _classes;
        private bool _isLoadingCharacter;

        public CharacterSheetForm(Character character, CharacterRepository repository)
        {
            InitializeComponent();
            _character = character;
            _repository = repository;

            LoadReferenceData();
            LoadCharacterIntoForm();

            this.Text = string.IsNullOrEmpty(_character.Name) ? "New Character" : _character.Name;
        }

        private void LoadReferenceData()
        {
            _races = _repository.GetAllRaces();
            _classes = _repository.GetAllClasses();

            raceComboBox.Items.Clear();
            raceComboBox.Items.Add("(none)");
            foreach (var race in _races)
                raceComboBox.Items.Add(race);

            classComboBox.Items.Clear();
            classComboBox.Items.Add("(none)");
            foreach (var characterClass in _classes)
                classComboBox.Items.Add(characterClass);
        }

        private void LoadCharacterIntoForm()
        {
            _isLoadingCharacter = true;

            nameTextBox.Text = _character.Name;
            subclassNoteTextBox.Text = _character.SubclassNote;
            campaignTagTextBox.Text = _character.CampaignTag;

            raceComboBox.SelectedItem = FindComboItemById(raceComboBox, _character.RaceId);
            if (raceComboBox.SelectedIndex < 0)
                raceComboBox.SelectedIndex = 0;

            classComboBox.SelectedItem = FindComboItemById(classComboBox, _character.ClassId);
            if (classComboBox.SelectedIndex < 0)
                classComboBox.SelectedIndex = 0;

            xpNumericUpDown.Value = _character.ExperiencePoints;
            levelOverrideCheckBox.Checked = _character.LevelOverride.HasValue;
            levelOverrideNumericUpDown.Enabled = _character.LevelOverride.HasValue;
            levelOverrideNumericUpDown.Value = _character.LevelOverride ?? _character.Level;

            abilityScoreInputs[0].Value = _character.Abilities.Strength;
            abilityScoreInputs[1].Value = _character.Abilities.Dexterity;
            abilityScoreInputs[2].Value = _character.Abilities.Constitution;
            abilityScoreInputs[3].Value = _character.Abilities.Intelligence;
            abilityScoreInputs[4].Value = _character.Abilities.Wisdom;
            abilityScoreInputs[5].Value = _character.Abilities.Charisma;
            for (int i = 0; i < 6; i++)
                RefreshAbilityModifierLabel(i);

            maxHpOverrideCheckBox.Checked = _character.MaxHitPointsOverride.HasValue;
            maxHpOverrideNumericUpDown.Enabled = _character.MaxHitPointsOverride.HasValue;
            maxHpOverrideNumericUpDown.Value = _character.MaxHitPointsOverride ?? Math.Max(1, _character.MaxHitPoints);
            currentHpNumericUpDown.Value = _character.CurrentHitPoints;
            temporaryHpNumericUpDown.Value = _character.TemporaryHitPoints;

            acOverrideCheckBox.Checked = _character.ArmorClassOverride.HasValue;
            acOverrideNumericUpDown.Enabled = _character.ArmorClassOverride.HasValue;
            acOverrideNumericUpDown.Value = _character.ArmorClassOverride ?? _character.ArmorClass;

            inspirationCheckBox.Checked = _character.Inspiration;
            for (int i = 0; i < 3; i++)
            {
                deathSaveSuccessBoxes[i].Checked = i < _character.DeathSaveSuccesses;
                deathSaveFailureBoxes[i].Checked = i < _character.DeathSaveFailures;
            }
            conditionsTextBox.Text = _character.Conditions;
            concentrationTextBox.Text = _character.ConcentrationSpell;

            backgroundTextBox.Text = _character.Background;
            alignmentTextBox.Text = _character.Alignment;
            personalityTextBox.Text = _character.PersonalityTraits;
            idealsTextBox.Text = _character.Ideals;
            bondsTextBox.Text = _character.Bonds;
            flawsTextBox.Text = _character.Flaws;
            backstoryTextBox.Text = _character.BackstoryNotes;

            _isLoadingCharacter = false;
            RecalculateDerivedStats(this, EventArgs.Empty);
        }

        /// <summary>
        /// ComboBox.SelectedItem requires the exact object reference that's
        /// in the Items collection -- the Race/Class attached to the loaded
        /// Character comes from a separate query, so we look up the
        /// matching combo item by Id instead of comparing references.
        /// </summary>
        private static object FindComboItemById(ComboBox comboBox, int? id)
        {
            if (!id.HasValue)
                return null;

            foreach (var item in comboBox.Items)
            {
                var race = item as Race;
                if (race != null && race.Id == id.Value)
                    return item;

                var characterClass = item as CharacterClass;
                if (characterClass != null && characterClass.Id == id.Value)
                    return item;
            }
            return null;
        }

        private void UpdateAbilityModifierLabel(int index)
        {
            RefreshAbilityModifierLabel(index);
            RecalculateDerivedStats(this, EventArgs.Empty);
        }

        private void RefreshAbilityModifierLabel(int index)
        {
            int score = (int)abilityScoreInputs[index].Value;
            int modifier = AbilityScores.ModifierFor(score);
            abilityModifierLabels[index].Text = FormatModifier(modifier);
        }

        private static string FormatModifier(int modifier)
        {
            return modifier >= 0 ? "+" + modifier : modifier.ToString();
        }

        private void raceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculateDerivedStats(sender, e);
        }

        private void classComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculateDerivedStats(sender, e);
        }

        private void levelOverrideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            levelOverrideNumericUpDown.Enabled = levelOverrideCheckBox.Checked;
            RecalculateDerivedStats(sender, e);
        }

        private void maxHpOverrideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            maxHpOverrideNumericUpDown.Enabled = maxHpOverrideCheckBox.Checked;
            RecalculateDerivedStats(sender, e);
        }

        private void acOverrideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            acOverrideNumericUpDown.Enabled = acOverrideCheckBox.Checked;
            RecalculateDerivedStats(sender, e);
        }

        /// <summary>
        /// Recomputes every read-only derived value (level, proficiency
        /// bonus, max HP, AC, speed) from the current form inputs. Wired to
        /// the ValueChanged/CheckedChanged/SelectedIndexChanged events of
        /// everything that feeds a calculation, so the sheet always
        /// reflects live edits.
        /// </summary>
        private void RecalculateDerivedStats(object sender, EventArgs e)
        {
            if (_isLoadingCharacter)
                return;

            var snapshot = BuildCharacterSnapshot();

            levelValueLabel.Text = snapshot.Level.ToString();
            proficiencyBonusValueLabel.Text = FormatModifier(snapshot.ProficiencyBonus);

            maxHpValueLabel.Text = maxHpOverrideCheckBox.Checked
                ? ((int)maxHpOverrideNumericUpDown.Value).ToString()
                : snapshot.MaxHitPoints.ToString();

            acValueLabel.Text = acOverrideCheckBox.Checked
                ? ((int)acOverrideNumericUpDown.Value).ToString()
                : snapshot.ArmorClass.ToString();

            speedValueLabel.Text = snapshot.Speed + " ft.";
        }

        /// <summary>
        /// Builds a throwaway Character from the current form state so the
        /// calculation logic already in Models.Character (Level,
        /// ProficiencyBonus, MaxHitPoints, ArmorClass, Speed) can be reused
        /// instead of duplicated in the UI layer.
        /// </summary>
        private Character BuildCharacterSnapshot()
        {
            var snapshot = new Character();
            snapshot.ExperiencePoints = (int)xpNumericUpDown.Value;
            snapshot.LevelOverride = levelOverrideCheckBox.Checked ? (int?)levelOverrideNumericUpDown.Value : null;
            snapshot.Abilities.Strength = (int)abilityScoreInputs[0].Value;
            snapshot.Abilities.Dexterity = (int)abilityScoreInputs[1].Value;
            snapshot.Abilities.Constitution = (int)abilityScoreInputs[2].Value;
            snapshot.Abilities.Intelligence = (int)abilityScoreInputs[3].Value;
            snapshot.Abilities.Wisdom = (int)abilityScoreInputs[4].Value;
            snapshot.Abilities.Charisma = (int)abilityScoreInputs[5].Value;
            snapshot.Race = raceComboBox.SelectedItem as Race;
            snapshot.Class = classComboBox.SelectedItem as CharacterClass;
            return snapshot;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show(this, "Please enter a character name before saving.",
                    "Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReadFormIntoCharacter();

            try
            {
                _repository.Save(_character);
                this.Text = _character.Name;
                statusLabel.Text = "Saved.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not save this character:\n\n" + ex.Message,
                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadFormIntoCharacter()
        {
            _character.Name = nameTextBox.Text.Trim();
            _character.SubclassNote = subclassNoteTextBox.Text;
            _character.CampaignTag = campaignTagTextBox.Text;

            var selectedRace = raceComboBox.SelectedItem as Race;
            _character.Race = selectedRace;
            _character.RaceId = selectedRace != null ? (int?)selectedRace.Id : null;

            var selectedClass = classComboBox.SelectedItem as CharacterClass;
            _character.Class = selectedClass;
            _character.ClassId = selectedClass != null ? (int?)selectedClass.Id : null;

            _character.ExperiencePoints = (int)xpNumericUpDown.Value;
            _character.LevelOverride = levelOverrideCheckBox.Checked ? (int?)levelOverrideNumericUpDown.Value : null;

            _character.Abilities.Strength = (int)abilityScoreInputs[0].Value;
            _character.Abilities.Dexterity = (int)abilityScoreInputs[1].Value;
            _character.Abilities.Constitution = (int)abilityScoreInputs[2].Value;
            _character.Abilities.Intelligence = (int)abilityScoreInputs[3].Value;
            _character.Abilities.Wisdom = (int)abilityScoreInputs[4].Value;
            _character.Abilities.Charisma = (int)abilityScoreInputs[5].Value;

            _character.MaxHitPointsOverride = maxHpOverrideCheckBox.Checked ? (int?)maxHpOverrideNumericUpDown.Value : null;
            _character.CurrentHitPoints = (int)currentHpNumericUpDown.Value;
            _character.TemporaryHitPoints = (int)temporaryHpNumericUpDown.Value;
            _character.ArmorClassOverride = acOverrideCheckBox.Checked ? (int?)acOverrideNumericUpDown.Value : null;

            _character.Inspiration = inspirationCheckBox.Checked;

            int successes = 0;
            for (int i = 0; i < 3; i++)
                if (deathSaveSuccessBoxes[i].Checked)
                    successes++;

            int failures = 0;
            for (int i = 0; i < 3; i++)
                if (deathSaveFailureBoxes[i].Checked)
                    failures++;

            _character.DeathSaveSuccesses = successes;
            _character.DeathSaveFailures = failures;

            _character.Conditions = conditionsTextBox.Text;
            _character.ConcentrationSpell = concentrationTextBox.Text;

            _character.Background = backgroundTextBox.Text;
            _character.Alignment = alignmentTextBox.Text;
            _character.PersonalityTraits = personalityTextBox.Text;
            _character.Ideals = idealsTextBox.Text;
            _character.Bonds = bondsTextBox.Text;
            _character.Flaws = flawsTextBox.Text;
            _character.BackstoryNotes = backstoryTextBox.Text;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
