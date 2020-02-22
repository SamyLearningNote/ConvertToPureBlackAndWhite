/*
 * MIT License
 * 
 * Copyright (c) 2020 SamyLearningNote
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * GitHub:
 * https://github.com/SamyLearningNote
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvertToPureBlackAndWhite
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        String folderPath;
        String[] filePath;
        System.Drawing.Color pixelColor;
        System.Drawing.Color[,] convertedColor;
        int[,] redArray;
        int[,] greenArray;
        int[,] blueArray;
        Bitmap myBitmap;
        Bitmap convertedBitmap;

        string outputFilePath;

        string[] errorFileList = { };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ThresholdUpDownControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                filePath = Directory.GetFiles(FolderPathTextBox.Text, "*", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Cannot read the folder, please make sure the path is a folder path");
                return;
            }
            for (int i = 0; i < filePath.Length; i++)
            {
                LoadFile(filePath[i]);
                // update the progressbar
                double progressValue = ((i+1) * 1.0 / filePath.Length) * 100;
                ProcessingProgressBar.Value = progressValue;

                System.Windows.Forms.Application.DoEvents();

            }
            if (errorFileList.Length > 0)
            {
                string errorFiles = "";
                for (int filePointer = 0; filePointer < errorFileList.Length; filePointer++)
                {
                    errorFiles += "\n" + errorFileList[filePointer];
                }

                System.Windows.Forms.MessageBox.Show("The following files cannot be converted, please make sure they are image file and try again later:\n" + errorFiles);
            }
            System.Windows.Forms.MessageBox.Show("The conversion is done");
        }
        
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            var listOfStrings = new List<string>();
            errorFileList = listOfStrings.ToArray();

            DialogResult result = folderDialog.ShowDialog(); // Show the dialog.
            if (result.ToString().Equals("OK"))
            {
                folderPath = folderDialog.SelectedPath;
                FolderPathTextBox.Text = folderPath;
            }
        }

        bool LoadFile(string filePath)
        {
            try
            {
                // Reference: https://stackoverflow.com/questions/10127871/how-can-i-read-image-pixels-values-as-rgb-into-2d-array
                myBitmap = new Bitmap(filePath);

                redArray = new int[myBitmap.Width, myBitmap.Height];
                greenArray = new int[myBitmap.Width, myBitmap.Height];
                blueArray = new int[myBitmap.Width, myBitmap.Height];

                convertedBitmap = new Bitmap(myBitmap.Width, myBitmap.Height);

                System.IO.Directory.CreateDirectory(folderPath + @"\Converted\");

                outputFilePath = folderPath + @"\Converted\" + GetFileName(filePath);

                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        // Get the color of a pixel within myBitmap.
                        pixelColor = myBitmap.GetPixel(x, y);
                        int averageValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        if (averageValue >= ThresholdUpDownControl.Value)
                        {
                            // convert to white
                            convertedBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            // convert to black
                            convertedBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, 0));
                        }
                    }
                }

                convertedBitmap.Save(outputFilePath, ImageFormat.Png);
            }
            catch (Exception e)
            {
                Array.Resize(ref errorFileList, errorFileList.Length + 1);
                errorFileList[errorFileList.Length - 1] = filePath;
                return false;
            }
            return true;
        }

        string GetFileName(string originalFilePath)
        {
            string[] pathSection = originalFilePath.Split('\\');
            string[] saperatedFileName = pathSection[pathSection.Length - 1].Split('.');
            return saperatedFileName[0] + ".png";
        }

    }
}
