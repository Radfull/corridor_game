   M S B A M L   `   `   `  ����B  >Koridor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:KoridorKoridor  5   	       Koridor.SettingsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Koridor     5   	     � Title$  SettingsWindow��5   	   $	��400��6    $	��405��6-    � SizeToContent$ Manual=�5	   	   .��5
      � $��Center=�6   $��Center=�6(    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��192.5 �6   �� 5      $��192.5 �6   .+�5   
   �� $��0��6   $��2��6   
���iii� �� CornerRadius$ 20��6G   $��16,10,16,10q�6Y    �� Padding$ 10q�6n   ._�5      �� $!��НАСТРОЙКИ ИГРЫ��6   $��20(�6.   
�������$��Center=�6O   $��Center=�6l   $
��Bold"�6�   5      �� 5   
   $��1��6   $��0��6   
�������$ 20��6E   $��10,10,10,10q�6W   $ 10q�6l   $��172.5��5      $	��170��6   ._�5      �� .+�5      �� $#��ВЫБОР СОПЕРНИКА��6   $��14(�63   
���iii�$��Center=�6V   $
��Bold"�6s   	� 5      -   5         	�Name$ 	BotButton��5      
���iii�$ ��ИГРАТЬ С БОТОМ��6G   $��12(�6`   $��Verdana)�6n   $
��Bold"�6�    	� 	GroupName$ Opponent��6�   $��0,5,0,0q�6�    d� 	IsChecked$
 TrueY�6�   	� 5      -   5        $ FriendButton��5      
���iii�$"��ИГРАТЬ С ДРУГОМ��6K   $��12(�6e   $��Verdana)�6s   $
��Bold"�6�   $ Opponent��6�   $��0,5,0,0q�6�   5      5       �� 5"   
   $��1��6   $��1��6   
�������$ 20��6E   $��10,10,10,10q�6W   $ 10q�6l   $��172.5��5#      $	��170��6   ._�5$      �� .+�5%      �� $#��ПАРАМЕТРЫ ЗВУКА��6   $��14(�63   
���iii�$��Center=�6V   $
��Bold"�6s   �� 5'      $$��Общая громкость:��6   $��12(�64   
���iii�$��0,5,0,0q�6W   �� 5(      -   5        	 ��Name$	 MasterVolumeSlider��5(      
 � Minimum$
 0Q�65    � Maximum$	 100Q�6A    � Value$ 50Q�6O   $��0,5,0,0q�6Z   6k   �� 5*      $&�� Громкость музыки:��6   $��12(�65   
���iii�$��0,5,0,0q�6X   �� 5+      -   5        $	 MusicVolumeSlider��5+      $
 0Q�64   $	 100Q�6@   $ 50Q�6N   $��0,5,0,0q�6Y   6j   �� 5-      $*��$Громкость эффектов:��6   $��12(�67   
���iii�$��0,5,0,0q�6Z   �� 5.      -   5        $	 EffectsVolumeSlider��5.      $
 0Q�66   $	 100Q�6B   $ 50Q�6P   $��0,5,0,0q�6[   6l   5/      50      �� 52   
   $��2��6   $��2��6   
�������$ 20��6I   $��
10,0,10,10q�6[   $ 10q�6o   $��80��6|   ._�53      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   $��Center=�6O   .+�54      �� $��Размер поля:��6   $��14(�60   
���iii�$
��Bold"�6S   $��0,0,10,0q�6e   �� 55      -   5         ��Name$ BoardSizeComboBox��55      $��80��66   .@�56      �� $	��9x9��6#    �� 
IsSelected$
 True��61   �� 57      $��11x11��6#   �� 58      $��13x13��6#   �� 59      $��15x15��6#   5:      5;      5<      5=      5>      