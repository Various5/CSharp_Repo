using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Frequencio
{
    public partial class Frequencio_Form : Form
    {
        private string[] _audioFileExtensions = { ".mp3", ".wav" };
        private int _numberOfFilesToIndex;
        private string _outputPath;
        public Frequencio_Form()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Frequencio_Form_DragEnter);
            this.DragDrop += new DragEventHandler(Frequencio_Form_DragDrop);

        }

        private void Frequencio_Form_Load(object sender, EventArgs e)
        {

        }

        private void audioFilesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && audioFilesListBox.SelectedItem != null)
            {
                audioFilesListBox.DoDragDrop(audioFilesListBox.SelectedItem.ToString(), DragDropEffects.Move);
            }
        }

        private void Frequencio_Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Frequencio_Form_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // Do something with the dropped files here
        }

        private async void selectFolderButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var folderPath = dialog.SelectedPath;
                    _outputPath = Path.Combine(folderPath, "audio_attributes.json");
                    var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                        .Where(file => _audioFileExtensions.Contains(Path.GetExtension(file)))
                        .ToArray();
                    _numberOfFilesToIndex = files.Length;
                    var audioAttributesList = new AudioAttributesList();

                    // Check if the JSON file exists
                    if (File.Exists(_outputPath))
                    {
                        try
                        {
                            // Read the JSON file and deserialize the list of audio attributes
                            var jsonString = await Task.Run(() => File.ReadAllText(_outputPath));
                            audioAttributesList = JsonSerializer.Deserialize<AudioAttributesList>(jsonString);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading JSON file: {ex.Message}");
                        }
                    }
                    else
                    {
                        // If the JSON file does not exist, create a new list of audio attributes
                        audioAttributesList = new AudioAttributesList();
                    }

                    progressBar1.Maximum = _numberOfFilesToIndex;
                    progressBar1.Value = 0;

                    int skippedFiles = 0;

                    await Task.Run(() =>
                    {
                        Parallel.ForEach(files, file =>
                        {
                            try
                            {
                                using (var audioFile = new AudioFileReader(file))
                                {
                                    var audioAttributes = new AudioAttributes
                                    {
                                        FilePath = audioFile.FileName,
                                        BitsPerSample = audioFile.WaveFormat.BitsPerSample,
                                        SampleRate = audioFile.WaveFormat.SampleRate,
                                        TotalTime = audioFile.TotalTime.TotalSeconds
                                    };
                                    audioAttributesList.Add(audioAttributes);

                                    // Update the UI safely
                                    Invoke(new Action(() =>
                                    {
                                        numberOfFilesLabel.Text = $"{audioAttributesList.AudioAttributes.Count()}/{_numberOfFilesToIndex} processed.";
                                        progressBar1.Value++;

                                        // Add only filename to the listbox
                                        audioFilesListBox.Items.Add(Path.GetFileName(audioAttributes.FilePath));
                                    }));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Skipping file {file} due to error: {ex.Message}");
                                skippedFiles++;
                            }
                        });
                    });

                    await SaveToJsonAsync(audioAttributesList, _outputPath);
                    progressBar1.Value = 0;
                    MessageBox.Show($"Indexing completed with {skippedFiles} skipped files.");

                    if (audioFilesListBox != null)
                    {
                        audioFilesListBox.Items.Clear();
                        foreach (var audioAttribute in audioAttributesList.AudioAttributes)
                        {
                            audioFilesListBox.Items.Add(Path.GetFileName(audioAttribute.FilePath));
                        }
                    }
                }
            }
        }
        private async Task SaveToJsonAsync(AudioAttributesList audioAttributesList, string outputPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(audioAttributesList, options);
            File.WriteAllText(outputPath, jsonString);
        }

    }
    public class AudioAttributes
    {
        public string FilePath { get; set; }
        public int BitsPerSample { get; set; }
        public int SampleRate { get; set; }
        public double TotalTime { get; set; }
    }

}
