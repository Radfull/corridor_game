   M S B A M L   `   `   `  ����  Koridor    Koridor.StatisticWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$#  Статистика Игры��5   	   $	��400��6!   $	��600��6.   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   �� $!��Статистика игр��6   $��Center=�6)   $��10q�6F   $��18(�6R   $
��Bold"�6`   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ StatisticsDataGrid��5      $��1��6/      AutoGenerateColumns$ False��6<      
IsReadOnly$
 True��6X   $��10q�6j      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Дата игры��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6U   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *
GameDate+6U     5      $ Победитель��6%   $ * 6X    69   �� *
WinnerName+6X     5      $% Количество ходов��6%   $
 Auto 6]    6?   �� *	MoveCount+6]     5      $' !Длительность игры��6%   $
 Auto 6a    6@   �� *GameDuration+6a   5      5      5      