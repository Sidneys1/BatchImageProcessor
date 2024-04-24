﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BatchImageProcessor.Controls
{
    /// <summary>
    ///     Interaction logic for SplitButton.xaml
    /// </summary>
    public partial class SplitButton
    {
        public SplitButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event Action<object, EventArgs> Click = delegate { };

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // TODO ContextMenu is no longer supported. Use ContextMenuStrip instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
            if (ContextMenu == null) return;
            ContextMenu.IsEnabled = true;
            ContextMenu.PlacementTarget = (sender as Button);
            ContextMenu.Placement = PlacementMode.Bottom;
            ContextMenu.IsOpen = true;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Click(this, new EventArgs());
        }
    }
}