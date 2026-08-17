using System;
using System.Drawing;
using System.Windows.Forms;

namespace D_DCharacterCreatorAndTracker.Forms
{
    partial class RosterForm
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

        private ListView rosterListView;
        private ColumnHeader nameColumn;
        private ColumnHeader raceColumn;
        private ColumnHeader classColumn;
        private ColumnHeader levelColumn;
        private ColumnHeader campaignColumn;
        private Panel buttonPanel;
        private Button newCharacterButton;
        private Button openCharacterButton;
        private Button deleteCharacterButton;
        private Button refreshButton;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.rosterListView = new ListView();
            this.nameColumn = new ColumnHeader();
            this.raceColumn = new ColumnHeader();
            this.classColumn = new ColumnHeader();
            this.levelColumn = new ColumnHeader();
            this.campaignColumn = new ColumnHeader();
            this.buttonPanel = new Panel();
            this.newCharacterButton = new Button();
            this.openCharacterButton = new Button();
            this.deleteCharacterButton = new Button();
            this.refreshButton = new Button();

            //
            // rosterListView
            //
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 180;
            this.raceColumn.Text = "Race";
            this.raceColumn.Width = 140;
            this.classColumn.Text = "Class";
            this.classColumn.Width = 120;
            this.levelColumn.Text = "Level";
            this.levelColumn.Width = 60;
            this.campaignColumn.Text = "Campaign";
            this.campaignColumn.Width = 160;

            this.rosterListView.Columns.AddRange(new ColumnHeader[] {
                this.nameColumn, this.raceColumn, this.classColumn, this.levelColumn, this.campaignColumn });
            this.rosterListView.View = View.Details;
            this.rosterListView.FullRowSelect = true;
            this.rosterListView.MultiSelect = false;
            this.rosterListView.Dock = DockStyle.Fill;
            this.rosterListView.DoubleClick += new EventHandler(this.rosterListView_DoubleClick);

            //
            // buttonPanel
            //
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 48;

            this.newCharacterButton.Text = "New Character";
            this.newCharacterButton.Location = new Point(12, 10);
            this.newCharacterButton.Size = new Size(120, 28);
            this.newCharacterButton.Click += new EventHandler(this.newCharacterButton_Click);

            this.openCharacterButton.Text = "Open";
            this.openCharacterButton.Location = new Point(140, 10);
            this.openCharacterButton.Size = new Size(90, 28);
            this.openCharacterButton.Click += new EventHandler(this.openCharacterButton_Click);

            this.deleteCharacterButton.Text = "Delete";
            this.deleteCharacterButton.Location = new Point(238, 10);
            this.deleteCharacterButton.Size = new Size(90, 28);
            this.deleteCharacterButton.Click += new EventHandler(this.deleteCharacterButton_Click);

            this.refreshButton.Text = "Refresh";
            this.refreshButton.Location = new Point(336, 10);
            this.refreshButton.Size = new Size(90, 28);
            this.refreshButton.Click += new EventHandler(this.refreshButton_Click);

            this.buttonPanel.Controls.Add(this.newCharacterButton);
            this.buttonPanel.Controls.Add(this.openCharacterButton);
            this.buttonPanel.Controls.Add(this.deleteCharacterButton);
            this.buttonPanel.Controls.Add(this.refreshButton);

            //
            // RosterForm
            //
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(700, 450);
            this.MinimumSize = new Size(560, 320);
            this.Controls.Add(this.rosterListView);
            this.Controls.Add(this.buttonPanel);
            this.Text = "D&D Character Tracker - Roster";
        }
    }
}
