   M S B A M L   `   `   `  ���� J  FDrawingRecorder, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null)clr-namespace:DRnamespaceDRnamespace      DRnamespace.AreaWind   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:DRnamespace     -     � Title$  AreaWind��$��2q� � WindowStartupLocation$ Manual=���.  |�.   � WindowStyle$
 None=� � AllowsTransparency . ���	#02000000���	#FF263940$	��480��$	��720�� � 
ResizeMode$ CanResizeWithGrip=� � Topmost .  � Cursor$
 Hand�� � ShowInTaskbar .  $y�Visible=�$	��100��$	��100����Z� .��� $��0q� � RowDefinitions �� $��14 ��� $
��100* ��� $
��202* ��� $
��100* ��� $��14 �	 � ColumnDefinitions	 �� $��14 ��� $
��200* ��� $
��404* ��� $
��200* ��� $��14 �.+��� -   
 ��Name$
 Up��$��2��   
Background#���   $ SizeNS���� -   $

 Down��$��2��$��4��#���   $ SizeNS���� -   $
 Left_��$��2��#���   $ SizeWE���� -   $
 Right��$��4��$��2��#���   $ SizeWE���� -   $
 Center��$��1��$��1��$��3��$��3��� -    �Name$ SizeBar��$��1��$��0��$
��Left=�$	��150��	 �� $	��20* ��� $	��70* ��� $	��20* ��� $	��70* ��� $	��20* �.+��� -    ��Name$ WidthBox��$��1��
���   � �� TextAlignment$ Center=��� $��2��$��X��
������� �� TextAlignment$ Center=�$��Center=�$��Center=��� -	   $ 	HeightBox��$��3��
���   �$ Center=��� -
    ��Name$ AreaExit��$��0��$��15��$��15��$��Right=�$	��Top=�����  �� ImageSource$ Resources/exit.png�� j� Stretch$ Uniform=� �� RelativeTransform V� .~���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q� �� ScaleY$ 2Q� �� ScaleX$ 2Q���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q�T� 