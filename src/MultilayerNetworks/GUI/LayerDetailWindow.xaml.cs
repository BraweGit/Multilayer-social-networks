using MultilayerNetworks.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for LayerDetailWindow.xaml
    /// </summary>
    public partial class LayerDetailWindow : Window
    {
        public LayerDetailViewModel layerDetailViewModel { get; set; }
        public LayerDetailWindow(ConcurrentDictionary<int, ConcurrentDictionary<double, double>[]> layerDistributions, MultilayerNetwork multilayerNetwork)
        {
            InitializeComponent();
            layerDetailViewModel = new LayerDetailViewModel(layerDistributions, multilayerNetwork);
            DataContext = layerDetailViewModel;
            BindControls();
        }

        private void BindControls()
        {
            this.plotBtn.Click += new RoutedEventHandler(layerDetailViewModel.PlotBtnClick);
            this.plotIndex.SelectionChanged += new SelectionChangedEventHandler(layerDetailViewModel.PlotIndexChanged);

            this.logXToggle.Checked += new RoutedEventHandler(layerDetailViewModel.LogXChck_Checked);
            this.logXToggle.Unchecked += new RoutedEventHandler(layerDetailViewModel.LogXChck_Checked);
            this.logYToggle.Checked += new RoutedEventHandler(layerDetailViewModel.LogYChck_Checked);
            this.logYToggle.Unchecked += new RoutedEventHandler(layerDetailViewModel.LogYChck_Checked);

            this.exportPng.Click += new RoutedEventHandler(layerDetailViewModel.ChartToPng);
            this.exportPdf.Click += new RoutedEventHandler(layerDetailViewModel.ChartToPdf);
            this.exportCsv.Click += new RoutedEventHandler(layerDetailViewModel.ChartToCsv);


            this.cumulativeToggle.Checked += new RoutedEventHandler(layerDetailViewModel.CumulativeToggle_Checked);
            this.cumulativeToggle.Unchecked += new RoutedEventHandler(layerDetailViewModel.CumulativeToggle_Checked);
        }



        private void plotLayerChck_Checked(object sender, RoutedEventArgs e)
        {
            layerDetailViewModel.plotLayerChck(sender, e);
        }

        private void plotLayerChck_Unchecked(object sender, RoutedEventArgs e)
        {
            layerDetailViewModel.plotLayerChck(sender, e);
        }
    }
}
