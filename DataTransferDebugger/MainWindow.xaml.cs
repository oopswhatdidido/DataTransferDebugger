using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DataTransferDebugger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += MainWindow_DragEnter;
            this.DragOver += MainWindow_DragOver;
            this.Drop += MainWindow_Drop;
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            try
            {
                AnalyzeDragDropData(e.Data);
            }
            catch (Exception ex)
            {
                DisplayError($"Error analyzing drag data: {ex.Message}");
            }
        }

        private void AnalyzeClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clipboardData = Clipboard.GetDataObject();
                AnalyzeDataObject(clipboardData, "CLIPBOARD");
            }
            catch (Exception ex)
            {
                DisplayError($"Error analyzing clipboard: {ex.Message}");
            }
        }

        private void ClearResults_Click(object sender, RoutedEventArgs e)
        {
            ResultsTextBox.Clear();
        }

        private void AnalyzeDragDropData(IDataObject dataObject)
        {
            AnalyzeDataObject(dataObject, "DRAG & DROP");
        }

        private void AnalyzeDataObject(IDataObject dataObject, string source)
        {
            var results = new StringBuilder();
            results.AppendLine($"=== {source} DATA ANALYSIS ===");
            results.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            results.AppendLine();

            // Get all available formats
            string[] formats = dataObject.GetFormats();
            results.AppendLine($"Available Formats ({formats.Length}):");
            foreach (string format in formats)
            {
                results.AppendLine($"  • {format}");
            }
            results.AppendLine();

            // Analyze each format
            foreach (string format in formats)
            {
                results.AppendLine($"--- FORMAT: {format} ---");

                try
                {
                    object data = dataObject.GetData(format);
                    if (data == null)
                    {
                        results.AppendLine("  Data: null");
                    }
                    else
                    {
                        results.AppendLine($"  Type: {data.GetType().FullName}");
                        results.AppendLine($"  ToString(): {data.ToString()}");

                        // Format-specific analysis
                        AnalyzeSpecificFormat(format, data, results, dataObject);
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"  Error retrieving data: {ex.Message}");
                }

                results.AppendLine();
            }

            // Display results
            ResultsTextBox.AppendText(results.ToString());
            ResultsTextBox.AppendText("\n" + new string('=', 80) + "\n\n");
            ResultsTextBox.ScrollToEnd();
        }

        private void AnalyzeSpecificFormat(string format, object data, StringBuilder results, IDataObject dataObject)
        {
            switch (format.ToUpper())
            {
                case "TEXT":
                case "UNICODETEXT":
                case "OEMTEXT":
                    if (data is string text)
                    {
                        results.AppendLine($"  Length: {text.Length} characters");
                        results.AppendLine($"  Lines: {text.Split('\n').Length}");
                        if (text.Length <= 200)
                        {
                            results.AppendLine($"  Content: \"{text}\"");
                        }
                        else
                        {
                            results.AppendLine($"  Content (first 200 chars): \"{text.Substring(0, 200)}...\"");
                        }
                    }
                    break;

                case "FILES":
                case "FILEDROP":
                    if (data is string[] files)
                    {
                        results.AppendLine($"  File Count: {files.Length}");
                        foreach (string file in files)
                        {
                            results.AppendLine($"    File: {file}");
                            if (File.Exists(file))
                            {
                                var fileInfo = new FileInfo(file);
                                results.AppendLine($"      Size: {fileInfo.Length:N0} bytes");
                                results.AppendLine($"      Modified: {fileInfo.LastWriteTime}");
                                results.AppendLine($"      Extension: {fileInfo.Extension}");
                            }
                            else if (Directory.Exists(file))
                            {
                                results.AppendLine($"      Type: Directory");
                                try
                                {
                                    var dirInfo = new DirectoryInfo(file);
                                    var fileCount = dirInfo.GetFiles().Length;
                                    var dirCount = dirInfo.GetDirectories().Length;
                                    results.AppendLine($"      Contains: {fileCount} files, {dirCount} directories");
                                }
                                catch (Exception ex)
                                {
                                    results.AppendLine($"      Error reading directory: {ex.Message}");
                                }
                            }
                            else
                            {
                                results.AppendLine($"      Status: Path does not exist");
                            }
                        }
                    }
                    break;

                case "HTML FORMAT":
                case "TEXT/HTML":
                    if (data is string html)
                    {
                        results.AppendLine($"  HTML Length: {html.Length} characters");
                        if (html.Length <= 300)
                        {
                            results.AppendLine($"  HTML Content: {html}");
                        }
                        else
                        {
                            results.AppendLine($"  HTML Content (first 300 chars): {html.Substring(0, 300)}...");
                        }
                    }
                    break;

                case "RTF":
                    if (data is string rtf)
                    {
                        results.AppendLine($"  RTF Length: {rtf.Length} characters");
                        if (rtf.Length <= 200)
                        {
                            results.AppendLine($"  RTF Content: {rtf}");
                        }
                        else
                        {
                            results.AppendLine($"  RTF Content (first 200 chars): {rtf.Substring(0, 200)}...");
                        }
                    }
                    break;

                case "BITMAP":
                case "DIB":
                    try
                    {
                        // Try to get as BitmapSource (WPF native)
                        if (data is BitmapSource bitmapSource)
                        {
                            results.AppendLine($"  Image Size: {bitmapSource.PixelWidth}x{bitmapSource.PixelHeight}");
                            results.AppendLine($"  DPI: {bitmapSource.DpiX}x{bitmapSource.DpiY}");
                            results.AppendLine($"  Format: {bitmapSource.Format}");
                        }
                        else if (data is System.Windows.Interop.InteropBitmap interopBitmap)
                        {
                            results.AppendLine($"  Image Size: {interopBitmap.PixelWidth}x{interopBitmap.PixelHeight}");
                            results.AppendLine($"  DPI: {interopBitmap.DpiX}x{interopBitmap.DpiY}");
                            results.AppendLine($"  Format: {interopBitmap.Format}");
                        }
                        else
                        {
                            // Try to convert to BitmapSource
                            var bitmap = dataObject.GetData(DataFormats.Bitmap);
                            if (bitmap is BitmapSource wpfBitmap)
                            {
                                results.AppendLine($"  Image Size: {wpfBitmap.PixelWidth}x{wpfBitmap.PixelHeight}");
                                results.AppendLine($"  DPI: {wpfBitmap.DpiX}x{wpfBitmap.DpiY}");
                                results.AppendLine($"  Format: {wpfBitmap.Format}");
                            }
                            else
                            {
                                results.AppendLine($"  Bitmap data type: {data?.GetType().FullName ?? "null"}");
                                results.AppendLine($"  Unable to extract image details - may need System.Drawing.Common package");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"  Error analyzing bitmap: {ex.Message}");
                    }
                    break;

                case "FILECONTENTS":
                case "FILECONTENTSW":
                    if (data is Stream fileStream)
                    {
                        results.AppendLine($"  Stream Type: {fileStream.GetType().Name}");
                        results.AppendLine($"  Length: {fileStream.Length} bytes");
                        results.AppendLine($"  Position: {fileStream.Position}");
                        results.AppendLine($"  Can Read: {fileStream.CanRead}");
                        results.AppendLine($"  Can Seek: {fileStream.CanSeek}");
                    }
                    else if (data is byte[] bytes)
                    {
                        results.AppendLine($"  Byte Array Length: {bytes.Length}");
                        if (bytes.Length <= 50)
                        {
                            results.AppendLine($"  Bytes: {BitConverter.ToString(bytes)}");
                        }
                        else
                        {
                            results.AppendLine($"  First 50 bytes: {BitConverter.ToString(bytes.Take(50).ToArray())}");
                        }
                    }
                    break;

                default:
                    // Generic analysis for unknown formats
                    if (data is string str)
                    {
                        results.AppendLine($"  String Length: {str.Length}");
                        if (str.Length <= 200)
                        {
                            results.AppendLine($"  Content: \"{str}\"");
                        }
                        else
                        {
                            results.AppendLine($"  Content (first 200 chars): \"{str.Substring(0, 200)}...\"");
                        }
                    }
                    else if (data is byte[] genericBytes)
                    {
                        results.AppendLine($"  Byte Array Length: {genericBytes.Length}");
                        if (genericBytes.Length <= 20)
                        {
                            results.AppendLine($"  Bytes: {BitConverter.ToString(genericBytes)}");
                        }
                    }
                    else if (data is Stream dataStream)
                    {
                        results.AppendLine($"  Stream Type: {dataStream.GetType().Name}");
                        results.AppendLine($"  Length: {dataStream.Length} bytes");
                        results.AppendLine($"  Position: {dataStream.Position}");
                        results.AppendLine($"  Can Read: {dataStream.CanRead}");
                        results.AppendLine($"  Can Seek: {dataStream.CanSeek}");
                        results.AppendLine($"  Can Write: {dataStream.CanWrite}");

                        // Try to peek at the first few bytes if possible
                        if (dataStream.CanSeek && dataStream.CanRead && dataStream.Length > 0)
                        {
                            try
                            {
                                long originalPosition = dataStream.Position;
                                dataStream.Position = 0;

                                byte[] buffer = new byte[Math.Min(32, (int)dataStream.Length)];
                                int bytesRead = dataStream.Read(buffer, 0, buffer.Length);

                                if (bytesRead > 0)
                                {
                                    results.AppendLine($"  First {bytesRead} bytes: {BitConverter.ToString(buffer, 0, bytesRead)}");

                                    // Try to interpret as text if it looks like text
                                    if (IsLikelyText(buffer, bytesRead))
                                    {
                                        string textContent = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                        results.AppendLine($"  As Text: \"{textContent.Replace("\r", "\\r").Replace("\n", "\\n")}\"");
                                    }
                                }

                                dataStream.Position = originalPosition; // Restore position
                            }
                            catch (Exception ex)
                            {
                                results.AppendLine($"  Error reading stream content: {ex.Message}");
                            }
                        }
                    }
                    else if (data is Array array)
                    {
                        results.AppendLine($"  Array Type: {array.GetType().GetElementType()?.Name}");
                        results.AppendLine($"  Array Length: {array.Length}");
                        if (array.Length <= 10)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                results.AppendLine($"    [{i}]: {array.GetValue(i)}");
                            }
                        }
                    }
                    else
                    {
                        results.AppendLine($"  Object Properties:");
                        var props = data.GetType().GetProperties();

                        // Filter out problematic properties that always throw exceptions
                        var safeProps = props.Where(p =>
                            p.Name != "ReadTimeout" &&
                            p.Name != "WriteTimeout" &&
                            p.CanRead &&
                            p.GetIndexParameters().Length == 0) // Skip indexed properties
                            .Take(10);

                        foreach (var prop in safeProps)
                        {
                            try
                            {
                                var value = prop.GetValue(data);
                                results.AppendLine($"    {prop.Name}: {value}");
                            }
                            catch (Exception ex)
                            {
                                results.AppendLine($"    {prop.Name}: Error - {ex.Message}");
                            }
                        }
                    }
                    break;
            }
        }

        private bool IsLikelyText(byte[] buffer, int length)
        {
            // Simple heuristic: if most bytes are printable ASCII or common UTF-8, treat as text
            int printableCount = 0;
            for (int i = 0; i < length; i++)
            {
                byte b = buffer[i];
                if ((b >= 32 && b <= 126) || b == 9 || b == 10 || b == 13) // Printable ASCII + tab/newline/CR
                {
                    printableCount++;
                }
                else if (b >= 128) // Potential UTF-8
                {
                    printableCount++; // Give benefit of doubt for extended chars
                }
            }
            return printableCount >= length * 0.7; // If 70%+ looks printable
        }

        private void DisplayError(string message)
        {
            ResultsTextBox.AppendText($"ERROR: {message}\n");
            ResultsTextBox.AppendText(new string('=', 80) + "\n\n");
            ResultsTextBox.ScrollToEnd();
        }

        private void CopyResults_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultsTextBox.Text))
            {
                Clipboard.SetText(ResultsTextBox.Text);
                StatusLabel.Content = "Results copied to clipboard!";

                // Clear status after 3 seconds
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, args) =>
                {
                    StatusLabel.Content = "Ready";
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private void SaveResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = "txt",
                    FileName = $"DataTransferAnalysis_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, ResultsTextBox.Text);
                    StatusLabel.Content = $"Results saved to {Path.GetFileName(saveDialog.FileName)}";
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Error saving file: {ex.Message}");
            }
        }
    }
}