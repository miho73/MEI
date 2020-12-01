using MEI.Controls;
using MEI.Controls.MyClass;
using MEI.Controls.Sche;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MEI.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        public async void ExportData(object sender, RoutedEventArgs args)
        {
            SaveExportData sed = new SaveExportData();
            var picker = new FileSavePicker
            {
                SuggestedFileName = "exported.json",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                CommitButtonText = UIManager.Rl.GetString("ExportDataFileSelectionCommit")
            };
            picker.FileTypeChoices.Add("Database File", new List<string>() { ".json" });
            StorageFile file = await picker.PickSaveFileAsync();
            if(file != null)
            {
                try
                {
                    await sed.ExportData(file);
                    SaveLoadResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    SaveLoadResult.Text = UIManager.Rl.GetString("SettingsSaveSucess");
                }
                catch
                {
                    SaveLoadResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    SaveLoadResult.Text = UIManager.Rl.GetString("SettingsSaveFailure");
                }
            }
        }

        public async void LoadSettings(object sender, RoutedEventArgs args)
        {
            SaveExportData sed = new SaveExportData();
            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                CommitButtonText = UIManager.Rl.GetString("LoadDataFileSelectionCommit"),
            };
            picker.FileTypeFilter.Add(".json");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            MessageDialog dialog = new MessageDialog(UIManager.Rl.GetString("SettingDataLoadOverwriteWarning"));
            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("SettingDataLoadOverwriteWarningConfirm"), new UICommandInvokedHandler(async (IUICommand cmd) =>
            {
                try
                {
                    await sed.LoadData(file);
                    SaveLoadResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    SaveLoadResult.Text = UIManager.Rl.GetString("SettingsLoadSucess");
                }
                catch
                {
                    SaveLoadResult.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    SaveLoadResult.Text = UIManager.Rl.GetString("SettingsLoadFailure");
                }
            })));
            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("SettingDataLoadOverwriteWarningCancel")));
            await dialog.ShowAsync();
        }

        public ClassManager cManager = new ClassManager();
        public ScheManager sManager = new ScheManager();

        public async void ResetData(object sender, RoutedEventArgs args)
        {
            MessageDialog dialog = new MessageDialog(UIManager.Rl.GetString("SettingResetDataWarning"));
            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("SettingResetDataWarningConfirm"), new UICommandInvokedHandler(async (IUICommand cmd) =>
            {
                try
                {
                    await sManager.Clear();
                    await cManager.Clear();
                } catch
                {
                    //Nerver mind
                }
            })));
            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("SettingResetDataWarningCancel")));
            await dialog.ShowAsync();
        }
    }
}
