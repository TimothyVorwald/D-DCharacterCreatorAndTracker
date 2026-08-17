using System;
using System.Drawing;
using System.Windows.Forms;

namespace D_DCharacterCreatorAndTracker.Forms
{
    partial class CharacterSheetForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private TabControl tabControl;
        private TabPage coreTabPage;
        private TabPage backgroundTabPage;
        private TabPage skillsTabPage;
        private TabPage inventoryTabPage;
        private TabPage attacksTabPage;
        private TabPage spellcastingTabPage;
        private TabPage abilitiesTabPage;

        private GroupBox identityGroupBox;
        private TextBox nameTextBox;
        private ComboBox raceComboBox;
        private ComboBox classComboBox;
        private TextBox subclassNoteTextBox;
        private TextBox campaignTagTextBox;

        private GroupBox progressionGroupBox;
        private NumericUpDown xpNumericUpDown;
        private Label levelValueLabel;
        private Label proficiencyBonusValueLabel;
        private CheckBox levelOverrideCheckBox;
        private NumericUpDown levelOverrideNumericUpDown;

        private GroupBox abilityScoresGroupBox;
        private NumericUpDown[] abilityScoreInputs;
        private Label[] abilityModifierLabels;

        private GroupBox combatGroupBox;
        private Label maxHpValueLabel;
        private CheckBox maxHpOverrideCheckBox;
        private NumericUpDown maxHpOverrideNumericUpDown;
        private NumericUpDown currentHpNumericUpDown;
        private NumericUpDown temporaryHpNumericUpDown;
        private Label acValueLabel;
        private CheckBox acOverrideCheckBox;
        private NumericUpDown acOverrideNumericUpDown;
        private Label speedValueLabel;

        private GroupBox statusGroupBox;
        private CheckBox inspirationCheckBox;
        private CheckBox[] deathSaveSuccessBoxes;
        private CheckBox[] deathSaveFailureBoxes;
        private TextBox conditionsTextBox;
        private TextBox concentrationTextBox;

        private TextBox backgroundTextBox;
        private TextBox alignmentTextBox;
        private TextBox personalityTextBox;
        private TextBox idealsTextBox;
        private TextBox bondsTextBox;
        private TextBox flawsTextBox;
        private TextBox backstoryTextBox;

        private Panel bottomPanel;
        private Button saveButton;
        private Button closeButton;
        private Label statusLabel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.tabControl = new TabControl();
            this.coreTabPage = new TabPage("Core");
            this.backgroundTabPage = new TabPage("Background");
            this.skillsTabPage = new TabPage("Skills && Saves");
            this.inventoryTabPage = new TabPage("Inventory");
            this.attacksTabPage = new TabPage("Attacks");
            this.spellcastingTabPage = new TabPage("Spellcasting");
            this.abilitiesTabPage = new TabPage("Abilities && Features");

            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.TabPages.AddRange(new TabPage[] {
                this.coreTabPage, this.backgroundTabPage, this.skillsTabPage,
                this.inventoryTabPage, this.attacksTabPage, this.spellcastingTabPage,
                this.abilitiesTabPage
            });

            BuildCoreTab();
            BuildBackgroundTab();
            BuildPlaceholderTab(this.skillsTabPage,
                "Skills, saving throws, and weapon/armor proficiencies are coming in a later phase.");
            BuildPlaceholderTab(this.inventoryTabPage,
                "Structured inventory and equipment (linked to Attacks and Armor Class) are coming in a later phase.");
            BuildPlaceholderTab(this.attacksTabPage,
                "Structured attacks with auto-calculated attack bonus are coming in a later phase.");
            BuildPlaceholderTab(this.spellcastingTabPage,
                "Full spellcasting tracking (slots, prepared/known spells, save DC) is coming in a later phase.");
            BuildPlaceholderTab(this.abilitiesTabPage,
                "Class features, racial traits, and feats with use-tracking are coming in a later phase.");

            this.bottomPanel = new Panel();
            this.bottomPanel.Dock = DockStyle.Bottom;
            this.bottomPanel.Height = 48;

            this.saveButton = new Button();
            this.saveButton.Text = "Save";
            this.saveButton.Location = new Point(12, 10);
            this.saveButton.Size = new Size(100, 28);
            this.saveButton.Click += new EventHandler(this.saveButton_Click);

            this.closeButton = new Button();
            this.closeButton.Text = "Close";
            this.closeButton.Location = new Point(120, 10);
            this.closeButton.Size = new Size(100, 28);
            this.closeButton.Click += new EventHandler(this.closeButton_Click);

            this.statusLabel = new Label();
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new Point(240, 17);
            this.statusLabel.ForeColor = Color.DarkGreen;

            this.bottomPanel.Controls.Add(this.saveButton);
            this.bottomPanel.Controls.Add(this.closeButton);
            this.bottomPanel.Controls.Add(this.statusLabel);

            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(760, 620);
            this.MinimumSize = new Size(640, 480);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.bottomPanel);
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void BuildPlaceholderTab(TabPage page, string message)
        {
            var label = new Label();
            label.Text = message;
            label.AutoSize = false;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Padding = new Padding(24);
            page.Controls.Add(label);
        }

        private void BuildCoreTab()
        {
            var corePanel = new Panel();
            corePanel.Dock = DockStyle.Fill;
            corePanel.AutoScroll = true;

            var coreFlowPanel = new FlowLayoutPanel();
            coreFlowPanel.FlowDirection = FlowDirection.TopDown;
            coreFlowPanel.WrapContents = false;
            coreFlowPanel.AutoSize = true;
            coreFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            coreFlowPanel.Padding = new Padding(10);

            BuildIdentityGroup();
            BuildProgressionGroup();
            BuildAbilityScoresGroup();
            BuildCombatGroup();
            BuildStatusGroup();

            coreFlowPanel.Controls.Add(this.identityGroupBox);
            coreFlowPanel.Controls.Add(this.progressionGroupBox);
            coreFlowPanel.Controls.Add(this.abilityScoresGroupBox);
            coreFlowPanel.Controls.Add(this.combatGroupBox);
            coreFlowPanel.Controls.Add(this.statusGroupBox);

            corePanel.Controls.Add(coreFlowPanel);
            this.coreTabPage.Controls.Add(corePanel);
        }

        private static TableLayoutPanel CreateFieldTable()
        {
            var table = new TableLayoutPanel();
            table.ColumnCount = 2;
            table.AutoSize = true;
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table.Padding = new Padding(8);
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            return table;
        }

        private static void AddRow(TableLayoutPanel table, string labelText, Control control)
        {
            int row = table.RowStyles.Count;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label();
            label.Text = labelText;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.Margin = new Padding(3, 8, 3, 3);
            control.Margin = new Padding(3, 4, 3, 4);

            table.Controls.Add(label, 0, row);
            table.Controls.Add(control, 1, row);
        }

        private static FlowLayoutPanel Row(params Control[] controls)
        {
            var panel = new FlowLayoutPanel();
            panel.AutoSize = true;
            panel.WrapContents = false;
            panel.Margin = new Padding(0);
            foreach (var control in controls)
                panel.Controls.Add(control);
            return panel;
        }

        private void BuildIdentityGroup()
        {
            this.identityGroupBox = new GroupBox();
            this.identityGroupBox.Text = "Identity";
            this.identityGroupBox.AutoSize = true;
            this.identityGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.identityGroupBox.Width = 700;

            var table = CreateFieldTable();

            this.nameTextBox = new TextBox();
            this.nameTextBox.Width = 260;
            AddRow(table, "Character Name", this.nameTextBox);

            this.raceComboBox = new ComboBox();
            this.raceComboBox.Width = 260;
            this.raceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.raceComboBox.SelectedIndexChanged += new EventHandler(this.raceComboBox_SelectedIndexChanged);
            AddRow(table, "Race", this.raceComboBox);

            this.classComboBox = new ComboBox();
            this.classComboBox.Width = 260;
            this.classComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.classComboBox.SelectedIndexChanged += new EventHandler(this.classComboBox_SelectedIndexChanged);
            AddRow(table, "Class", this.classComboBox);

            this.subclassNoteTextBox = new TextBox();
            this.subclassNoteTextBox.Width = 260;
            AddRow(table, "Subclass (note)", this.subclassNoteTextBox);

            this.campaignTagTextBox = new TextBox();
            this.campaignTagTextBox.Width = 260;
            AddRow(table, "Campaign Tag", this.campaignTagTextBox);

            this.identityGroupBox.Controls.Add(table);
        }

        private void BuildProgressionGroup()
        {
            this.progressionGroupBox = new GroupBox();
            this.progressionGroupBox.Text = "Progression";
            this.progressionGroupBox.AutoSize = true;
            this.progressionGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.progressionGroupBox.Width = 700;

            var table = CreateFieldTable();

            this.xpNumericUpDown = new NumericUpDown();
            this.xpNumericUpDown.Minimum = 0;
            this.xpNumericUpDown.Maximum = 999999;
            this.xpNumericUpDown.Width = 100;
            this.xpNumericUpDown.ValueChanged += new EventHandler(this.RecalculateDerivedStats);
            AddRow(table, "Experience Points", this.xpNumericUpDown);

            this.levelValueLabel = new Label();
            this.levelValueLabel.AutoSize = true;
            this.levelValueLabel.Text = "1";
            this.levelValueLabel.Font = new Font(this.Font, FontStyle.Bold);

            this.levelOverrideCheckBox = new CheckBox();
            this.levelOverrideCheckBox.Text = "Override";
            this.levelOverrideCheckBox.AutoSize = true;
            this.levelOverrideCheckBox.CheckedChanged += new EventHandler(this.levelOverrideCheckBox_CheckedChanged);

            this.levelOverrideNumericUpDown = new NumericUpDown();
            this.levelOverrideNumericUpDown.Minimum = 1;
            this.levelOverrideNumericUpDown.Maximum = 20;
            this.levelOverrideNumericUpDown.Width = 60;
            this.levelOverrideNumericUpDown.Enabled = false;
            this.levelOverrideNumericUpDown.ValueChanged += new EventHandler(this.RecalculateDerivedStats);

            AddRow(table, "Level (from XP)", Row(this.levelValueLabel, this.levelOverrideCheckBox, this.levelOverrideNumericUpDown));

            this.proficiencyBonusValueLabel = new Label();
            this.proficiencyBonusValueLabel.AutoSize = true;
            this.proficiencyBonusValueLabel.Text = "+2";
            AddRow(table, "Proficiency Bonus", this.proficiencyBonusValueLabel);

            this.progressionGroupBox.Controls.Add(table);
        }

        private void BuildAbilityScoresGroup()
        {
            this.abilityScoresGroupBox = new GroupBox();
            this.abilityScoresGroupBox.Text = "Ability Scores";
            this.abilityScoresGroupBox.AutoSize = true;
            this.abilityScoresGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.abilityScoresGroupBox.Width = 700;

            var table = CreateFieldTable();
            this.abilityScoreInputs = new NumericUpDown[6];
            this.abilityModifierLabels = new Label[6];

            string[] labels = { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };

            for (int i = 0; i < 6; i++)
            {
                var numeric = new NumericUpDown();
                numeric.Minimum = 1;
                numeric.Maximum = 30;
                numeric.Value = 10;
                numeric.Width = 60;

                int abilityIndex = i;
                numeric.ValueChanged += (s, e) => this.UpdateAbilityModifierLabel(abilityIndex);

                var modifierLabel = new Label();
                modifierLabel.AutoSize = true;
                modifierLabel.Text = "+0";
                modifierLabel.Margin = new Padding(12, 8, 3, 3);

                AddRow(table, labels[i], Row(numeric, modifierLabel));

                this.abilityScoreInputs[i] = numeric;
                this.abilityModifierLabels[i] = modifierLabel;
            }

            this.abilityScoresGroupBox.Controls.Add(table);
        }

        private void BuildCombatGroup()
        {
            this.combatGroupBox = new GroupBox();
            this.combatGroupBox.Text = "Combat Stats";
            this.combatGroupBox.AutoSize = true;
            this.combatGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.combatGroupBox.Width = 700;

            var table = CreateFieldTable();

            this.maxHpValueLabel = new Label();
            this.maxHpValueLabel.AutoSize = true;
            this.maxHpValueLabel.Text = "0";
            this.maxHpOverrideCheckBox = new CheckBox();
            this.maxHpOverrideCheckBox.Text = "Override";
            this.maxHpOverrideCheckBox.AutoSize = true;
            this.maxHpOverrideCheckBox.CheckedChanged += new EventHandler(this.maxHpOverrideCheckBox_CheckedChanged);
            this.maxHpOverrideNumericUpDown = new NumericUpDown();
            this.maxHpOverrideNumericUpDown.Minimum = 1;
            this.maxHpOverrideNumericUpDown.Maximum = 9999;
            this.maxHpOverrideNumericUpDown.Width = 70;
            this.maxHpOverrideNumericUpDown.Enabled = false;
            this.maxHpOverrideNumericUpDown.ValueChanged += new EventHandler(this.RecalculateDerivedStats);
            AddRow(table, "Max Hit Points (auto)", Row(this.maxHpValueLabel, this.maxHpOverrideCheckBox, this.maxHpOverrideNumericUpDown));

            this.currentHpNumericUpDown = new NumericUpDown();
            this.currentHpNumericUpDown.Minimum = -999;
            this.currentHpNumericUpDown.Maximum = 9999;
            this.currentHpNumericUpDown.Width = 70;
            AddRow(table, "Current Hit Points", this.currentHpNumericUpDown);

            this.temporaryHpNumericUpDown = new NumericUpDown();
            this.temporaryHpNumericUpDown.Minimum = 0;
            this.temporaryHpNumericUpDown.Maximum = 999;
            this.temporaryHpNumericUpDown.Width = 70;
            AddRow(table, "Temporary Hit Points", this.temporaryHpNumericUpDown);

            this.acValueLabel = new Label();
            this.acValueLabel.AutoSize = true;
            this.acValueLabel.Text = "10";
            this.acOverrideCheckBox = new CheckBox();
            this.acOverrideCheckBox.Text = "Override";
            this.acOverrideCheckBox.AutoSize = true;
            this.acOverrideCheckBox.CheckedChanged += new EventHandler(this.acOverrideCheckBox_CheckedChanged);
            this.acOverrideNumericUpDown = new NumericUpDown();
            this.acOverrideNumericUpDown.Minimum = 1;
            this.acOverrideNumericUpDown.Maximum = 40;
            this.acOverrideNumericUpDown.Width = 60;
            this.acOverrideNumericUpDown.Enabled = false;
            this.acOverrideNumericUpDown.ValueChanged += new EventHandler(this.RecalculateDerivedStats);
            AddRow(table, "Armor Class (unarmored, auto)", Row(this.acValueLabel, this.acOverrideCheckBox, this.acOverrideNumericUpDown));

            this.speedValueLabel = new Label();
            this.speedValueLabel.AutoSize = true;
            this.speedValueLabel.Text = "30 ft.";
            AddRow(table, "Speed (from race)", this.speedValueLabel);

            this.combatGroupBox.Controls.Add(table);
        }

        private void BuildStatusGroup()
        {
            this.statusGroupBox = new GroupBox();
            this.statusGroupBox.Text = "Live Play Status";
            this.statusGroupBox.AutoSize = true;
            this.statusGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.statusGroupBox.Width = 700;

            var table = CreateFieldTable();

            this.inspirationCheckBox = new CheckBox();
            this.inspirationCheckBox.Text = "Has Inspiration";
            this.inspirationCheckBox.AutoSize = true;
            AddRow(table, "Inspiration", this.inspirationCheckBox);

            this.deathSaveSuccessBoxes = new CheckBox[3];
            this.deathSaveFailureBoxes = new CheckBox[3];

            var deathSaveRow = new FlowLayoutPanel();
            deathSaveRow.AutoSize = true;
            deathSaveRow.WrapContents = false;

            var successLabel = new Label();
            successLabel.Text = "Successes:";
            successLabel.AutoSize = true;
            successLabel.Margin = new Padding(3, 6, 3, 3);
            deathSaveRow.Controls.Add(successLabel);
            for (int i = 0; i < 3; i++)
            {
                var box = new CheckBox();
                box.AutoSize = true;
                deathSaveRow.Controls.Add(box);
                this.deathSaveSuccessBoxes[i] = box;
            }

            var failureLabel = new Label();
            failureLabel.Text = "     Failures:";
            failureLabel.AutoSize = true;
            failureLabel.Margin = new Padding(10, 6, 3, 3);
            deathSaveRow.Controls.Add(failureLabel);
            for (int i = 0; i < 3; i++)
            {
                var box = new CheckBox();
                box.AutoSize = true;
                deathSaveRow.Controls.Add(box);
                this.deathSaveFailureBoxes[i] = box;
            }

            AddRow(table, "Death Saves", deathSaveRow);

            this.conditionsTextBox = new TextBox();
            this.conditionsTextBox.Width = 400;
            AddRow(table, "Conditions", this.conditionsTextBox);

            this.concentrationTextBox = new TextBox();
            this.concentrationTextBox.Width = 260;
            AddRow(table, "Concentrating On", this.concentrationTextBox);

            this.statusGroupBox.Controls.Add(table);
        }

        private void BuildBackgroundTab()
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;

            var table = CreateFieldTable();

            this.backgroundTextBox = new TextBox();
            this.backgroundTextBox.Width = 400;
            AddRow(table, "Background", this.backgroundTextBox);

            this.alignmentTextBox = new TextBox();
            this.alignmentTextBox.Width = 200;
            AddRow(table, "Alignment", this.alignmentTextBox);

            this.personalityTextBox = CreateMultilineBox();
            AddRow(table, "Personality Traits", this.personalityTextBox);

            this.idealsTextBox = CreateMultilineBox();
            AddRow(table, "Ideals", this.idealsTextBox);

            this.bondsTextBox = CreateMultilineBox();
            AddRow(table, "Bonds", this.bondsTextBox);

            this.flawsTextBox = CreateMultilineBox();
            AddRow(table, "Flaws", this.flawsTextBox);

            this.backstoryTextBox = CreateMultilineBox();
            this.backstoryTextBox.Height = 140;
            AddRow(table, "Backstory Notes", this.backstoryTextBox);

            panel.Controls.Add(table);
            this.backgroundTabPage.Controls.Add(panel);
        }

        private static TextBox CreateMultilineBox()
        {
            var box = new TextBox();
            box.Multiline = true;
            box.Width = 400;
            box.Height = 60;
            box.ScrollBars = ScrollBars.Vertical;
            return box;
        }
    }
}
