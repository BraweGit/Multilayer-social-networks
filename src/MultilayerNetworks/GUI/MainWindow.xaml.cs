using OxyPlot;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MultilayerNetworkViewModel multilayerNetworkViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            
        }

        private void Initialize()
        {
            multilayerNetworkViewModel = new MultilayerNetworkViewModel();
            DataContext = multilayerNetworkViewModel;
            BindControls();
        }

        private void BindControls()
        {
            this.fileOpenBtn.Click += new RoutedEventHandler(multilayerNetworkViewModel.ImportMultilayerNetwork);
            this.toCsvBtn.Click += new RoutedEventHandler(multilayerNetworkViewModel.ToCsv);
            this.layersGrp.SelectionChanged += new SelectionChangedEventHandler(multilayerNetworkViewModel.SelectedLayerChanged);
            this.plotBtn.Click += new RoutedEventHandler(multilayerNetworkViewModel.PlotBtnClick);
            this.tabControl.SelectionChanged += new SelectionChangedEventHandler(multilayerNetworkViewModel.TabControl_SelectionChanged);
            this.plotIndex.SelectionChanged += new SelectionChangedEventHandler(multilayerNetworkViewModel.PlotIndexChanged);


            this.logXToggle.Checked += new RoutedEventHandler(multilayerNetworkViewModel.LogXChck_Checked);
            this.logXToggle.Unchecked += new RoutedEventHandler(multilayerNetworkViewModel.LogXChck_Checked);
            this.logYToggle.Checked += new RoutedEventHandler(multilayerNetworkViewModel.LogYChck_Checked);
            this.logYToggle.Unchecked += new RoutedEventHandler(multilayerNetworkViewModel.LogYChck_Checked);

            this.cumulativeToggle.Checked += new RoutedEventHandler(multilayerNetworkViewModel.CumulativeToggle_Checked);
            this.cumulativeToggle.Unchecked += new RoutedEventHandler(multilayerNetworkViewModel.CumulativeToggle_Checked);

            this.exportPng.Click += new RoutedEventHandler(multilayerNetworkViewModel.ChartToPng);
            this.exportPdf.Click += new RoutedEventHandler(multilayerNetworkViewModel.ChartToPdf);


            this.layerComparisonBtn.Click += new RoutedEventHandler(multilayerNetworkViewModel.LayerDetail);
            this.summaryBtn.Click += new RoutedEventHandler(multilayerNetworkViewModel.ExportSummary);
            this.summaryBtnTxt.Click += new RoutedEventHandler(multilayerNetworkViewModel.ExportSummaryTxt);


        }




    }
}
