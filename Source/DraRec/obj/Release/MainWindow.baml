   M S B A M L   `   `   `  ���� J  FDrawingRecorder, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null)clr-namespace:DRnamespaceDRnamespace      DRnamespace.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:DRnamespace     -     �Name$  ControlBoard�� � Title$ Drawing Recorder��$��80��$	��521�� � WindowStyle$
 None=� � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=�$��0q����	#BF191919$��3q� J� SnapsToDevicePixels . ��Z� .��� -    �Name$ baseGrid��$��3,0,3,3q� � RowDefinitions �� $��12 ��� -    ��Name$ tool_bar��$	��11* ��� -   $ setting_bar��$	��48* �	 � ColumnDefinitions	 �� $	��22* ��� $	��15* ��� $��7 ��� $
��361* ��� $
��104* �.+��� $��5��   
Background#���   �� -   
 ��Name$
 DragBox��
������� � Cursor$
 Hand��$
��Bold"� J� Opacity$	 0.8Q�$��1��$��2������  �� ImageSource$ Resources/catch_bar.png�� j� Stretch$ Uniform=��� $��2��#���   $��1��$��2���� -    ��Name$ 
BarChanger��
�������|�.  $��1��$��0q�$��1��$��2��$	 0.8Q����� $ Resources/BarBut.png��$ Uniform=�� $��3��$��0,0,0,1q�$��1��$��2��	 �� $��60 ��� $��60 ��� $��60 ��� $��60 ��� $��96 ��� �� $��60 ��� $��60 �.+��� -   $ CaptureButton��|�.  $��Capture��
���   �$��1��
�������$
��Bold"�$	 0.8Q���Z� �� -   $ RecordButton��|�.  $��2��
������� J� RenderTransformOrigin$ 0.5,0.5&�$��Record��$
��Bold"�$	 0.8Q���Z� �� -	   $ 
AreaButton��|�.  $
��Area��
���   �
�������$
��Bold"�$	 0.8Q���Z� �� -
   $ 
ExitButton��$��7��$�� �� �� IsCancel . ��Z� ��Z� ���� $ Resources/Exit.png��$ Uniform=��� -   $ Lock_Button��$��4��$��16(�
�������|�.  
�������$	 0.8Q��� -   $ ProportionButtion��|�.  
�������$
��None��$��3��$
��Bold"�$	 0.8Q���Z� �� -   $ 
HideButton��$��6��$�� �� . $��24(���Z� ��Z� ���� $ Resources/hide.png��$ Uniform=�� -   $ SettingGrid��$��2�����	#FF262829$��3��$��2��	 �� $��10 ��� $	��73* ��� $	��65* ��� $��5 ��� $
��109* ��� $	��98* ��� $
��100* ��� $��5 � �� $��5 ��� �� $��5 ��� �� $��5 �.+��� -   $
 frameRate_textBlock��$
��Wrap=�$��	FrameRate��$��0,0,3,0q�$��1��$��1��  TextBlock_TextColor#���  �� -   $
 speed_textBlock��$��3��$
��Wrap=�$��Speed��$��1��#���  �� -    ��Name$ FrameRateBox��$��2�� �� TextWrapping$
 Wrap=�$��30��$��1��  TextBox_CaretColor �� 
CaretBrush# �    TextBox_TextColor#���    TextBox_Color#���  $��Stretch=�$��Stretch=��� -   $ SpeedBox��$��2��$��3��$
 Wrap=�$	��0.1��# �  #���  #���  $��Stretch=�$��Stretch=��� -    ��Name$ DC_box��$��DetectChange��$��3��$��4�� d� 	IsChecked$
 TrueY�#���  $��0,0,0,1q��� -   $
 fileNane_textBlock��$��4��$��1��$
��Wrap=�$��FileName��#���  �� -   $ OpenFolderButton��$��4��$��1��$ 0.5,0.5&�$��21��$��0,0,5,0q�$��Right=����� $ Resources/folder.png��$ UniformToFill=��� -   $
 slash��$��5��$��1��$
��Wrap=�$��/��#���  $��	97,0,90,0q� �� TextAlignment$ Center=�$��2���� -   $ FileNameBox��$��6��$��1��$
 Wrap=�$��output��#���  #���  # �  $��Stretch=�$��Stretch=�$��15,0,0,0q��� -   $ DirectoryBox��$��5��$��1��$
 Wrap=�$��output��#���  #���  # �  $��Stretch=�$��Stretch=�$��0,0,6,0q��� -   $ ArgumentBox��$��5��$��3��$�� ��#���  #���  # �  $��Stretch=�$��Stretch=�$��2��{�.  $y�Hidden=��� -   $ MD_box��$��	MouseDown��$��3��$��5��$
 TrueY�#���  $��0,0,1,1q��� -   $ RM_box��$��RecordMouse��$��3��$��6��$
 TrueY�#���  $��0,0,1,1q��� $��3��#���   $��
336,0,17,1q�$��1��