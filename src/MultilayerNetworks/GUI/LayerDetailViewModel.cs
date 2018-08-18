using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultilayerNetworks.Components;
using MultilayerNetworks.Measures;
using MultilayerNetworks.Transformation;
using MultilayerNetworks;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System;
using System.Text.RegularExpressions;

namespace GUI
{
    /// <summary>
    /// ViewModel for layer detail window.
    /// </summary>
    public class LayerDetailViewModel : INotifyPropertyChanged
    {
        #region Properties
        private bool[] _modeArray = new bool[] { false, true, false, false };
        private int exportFlag;
        private const int cumulativeOffset = 6;
        public bool[] ModeArray
        {
            get { return _modeArray; }
        }
        private int selectedLegendIndex
        {
            get { return Array.IndexOf(_modeArray, true); }
        }

        private HashSet<Layer> selectedLayers;

        private ObservableCollection<Layer> layers;

        // ML network.
        private MultilayerNetwork multilayerNetwork;

        // Plots.
        private PlotModel layerPlotModel;
        public string PlotTitle { get; private set; }
        public List<DataPoint> PlotPoints { get; private set; }
        private bool logX;
        private bool logY;
        private int selectedPlotIndex;
        private bool cumulative;

        private ConcurrentDictionary<int, ConcurrentDictionary<double, double>[]> layerDistributions;
        private string[][] distributionLabels = new string[12][];

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public int SelectedPlotIndex
        {
            get
            {
                return selectedPlotIndex;
            }
            private set
            {
                selectedPlotIndex = value;
                OnPropertyChanged("SelectedPlotIndex");
            }
        }

        public ObservableCollection<Layer> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                layers = value;
                OnPropertyChanged("Layers");
            }
        }

        public bool Cumulative
        {
            get
            {
                return cumulative;
            }
            private set
            {
                cumulative = value;
                OnPropertyChanged("Cumulative");
            }
        }

        public bool LogY
        {
            get
            {
                return logY;
            }
            private set
            {
                logY = value;
                OnPropertyChanged("LogY");
            }
        }

        public bool LogX
        {
            get
            {
                return logX;
            }
            private set
            {
                logX = value;
                OnPropertyChanged("LogX");
            }
        }

        public void PlotIndexChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            if ((sender as System.Windows.Controls.ComboBox).Items.Count > 0 && (sender as System.Windows.Controls.ComboBox).SelectedValue != null)
            {
                selectedPlotIndex = (int)(sender as System.Windows.Controls.ComboBox).SelectedIndex;
                Plot(selectedPlotIndex, logX, logY);
            }

        }

        #endregion Properties

        /// <summary>
        /// Constructor for layerdetailviewmodel.
        /// </summary>
        /// <param name="layerDistributions">Distributions for each layer.</param>
        /// <param name="multilayerNetwork">Multilayer network model.</param>
        public LayerDetailViewModel(ConcurrentDictionary<int, ConcurrentDictionary<double, double>[]> layerDistributions, MultilayerNetwork multilayerNetwork)
        {
            this.layerDistributions = layerDistributions;
            this.multilayerNetwork = multilayerNetwork;
            Layers = new ObservableCollection<Layer>(this.multilayerNetwork.Layers);
            selectedLayers = new HashSet<Layer>();
            cumulative = true;

            // Labels for distributions.
            distributionLabels[0] = new string[3] { "Degree distribution", "degree", "count" };
            distributionLabels[1] = new string[3] { "Clustering coefficient distribution", "clustering coefficient", "count" };
            distributionLabels[2] = new string[3] { "Average clustering coefficient by degree", "degree", "average CC" };
            distributionLabels[3] = new string[3] { "Relevance distribution", "relevance", "count" };
            distributionLabels[4] = new string[3] { "Exclusive neighborhood distribution", "exclusive neighborhood", "count" };
            distributionLabels[5] = new string[3] { "Exclusive relevance distribution", "exclusive relevance", "count" };

            distributionLabels[6] = new string[3] { "Cumulative degree distribution", "relative degree", "relative cumulative frequency" };
            distributionLabels[7] = new string[3] { "Cumulative CC distribution", "relative CC", "relative cumulative frequency" };
            distributionLabels[8] = new string[3] { "Cumulative average CC by degree", "relative degree", "rel cumulative average cc" };
            distributionLabels[9] = new string[3] { "Cumulative relevance distribution", "relative relevance", "relative cumulative frequency" };
            distributionLabels[10] = new string[3] { "Cumulative exclusive neighborhood\r\ndistribution", "relative exclusive neighborhood", "relative cumulative frequency" };
            distributionLabels[11] = new string[3] { "Cumulative exclusive relevance\r\n distribution", "relative exclusive relevance", "relative cumulative frequency" };
        }


        /// <summary>
        /// Exports distribution as csv format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartToCsv(object sender, RoutedEventArgs e)
        {
            if (LayerPlotModel == null)
                return;

            exportFlag = 0;
            ExportIntoDirectory();
        }

        public void ChartToPng(object sender, RoutedEventArgs e)
        {
            if (LayerPlotModel == null)
                return;

            exportFlag = 1;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Exports plots or csv into directory.
        /// </summary>
        private void ExportIntoDirectory()
        {
            // Flag 0 == Summary
            // Flag 1 == Plot PNG
            // Flag 2 == Plot PDF
            var exportFolderName = "multinet_export";
            var plotFolder = "plots";
            var summaryFolder = "summary";
            var networkDir = multilayerNetwork.Name;

            // Export directory.
            if (!Directory.Exists(exportFolderName))
                Directory.CreateDirectory(exportFolderName);

            // Network directory.
            if (!Directory.Exists(@exportFolderName + "\\" + networkDir))
                Directory.CreateDirectory(@exportFolderName + "\\" + networkDir);

            // Plot directory.
            if (!Directory.Exists(@exportFolderName + "\\" + networkDir + "\\" + plotFolder))
                Directory.CreateDirectory(@exportFolderName + "\\" + networkDir + "\\" + plotFolder);

            // Summary directory.
            if (!Directory.Exists(@exportFolderName + "\\" + networkDir + "\\" + summaryFolder))
                Directory.CreateDirectory(@exportFolderName + "\\" + networkDir + "\\" + summaryFolder);


            // CSV.
            if (exportFlag == 0)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0].Replace(' ', '_'); 
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_'); 

                var csv = new StringBuilder();

                foreach (var item in selectedLayers)
                {
                    var sorted = layerDistributions[item.Id][selectedPlotIndex].Keys.ToList();
                    sorted.Sort();
                    csv.AppendLine(String.Format("###{0}###", item.Name));
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        csv.AppendLine(String.Format("{0};{1}", sorted[i], layerDistributions[item.Id][selectedPlotIndex][sorted[i]]));

                    }
                }


                File.WriteAllText(@exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\" + multilayerNetwork.Name + "_layers" + "_" + Regex.Replace(dist, @"\t|\n|\r", "_") + ".csv", csv.ToString());
            }
            // PNG.
            else if (exportFlag == 1)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0].Replace(' ', '_'); 
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_'); 

                var fileName = Regex.Replace(multilayerNetwork.Name + "_layers" + "_" + dist + ".PNG", @"\t|\n|\r", "_");
                var path = @exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\";

                using (var fileStream = File.Create(path + fileName))
                {
                    var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 1920, Height = 1700, Background = OxyColors.White };
                    pngExporter.Export(GetExportPlotModel(selectedPlotIndex, logX, logY), fileStream);
                }
            }
            // PDF.
            else if (exportFlag == 2)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0].Replace(' ', '_'); 
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_'); 

                var fileName = Regex.Replace(multilayerNetwork.Name + "_layers" + "_" + dist + ".PDF", @"\t|\n|\r", "_");
                var path = @exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\";

                using (var stream = File.Create(path + fileName))
                {
                    var pdfExporter = new PdfExporter { Width = 1920, Height = 1700 };
                    pdfExporter.Export(GetExportPlotModel(selectedPlotIndex, logX, logY), stream);
                }
            }
            // SVG.
            else if (exportFlag == 3)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0].Replace(' ', '_');
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_');

                var fileName = Regex.Replace(multilayerNetwork.Name + "_layers" + "_" + dist + ".SVG", @"\t|\n|\r", "_");
                var path = @exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\";

                using (var stream = File.Create(path + fileName))
                {
                    var exporter = new SvgExporter { Width = 600, Height = 400 };
                    exporter.Export(layerPlotModel, stream);
                }
            }

        }


        /// <summary>
        /// Exports chart to pdf.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartToPdf(object sender, RoutedEventArgs e)
        {
            if (LayerPlotModel == null)
                return;

            exportFlag = 2;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Checkbox for layer plot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void plotLayerChck(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var layer = checkbox.DataContext as Layer;

            if ((bool)checkbox.IsChecked)
                selectedLayers.Add(layer);
            else
                selectedLayers.Remove(layer);
        }

        /// <summary>
        /// Plot button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlotBtnClick(object sender, RoutedEventArgs e)
        {
            Plot(selectedPlotIndex, logX, logY);
        }

        /// <summary>
        /// Cumulative toggle button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CumulativeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Cumulative = (sender as MenuItem).IsChecked;
        }

        /// <summary>
        /// Log X axis toggle button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LogXChck_Checked(object sender, RoutedEventArgs e)
        {
            LogX = (sender as MenuItem).IsChecked;
        }

        /// <summary>
        /// Log Y axis toggle button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LogYChck_Checked(object sender, RoutedEventArgs e)
        {
            LogY = (sender as MenuItem).IsChecked;
        }

        /// <summary>
        /// Plot model.
        /// </summary>
        public PlotModel LayerPlotModel
        {
            get
            {
                return layerPlotModel;
            }
            private set
            {
                layerPlotModel = value;
                OnPropertyChanged("LayerPlotModel");
            }
        }

        /// <summary>
        /// Plots distribution.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if logx.</param>
        /// <param name="logY">True if logy.</param>
        public PlotModel GetExportPlotModel(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (layerDistributions.Any(x => x.Value[distributionIndex].Count == 0))
                return null;

            // Size export
            var axisFontSize = 68;
            var titleFontSize = 68;
            var legendFontSize = 68;

            var minorGridlineThickness = 3;
            var majorGridlineThickness = 3;

            var minMinorStep = 0.1;
            var minMajorStep = 0.5;
            var minTickSize = 20;
            var maxTickSize = 40;

            var paddingOxyThickness = 10;
            var borderOxyThickness = 2;
            var titlePadding = 50;

            var legendSymbolLen = 100;
            var legendPadding = 50;

            var strokeThickness = 8;
            var markerSize = 8;

            // Size app
            //var axisFontSize = 18;
            //var titleFontSize = 18;
            //var legendFontSize = 18;

            //var minorGridlineThickness = 1;
            //var majorGridlineThickness = 1;

            //var minMinorStep = 0.1;
            //var minMajorStep = 0.5;
            //var minTickSize = 5;
            //var maxTickSize = 10;

            //var paddingOxyThickness = 10;
            //var borderOxyThickness = 1;
            //var titlePadding = 20;

            //var legendSymbolLen = 20;
            //var legendPadding = 10;

            //var strokeThickness = 3;
            //var markerSize = 3;

            var legendPosition = new LegendPosition[]
            {
                OxyPlot.LegendPosition.LeftTop,
                OxyPlot.LegendPosition.RightTop,
                OxyPlot.LegendPosition.LeftBottom,
                OxyPlot.LegendPosition.RightBottom
            };

            var exportLayerPlotModel = new PlotModel();

            if (cumulative)
                exportLayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                exportLayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //LayerPlotModel.LegendBorderThickness = 2;
            exportLayerPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            exportLayerPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            exportLayerPlotModel.TitlePadding = titlePadding;
            exportLayerPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            exportLayerPlotModel.LegendTitle = "Layer";
            exportLayerPlotModel.LegendTitleFontSize = legendFontSize;
            exportLayerPlotModel.LegendPosition = legendPosition[selectedLegendIndex];
            exportLayerPlotModel.LegendSymbolLength = legendSymbolLen;
            exportLayerPlotModel.LegendPadding = legendPadding;

            Axis axisX = null;
            Axis axisY = null;

            exportLayerPlotModel.LegendFontSize = legendFontSize;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex + cumulativeOffset][1], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            if (logY)
            {
                axisY = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logY)
            {
                if (cumulative)
                {
                    axisY = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex + cumulativeOffset][2], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisY = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            exportLayerPlotModel.Axes.Add(axisX);
            exportLayerPlotModel.Axes.Add(axisY);

            // Cumulative distribution.
            if (cumulative)
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = item.Name };
                    exportLayerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    var sum = 0.0;
                    double maxX = layerDistributions[item.Id][distributionIndex].Keys.Max();
                    double maxY = layerDistributions[item.Id][distributionIndex].Values.Sum();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sum += layerDistributions[item.Id][distributionIndex][sorted[i]];

                        double x = logX == true ? sorted[i] : sorted[i] / maxX;
                        double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                        x = maxX == 0 ? 0 : x;
                        y = maxY == 0 ? 0 : y;

                        mySeries.Points.Add(new DataPoint(x, y));
                    }
                }
            }
            // Just distribution.
            else
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Bold, FontSize = legendFontSize, Title = item.Name };
                    exportLayerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        mySeries.Points.Add(new DataPoint(sorted[i], layerDistributions[item.Id][distributionIndex][sorted[i]]));
                    }
                }

            }



            //exportLayerPlotModel.InvalidatePlot(true);
            return exportLayerPlotModel;
        }

        /// <summary>
        /// Plots distribution.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if logx.</param>
        /// <param name="logY">True if logy.</param>
        public void Plot_(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (layerDistributions.Any(x => x.Value[distributionIndex].Count == 0))
                return;

            var axisFontSize = 68;
            var titleFontSize = 68;
            var legendFontSize = 68;

            var legendPosition = new LegendPosition[] 
            {
                OxyPlot.LegendPosition.LeftTop,
                OxyPlot.LegendPosition.RightTop,
                OxyPlot.LegendPosition.LeftBottom,
                OxyPlot.LegendPosition.RightBottom
            };

            if (cumulative)
                LayerPlotModel = new PlotModel { Title = "Cumulative rw betweenness/betweenness", TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold };
            else
                LayerPlotModel = new PlotModel { Title = "rw betweenness/betweenness", TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold };

            //LayerPlotModel.LegendBorderThickness = 2;
            LayerPlotModel.PlotAreaBorderThickness = new OxyThickness(2);

            LayerPlotModel.Padding = new OxyThickness(10);
            LayerPlotModel.TitlePadding = 50;
            LayerPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            LayerPlotModel.LegendTitle = "Layer";
            LayerPlotModel.LegendTitleFontSize = legendFontSize;
            LayerPlotModel.LegendPosition = legendPosition[selectedLegendIndex];
            LayerPlotModel.LegendSymbolLength = 100;
            LayerPlotModel.LegendPadding = 50;

            Axis axisX = null;
            Axis axisY = null;

            LayerPlotModel.LegendFontSize = legendFontSize;

            var minorGridlineThickness = 3;
            var majorGridlineThickness = 3;

            var minMinorStep = 0.1;
            var minMajorStep = 0.5;
            var minTickSize = 20;
            var maxTickSize = 40;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = "relative betweenness", Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = "betweenness", Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            if (logY)
            {
                axisY = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logY)
            {
                if (cumulative)
                {
                    axisY = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex + cumulativeOffset][2], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisY = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep,
                        MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness,
                        MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left,
                        Title = "count", TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            LayerPlotModel.Axes.Add(axisX);
            LayerPlotModel.Axes.Add(axisY);

            var path = @"C:\Users\Honza\Desktop\R Export\florentine\betweenness.txt";
            var lines = File.ReadAllLines(path);
            var betweenness = new ConcurrentDictionary<double, double>();

            for (int i = 0; i < lines.Length; i++)
            {
                var key = Convert.ToDouble(lines[i]);
                if (betweenness.ContainsKey(key))
                {
                    betweenness[key] = betweenness[key] + 1;
                }
                else
                {
                    betweenness.TryAdd(key, 1);
                }
            }

            if (cumulative)
            {
                var toPlot = new ConcurrentDictionary<double, double>[2];
                toPlot[0] = layerDistributions[0][0];
                toPlot[1] = betweenness;

                var names = new string[] { "rw betweenness", "betweenness" };

                var j = 0;
                foreach (var item in toPlot)
                {

                    var mySeries = new LineSeries() { StrokeThickness = 8, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = 8, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize,  Title = names[j] };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = item.Keys.ToList();
                    sorted.Sort();

                    var sum = 0.0;
                    double maxX = item.Keys.Max();
                    double maxY = item.Values.Sum();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sum += item[sorted[i]];

                        double x = logX == true ? sorted[i] : sorted[i] / maxX;
                        double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                        x = maxX == 0 ? 0 : x;
                        y = maxY == 0 ? 0 : y;

                        mySeries.Points.Add(new DataPoint(x, y));
                    }
                    j++;
                }
            }
            else
            {
                var toPlot = new ConcurrentDictionary<double, double>[2];
                toPlot[0] = layerDistributions[0][0];
                toPlot[1] = betweenness;

                var names = new string[] { "rw betweenness", "betweenness" };

                var j = 0;
                foreach (var item in toPlot)
                {
                    var mySeries = new LineSeries() { StrokeThickness = 5, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = 5, FontWeight = OxyPlot.FontWeights.Bold, FontSize = legendFontSize, Title = names[j] };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = item.Keys.ToList();
                    sorted.Sort();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        mySeries.Points.Add(new DataPoint(sorted[i], item[sorted[i]]));
                    }
                    j++;
                }

            }



            layerPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Plots distribution.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if logx.</param>
        /// <param name="logY">True if logy.</param>
        public void Plot(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (layerDistributions.Any(x => x.Value[distributionIndex].Count == 0))
                return;

            // Size export
            //var axisFontSize = 68;
            //var titleFontSize = 68;
            //var legendFontSize = 68;

            //var minorGridlineThickness = 3;
            //var majorGridlineThickness = 3;

            //var minMinorStep = 0.1;
            //var minMajorStep = 0.5;
            //var minTickSize = 20;
            //var maxTickSize = 40;

            //var paddingOxyThickness = 10;
            //var borderOxyThickness = 2;
            //var titlePadding = 50;

            //var legendSymbolLen = 100;
            //var legendPadding = 50;

            //var strokeThickness = 8;
            //var markerSize = 8;

            // Size app
            var axisFontSize = 14;
            var titleFontSize = 14;
            

            var minorGridlineThickness = 1;
            var majorGridlineThickness = 1;

            var minMinorStep = 0.1;
            var minMajorStep = 0.5;
            var minTickSize = 5;
            var maxTickSize = 10;

            var paddingOxyThickness = 10;
            var borderOxyThickness = 1;
            var titlePadding = 20;

            var strokeThickness = 3;
            var markerSize = 3;

            var legendFontSize = 12;
            var legendSymbolLen = 10;
            var legendPadding = 8;

            var legendPosition = new LegendPosition[]
            {
                OxyPlot.LegendPosition.LeftTop,
                OxyPlot.LegendPosition.RightTop,
                OxyPlot.LegendPosition.LeftBottom,
                OxyPlot.LegendPosition.RightBottom
            };

            if (cumulative)
                LayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                LayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //LayerPlotModel.LegendBorderThickness = 2;
            LayerPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            LayerPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            LayerPlotModel.TitlePadding = titlePadding;
            LayerPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            LayerPlotModel.LegendTitle = "Layer";
            LayerPlotModel.LegendTitleFontSize = legendFontSize;
            LayerPlotModel.LegendPosition = legendPosition[selectedLegendIndex];
            LayerPlotModel.LegendSymbolLength = legendSymbolLen;
            LayerPlotModel.LegendPadding = legendPadding;

            Axis axisX = null;
            Axis axisY = null;

            LayerPlotModel.LegendFontSize = legendFontSize;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex + cumulativeOffset][1], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            if (logY)
            {
                axisY = new LogarithmicAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logY)
            {
                if (cumulative)
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex + cumulativeOffset][2], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            LayerPlotModel.Axes.Add(axisX);
            LayerPlotModel.Axes.Add(axisY);

            if (cumulative)
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = item.Name };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    var sum = 0.0;
                    double maxX = layerDistributions[item.Id][distributionIndex].Keys.Max();
                    double maxY = layerDistributions[item.Id][distributionIndex].Values.Sum();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sum += layerDistributions[item.Id][distributionIndex][sorted[i]];

                        double x = logX == true ? sorted[i] : sorted[i] / maxX;
                        double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                        x = maxX == 0 ? 0 : x;
                        y = maxY == 0 ? 0 : y;

                        mySeries.Points.Add(new DataPoint(x, y));
                    }
                }
            }
            else
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Bold, FontSize = legendFontSize, Title = item.Name };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        mySeries.Points.Add(new DataPoint(sorted[i], layerDistributions[item.Id][distributionIndex][sorted[i]]));
                    }
                }

            }



            layerPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Used for mass export.
        /// </summary>
        public void GeneratePlots()
        {
            exportFlag = 3;

            distributionLabels[SelectedPlotIndex + cumulativeOffset][0] = "occup_degree";
            GetExportPlot(0, 1, 1);
            ExportIntoDirectory();

            distributionLabels[SelectedPlotIndex + cumulativeOffset][0] = "rwbw";
            GetExportPlot(2, 3, 1);
            ExportIntoDirectory();

            distributionLabels[SelectedPlotIndex + cumulativeOffset][0] = "rwcl";
            GetExportPlot(4, 5, 1);
            ExportIntoDirectory();

        }

        /// <summary>
        /// Old method, used for mass export.
        /// </summary>
        public void GeneratePlots_()
        {
            //var legendPos = new int[6] { 1, 1, 1, 1, 1, 1 };


            //exportFlag = 3;

            //selectedLayers = new HashSet<Layer>(multilayerNetwork.Layers);

            //for (int i = 0; i < 6; i++)
            //{
            //    SelectedPlotIndex = i;
            //    GetExportPlot(i, legendPos[i]);
            //    ExportIntoDirectory();
            //}


        }

        /// <summary>
        /// Utility method for exporting plots.
        /// </summary>
        public void GetExportPlot(int distributionIndex, int distIndex2, int legendIndex, bool logX = false, bool logY = false)
        {
            if (layerDistributions.Any(x => x.Value[distributionIndex].Count == 0))
                return;

            var names = new string[] { "Cumulative occupation/degree","", "Cumulative rw betweenness/betweenness","", "Cumulative rw closeness/closeness" };
            var axesNames = new string[] { "Cumulative occupation/degree","", "Cumulative rw betweenness/betweenness","", "Cumulative rw closeness/closeness" };
            // Size export
            //var axisFontSize = 68;
            //var titleFontSize = 68;
            //var legendFontSize = 68;

            //var minorGridlineThickness = 3;
            //var majorGridlineThickness = 3;

            //var minMinorStep = 0.1;
            //var minMajorStep = 0.5;
            //var minTickSize = 20;
            //var maxTickSize = 40;

            //var paddingOxyThickness = 10;
            //var borderOxyThickness = 2;
            //var titlePadding = 50;

            //var legendSymbolLen = 100;
            //var legendPadding = 50;

            //var strokeThickness = 8;
            //var markerSize = 8;

            // Size app
            var axisFontSize = 18;
            var titleFontSize = 18;
            var legendFontSize = 18;

            var minorGridlineThickness = 1;
            var majorGridlineThickness = 1;

            var minMinorStep = 0.1;
            var minMajorStep = 0.5;
            var minTickSize = 5;
            var maxTickSize = 10;

            var paddingOxyThickness = 10;
            var borderOxyThickness = 1;
            var titlePadding = 20;

            var strokeThickness = 3;
            var markerSize = 3;

            var legendSymbolLen = 20;
            var legendPadding = 20;

            var legendPosition = new LegendPosition[]
            {
                OxyPlot.LegendPosition.LeftTop,
                OxyPlot.LegendPosition.RightTop,
                OxyPlot.LegendPosition.LeftBottom,
                OxyPlot.LegendPosition.RightBottom
            };

            if (cumulative)
                LayerPlotModel = new PlotModel
                {
                    Title = names[distributionIndex],//distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                LayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //LayerPlotModel.LegendBorderThickness = 2;
            LayerPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            LayerPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            LayerPlotModel.TitlePadding = titlePadding;
            LayerPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            LayerPlotModel.LegendTitle = "Distribution";
            LayerPlotModel.LegendTitleFontSize = legendFontSize;
            LayerPlotModel.LegendPosition = legendPosition[2];
            LayerPlotModel.LegendSymbolLength = legendSymbolLen;
            LayerPlotModel.LegendPadding = legendPadding;


            Axis axisX = null;
            Axis axisY = null;

            var xNames = new string[] { "relative occupation/degree", "", "relative betweenness","", "relative closeness" };

            LayerPlotModel.LegendFontSize = legendFontSize;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep,
                        MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness,
                        FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom,
                        Title = xNames[distributionIndex], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            if (logY)
            {
                axisY = new LogarithmicAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logY)
            {
                if (cumulative)
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep,
                        MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness,
                        FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left,
                        Title = "relative cumulative frequency", Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            LayerPlotModel.Axes.Add(axisX);
            LayerPlotModel.Axes.Add(axisY);

            if (cumulative)
            {
                var dists = new ConcurrentDictionary<double, double>[2];
                dists[0] = layerDistributions[0][distributionIndex];
                dists[1] = layerDistributions[0][distIndex2];

                var legendNames = new string[] { "occupation","degree", "rw betweenness","betweenness", "closeness", "rw closeness" };

                var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = legendNames[distributionIndex] };
                layerPlotModel.Series.Add(mySeries);

                var sorted = layerDistributions[0][distributionIndex].Keys.ToList();
                sorted.Sort();

                var sum = 0.0;
                double maxX = layerDistributions[0][distributionIndex].Keys.Max();
                double maxY = layerDistributions[0][distributionIndex].Values.Sum();

                for (int i = 0; i < sorted.Count; i++)
                {
                    sum += layerDistributions[0][distributionIndex][sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                    x = maxX == 0 ? 0 : x;
                    y = maxY == 0 ? 0 : y;

                    mySeries.Points.Add(new DataPoint(x, y));
                }

                mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = legendNames[distIndex2] };
                layerPlotModel.Series.Add(mySeries);

                sorted = layerDistributions[0][distIndex2].Keys.ToList();
                sorted.Sort();

                sum = 0.0;
                maxX = layerDistributions[0][distIndex2].Keys.Max();
                maxY = layerDistributions[0][distIndex2].Values.Sum();

                for (int i = 0; i < sorted.Count; i++)
                {
                    sum += layerDistributions[0][distIndex2][sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                    x = maxX == 0 ? 0 : x;
                    y = maxY == 0 ? 0 : y;

                    mySeries.Points.Add(new DataPoint(x, y));
                }


                //foreach (var item in dists)
                //{


                //    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = name };
                //    layerPlotModel.Series.Add(mySeries);

                //    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                //    sorted.Sort();

                //    var sum = 0.0;
                //    double maxX = layerDistributions[item.Id][distributionIndex].Keys.Max();
                //    double maxY = layerDistributions[item.Id][distributionIndex].Values.Sum();

                //    for (int i = 0; i < sorted.Count; i++)
                //    {
                //        sum += layerDistributions[item.Id][distributionIndex][sorted[i]];

                //        double x = logX == true ? sorted[i] : sorted[i] / maxX;
                //        double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                //        x = maxX == 0 ? 0 : x;
                //        y = maxY == 0 ? 0 : y;

                //        mySeries.Points.Add(new DataPoint(x, y));
                //    }
                //}
            }
            else
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Bold, FontSize = legendFontSize, Title = item.Name };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        mySeries.Points.Add(new DataPoint(sorted[i], layerDistributions[item.Id][distributionIndex][sorted[i]]));
                    }
                }

            }
        }

        /// <summary>
        /// Old utility method for exporting plots.
        /// </summary>
        public void GetExportPlot_(int distributionIndex, int legendIndex, bool logX = false, bool logY = false)
        {
            if (layerDistributions.Any(x => x.Value[distributionIndex].Count == 0))
                return;


            // Size export
            //var axisFontSize = 68;
            //var titleFontSize = 68;
            //var legendFontSize = 68;

            //var minorGridlineThickness = 3;
            //var majorGridlineThickness = 3;

            //var minMinorStep = 0.1;
            //var minMajorStep = 0.5;
            //var minTickSize = 20;
            //var maxTickSize = 40;

            //var paddingOxyThickness = 10;
            //var borderOxyThickness = 2;
            //var titlePadding = 50;

            //var legendSymbolLen = 100;
            //var legendPadding = 50;

            //var strokeThickness = 8;
            //var markerSize = 8;

            // Size app
            var axisFontSize = 18;
            var titleFontSize = 18;
            var legendFontSize = 18;

            var minorGridlineThickness = 1;
            var majorGridlineThickness = 1;

            var minMinorStep = 0.1;
            var minMajorStep = 0.5;
            var minTickSize = 5;
            var maxTickSize = 10;

            var paddingOxyThickness = 10;
            var borderOxyThickness = 1;
            var titlePadding = 20;

            var strokeThickness = 3;
            var markerSize = 3;

            var legendSymbolLen = 20;
            var legendPadding = 20;

            var legendPosition = new LegendPosition[]
            {
                OxyPlot.LegendPosition.LeftTop,
                OxyPlot.LegendPosition.RightTop,
                OxyPlot.LegendPosition.LeftBottom,
                OxyPlot.LegendPosition.RightBottom
            };

            if (cumulative)
                LayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                LayerPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //LayerPlotModel.LegendBorderThickness = 2;
            LayerPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            LayerPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            LayerPlotModel.TitlePadding = titlePadding;
            LayerPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            LayerPlotModel.LegendTitle = "Layer";
            LayerPlotModel.LegendTitleFontSize = legendFontSize;
            LayerPlotModel.LegendPosition = legendPosition[legendIndex];
            LayerPlotModel.LegendSymbolLength = legendSymbolLen;
            LayerPlotModel.LegendPadding = legendPadding;


            Axis axisX = null;
            Axis axisY = null;

            LayerPlotModel.LegendFontSize = legendFontSize;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex + cumulativeOffset][1], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            if (logY)
            {
                axisY = new LogarithmicAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logY)
            {
                if (cumulative)
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex + cumulativeOffset][2], Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
                }
                else
                {
                    axisY = new LinearAxis { AxisTitleDistance = 16, FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Left, Title = distributionLabels[distributionIndex][2], TitleFontWeight = OxyPlot.FontWeights.Normal };
                }

            }

            LayerPlotModel.Axes.Add(axisX);
            LayerPlotModel.Axes.Add(axisY);

            if (cumulative)
            {
                foreach (var item in selectedLayers)
                {
                    var name = "";
                    if (item.Name == "positive_influence")
                    {
                        name = "p_influence";
                    }
                    else if (item.Name == "negative_influence")
                    {
                        name = "n_influence";
                    }
                    else
                    {
                        name = item.Name;
                    }


                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = name };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    var sum = 0.0;
                    double maxX = layerDistributions[item.Id][distributionIndex].Keys.Max();
                    double maxY = layerDistributions[item.Id][distributionIndex].Values.Sum();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sum += layerDistributions[item.Id][distributionIndex][sorted[i]];

                        double x = logX == true ? sorted[i] : sorted[i] / maxX;
                        double y = logY == true ? maxY - sum : 1 - (sum / maxY);

                        x = maxX == 0 ? 0 : x;
                        y = maxY == 0 ? 0 : y;

                        mySeries.Points.Add(new DataPoint(x, y));
                    }
                }
            }
            else
            {
                foreach (var item in selectedLayers)
                {
                    var mySeries = new LineSeries() { StrokeThickness = strokeThickness, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = markerSize, FontWeight = OxyPlot.FontWeights.Bold, FontSize = legendFontSize, Title = item.Name };
                    layerPlotModel.Series.Add(mySeries);

                    var sorted = layerDistributions[item.Id][distributionIndex].Keys.ToList();
                    sorted.Sort();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        mySeries.Points.Add(new DataPoint(sorted[i], layerDistributions[item.Id][distributionIndex][sorted[i]]));
                    }
                }

            }
        }
    }
}
