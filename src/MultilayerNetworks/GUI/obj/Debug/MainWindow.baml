   M S B A M L   `   `   `  ����R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35L5clr-namespace:System.Diagnostics;assembly=WindowsBaseSystem.Diagnostics  5   	   > :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI 5   	      GUI.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml      5   	   @d2http://schemas.microsoft.com/expression/blend/2008      5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006      5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	   @diag5clr-namespace:System.Diagnostics;assembly=WindowsBase  5   	   localclr-namespace:GUI  5   	     � Title$  Multilayer Networks��5
   	   $	��720��6%   $	��720��62   $	��720��6B   $	��920��6R   $	��920��6^   $	��920��6m    � DataContext 5      $  GUI.MultilayerNetworkViewModel  5   
    � 	Resources 5       �� 
TargetType �� Property �� Value   FocusVisual J� SnapsToDevicePixels �� StrokeDashArray  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground 
 
btnStyle_1	 �� 
TargetType
 ��Name �� CornerRadius � Cursor ��Name �� Triggers P� Property P� Value �� 
TargetName��%$	  '��       '�� �     &	      &	 )    &	 L    &	 o    &	 �    &	 �    &	 �    &	 �    &	     &		 A    &	
 d    7   7  7  7  7  7  7  7  7  7 	 �� 5   
   " ��6   .�5      ��  � ��$ 14(�6)   �� 5       � ��$ Calibri)�6+   �� 5       � ��$ Medium"�6+   5      �� 5   
   " ��6   .�5      ��  � ��$ 25,1,5,5q�6'   �� 5       � �� 5      �� 5      .R�5      �� $
��Bold"�65   $��Calibri)�6G   $��13(�6\   ��6$   �� 6\   5      5      5      �� 5   
   .�5      ��  � �� 5       �� 5!      .R�5"      �� $��2q�6$    . #��� ��6J   $��1��6�   $	 1 2R�6�   5#      5%      5&      �� 5'   
   $ 	#FFDDDDDD��6;   �� 5(   
   $ 	#FF707070��67   �� 5)   
   $ 	#FFBEE6FD��6>   �� 5*   
   $ 	#FF3C7FB1��6:   �� 5+   
   $ 	#FFC4E5F6��6<   �� 5,   
   $ 	#FF2C628B��68   �� 5-   
   $ 	#FFF4F4F4��6=   �� 5.   
   $ 	#FFADB2B5��69   �� 5/   
   $ 	#FF838383��6=   �� 50   
   " ��6#   .�51      ��  �@�FocusVisualStyle8   61   �� 52       � ��8  6+   �� 53       � ��8  6,   �� 54       � ��# � ��6+   �� 55       � ��$ 1q�60   �� 56       � ��$ Center=�6;   �� 57       � ��$ Center=�69   �� 58       � ��$ 1q�6(   �� 59       � �� 5:      �� 5;      "	 ��6&   .R�5<      �� $
 border��6!   #��z��61   #��z��6]   #��z��6�    . $ 5��6�   $��70��6�   $��35��6�   $��70��6�   $��25��6  $
 Hand��6  ._�5=      �� $ contentPresenter��6/   |�.  #��z��6[   #��z��6�   ��. # z 6�   #��z��6  5>       5?      P� 5@       �@��IsDefaulted . . �5A   "   �� $ border��6@   # � ��6T    � ��5B      P� 5C       �@J�IsMouseOver . . �5D   "   �� $ border��6?   8   � ��6S   �� 5E   "   $ border��6@   8   � ��6T   5F      P� 5G       � �� . . �5H   "   �� $ border��6?   8   � ��6S   �� 5I   "   $ border��6@   8   � ��6T   5J      P� 5K       � {� .  . �5L   "   �� $ border��6?   8   � ��6S   �� 5M   "   $ border��6@   8   � ��6T   �� 5N   "   $ contentPresenter��6K   8 	  � ��6i   5O      5Q      5S      5T      5U      .��5V      � -   5        $��mainGrid��5V       � ColumnDefinitions 5W   
   �� 5X      $	��650 �6   �� 5Y      $��* �6    � RowDefinitions 5[   
   �� 5\      $��25��6   �� 5]      $	��155��6   �� 5^      $	��285 �6   $	��285��6)   �� 5_      $��* �6   .+�5b   
   �� -   5        $��
tabControl��5b      $��0��6'   $��2��67   $��2��6K   $��2��6X   .@�5c      �� $��Actors��6   .��5d      �� -   5        $��ListViewActors��5d   '   $��10q�6   ��6=   �� *Actors+ �� View 5e      �� 5f      .P�5g      �� $
��Name��6-    �� Width$	 200��6;    �� DisplayMemberBinding 6G   �� *Name+�� 5h      $��Degree��6-   $ 60��6=    6H   �� *Degree+�� 5i      $��Degree deviation��6-   $	 110��6G    6S   �� *DegreeDeviation+ �� StringFormat$ N5���� 5k      $��	Closeness��6-   $ 70��6@    6K   �� *	Closeness+$ N5���� 5l      $��CC��6-   $ 60��69    6D   �� *ClusteringCoefficient+$ N5���� 5m      $��Neighborhood��6-   $ 90��6C    6N   �� *NeighborhoodCentrality+5o      5q      5r      �� 5s      $��Nodes��6   .��5t      �� -   5        $��ListViewNodes��5t   '   $��10q�6   ��6<   �� *Nodes+ 5u      �� 5v      .P�5w      �� $
��Name��6-   $	 200��6;    6G   �� *Name+�� 5x      $��Degree��6-   $	 100��6=    6I   �� *Degree+�� 5y      $��	Closeness��6-   $	 100��6@    6L   �� *ClosenessCentrality+$ N5���� 5z      $��CC��6-   $	 100��69    6E   �� *ClusteringCoefficient+$ N5��5{      5}      5~      �� 5      $��Layers��6   .��5�      �� -   5        $��ListViewLayers��5�   '   $��10q�6   ��6=   �� *Layers+ 5�      �� 5�      .P�5�      �� $
��Name��6-   $	 200��6;    6G   �� *Name+�� 5�      $��n��6-   $ 50��68    6C   �� *
NodesCount+�� 5�      $��Density��6-   $	 100��6>    6J   �� *	Density+$ N5���� 5�      $��CC��6-   $	 200��69    6E   �� *AverageClusteringCoefficient+$ N5���� 5�      $��Avg degree centrality��6-   $	 200��6L    6X   �� *AverageDegreeCentrality+$ N5��5�      5�      5�      �� 5�      $��Selected layer��6   .��5�      �� -   5        $��ListViewSelectedLayer��5�   '   $��10q�6   ��6D   �� *SelectedActors+ 5�      �� 5�      .P�5�      �� $
��Name��6-   $	 200��6;    6G   �� *Name+�� 5�      $��Degree��6-   $	 100��6=    6I   �� *SelectedDegree+�� 5�      $��Connective redundancy��6-   $	 150��6L    6X   �� *SelectedConnectiveRedundancy+$ N5���� 5�      $��CC��6-   $	 100��69    6E   �� *SelectedClusteringCoefficient+$ N5���� 5�      $��	Relevance��6-   $	 100��6@    6L   �� *SelectedRelevance+$ N5��5�      5�      5�      �� 5�      $
��Plot��6   .��5�        OxyPlot.Wpf.PlotView     Model 6   �� *MainPlotModel+5�      5�      5�      �� 5�   
   $��1��6   $��1��6#   $��Network��60   $��Stretch=�6A   $
��Auto��6_   $��10,10,10,10q�6m   $	��Top=�6�   $
��Auto��6�   .��5�      � -   5        $��networkDescriptionGrid��5�       5�      �� 5�      $
��Auto �6'   �� 5�      $
��Auto �6'    5�      �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   .+�5�      �� $��0��6   $��0��6(   .��65   Network name:6D   �� 5�      $��0��6   $��1��6(   .��65   Number of actors:6H   �� 5�      $��0��6   $��2��6(   .��65   Number of nodes:6G   �� 5�      $��0��6   $��3��6(   .��65   Number of edges:6G   �� 5�      $��0��6   $��4��6(   .��65   Number of layers:6H   �� 5�      $��1��6   $��0��6(   ��65   �� *NetworkName+ �� UpdateSourceTrigger$ PropertyChanged=��� 5�      $��1��6   $��2��6(   ��65   �� *
NodesCount+$ PropertyChanged=��� 5�      $��1��6   $��3��6(   ��65   �� *
EdgesCount+$ PropertyChanged=��� 5�      $��1��6   $��4��6(   ��65   �� *LayersCount+$ PropertyChanged=��� 5�      $��1��6   $��1��6(   ��65   �� *ActorsCount+$ PropertyChanged=�5�      5�      �� 5�   
   $��0��6   $��1��6#   $��Layer��60   $��Stretch=�6?   $
��Auto��6]   $��10,10,10,10q�6k   $	��Top=�6�   $
��Auto��6�   .��5�      � -   5        $��selectedLayerGrid��5�       5�      �� 5�      $
��Auto �6'   �� 5�      $
��Auto �6'   �� 5      $
��Auto �6'   �� 5     $
��Auto �6'   �� 5     $
��Auto �6'   �� 5     $
��Auto �6'   �� 5     $
��Auto �6'    5     �� 5     $
��Auto �6$   �� 5	     $
��Auto �6$   �� 5
     $
��Auto �6$   �� 5     $
��Auto �6$   .+�5     �� $��5,0,5,0q�6   $��3��6)   $��3��69   .��6F   Layer density:6V   �� 5     $��5,0,5,0q�6   $��4��6)   $��3��69    �� ContentStringFormat$ 
{0:0.####}��6�   ��6F   �� *SelectedDensity+$ PropertyChanged=�6�   �� 5     $��5,0,5,0q�6   $��1��6)   $��1��69   .��6F   Min degree:6S   �� 5     $��5,0,5,0q�6   $��2��6)   $��1��69   ��6F   �� *SelectedMinDegree+$ PropertyChanged=��� 5     $��5,0,5,0q�6   $��1��6)   $��2��69   .��6F   Max degree:6S   �� 5     $��5,0,5,0q�6   $��2��6)   $��2��69   ��6F   �� *SelectedMaxDegree+$ PropertyChanged=��� 5     $��5,0,5,0q�6   $��1��6)   $��3��69   .��6F   Average degree:6W   �� 5     $��5,0,5,0q�6   $��2��6)   $��3��69   $ 
{0:0.####}��6�   ��6F   �� *SelectedAverageDegree+$ PropertyChanged=�6�   �� 5     $��5,0,5,0q�6   $��5��6)   $��2��69   .��6F   Average cc:6S   �� 5     $��5,0,5,0q�6   $��6��6)   $��2��69   $ 
{0:0.####}��6�   ��6F   �� *&$SelectedAverageClusteringCoefficient+$ PropertyChanged=�6�   �� 5     $��5,0,5,0q�6   $��3��6)   $��1��69   .��6F   
Average:6P   �� 5     $��5,0,5,0q�6   $��4��6)   $��1��69   $ 
{0:0.####}��6�   ��6F   �� *SelectedAverage+$ PropertyChanged=�6�   �� 5      $��5,0,5,0q�6   $��3��6)   $��2��69   .��6F   Average distance:6Y   �� 5!     $��5,0,5,0q�6   $��4��6)   $��2��69   $ 
{0:0.####}��6�   ��6F   �� *SelectedAverageDistance+$ PropertyChanged=�6�   �� 5#     $��5,0,5,0q�6   $��5��6)   $��3��69   .��6F   Connected components:6]   �� 5$     $��5,0,5,0q�6   $��6��6)   $��3��69   ��6F   �� *" SelectedConnectedComponentsCount+$ PropertyChanged=��� 5&     $��5,0,5,0q�6   $��5��6)   $��1��69   .��6F   Largest connected component:6d   �� 5'     $��5,0,5,0q�6   $��6��6)   $��1��69   ��6F   �� *#!SelectedLargestConnectedComponent+$ PropertyChanged=�5)     5+     ]� 53  
   $��2��6   .+�54     `� $	��Top=�6    `� 
Background
 ���� .�55     c� 
������ .@�56     o� 
������ .@�57     m� $
��File��6#   .@�58     m� -	   5         m�Name$ fileOpenBtn��58  '   $
��Open��6<   m� 59     -
   5        $ toCsvBtn��59  '   $��To CSV��69   5:     5;     �� 5<     o� 5=     
������ .@�5>     m� $��Plots��6#   .@�5?     m� -   5        $ plotBtn��5?  '   $
��Plot��68   m� 5@     -   5        $ cumulativeToggle��5@  '    m� IsCheckable .  m� 	IsChecked . $��Cumulative complementary��6e   m� 5A     -   5        $ 
logXToggle��5A  '    .  .  $��
Log Axis X��6`   m� 5B     -   5        $ 
logYToggle��5B  '    .  .  $��
Log Axis Y��6`   m� 5C     $��Export��6'   .@�5D  "   m� -   5        $ 	exportPdf��5D  +   $	��PDF��6>   m� 5E  "   -   5        $ 	exportPng��5E  +   $	��PNG��6>   5F     5G     5H     �� 5I     �� 5J     .��6   Distribution:6+   �� 5K     -   5        $��	plotIndex��5K     $��Center=�60   $	��130��6M   $��5,0,0,1q�6Y    �� SelectedIndex$ 0��6j   ��6|   �� *DistributionsComboBox+�� 5L     �� 5M     .��6   Layer:6$   �� 5N     -   5        $��	layersGrp��5N     $	��100��60     �� DisplayMemberPath$
  Name��6^   ! �� SelectedValuePath$! Id��6w   $ 0��6�   $��5,0,0,1q�6�   ��6?   �� *Layers+6�   o� 5O     
������ .@�5P     m� -   5        $ layerComparisonBtn��5P  #   $��Layer Comparison��6?   5Q     o� 5R     
������ .@�5S     m� $��Export summary��6#   .@�5T     m� -   5        $ 
summaryBtn��5T  '   $	��CSV��6;   m� 5U     -   5        $ summaryBtnTxt��5U  '   $	��TXT��6>   5V     5W     5Y     5Z     5[     5\     5]     