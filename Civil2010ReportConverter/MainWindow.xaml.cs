using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Civil2010ReportConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string _fileToOpen;
        public string _fileToSave;

        public const int htmlTableWidth = 15;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInFile(_fileToOpen))
            {

                List<string> linesFromFile = GetLinesFromFile(_fileToOpen, out int numberOfFetchedLines);

                List<string> listOfStations = GetNumberOfStations(linesFromFile);

                string[,] matrixFromSource = new string[numberOfFetchedLines, htmlTableWidth];

                matrixFromSource = GetCleanMatrix(linesFromFile, numberOfFetchedLines, htmlTableWidth);

                List<DataPoint> points = GetPointsFromMatrix(matrixFromSource, numberOfFetchedLines, htmlTableWidth, listOfStations);

                //SaveNXYH(points, "test.txt");


                if (CheckOutFile(_fileToSave))
                {
                    try
                    {
                        SaveFull(points, _fileToSave);

                        string msg = "Operation completed.";
                        MessageBoxResult result = MessageBox.Show(msg,
                        "Operation completed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("Unexpected error. Failed to save file.");
                    }
                }






               // Console.WriteLine("End of operation");

               // Console.ReadLine();
            }

        }

        private void btnSelect_Click_1(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Html files (*.html)|*.html|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _fileToOpen = openFileDialog.FileName;
                _fileToSave = Regex.Replace(openFileDialog.FileName, ".html", ".txt"); 
                txtSelect.Text = _fileToOpen;
                txtOutput.Text = _fileToSave;
            }



            //File.ReadAllText(openFileDialog.FileName)
        }


        private bool CheckInFile(string fileName)
        {
            if (_fileToOpen=="" || _fileToOpen == null || _fileToSave == "" || _fileToSave ==null)
            {
                return false;
            }
            return true;
        }

        private bool CheckOutFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                string msg = "File already exists. Do you want to overwrite it?";
                MessageBoxResult result = MessageBox.Show(msg,
                "Operation completed", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public string[,] TransposeStringMatrix(string[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            string[,] result = new string[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }


        public static List<string> GetLinesFromFile(string filename, out int noOflines)
        {
            List<string> results = new List<string>();

            // Read data from file.
            using (StreamReader sr = File.OpenText(filename))
            {
                string input = null;
                int i = 0;
                while ((input = sr.ReadLine()) != null)
                {
                    results.Add(input);
                    i++;
                }
                noOflines = i;
            }
            return results;
        }

        public static void SaveNXYH(List<DataPoint> pointList, string filename)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                foreach (DataPoint p in pointList)
                {
                    writer.Write(p.Number + "\t");
                    writer.Write(p.Easting + "\t");
                    writer.Write(p.Northing + "\t");
                    writer.Write(p.Elevation);
                    writer.Write(writer.NewLine);
                }
            }
        }

        public static void SaveFull(List<DataPoint> pointList, string filename)
        {
            using (StreamWriter writer = File.CreateText(filename))
            {
                foreach (DataPoint p in pointList)
                {
                    writer.Write(p.Number + "\t");
                    writer.Write(p.Easting + "\t");
                    writer.Write(p.Northing + "\t");
                    writer.Write(p.Elevation + "\t");
                    writer.Write(p.Station + "\t");
                    writer.Write(p.Offset + "\t");
                    writer.Write(p.Slope + "\t");
                    writer.Write(p.Code);
                    writer.Write(writer.NewLine);
                }
            }
        }

        public static List<string> GetNumberOfStations(List<string> input)
        {
            List<string> result = new List<string>();

            foreach (string item in input)
            {
                string newItem = null;

                if (item != null)
                {
                    newItem = Regex.Replace(item, "\t", "");
                    newItem = Regex.Replace(newItem, ",", "");
                    newItem = Regex.Replace(newItem, "\\+", "");
                    newItem = Regex.Replace(newItem, "m", "");

                    Regex r1 = new Regex("Station:", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches1 = r1.Matches(newItem);

                    foreach (Match match in matches1)
                    {
                        result.Add(newItem);
                        //Console.Write(match.Groups[1].ToString());
                        //Console.WriteLine();
                    }
                }
            }

            // clean list of stations            
            for (int j = 0; j < result.Count; j++)
            {
                result[j] = result[j].Replace("Station: ", "");
            }

            return result;
        }

        public static string[,] GetCleanMatrix(List<string> input, int linesNumber, int tableWidth)
        {
            string[,] cleanStrings = new string[linesNumber, tableWidth];
            string[,] cleanStringsCurated = new string[linesNumber, tableWidth];

            int i = 0;
            foreach (string item in input)
            {
                string newItem = null;

                if (item != null)
                {
                    newItem = Regex.Replace(item, "\t", "");
                    newItem = Regex.Replace(newItem, ",", "");
                    newItem = Regex.Replace(newItem, "\\+", "");
                    newItem = Regex.Replace(newItem, "m", "");

                    Regex r2 = new Regex("<td.*?>(.*?)</td>", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection matches2 = r2.Matches(newItem);

                    int j = 0;
                    foreach (Match match in matches2)
                    {
                        cleanStrings[i, j] = match.Groups[1].ToString();
                        //Console.Write(cleanStrings[i, j] + " ");
                        j++;
                    }

                    i++;
                    //Console.WriteLine();
                }
            }

            for (int x = 0; x < linesNumber; x++)
            {
                for (int y = 0; y < tableWidth; y++)
                {
                    if (cleanStrings[x, 0] == "Code" ||
                        cleanStrings[x, 0] == "Offset" ||
                        cleanStrings[x, 0] == "Elevation" ||
                        cleanStrings[x, 0] == "Slope" ||
                        cleanStrings[x, 0] == "Easting" ||
                        cleanStrings[x, 0] == "Northing")
                        if (cleanStrings[x, y] != "&nbsp;")
                            cleanStringsCurated[x, y] = cleanStrings[x, y];
                }
            }

            return cleanStringsCurated;
        }

        public static List<DataPoint> GetPointsFromMatrix(string[,] input, int linesNumber, int tableWidth, List<string> stations)
        {
            int pointCounter = 0;
            int stationCounter = 0;

            List<DataPoint> newPoints = new List<DataPoint>();

            for (int x = 0; x < linesNumber; x++)
            {

                if (input[x, 0] == "Offset")
                {
                    for (int y = 0; y < tableWidth; y++)
                    {
                        if (double.TryParse(input[x, y], out _))
                        {
                            pointCounter++;
                            DataPoint p = new DataPoint();
                            p.Number = pointCounter;
                            p.Station = stations[stationCounter];
                            p.Code = input[(x - 2), y] ?? input[(x - 2), (y + 1)];
                            p.Offset = input[x, y];
                            p.Elevation = input[(x + 2), y];
                            p.Slope = input[(x + 4), y];
                            p.Easting = input[(x + 6), y];
                            p.Northing = input[(x + 8), y];
                            newPoints.Add(p);
                        }
                    }

                    stationCounter++;
                }
            }

            return newPoints;
        }

        private void btnOutput_Click(object sender, RoutedEventArgs e)
        {
            CheckOutFile(_fileToSave);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                _fileToSave = saveFileDialog.FileName;
                txtOutput.Text = _fileToSave;
            }
        }

        private void txtOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _fileToSave = txtOutput.Text;
        }

        private void txtSelect_TextChanged(object sender, TextChangedEventArgs e)
        {
            _fileToOpen = txtSelect.Text;
        }
    }
}

