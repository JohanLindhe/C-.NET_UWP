using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Labb3Filer
{
    public sealed partial class MainPage : Page
    {
        private StorageFile currentFile = null; // nuvarande fil
        private bool changeInText = false; // kontrolera ändringar text
        private bool fileOpened = false; // kontrolera öppnad fil

        public MainPage()
        {
            this.InitializeComponent();
            FileNameTextBlock.Text = "Namnlös.txt"; // Uppdatera FileNameTextBlock

          
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (changeInText)
            {
                MessageDialog msgOpen = new MessageDialog("Vill du spara innan ny fil öppnas?");
                msgOpen.Commands.Add(new UICommand("Ja"));
                msgOpen.Commands.Add(new UICommand("Nej"));
                msgOpen.Commands.Add(new UICommand("Avbryt"));
                var msgResultOpen = await msgOpen.ShowAsync();

                if (msgResultOpen.Label == "Ja")
                {
                    await SaveFile_Click();
                }
                else if (msgResultOpen.Label == "Avbryt")
                {
                    return;
                }
            }

            await OpenFile();
        }

        private async Task OpenFile()
        {
            var openFile = new FileOpenPicker();
            openFile.FileTypeFilter.Add(".txt");
            var result = await openFile.PickSingleFileAsync();

            if (result != null)
            {
                currentFile = result; // Öppnade filen blir den senaste
                var text = await FileIO.ReadTextAsync(result);
                MyTextBox.Text = text.Trim();

                fileOpened = true; // fil öppnad
                UpdateFileName();
            }
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveFile_Click();
        }

        private async Task SaveFile_Click()
        {
            if (!fileOpened && currentFile == null) // Om ingen fil och inget är öppnat
            {
                await SaveAsNewFile();
            }
            else // Om en fil är öppnad
            {
                MessageDialog msg = new MessageDialog("Vill du spara i senast valda fil eller spara som ny egen fil?");
                msg.Commands.Add(new UICommand("Spara"));
                msg.Commands.Add(new UICommand("Spara som"));
                var msgResult = await msg.ShowAsync();

                if (msgResult.Label == "Spara")
                {
                    if (currentFile != null)
                    {
                        await FileIO.WriteTextAsync(currentFile, MyTextBox.Text.Trim());
                        changeInText = false;
                        UpdateFileName();
                    }
                    else
                    {
                        await SaveAsNewFile();
                    }
                }
                else if (msgResult.Label == "Spara som")
                {
                    await SaveAsNewFile();
                }
            }
        }

        private async Task SaveAsNewFile()
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            var result = await savePicker.PickSaveFileAsync();

            if (result != null)
            {
                currentFile = result; // Senast spara som blir current
                await FileIO.WriteTextAsync(result, MyTextBox.Text);
                changeInText = false; // Filen är sparad
                UpdateFileName();
            }
        }

        private void UpdateFileName()
        {
            if (currentFile != null)
            {
                if (changeInText)
                {
                    FileNameTextBlock.Text = "*" + currentFile.Name;
                }
                else
                {
                    FileNameTextBlock.Text = currentFile.Name;
                }
            }
            else
            {
                if (changeInText)
                {
                    FileNameTextBlock.Text = "*Namnlös.txt"; // Lägg till asterisken även när ingen fil är vald men ändring skett
                }
                else
                {
                    FileNameTextBlock.Text = "Namnlös.txt"; // Om ingen fil valts och ingen ändring
                }
            }
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            MyTextBox.Text = string.Empty;
            
            UpdateFileName();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/2820357/how-do-i-exit-a-wpf-application-programmatically
            Application.Current.Exit();
        }

        private async void MyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (currentFile != null)
            {
                var fileCont = await FileIO.ReadTextAsync(currentFile);
                // ChatGPT. Om texten inte matchar det som hämtats in
                changeInText = MyTextBox.Text.Trim() != fileCont;
            }
            else
            {
                // https://stackoverflow.com/questions/16762201/how-to-use-string-isnullorempty-in-textbox
                // https://learn.microsoft.com/en-us/dotnet/api/system.string.isnullorempty?view=net-8.0                
                changeInText = !string.IsNullOrEmpty(MyTextBox.Text);
            }

            UpdateFileName();
        }

    }
}
