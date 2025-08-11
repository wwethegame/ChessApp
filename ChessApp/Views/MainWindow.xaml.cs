using ChessApp.Models;
using ChessApp.Viewmodel;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
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
            this.ViewModel = new ChessViewModel();


            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Assuming 'this' is your Window instance
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(1000, 1000)); // Set width to 800 pixels and height to 600 pixels
                                                         // Create 8×8 chessboard
            InitializeComponent();
            //RootGrid.DataContext = ViewModel;

        }
        private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // get pointer position relative to the Grid
            var pt = e.GetCurrentPoint(RootGrid).Position;

            // DragPreview.ActualWidth == 36 once the template is applied
            double half = DragPreview.ActualWidth / 2;


            Canvas.SetLeft(DragPreview, pt.X - half);
            Canvas.SetTop(DragPreview, pt.Y - half);
        }



        private void Cell_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var border = sender as Border;

            border.BorderBrush = new SolidColorBrush(Colors.Yellow); // Glowing color
        }

        private void Cell_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var border = sender as Border;
            border.BorderBrush = border.Background;
        }


        private void Cell_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {

                var border = (Border)sender;
                var cell = (Cell)border.Tag;
                ViewModel.handleClick(cell);
            }

        }

        private void Cell_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {

                var border = (Border)sender;
                var cell = (Cell)border.Tag;

                ViewModel.handleClickRelease(cell);
            }

        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RestartGame();
        }

    }


}
