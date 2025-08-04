using ChessApp.Models;
using ChessApp.Viewmodel;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ChessApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ChessViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Assuming 'this' is your Window instance
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(1000, 1000)); // Set width to 800 pixels and height to 600 pixels
            // Create 8×8 chessboard
            this.ViewModel = new ChessViewModel();
            //RootGrid.DataContext = ViewModel;

        }

        private void Cell_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            var border = (Border)sender;
            var cell = (Cell)border.Tag;
            ViewModel.handleClick(cell);
        }
      

    }

  
}
