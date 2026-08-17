using System;
using System.Windows.Forms;
using D_DCharacterCreatorAndTracker.Data;
using D_DCharacterCreatorAndTracker.Models;

namespace D_DCharacterCreatorAndTracker.Forms
{
    /// <summary>
    /// The app's entry-point screen: a flat list of every saved character
    /// (roster/library), with create, open, and delete actions.
    /// </summary>
    public partial class RosterForm : Form
    {
        private readonly CharacterRepository _repository;

        public RosterForm()
        {
            InitializeComponent();
            _repository = new CharacterRepository();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Could not open or create the character database:\n\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadRoster();
        }

        private void LoadRoster()
        {
            rosterListView.Items.Clear();
            foreach (var summary in _repository.GetRoster())
            {
                var item = new ListViewItem(summary.Name);
                item.SubItems.Add(summary.RaceName);
                item.SubItems.Add(summary.ClassName);
                item.SubItems.Add(summary.Level.ToString());
                item.SubItems.Add(summary.CampaignTag);
                item.Tag = summary.Id;
                rosterListView.Items.Add(item);
            }
        }

        private void newCharacterButton_Click(object sender, EventArgs e)
        {
            var newCharacter = new Character { Name = "New Character" };
            OpenCharacterSheet(newCharacter);
        }

        private void openCharacterButton_Click(object sender, EventArgs e)
        {
            OpenSelected();
        }

        private void rosterListView_DoubleClick(object sender, EventArgs e)
        {
            OpenSelected();
        }

        private void OpenSelected()
        {
            if (rosterListView.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Select a character first.", "No Character Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)rosterListView.SelectedItems[0].Tag;
            var character = _repository.GetById(id);
            if (character == null)
            {
                MessageBox.Show(this, "That character could not be found. It may have been deleted.",
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadRoster();
                return;
            }

            OpenCharacterSheet(character);
        }

        private void OpenCharacterSheet(Character character)
        {
            using (var sheet = new CharacterSheetForm(character, _repository))
            {
                sheet.ShowDialog(this);
            }
            LoadRoster();
        }

        private void deleteCharacterButton_Click(object sender, EventArgs e)
        {
            if (rosterListView.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Select a character first.", "No Character Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = rosterListView.SelectedItems[0].Text;
            var confirmation = MessageBox.Show(this, "Delete \"" + name + "\"? This cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmation != DialogResult.Yes)
                return;

            int id = (int)rosterListView.SelectedItems[0].Tag;
            _repository.Delete(id);
            LoadRoster();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            LoadRoster();
        }
    }
}
