   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	       GUI.LayerDetailWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	     � Title$  LayerDetailWindow��5
   	   $	��640��6#   $	��640��63   $	��800��6@   $	��800��6O   .��5      � -   5        $��mainGrid��5       � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��10,10,11,10q�6   $��Layers��6(   $��1��68   $��0��6E   .��5      �� -   5        $��listViewLayers��5      ��6-   �� *Layers+ �� View 5      �� 5       �� ColumnHeaderToolTip$ Layers��6   .P�5      �� $��
Layer Name��6T    �� Width$	 225��6h    �� DisplayMemberBinding 6)   ��  �� Path$
 Name�6h   �� 5      $
��Plot��6)   $ 50��67    �� CellTemplate 5       �� 5!   "   .R�5"   &   �� 	 ��Name$	 plotLayerChck��6B   -   5        $��10,0,0,0q�5"   0   6y   5#   #   5%      5'      5*      5+      �� 5-   
   $��Chart��6   $��10,11,10,10q�6"   $��2��67   $��2��6D   $��0��6U   $��3��6e   $��Stretch=�6y   $��Stretch=�6�   .��5.        OxyPlot.Wpf.PlotView  -   5        $��layerDetailPlot��5.      
   Model
 62   �� *LayerPlotModel+5/      51      � 5<   
   $��0��6   $��0��6   $��3��6,    5=      �� 5>      $
��Auto �6#   �� 5?      $
��Auto �6#   �� 5@      $
��Auto �6#    5B      �� 5C      $
��Auto �6    �� 5D      $
��Auto �6    �� 5E      $
��Auto �6    �� 5F      $
��Auto �6    5H      ]� 5J   
   .+�5K      `� $	��Top=�6    `� 
Background
 ���� .�5L      c� 
������ .@�5M      o� 
������ .@�5N      m� $��Plots��6#   .@�5O      m� -   5         m�Name$ plotBtn��5O   '   $
��Plot��68   m� 5P      -   5        $ cumulativeToggle��5P   '    m� IsCheckable .  m� 	IsChecked . $��Cumulative complementary��6e   m� 5Q      -   5        $ 
logXToggle��5Q   '    .  .  $��
Log Axis X��6`   m� 5R      -   5        $ 
logYToggle��5R   '    .  .  $��
Log Axis Y��6`   m� 5S      $��Legend Position��6'   .@�5U   "   m� -	   5        $ legendLeftTop��5U   +   $��Left Top��6B    m� Icon 5V   &   	� 5W   *    	� 	GroupName$ LegendPositionGroup��66    d� 	IsChecked 6V   �� $ ModeArray[0]� �� Mode$ TwoWay=�5Y   #   m� 5Z   "   -
   5        $ legendRightTop��5Z   +   $��	Right Top��6C    5[   &   	� 5\   *   $ LegendPositionGroup��66    6V   �� $ ModeArray[1]�$ TwoWay=�5^   #   m� 5_   "   -   5        $ legendLeftBottom��5_   +   $��Left Bottom��6E    5`   &   	� 5a   *   $ LegendPositionGroup��66    6V   �� $ ModeArray[2]�$ TwoWay=�5c   #   m� 5d   "   -   5        $ legendRightBottom��5d   +   $��Right Bottom��6F    5e   &   	� 5f   *   $ LegendPositionGroup��66    6V   �� $ ModeArray[3]�$ TwoWay=�5h   #   5i      m� 5j      $��Export��6'   .@�5k   "   m� -   5        $ 	exportPdf��5k   +   $	��PDF��6>   m� 5l   "   -   5        $ 	exportPng��5l   +   $	��PNG��6>   m� 5m   "   -   5        $ 	exportCsv��5m   +   $	��CSV��6>   5n      5o      5p      �� 5q      �� 5r      .��6   Distribution:6+   �� 5s      -   5        $��	plotIndex��5s      $��Center=�60   $	��130��6M   $��5,0,0,1q�6Y   .@�5t      ��  �� 
IsSelected . .��69   Degree6A   �� 5u      .��6'   Clustering coefficient6?   �� 5v      .��6'   Avg CC by Degree69   �� 5w      .��6'   	Relevance62   �� 5x      .��6'   Exclusive neighborhood6?   �� 5y      .��6'   Exclusive relevance6<   5z      o� 5{      
������ 5|      5~      5      5�      5�      5�      