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

namespace GUI
{
    /// <summary>
    /// ViewModel for main window.
    /// </summary>
    public class MultilayerNetworkViewModel : INotifyPropertyChanged
    {
        #region Properties
        private MultilayerNetwork multilayerNetwork;
        private Layer flattenedNetwork;
        private IO io;
        private Transformation transformation;
        private Measures measures;

        private bool randomWalkMeasures;
        private bool distanceMeasures;

        private Dictionary<int, int[][]> floydWarshallMatrixByLayerId;
        private Dictionary<int, int[][]> adjacencyMatrixByLayerId;

        private int SelectedLayerId { get; set; }

        private string networkName;
        private int nodesCount;
        private int edgesCount;
        private int layersCount;
        private int actorsCount;

        private Layer selectedLayer { get; set; }

        private int selectedActorsCount;
        private int selectedEdgesCount;
        private int selectedMinDegree;
        private int selectedMaxDegree;
        private double selectedAverageDegree;
        private double selectedAverageDistance;
        private double selectedAverage;
        private double selectedAverageClusteringCoefficient;
        private int selectedConnectedComponentsCount;
        private int selectedLargestConnectedComponent;
        private double selectedDensity;

        private double flattenedDensity;
        private double averageDegree;
        private int minDegree;
        private int maxDegree;
        private double averageDistance;
        private int average;

        private double averageClusteringCoefficient;
        private List<List<Node>> connectedComponents;
        private int connectedComponentsCount;
        private int largestConnectedComponent;

        private HashSet<int> selectedLayers;

        //private int distributionsCount = 4;
        // Layer ID, Distributions [0] == degree, [1], clustering coefficient, [2] == TODO
        private ConcurrentDictionary<int, ConcurrentDictionary<double, double>[]> layerDistributions;
        // Degree, Count
        private ConcurrentDictionary<double, double>[] distributions;
        // CC, Count
        // Degree, Average CC for nodes with the degree
        //private ConcurrentDictionary<double, double> averageCcByDegree;

        private string[][] distributionLabels;


        public event PropertyChangedEventHandler PropertyChanged;

        // Colletions.
        private ObservableCollection<Actor> selectedActors;
        private ObservableCollection<Actor> actors;
        private ObservableCollection<Node> nodes;
        private ObservableCollection<Layer> layers;
        private ObservableCollection<string> distributionsComboBox;

        private int exportFlag;

        // Plots.
        //public MenuItem cumulativeMenuItem;
        //public MenuItem logXToggleItem;
        //public MenuItem logYToggleItem;

        private PlotModel mainPlotModel;
        private int invalidateFlag = 0;
        public string PlotTitle { get; private set; }
        public List<DataPoint> PlotPoints { get; private set; }
        private bool logX;
        private bool logY;
        private int selectedTabItem;
        private int selectedPlotIndex;
        private bool cumulative;
        private const int cumulativeOffset = 6;


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

        public int SelectedTabItem
        {
            get
            {
                return selectedTabItem;
            }
            private set
            {
                selectedTabItem = value;
                OnPropertyChanged("SelectedTabItem");
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

        public PlotModel MainPlotModel
        {
            get
            {
                return mainPlotModel;
            }
            private set
            {
                mainPlotModel = value;
                OnPropertyChanged("MainPlotModel");
            }
        }

        public int InvalidateFlag
        {
            get
            {
                return invalidateFlag;
            }
            private set
            {
                invalidateFlag = value;
                OnPropertyChanged("InvalidateFlag");
            }
        }

        public double SelectedDensity
        {
            get
            {
                return selectedDensity;
            }
            private set
            {
                selectedDensity = value;
                OnPropertyChanged("SelectedDensity");
            }
        }

        public double SelectedAverage
        {
            get
            {
                return selectedAverage;
            }
            private set
            {
                selectedAverage = value;
                OnPropertyChanged("SelectedAverage");
            }
        }

        public double SelectedAverageDistance
        {
            get
            {
                return selectedAverageDistance;
            }
            private set
            {
                selectedAverageDistance = value;
                OnPropertyChanged("SelectedAverageDistance");
            }
        }


        public double SelectedAverageDegree
        {
            get
            {
                return selectedAverageDegree;
            }
            private set
            {
                selectedAverageDegree = value;
                OnPropertyChanged("SelectedAverageDegree");
            }
        }

        public int SelectedMaxDegree
        {
            get
            {
                return selectedMaxDegree;
            }
            private set
            {
                selectedMaxDegree = value;
                OnPropertyChanged("SelectedMaxDegree");
            }
        }

        public int SelectedMinDegree
        {
            get
            {
                return selectedMinDegree;
            }
            private set
            {
                selectedMinDegree = value;
                OnPropertyChanged("SelectedMinDegree");
            }
        }

        public int SelectedEdgesCount
        {
            get
            {
                return selectedEdgesCount;
            }
            private set
            {
                selectedEdgesCount = value;
                OnPropertyChanged("SelectedEdgesCount");
            }
        }

        public int SelectedActorsCount
        {
            get
            {
                return selectedActorsCount;
            }
            private set
            {
                selectedActorsCount = value;
                OnPropertyChanged("SelectedActorsCount");
            }
        }

        public ObservableCollection<Actor> SelectedActors
        {
            get
            {
                return selectedActors;
            }
            private set
            {
                selectedActors = value;
                OnPropertyChanged("SelectedActors");
            }
        }

        public ObservableCollection<Actor> Actors
        {
            get
            {
                return actors;
            }
            private set
            {
                actors = value;
                OnPropertyChanged("Actors");
            }
        }

        public ObservableCollection<Node> Nodes
        {
            get
            {
                return nodes;
            }
            private set
            {
                nodes = value;
                OnPropertyChanged("Nodes");
            }
        }

        public ObservableCollection<string> DistributionsComboBox
        {
            get
            {
                return distributionsComboBox;
            }
            private set
            {
                distributionsComboBox = value;
                OnPropertyChanged("DistributionsComboBox");
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

        public int SelectedConnectedComponentsCount
        {
            get
            {
                return selectedConnectedComponentsCount;
            }
            private set
            {
                selectedConnectedComponentsCount = value;
                OnPropertyChanged("SelectedConnectedComponentsCount");
            }
        }

        public int SelectedLargestConnectedComponent
        {
            get
            {
                return selectedLargestConnectedComponent;
            }
            private set
            {
                selectedLargestConnectedComponent = value;
                OnPropertyChanged("SelectedLargestConnectedComponent");
            }
        }

        public int ConnectedComponentsCount
        {
            get
            {
                return connectedComponentsCount;
            }
            private set
            {
                connectedComponentsCount = value;
                OnPropertyChanged("ConnectedComponentsCount");
            }
        }

        public int LargestConnectedComponent
        {
            get
            {
                return largestConnectedComponent;
            }
            private set
            {
                largestConnectedComponent = value;
                OnPropertyChanged("LargestConnectedComponent");
            }
        }

        public int ActorsCount
        {
            get
            {
                return actorsCount;
            }
            private set
            {
                actorsCount = value;
                OnPropertyChanged("ActorsCount");
            }
        }

        public double AverageDistance
        {
            get
            {
                return averageDistance;
            }
            private set
            {
                averageDistance = value;
                OnPropertyChanged("AverageDistance");
            }
        }

        public int Average
        {
            get
            {
                return average;
            }
            private set
            {
                average = value;
                OnPropertyChanged("Average");
            }
        }

        public int LayersCount
        {
            get
            {
                return layersCount;
            }
            private set
            {
                layersCount = value;
                OnPropertyChanged("LayersCount");
            }
        }

        public double SelectedAverageClusteringCoefficient
        {
            get
            {
                return selectedAverageClusteringCoefficient;
            }
            private set
            {
                selectedAverageClusteringCoefficient = value;
                OnPropertyChanged("SelectedAverageClusteringCoefficient");
            }
        }

        public double AverageClusteringCoefficient
        {
            get
            {
                return averageClusteringCoefficient;
            }
            private set
            {
                averageClusteringCoefficient = value;
                OnPropertyChanged("AverageClusteringCoefficient");
            }
        }

        public double AverageDegree
        {
            get
            {
                return averageDegree;
            }
            private set
            {
                averageDegree = value;
                OnPropertyChanged("AverageDegree");
            }
        }

        public int MinDegree
        {
            get
            {
                return minDegree;
            }
            private set
            {
                minDegree = value;
                OnPropertyChanged("MinDegree");
            }
        }

        public int MaxDegree
        {
            get
            {
                return maxDegree;
            }
            private set
            {
                maxDegree = value;
                OnPropertyChanged("MaxDegree");
            }
        }

        public double FlattenedDensity
        {
            get
            {
                return flattenedDensity;
            }
            private set
            {
                flattenedDensity = value;
                OnPropertyChanged("FlattenedDensity");
            }
        }

        public string NetworkName
        {
            get
            {
                return networkName;
            }
            private set
            {
                networkName = value;
                OnPropertyChanged("NetworkName");
            }
        }

        public int NodesCount
        {
            get
            {
                return nodesCount;
            }
            private set
            {
                nodesCount = value;
                OnPropertyChanged("NodesCount");
            }
        }

        public int EdgesCount
        {
            get
            {
                return edgesCount;
            }
            private set
            {
                edgesCount = value;
                OnPropertyChanged("EdgesCount");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void CumulativeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Cumulative = (sender as MenuItem).IsChecked;
        }

        public void LogXChck_Checked(object sender, RoutedEventArgs e)
        {
            LogX = (sender as MenuItem).IsChecked;
        }

        public void LogYChck_Checked(object sender, RoutedEventArgs e)
        {
            LogY = (sender as MenuItem).IsChecked;

        }

        #endregion Properties

        /// <summary>
        /// Selected tab in the main window changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                SelectedTabItem = (e.Source as TabControl).SelectedIndex;
            }
        }

        /// <summary>
        /// Plots distribution in the main window.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if log x.</param>
        /// <param name="logY">True if log y.</param>
        public void Plot(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (distributions[distributionIndex] == null || distributions[distributionIndex].Keys.Count == 0)
                return;

            //Size export
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

            // Plot name.
            if (cumulative)
                MainPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                MainPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //MainPlotModel.LegendBorderThickness = 5;
            MainPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            MainPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            MainPlotModel.TitlePadding = titlePadding;
            MainPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            //MainPlotModel.LegendTitle = "Distribution";
            //MainPlotModel.LegendTitleFontSize = legendFontSize;
            //MainPlotModel.LegendPosition = LegendPosition.RightTop;
            //MainPlotModel.LegendSymbolLength = 100;
            //MainPlotModel.LegendPadding = 50;

            Axis axisX = null;
            Axis axisY = null;

            if (logX)
            {
                axisX = new LogarithmicAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep, MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness, FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom, Title = distributionLabels[distributionIndex][1], TitleFontWeight = OxyPlot.FontWeights.Normal };
            }
            else if (!logX)
            {
                if (cumulative)
                {
                    //MinimumPadding = 1,MaximumPadding = 5,
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


            MainPlotModel.Axes.Add(axisX);
            MainPlotModel.Axes.Add(axisY);

            var mySeries = new LineSeries()
            {
                StrokeThickness = strokeThickness,
                Smooth = false,
                MarkerType = MarkerType.Circle,
                MarkerSize = markerSize,
                FontWeight = OxyPlot.FontWeights.Bold
            };

            MainPlotModel.Series.Add(mySeries);

            var sorted = distributions[distributionIndex].Keys.ToList();
            sorted.Sort();



            if (cumulative)
            {
                var sum = 0.0;

                var maxX = distributions[distributionIndex].Keys.Max();
                var maxY = distributions[distributionIndex].Values.Sum();


                for (int i = 0; i < sorted.Count; i++)
                {
                    sum += distributions[distributionIndex][sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);
                    mySeries.Points.Add(new DataPoint(x, y));
                }
            }
            else
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    mySeries.Points.Add(new DataPoint(sorted[i], distributions[distributionIndex][sorted[i]]));
                }

            }

            MainPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Used for png export, we need bigger resolution in png than in the app.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if log x.</param>
        /// <param name="logY">True if log y.</param>
        /// <returns>Plot model.</returns>
        public PlotModel GetExportPlotModel(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (distributions[distributionIndex] == null || distributions[distributionIndex].Keys.Count == 0)
                return null;

            //Size export
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

            var exportMainPlotModel = new PlotModel();

            // Plot name.
            if (cumulative)
                exportMainPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex + cumulativeOffset][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };
            else
                exportMainPlotModel = new PlotModel
                {
                    Title = distributionLabels[distributionIndex][0],
                    TitleFontSize = titleFontSize,
                    TitleFontWeight = OxyPlot.FontWeights.Bold
                };

            //MainPlotModel.LegendBorderThickness = 5;
            exportMainPlotModel.PlotAreaBorderThickness = new OxyThickness(borderOxyThickness);

            exportMainPlotModel.Padding = new OxyThickness(paddingOxyThickness);
            exportMainPlotModel.TitlePadding = titlePadding;
            exportMainPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            //MainPlotModel.LegendTitle = "Distribution";
            //MainPlotModel.LegendTitleFontSize = legendFontSize;
            //MainPlotModel.LegendPosition = LegendPosition.RightTop;
            //MainPlotModel.LegendSymbolLength = 100;
            //MainPlotModel.LegendPadding = 50;

            Axis axisX = null;
            Axis axisY = null;

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


            exportMainPlotModel.Axes.Add(axisX);
            exportMainPlotModel.Axes.Add(axisY);

            var mySeries = new LineSeries()
            {
                StrokeThickness = 5,
                Smooth = false,
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                FontWeight = OxyPlot.FontWeights.Bold
            };

            exportMainPlotModel.Series.Add(mySeries);

            var sorted = distributions[distributionIndex].Keys.ToList();
            sorted.Sort();



            if (cumulative)
            {
                var sum = 0.0;

                var maxX = distributions[distributionIndex].Keys.Max();
                var maxY = distributions[distributionIndex].Values.Sum();


                for (int i = 0; i < sorted.Count; i++)
                {
                    sum += distributions[distributionIndex][sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);
                    mySeries.Points.Add(new DataPoint(x, y));
                }
            }
            else
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    mySeries.Points.Add(new DataPoint(sorted[i], distributions[distributionIndex][sorted[i]]));
                }

            }


            return exportMainPlotModel;
            //MainPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Old mehtod for plotting.
        /// </summary>
        /// <param name="distributionIndex">Index of distribution to plot.</param>
        /// <param name="logX">True if log x.</param>
        /// <param name="logY">True if log y.</param>
        /*public void Plot_(int distributionIndex, bool logX = false, bool logY = false)
        {
            if (distributions[distributionIndex] == null || distributions[distributionIndex].Keys.Count == 0)
                return;

            //var axisFontSize = 24;
            //var titleFontSize = 24;
            //var legendFontSize = 24;

            //var minorGridlineThickness = 3;
            //var majorGridlineThickness = 3;

            //var minMinorStep = 0.1;
            //var minMajorStep = 0.5;
            //var minTickSize = 5;
            //var maxTickSize = 10;

            //var paddingOxyThickness = 10;
            //var borderOxyThickness = 2;
            //var titlePadding = 50;

            var axisFontSize = 68;
            var titleFontSize = 68;
            var legendFontSize = 68.0;

            // Plot name.
            //if (cumulative)
            //    MainPlotModel = new PlotModel { Title = distributionLabels[distributionIndex + cumulativeOffset][0], TitleFontSize = titleFontSize,
            //        TitleFontWeight = OxyPlot.FontWeights.Bold };
            //else
            //    MainPlotModel = new PlotModel { Title = distributionLabels[distributionIndex][0], TitleFontSize = titleFontSize,
            //        TitleFontWeight = OxyPlot.FontWeights.Bold };

            MainPlotModel = new PlotModel
            {
                Title = "Cumulative rw betweenness/betwenness",
                TitleFontSize = titleFontSize,TitleFontWeight = OxyPlot.FontWeights.Bold };

                //MainPlotModel.LegendBorderThickness = 5;
                MainPlotModel.PlotAreaBorderThickness = new OxyThickness(2);

            MainPlotModel.Padding = new OxyThickness(10);
            MainPlotModel.TitlePadding = 50;
            MainPlotModel.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea;

            MainPlotModel.LegendTitle = "Distribution";
            MainPlotModel.LegendTitleFontSize = legendFontSize;
            MainPlotModel.LegendPosition = LegendPosition.RightTop;
            MainPlotModel.LegendSymbolLength = 100;
            MainPlotModel.LegendPadding = 50;

            Axis axisX = null;
            Axis axisY = null;

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
                    axisX = new LinearAxis { FontSize = axisFontSize, MinorTickSize = minTickSize, MajorTickSize = maxTickSize, MinimumMinorStep = minMinorStep,
                        MinimumMajorStep = minMajorStep, MinorGridlineThickness = minorGridlineThickness, MajorGridlineThickness = majorGridlineThickness,
                        FontWeight = OxyPlot.FontWeights.Normal, Position = AxisPosition.Bottom,
                        Title = "rw betweenness/betweenness",
                        Minimum = 0, Maximum = 1, TitleFontWeight = OxyPlot.FontWeights.Normal };
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


            MainPlotModel.Axes.Add(axisX);
            MainPlotModel.Axes.Add(axisY);

            var mySeries = new LineSeries() { StrokeThickness = 8, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = 8, FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = "rw betweenness" };

            MainPlotModel.Series.Add(mySeries);

            //var sorted = distributions[distributionIndex].Keys.ToList();
            //sorted.Sort();

            var path = @"C:\Users\Honza\Desktop\R Export\aucs\betweenness.txt";
            var lines = File.ReadAllLines(path);
            var betweenness = new Dictionary<double, double>();

            

            for (int i = 0; i < lines.Length; i++)
            {
                var key = Convert.ToDouble(lines[i]);
                if (betweenness.ContainsKey(key))
                {
                    betweenness[key] = betweenness[key] + 1;
                }
                else
                {
                    betweenness.Add(key, 1);
                }
            }

            var sorted = betweenness.Keys.ToList();
            sorted.Sort();

            if (cumulative)
            {
                var sum = 0.0;

                //var maxX = distributions[distributionIndex].Keys.Max();
                //var maxY = distributions[distributionIndex].Values.Sum();
                var maxX = betweenness.Keys.Max();
                var maxY = betweenness.Values.Sum();


                for (int i = 0; i < sorted.Count; i++)
                {
                    //sum += distributions[distributionIndex][sorted[i]];
                    sum += betweenness[sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);
                    mySeries.Points.Add(new DataPoint(x, y)); 
                }

                sorted = distributions[distributionIndex].Keys.ToList();
                sorted.Sort();
                var sSeries = new LineSeries() { StrokeThickness = 8, Smooth = false, MarkerType = MarkerType.Circle, MarkerSize = 8,
                    FontWeight = OxyPlot.FontWeights.Normal, FontSize = legendFontSize, Title = "betweenness" };

                MainPlotModel.Series.Add(sSeries);

                sum = 0.0;

                maxX = distributions[distributionIndex].Keys.Max();
                tmpbet = distributions[3];
                maxY = distributions[distributionIndex].Values.Sum();


                for (int i = 0; i < sorted.Count; i++)
                {
                    sum += distributions[distributionIndex][sorted[i]];

                    double x = logX == true ? sorted[i] : sorted[i] / maxX;
                    double y = logY == true ? maxY - sum : 1 - (sum / maxY);
                    sSeries.Points.Add(new DataPoint(x, y));
                }
            }
            else
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    mySeries.Points.Add(new DataPoint(sorted[i], distributions[distributionIndex][sorted[i]]));
                }

            }

            MainPlotModel.InvalidatePlot(true);
        }*/

        /// <summary>
        /// Init mehtod, initializes all important properties etc.
        /// </summary>
        private void Init()
        {
            io = IO.Instance; //new IO();
            transformation = new Transformation();
            measures = new Measures();
            //degreeDistribution = new ConcurrentDictionary<double, double>();
            //clusteringCoefficientDistribution = new ConcurrentDictionary<double, double>();
            //averageCcByDegree = new ConcurrentDictionary<double, double>();
            floydWarshallMatrixByLayerId = new Dictionary<int, int[][]>();
            adjacencyMatrixByLayerId = new Dictionary<int, int[][]>();
            connectedComponents = new List<List<Node>>();
            distributions = new ConcurrentDictionary<double, double>[6];
            layerDistributions = new ConcurrentDictionary<int, ConcurrentDictionary<double, double>[]>();
            cumulative = true;
            randomWalkMeasures = true;
            distanceMeasures = true;

            selectedLayers = new HashSet<int>();

            networkName = "";
        }

        /// <summary>
        /// Constructor, just calls init method.
        /// </summary>
        public MultilayerNetworkViewModel()
        {
            Init();
        }

        /// <summary>
        /// Layer selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SelectedLayerChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            if ((sender as System.Windows.Controls.ComboBox).Items.Count > 0 && (sender as System.Windows.Controls.ComboBox).SelectedValue != null)
            {
                SelectedLayerId = (int)(sender as System.Windows.Controls.ComboBox).SelectedValue;
                selectedLayer = layers.FirstOrDefault(x => x.Id == SelectedLayerId);
                BindSelectedLayer();
                BindSelectedActors();
            }

        }

        /// <summary>
        /// Plot selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PlotIndexChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            if ((sender as System.Windows.Controls.ComboBox).Items.Count > 0 && (sender as System.Windows.Controls.ComboBox).SelectedValue != null)
            {
                selectedPlotIndex = (int)(sender as System.Windows.Controls.ComboBox).SelectedIndex;
                Plot(SelectedPlotIndex, logX, logY);
            }

        }

        /// <summary>
        /// Summary layer measures.
        /// </summary>
        /// <returns></returns>
        private string LayerSummary()
        {
            var summary = new StringBuilder();

            // Headers.
            if (distanceMeasures)
            {
                summary.AppendLine("Layer; Edges; Nodes; Minimum degree; Maximum degree; Average degree; Average clustering coefficient; Average distance; Average; Number of connected components; Largest component size; Density");
            }
            else
            {
                summary.AppendLine("Layer; Edges; Nodes; Minimum degree; Maximum degree; Average degree; Average clustering coefficient; Number of connected components; Largest component size; Density");
            }

            // Layer values.
            foreach (var layer in multilayerNetwork.Layers)
            {
                if (distanceMeasures)
                {
                    summary.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}", layer.Name, multilayerNetwork.GetEdges(layer, layer).Count, multilayerNetwork.GetNodes(layer).Count, layer.MinDegree, layer.MaxDegree, layer.AvgDegree, layer.AverageClusteringCoefficient, layer.Average, layer.AvgDistance, layer.ConnectedComponentsCount, layer.LargestComponent, layer.Density));
                }
                else
                {

                    summary.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", layer.Name, multilayerNetwork.GetEdges(layer, layer).Count, multilayerNetwork.GetNodes(layer).Count, layer.MinDegree, layer.MaxDegree, layer.AvgDegree, layer.AverageClusteringCoefficient, layer.ConnectedComponentsCount, layer.LargestComponent, layer.Density));

                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Bind components of selected layer to the GUI.
        /// </summary>
        private void BindSelectedLayer()
        {
            if (!multilayerNetwork.EdgesByLayerPair[SelectedLayerId].ContainsKey(SelectedLayerId))
                return;

            SelectedEdgesCount = selectedLayer.EdgesCount;
            SelectedAverageDistance = selectedLayer.AvgDistance;
            SelectedAverage = selectedLayer.Average;
            SelectedAverageClusteringCoefficient = selectedLayer.AverageClusteringCoefficient;
            SelectedConnectedComponentsCount = selectedLayer.ConnectedComponentsCount;
            SelectedLargestConnectedComponent = selectedLayer.LargestComponent;
            SelectedDensity = selectedLayer.Density;
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
        /// Export summary button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExportSummary(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null || layerDistributions.Count != multilayerNetwork.Layers.Count)
                return;

            exportFlag = 0;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Export summary into txt button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExportSummaryTxt(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null || layerDistributions.Count != multilayerNetwork.Layers.Count)
                return;

            exportFlag = 3;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Button for opening layer detail window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LayerDetail(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null || layerDistributions.Count != multilayerNetwork.Layers.Count)
                return;

            var layerDetailWindow = new LayerDetailWindow(layerDistributions, multilayerNetwork);
            layerDetailWindow.Show();
        }

        /// <summary>
        /// Imports multilayer network.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ImportMultilayerNetwork(object sender, RoutedEventArgs e)
        {
            // Reinitialize.
            Init();

            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Multiplex network files (*.mpx)|*.mpx";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = openFileDialog.FileName;
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                try
                {
                    multilayerNetwork = await io.ReadMultilayerAsync(path, name);
                }
                catch (System.Exception ex)
                {

                    System.Console.WriteLine(ex.Message);
                    return;
                }

                // Warning for bigger multilayer networks.
                // Every layer has its own adjacency matrix => Big memory usage.
                // And flattened layer has also its own adjacency matrix.
                // + RandomWalk is not parallel, it takes a lot of time to traverse whole network.
                var messageBoxResultRw = MessageBox.Show("Do you want to calculate random-walk centralities? Warning: This takes more than 2 minutes even for small networks!", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                randomWalkMeasures = messageBoxResultRw == MessageBoxResult.Yes;

                if (multilayerNetwork.Actors.Count > 1000 || multilayerNetwork.Edges.Count > 100000)
                {
                    var messageBoxResultDs = MessageBox.Show("Do you want to calculate distance measures? Warning: This takes a lot of memory and time!", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    distanceMeasures = messageBoxResultDs == MessageBoxResult.Yes;
                }

                // Rwalk measures.
                if (randomWalkMeasures)
                {
                    distributionLabels = new string[14][];

                    // Key, value.
                    distributionLabels[0] = new string[3] { "Degree distribution", "degree", "count" };
                    distributionLabels[1] = new string[3] { "Clustering coefficient distribution", "clustering coefficient", "count" };
                    distributionLabels[2] = new string[3] { "Average clustering coefficient by degree", "degree", "average clustering coefficient" };
                    distributionLabels[3] = new string[3] { "RandomWalk Betweenness distribution", "betweenness", "count" };
                    distributionLabels[4] = new string[3] { "RandomWalk Closeness distribution", "closeness", "count" };
                    distributionLabels[5] = new string[3] { "Occupation distribution", "occupation", "count" };
                    //distributionLabels[6] = new string[3] { "Exclusive neighborhood distribution", "exclusive neighborhood", "count" };
                    //distributionLabels[7] = new string[3] { "Exclusive relevance distribution", "exclusive relevance", "count" };

                    distributionLabels[6] = new string[3] { "Cumulative degree distribution", "relative degree", "relative cumulative frequency" };
                    distributionLabels[7] = new string[3] { "Cumulative clustering coefficient distribution", "relative clustering coefficient", "relative cumulative frequency" };
                    distributionLabels[8] = new string[3] { "Cumulative average clustering coefficient by degree", "relative degree", "relative cumulative avg cc" };
                    distributionLabels[9] = new string[3] { "RandomWalk Betweenness distribution", "relative betweenness", "relative cumulative frequency" };
                    distributionLabels[10] = new string[3] { "RandomWalk Closeness distribution", "relative closeness", "relative cumulative frequency" };
                    distributionLabels[11] = new string[3] { "Cumulative occupation distribution", "relative occupation", "relative cumulative frequency" };
                    //distributionLabels[14] = new string[3] { "Exclusive neighborhood distribution", "relative exclusive neighborhood", "relative cumulative frequency" };
                    //distributionLabels[15] = new string[3] { "Exclusive relevance distribution", "relative exclusive relevance", "relative cumulative frequency" };

                    DistributionsComboBox = new ObservableCollection<string>()
                    {
                        "Degree",
                        "Clustering coefficient",
                        "Avg CC by Degree",
                        "RandomWalk Betweenness",
                        "RandomWalk Closeness",
                        "Occupation"
                    };
                }
                // All other measures.
                else
                {
                    distributionLabels = new string[14][];

                    // Key, value.
                    distributionLabels[0] = new string[3] { "Degree distribution", "degree", "count" };
                    distributionLabels[1] = new string[3] { "Clustering coefficient distribution", "clustering coefficient", "count" };
                    distributionLabels[2] = new string[3] { "Average clustering coefficient by degree", "degree", "average clustering coefficient" };
                    //distributionLabels[3] = new string[3] { "Exclusive neighborhood distribution", "exclusive neighborhood", "count" };
                    //distributionLabels[4] = new string[3] { "Exclusive relevance distribution", "exclusive relevance", "count" };

                    distributionLabels[6] = new string[3] { "Cumulative degree distribution", "relative degree", "relative cumulative frequency" };
                    distributionLabels[7] = new string[3] { "Cumulative clustering coefficient distribution", "relative clustering coefficient", "relative cumulative frequency" };
                    distributionLabels[8] = new string[3] { "Cumulative average clustering coefficient by degree", "relative degree", "relative cumulative frequency" };
                    //distributionLabels[11] = new string[3] { "Exclusive neighborhood distribution", "relative exclusive neighborhood", "relative cumulative frequency" };
                    //distributionLabels[12] = new string[3] { "Exclusive relevance distribution", "relative exclusive relevance", "relative cumulative frequency" };

                    DistributionsComboBox = new ObservableCollection<string>()
                    {
                        "Degree",
                        "Clustering coefficient",
                        "Avg CC by Degree",
                    };

                    
                }

                CalculateMetrics();
                BindProperties();
            }
        }
        
        /// <summary>
        /// Exports plot of distribution to PNG.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartToPng(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null)
                return;

            exportFlag = 1;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Exports plot of distribution to PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartToPdf(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null)
                return;

            exportFlag = 2;
            ExportIntoDirectory();
        }

        /// <summary>
        /// Exports selected layer to CSV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToCsv(object sender, RoutedEventArgs e)
        {
            if (multilayerNetwork == null)
                return;

            var fileName = "\\" + selectedLayer.Name + ".csv";
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                var path = folderBrowserDialog.SelectedPath;
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                io.ToCsv(multilayerNetwork, selectedLayer, path + fileName);
                //io.ToEdgelist(multilayerNetwork, selectedLayer, path + fileName);
            }
        }

        /// <summary>
        /// Calculate measures on flattened networks (only density).
        /// </summary>
        private void FlattenedMeasures()
        {
            FlattenedDensity = measures.Density(multilayerNetwork, flattenedNetwork);
        }

        /// <summary>
        /// Calculates degree distribution for all nodes/actors.
        /// </summary>
        /// <param name="layer">Layer to calculate it on. If null, all layers.</param>
        private void GetDegreeDistribution(Layer layer = null)
        {
            // Layer ID, Degree, SumCc
            var layerDegreeSumCc = new ConcurrentDictionary<int, ConcurrentDictionary<double, double>>();
            var networkDegreeSumCc = new ConcurrentDictionary<double, double>();

            // Helper dicts for counting nodes with degree.
            var layerDegreeCount = new ConcurrentDictionary<int, ConcurrentDictionary<double, double>>();
            var networkDegreeCount =  new ConcurrentDictionary<double, double>();

            var degreeDistribution = new ConcurrentDictionary<double, double>();


            Parallel.ForEach(multilayerNetwork.Nodes, (n) =>
            {
                // Degree distribution for actor on specific layer.
                var degree = measures.Degree(multilayerNetwork, n.Actor, n.Layer, EdgeMode.InOut);
                var relevance = measures.Relevance(multilayerNetwork, n.Actor, n.Layer, EdgeMode.InOut);
                var exclusiveNeighborhood = measures.ExclusiveNeighborhood(multilayerNetwork, n.Actor, n.Layer, EdgeMode.InOut);
                var exclusiveRelevance= measures.ExclusiveRelevance(multilayerNetwork, n.Actor, n.Layer, EdgeMode.InOut);
                var networkDegree = measures.Degree(multilayerNetwork, n.Actor, multilayerNetwork.Layers, EdgeMode.InOut);

                var cc = measures.ClusteringCoefficient(multilayerNetwork, n);

                // Degree distribution.
                layerDistributions[n.Layer.Id][0].AddOrUpdate(degree, 1, (key, oldValue) => oldValue + 1);
                layerDistributions[n.Layer.Id][3].AddOrUpdate(relevance, 1, (key, oldValue) => oldValue + 1);
                layerDistributions[n.Layer.Id][4].AddOrUpdate(exclusiveNeighborhood, 1, (key, oldValue) => oldValue + 1);
                layerDistributions[n.Layer.Id][5].AddOrUpdate(exclusiveRelevance, 1, (key, oldValue) => oldValue + 1);



                // Average CC by Degree.
                layerDegreeSumCc.TryAdd(n.Layer.Id, new ConcurrentDictionary<double, double>());
                layerDegreeCount.TryAdd(n.Layer.Id, new ConcurrentDictionary<double, double>());
                layerDegreeCount[n.Layer.Id].AddOrUpdate(degree, 1, (key, oldValue) => oldValue + 1);
                layerDegreeSumCc[n.Layer.Id].AddOrUpdate(degree, cc, (key, oldValue) => (oldValue * (layerDegreeCount[n.Layer.Id][degree] - 1) + cc) / layerDegreeCount[n.Layer.Id][degree]);


                networkDegreeSumCc.TryAdd(networkDegree, cc);
                networkDegreeCount.TryAdd(n.Degree, 0);
                networkDegreeCount.AddOrUpdate(degree, 1, (key, oldValue) => oldValue + 1);
                networkDegreeSumCc.AddOrUpdate(networkDegree, cc, (key, oldValue) => (oldValue * (networkDegreeCount[degree] - 1) + cc) / networkDegreeCount[degree]);

                var checkedActors = new HashSet<int>();

                // And for all actors on all layers.
                if (!checkedActors.Contains(n.Actor.Id) || checkedActors.Count == multilayerNetwork.Actors.Count)
                {
                    checkedActors.Add(n.Actor.Id);
                    n.Actor.Degree = measures.Degree(multilayerNetwork, n.Actor, multilayerNetwork.Layers, EdgeMode.InOut);
                    degreeDistribution.AddOrUpdate(n.Actor.Degree, 1, (key, oldValue) => oldValue + 1);
                }
            });

            distributions[0] = degreeDistribution;
            distributions[2] = networkDegreeSumCc;

            foreach (var item in layerDegreeSumCc)
            {
                layerDistributions[item.Key][2] = item.Value;
            }

            MinDegree = (int)degreeDistribution.Keys.Min();
            MaxDegree = (int)degreeDistribution.Keys.Max();
            AverageDegree = degreeDistribution.Keys.Average();
        }

        /// <summary>
        /// Calculates clustering coefficient distribution for all nodes/actors.
        /// </summary>
        /// <param name="layer">Layer to calculate it on. If null, all layers.</param>
        private void GetClusteringCoefficientDistribution(Layer layer = null)
        {
            var nodes = new HashSet<Node>();
            Nodes = new ObservableCollection<Node>();
            Actors = new ObservableCollection<Actor>();
            var visitedActors = new HashSet<Actor>();

            if (layer != null)
            {
                nodes = multilayerNetwork.NodesByLayer[layer.Id];
            }
            else
            {
                nodes = multilayerNetwork.Nodes;
            }

            double sumCc = 0;

            var clusteringCoefficientDistribution = new ConcurrentDictionary<double, double>();

            Parallel.ForEach(nodes, (n) =>
            {
                n.ClusteringCoefficient = measures.ClusteringCoefficient(multilayerNetwork, n);
                sumCc += n.ClusteringCoefficient;
                n.Actor.ClusteringCoefficient = n.ClusteringCoefficient;

                // Whole network.
                clusteringCoefficientDistribution.AddOrUpdate(n.ClusteringCoefficient, 1, (key, oldValue) => oldValue + 1);

                // Only layer.
                layerDistributions[n.Layer.Id][1].AddOrUpdate(n.ClusteringCoefficient, 1, (key, oldValue) => oldValue + 1);


                // Bound nodes.
                n.Degree = multilayerNetwork.Neighbors(n, EdgeMode.InOut).Count;
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    Nodes.Add(n);
                });

                // Bound actors.
                if (!visitedActors.Contains(n.Actor))
                {
                    visitedActors.Add(n.Actor);
                    n.Actor.NeighborhoodCentrality = measures.NeighborhoodCentrality(multilayerNetwork, n.Actor, flattenedNetwork);
                    n.Actor.DegreeDeviation = measures.DegreeDeviation(multilayerNetwork, n.Actor, multilayerNetwork.Layers, EdgeMode.InOut);
                    n.Actor.ConnectiveRedundancy = measures.ConnectiveRedundancy(multilayerNetwork, n.Actor, multilayerNetwork.Layers, EdgeMode.InOut);
                    n.Actor.Relevance = measures.Relevance(multilayerNetwork, n.Actor, flattenedNetwork, EdgeMode.InOut);

                    App.Current.Dispatcher.Invoke((System.Action)delegate
                    {

                        Actors.Add(n.Actor);
                    });
                }
            });

            distributions[1] = clusteringCoefficientDistribution;

            //var test = measures.AverageClusteringCoefficient(multilayerNetwork, flattenedNetwork);

            // Calculate average cc, so we don't have to iterate through nodes again.
            AverageClusteringCoefficient = (1.0 / nodes.Count) * sumCc;

            //foreach (var n in nodes)
            //{
            //    n.Actor.ClusteringCoefficient = measures.ClusteringCoefficient(multilayerNetwork, n);
            //}
        }

       
        /// <summary>
        /// Calculate closeness centrality for all nodes/actors.
        /// </summary>
        /// <param name="layer">Layer to calculate it on. If null, all layers.</param>
        private void GetClosenessCentrality(Layer layer = null)
        {
            // For actors calculate it on flattened network.
            foreach (var actor in multilayerNetwork.Actors)
            {
                if (layer == null)
                {

                    actor.Closeness = measures.ClosenessCentrality(actor, flattenedNetwork, floydWarshallMatrixByLayerId, multilayerNetwork);
                }
                else
                {
                    actor.Closeness = measures.ClosenessCentrality(actor, layer, floydWarshallMatrixByLayerId, multilayerNetwork);
                }
            }

            // For nodes, calculate it on the layer.
            foreach (var node in multilayerNetwork.Nodes)
            {
                if (layer == null)
                {
                    node.ClosenessCentrality = measures.ClosenessCentrality(node.Actor, flattenedNetwork, floydWarshallMatrixByLayerId, multilayerNetwork);
                }
                else
                {
                    node.ClosenessCentrality = measures.ClosenessCentrality(node.Actor, layer, floydWarshallMatrixByLayerId, multilayerNetwork);
                }
            }


        }

        /// <summary>
        /// Average clustering coefficient.
        /// </summary>
        /// <param name="layer">Layer to calculate it on.</param>
        /// <returns></returns>
        private double GetLayerAverageClusteringCoefficient(Layer layer)
        {
            return measures.AverageClusteringCoefficient(multilayerNetwork, layer);
        }

        /// <summary>
        /// Average clustering coefficient.
        /// </summary>
        /// <param name="layer">Layer to calculate it on. If null, flattened network is selected.</param>
        /// <returns></returns>
        private void GetAverageClusteringCoefficient(Layer layer = null)
        {
            if (layer == null)
            {
                AverageClusteringCoefficient = measures.AverageClusteringCoefficient(multilayerNetwork, flattenedNetwork);
            }
            else
            {
                layer.AverageClusteringCoefficient = measures.AverageClusteringCoefficient(multilayerNetwork, layer);
            }
        }

        /// <summary>
        /// Calculate shortest paths between all nodes on all layers.
        /// </summary>
        private void GetFloydWarshallMatrices()
        {
            //foreach (var keyValuePair in adjacencyMatrixByLayerId)
            Parallel.ForEach(adjacencyMatrixByLayerId, keyValuePair =>
            {
                floydWarshallMatrixByLayerId.Add(keyValuePair.Key, measures.FloydWarshall(keyValuePair.Value));
            });
        }

        /// <summary>
        /// Flatten the network.
        /// </summary>
        private void Flatten()
        {
            // Flatten the whole network into single layer for basic measures.
            flattenedNetwork = transformation.FlattenUnweighted(multilayerNetwork, "flattened", multilayerNetwork.Layers, false, false);
            multilayerNetwork.Layers.Add(flattenedNetwork);
        }

        /// <summary>
        /// Bind properties to GUI.
        /// </summary>
        private void BindProperties()
        {
            NetworkName = multilayerNetwork.Name;
            NodesCount = multilayerNetwork.Nodes.Count;
            EdgesCount = multilayerNetwork.Edges.Count;
            ActorsCount = multilayerNetwork.Actors.Count;
            LayersCount = multilayerNetwork.Layers.Count;

            //BindActors();
            Task.Run(() =>
            {
                BindSelectedActors();
            });

            //BindNodes();

            Task.Run(() =>
            {
                BindLayers();
            });
        }

        /// <summary>
        /// Bind actors of selected layer to GUI.
        /// </summary>
        private void BindSelectedActors()
        {
            SelectedActors = new ObservableCollection<Actor>();
            var visitedActors = new HashSet<Actor>();
            selectedLayer = multilayerNetwork.Layers.FirstOrDefault(x => x.Id == SelectedLayerId);
            var degrees = new List<int>();
            foreach (var node in multilayerNetwork.NodesByLayer[SelectedLayerId])
            {
                if (!visitedActors.Contains(node.Actor))
                {
                    visitedActors.Add(node.Actor);

                    node.Actor.SelectedDegree = measures.Degree(multilayerNetwork, node.Actor, selectedLayer, EdgeMode.InOut);
                    degrees.Add(node.Actor.SelectedDegree);
                    node.Actor.SelectedNeighborhoodCentrality = measures.NeighborhoodCentrality(multilayerNetwork, node.Actor, selectedLayer);
                    node.Actor.SelectedConnectiveRedundancy = measures.ConnectiveRedundancy(node.Actor);
                    node.Actor.SelectedRelevance = measures.Relevance(multilayerNetwork, node.Actor, selectedLayer, EdgeMode.InOut);
                    node.Actor.SelectedClusteringCoefficient = measures.ClusteringCoefficient(multilayerNetwork, node);

                    App.Current.Dispatcher.Invoke((System.Action)delegate
                    {
                        SelectedActors.Add(node.Actor);
                    });
                }
            }

            SelectedActorsCount = degrees.Count;
            SelectedAverageDegree = degrees.Average();
            SelectedMinDegree = degrees.Min();
            SelectedMaxDegree = degrees.Max();

            if (!selectedLayers.Contains(SelectedLayerId))
            {
                selectedLayers.Add(selectedLayer.Id);

                selectedLayer.MinDegree = SelectedMinDegree;
                selectedLayer.MaxDegree = SelectedMaxDegree;
                selectedLayer.AvgDegree = SelectedAverageDegree;

            }
        }

        /// <summary>
        /// Bind nodes to GUI.
        /// </summary>
        private void BindNodes()
        {
            Nodes = new ObservableCollection<Node>();
            foreach (var node in multilayerNetwork.Nodes)
            {
                node.Degree = multilayerNetwork.Neighbors(node, EdgeMode.InOut).Count;
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Bind layers to GUI.
        /// </summary>
        private void BindLayers()
        {
            Layers = new ObservableCollection<Layer>();
            foreach (var layer in multilayerNetwork.Layers)
            {
                layer.NodesCount = multilayerNetwork.NodesByLayer[layer.Id].Count;
                layer.EdgesCount = multilayerNetwork.EdgesByLayerPair[layer.Id][layer.Id].Count;
                layer.Density = measures.Density(multilayerNetwork, layer);
                layer.AverageClusteringCoefficient = measures.AverageClusteringCoefficient(multilayerNetwork, layer);

                var sumDegree = 0.0;
                var counter = 0;
                var degrees = new List<int>();
                foreach (var node in multilayerNetwork.NodesByLayer[layer.Id])
                {
                    var dgr = measures.Degree(multilayerNetwork, node.Actor, layer, EdgeMode.InOut);
                    sumDegree += dgr;
                    degrees.Add(dgr);
                    counter++;
                }

                layer.AverageDegreeCentrality = sumDegree / counter;
                layer.MinDegree = degrees.Min();
                layer.MaxDegree = degrees.Max();
                layer.AvgDegree = degrees.Average();


                var connectedComponents = measures.GetConnectedComponents(multilayerNetwork, layer);
                layer.ConnectedComponentsCount = connectedComponents[0];
                layer.LargestComponent = connectedComponents[1];

                if (distanceMeasures)
                {
                    Task avgDist = Task.Run(() =>
                    {
                        layer.AvgDistance = measures.GetAverageDistance(multilayerNetwork, layer, floydWarshallMatrixByLayerId);
                    });
                    Task avg = Task.Run(() =>
                    {
                        layer.Average = measures.GetAverage(layer, floydWarshallMatrixByLayerId);
                    });

                    Task.WaitAll(new Task[] { avg });
                }

                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    Layers.Add(layer);
                });
            }
            
        }

        /// <summary>
        /// Occupation centrality distribution.
        /// </summary>
        private void GetOccupationDistribution()
        {
            var occupation = measures.Occupation(multilayerNetwork);

            var occupationDistribution = new ConcurrentDictionary<double, double>();

            Parallel.ForEach(occupation, (item) =>
            {
                occupationDistribution.AddOrUpdate(item.Value, 1, (key, oldValue) => oldValue + 1);
            });

            distributions[5] = occupationDistribution;
        }

        /// <summary>
        /// Random walk betweenness distribution.
        /// </summary>
        private void GetRandomWalkBetweennessDistribution()
        {
            var rwBetweenness = measures.RandomWalkBetweenness(multilayerNetwork);

            var rwBetweennessDistribution = new ConcurrentDictionary<double, double>();

            Parallel.ForEach(rwBetweenness, (item) =>
            {
                rwBetweennessDistribution.AddOrUpdate(item.Value, 1, (key, oldValue) => oldValue + 1);
            });

            distributions[3] = rwBetweennessDistribution;
        }

        /// <summary>
        /// Random walk closeness distribution.
        /// </summary>
        private void GetRandomWalkClosenessDistribution()
        {
            var rwCloseness = measures.RandomWalkCloseness(multilayerNetwork);

            var rwClosenessDistribution = new ConcurrentDictionary<double, double>();

            Parallel.ForEach(rwCloseness, (item) =>
            {
                rwClosenessDistribution.AddOrUpdate(item.Value, 1, (key, oldValue) => oldValue + 1);
            });

            distributions[4] = rwClosenessDistribution;
        }

        /// <summary>
        /// Exports items into directories.
        /// </summary>
        private void ExportIntoDirectory()
        {
            // Flag 0/3 == Summary
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


            if (exportFlag == 0 || exportFlag == 3)
            {
                var fileType = exportFlag == 0 ? ".csv" : ".txt";

                File.WriteAllText(@exportFolderName + "\\" + networkDir + "\\" + summaryFolder + "\\" + multilayerNetwork.Name + "_" + multilayerNetwork.Actors.Count + "a" + fileType, LayerSummary());
            }
            else if (exportFlag == 1)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0].Replace(' ', '_'); 
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_');


                var fileName = multilayerNetwork.Name + "_" + dist + ".PNG";
                var path = @exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\";

                using (var fileStream = File.Create(path + fileName))
                {
                    var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 1980, Height = 1700, Background = OxyColors.White };
                    pngExporter.Export(GetExportPlotModel(SelectedPlotIndex, logX, LogY), fileStream);
                    //pngExporter.Export(mainPlotModel, fileStream);
                }

                //fileName = multilayerNetwork.Name + "_" + dist + ".SVG";

                //using (var stream = File.Create(path + fileName))
                //{
                //    var exporter = new SvgExporter { Width = 600, Height = 400 };
                //    exporter.Export(mainPlotModel, stream);
                //}
            }
            else if (exportFlag == 2)
            {
                var dist = "";

                if (cumulative)
                    dist = distributionLabels[SelectedPlotIndex + cumulativeOffset][0];
                else
                    dist = distributionLabels[SelectedPlotIndex][0].Replace(' ', '_');

                var fileName = multilayerNetwork.Name + "_" + dist + ".PDF";
                var path = @exportFolderName + "\\" + networkDir + "\\" + plotFolder + "\\";

                using (var stream = File.Create(path + fileName))
                {
                    var pdfExporter = new PdfExporter { Width = 1980, Height = 1700 };
                    pdfExporter.Export(GetExportPlotModel(SelectedPlotIndex, logX, LogY), stream);
                }
            }

        }

        /// <summary>
        /// Calculates most measures.
        /// </summary>
        private void CalculateMetrics()
        {
            if (randomWalkMeasures)
            {
                var st = new Stopwatch();
                st.Start();
                // Wait cursor.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                Task occup = Task.Run(() =>
                {
                    GetOccupationDistribution();
                });

                Task rwBetweenness = Task.Run(() =>
                {
                    GetRandomWalkBetweennessDistribution();
                });

                Task rwCloseness = Task.Run(() =>
                {
                    GetRandomWalkClosenessDistribution();
                });

                // Wait for random walks to finish.
                Task.WaitAll(new Task[] { occup, rwBetweenness, rwCloseness });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }


            // RandomWalks finished, we can flatten the network.
            Flatten();

            foreach (var layer in multilayerNetwork.Layers)
            {
                layerDistributions.TryAdd(layer.Id, new ConcurrentDictionary<double, double>[]
                {
                        new ConcurrentDictionary<double, double>(),
                        new ConcurrentDictionary<double, double>(),
                        new ConcurrentDictionary<double, double>(),
                        new ConcurrentDictionary<double, double>(),
                        new ConcurrentDictionary<double, double>(),
                        new ConcurrentDictionary<double, double>()
                });   
            }


            FlattenedMeasures();

            // Get degree distribution.
            Task.Run(() =>
            {
                GetDegreeDistribution();
                Plot(0, logX, logY);
            });

            // Get local clustering coefficient distribution.
            // And also average clustering coefficient.
            Task.Run(() =>
            {
                GetClusteringCoefficientDistribution();
            });

            // Shortest paths etc.
            if (distanceMeasures)
            {
                var st = new Stopwatch();
                st.Start();
                // Wait cursor.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                adjacencyMatrixByLayerId = multilayerNetwork.ToAdjacencyMatrix();

                GetFloydWarshallMatrices();

                Task closeness = Task.Run(() =>
                {
                    GetClosenessCentrality();
                });

                // Wait for distance measures to finish.
                Task.WaitAll(new Task[] { closeness });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }

            // Connected components + Largest.
            Task.Run(() =>
            {
                var connectedComponents = measures.GetConnectedComponents(multilayerNetwork, flattenedNetwork);
                ConnectedComponentsCount = connectedComponents[0];
                LargestConnectedComponent = connectedComponents[1];
            });

        }

    }
}
