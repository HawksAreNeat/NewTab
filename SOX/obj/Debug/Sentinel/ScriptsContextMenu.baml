   M S B A M L   `   `   `  ����  bootstrapper9,clr-namespace:Sentinel;assembly=bootstrapperSentinel  5   �   SOX!   Sentinel.ScriptsContextMenu   6	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6/   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6q   @d2http://schemas.microsoft.com/expression/blend/2008     6�   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�   Asentinel,clr-namespace:Sentinel;assembly=bootstrapper     6�  ���	#00FFFFFF  � AllowsTransparency  .  � WindowStyle$
 None=�6]   � 
ResizeMode$ NoResize=�6p   � Title$ ScriptsContextMenu��6�   � SizeToContent$ WidthAndHeight=�6�   � ShowInTaskbar .  .��5      � .+�5   
   �� $��10q�6    �� Padding$ 3q�6   ���#202020 �� CornerRadius$ 6��6>    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      	   
BlurRadius$	 12Q�6#   
   	Direction$	
 -90Q�63      ShadowDepth$ 2Q�6C   ._�5      �� $ 6��6   ���#181818._�5	      �  J� OpacityMask 5
      !� 5       j� Stretch$
 None=�6&    !� Visual 65   ��  �� ElementName$
 Mask��.+�5      �� -   5        $
��Mask��5      $ 4��6)   ���#1818186:   �� 5       � 	Resources 5         
ListButton �� BasedOn �� 
TargetType �� Property �� Value�� 5      %[   '��       7   �� 5   "   8   " ��6f   .�5   &   ��  � ��$ 21��6?   5   #   5      .+�5      �� -   5        .��5   1   	Execute6:   �� 5      -   5        .��5   .   Load64   5      5      5      5      5      5      