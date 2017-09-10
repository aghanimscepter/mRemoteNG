using System.Collections.Generic;
using System;
using System.Windows.Forms;
using mRemoteNG.App;
using WeifenLuo.WinFormsUI.Docking;
using mRemoteNG.UI.Forms;


namespace mRemoteNG.UI.Window
{
	public partial class ExternalToolsWindow
	{
        #region Constructors
		public ExternalToolsWindow(DockContent panel)
		{
			InitializeComponent();
					
			WindowType = WindowType.ExternalApps;
			DockPnl = panel;
		}
        #endregion
				
        #region Private Fields
		private Tools.ExternalTool _selectedTool;
        #endregion
				
        #region Private Methods
        #region Event Handlers
		private void ExternalTools_Load(object sender, EventArgs e)
		{
			ApplyLanguage();
			UpdateToolsListView();
		}

        private void ExternalToolsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.Settings.SettingsSaver.SaveExternalAppsToXML();
        }

        private void NewTool_Click(object sender, EventArgs e)
		{
			try
			{
				var externalTool = new Tools.ExternalTool(Language.strExternalToolDefaultName);
				Runtime.ExternalTools.Add(externalTool);
				UpdateToolsListView(externalTool);
				DisplayNameTextBox.Focus();
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.NewTool_Click() failed.", ex: ex, logOnly: true);
			}
		}

        private void DeleteTool_Click(object sender, EventArgs e)
		{
			try
			{
				string message;
				if (ToolsListView.SelectedItems.Count == 1)
				{
					message = string.Format(Language.strConfirmDeleteExternalTool, ToolsListView.SelectedItems[0].Text);
				}
				else if (ToolsListView.SelectedItems.Count > 1)
				{
					message = string.Format(Language.strConfirmDeleteExternalToolMultiple, ToolsListView.SelectedItems.Count);
				}
				else
				{
					return;
				}
				
				if (MessageBox.Show(frmMain.Default, message, "Question?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
						
				foreach (ListViewItem listViewItem in ToolsListView.SelectedItems)
				{
					var externalTool = listViewItem.Tag as Tools.ExternalTool;
					if (externalTool == null)
					{
						continue;
					}
							
					Runtime.ExternalTools.Remove(listViewItem.Tag);
					listViewItem.Remove();
				}
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.DeleteTool_Click() failed.", ex: ex, logOnly: true);
			}
		}

        private void LaunchTool_Click(object sender, EventArgs e)
		{
			LaunchTool();
		}

        private void ToolsListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (ToolsListView.SelectedItems.Count == 1)
				{
					PropertiesGroupBox.Enabled = true;
					_selectedTool = ToolsListView.SelectedItems[0].Tag as Tools.ExternalTool;
					if (_selectedTool == null)
					{
						return;
					}
							
					DisplayNameTextBox.Text = _selectedTool.DisplayName;
					FilenameTextBox.Text = _selectedTool.FileName;
					ArgumentsCheckBox.Text = _selectedTool.Arguments;
                    WorkingDirTextBox.Text = _selectedTool.WorkingDir;
					WaitForExitCheckBox.Checked = _selectedTool.WaitForExit;
					TryToIntegrateCheckBox.Checked = _selectedTool.TryIntegrate;
                    RunElevatedCheckBox.Checked = _selectedTool.RunElevated;
				}
				else
				{
					PropertiesGroupBox.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.ToolsListView_SelectedIndexChanged() failed.", ex: ex, logOnly: true);
			}
		}

        private void ToolsListView_DoubleClick(object sender, EventArgs e)
		{
			if (ToolsListView.SelectedItems.Count > 0)
			{
				LaunchTool();
			}
		}

        private void PropertyControl_ChangedOrLostFocus(object sender, EventArgs e)
		{
			if (_selectedTool == null)
			{
				return;
			}
					
			try
			{
				_selectedTool.DisplayName = DisplayNameTextBox.Text;
				_selectedTool.FileName = FilenameTextBox.Text;
				_selectedTool.Arguments = ArgumentsCheckBox.Text;
                _selectedTool.WorkingDir = WorkingDirTextBox.Text;
				_selectedTool.WaitForExit = WaitForExitCheckBox.Checked;
				_selectedTool.TryIntegrate = TryToIntegrateCheckBox.Checked;
                _selectedTool.RunElevated = RunElevatedCheckBox.Checked;
						
				UpdateToolsListView();
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.PropertyControl_ChangedOrLostFocus() failed.", ex: ex, logOnly: true);
			}
		}

        private void BrowseButton_Click(object sender, EventArgs e)
		{
			try
			{
				using (var browseDialog = new OpenFileDialog())
				{
					browseDialog.Filter = string.Join("|", new string[] {Language.strFilterApplication, "*.exe", Language.strFilterAll, "*.*"});
					if (browseDialog.ShowDialog() == DialogResult.OK)
					{
						FilenameTextBox.Text = browseDialog.FileName;
                        PropertyControl_ChangedOrLostFocus(this, e);

                    }
				}
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.BrowseButton_Click() failed.", ex: ex, logOnly: true);
			}
        }

        private void BrowseWorkingDir_Click(object sender, EventArgs e)
        {
            try
            {
                using (var browseDialog = new FolderBrowserDialog())
                {
                    if (browseDialog.ShowDialog() == DialogResult.OK)
                    {
                        WorkingDirTextBox.Text = browseDialog.SelectedPath;
                        PropertyControl_ChangedOrLostFocus(this, e);
                    }
                }

            }
            catch (Exception ex)
            {
                Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.BrowseButton_Click() failed.", ex: ex, logOnly: true);
            }
        }

        private void TryToIntegrateCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (TryToIntegrateCheckBox.Checked)
			{
				WaitForExitCheckBox.Enabled = false;
				WaitForExitCheckBox.Checked = false;
			}
			else
			{
				WaitForExitCheckBox.Enabled = true;
			}
		}
        #endregion
				
		private void ApplyLanguage()
		{
			Text = Language.strMenuExternalTools;
			TabText = Language.strMenuExternalTools;
					
			NewToolToolstripButton.Text = Language.strButtonNew;
			DeleteToolToolstripButton.Text = Language.strOptionsKeyboardButtonDelete;
			LaunchToolToolstripButton.Text = Language.strButtonLaunch;
					
			DisplayNameColumnHeader.Text = Language.strColumnDisplayName;
			FilenameColumnHeader.Text = Language.strColumnFilename;
			ArgumentsColumnHeader.Text = Language.strColumnArguments;
			WaitForExitColumnHeader.Text = Language.strColumnWaitForExit;
			TryToIntegrateCheckBox.Text = Language.strTryIntegrate;
					
			PropertiesGroupBox.Text = Language.strGroupboxExternalToolProperties;
					
			DisplayNameLabel.Text = Language.strLabelDisplayName;
			FilenameLabel.Text = Language.strLabelFilename;
			ArgumentsLabel.Text = Language.strLabelArguments;
			OptionsLabel.Text = Language.strLabelOptions;
			WaitForExitCheckBox.Text = Language.strCheckboxWaitForExit;
			BrowseButton.Text = Language.strButtonBrowse;
					
			NewToolMenuItem.Text = Language.strMenuNewExternalTool;
			DeleteToolMenuItem.Text = Language.strMenuDeleteExternalTool;
			LaunchToolMenuItem.Text = Language.strMenuLaunchExternalTool;
		}
				
		private void UpdateToolsListView(Tools.ExternalTool selectTool = null)
		{
			try
			{
				var selectedTools = new List<Tools.ExternalTool>();
				if (selectTool == null)
				{
					foreach (ListViewItem listViewItem in ToolsListView.SelectedItems)
					{
						var externalTool = listViewItem.Tag as Tools.ExternalTool;
						if (externalTool != null)
						{
							selectedTools.Add(externalTool);
						}
					}
				}
				else
				{
					selectedTools.Add(selectTool);
				}
						
				ToolsListView.BeginUpdate();
				ToolsListView.Items.Clear();
						
				foreach (Tools.ExternalTool externalTool in Runtime.ExternalTools)
				{
				    var listViewItem = new ListViewItem {Text = externalTool.DisplayName};
				    listViewItem.SubItems.Add(externalTool.FileName);
					listViewItem.SubItems.Add(externalTool.Arguments);
                    listViewItem.SubItems.Add(externalTool.WorkingDir);
                    listViewItem.SubItems.Add(externalTool.WaitForExit.ToString());
					listViewItem.SubItems.Add(externalTool.TryIntegrate.ToString());
                    listViewItem.SubItems.Add(externalTool.RunElevated.ToString());
                    listViewItem.Tag = externalTool;
							
					ToolsListView.Items.Add(listViewItem);
							
					if (selectedTools.Contains(externalTool))
					{
						listViewItem.Selected = true;
					}
				}
						
				ToolsListView.EndUpdate();
						
				frmMain.Default.AddExternalToolsToToolBar();
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.PopulateToolsListView()", ex: ex, logOnly: true);
			}
		}
				
		private void LaunchTool()
		{
			try
			{
				foreach (ListViewItem listViewItem in ToolsListView.SelectedItems)
				{
					var externalTool = listViewItem.Tag as Tools.ExternalTool;

				    externalTool?.Start();
				}
			}
			catch (Exception ex)
			{
				Runtime.MessageCollector.AddExceptionMessage(message: "UI.Window.ExternalTools.LaunchTool() failed.", ex: ex, logOnly: true);
			}
		}
        #endregion
    }
}
