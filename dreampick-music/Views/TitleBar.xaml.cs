using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dreampick_music;

public partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }

    private void HideButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            if (e.ClickCount == 2)
            {
                Application.Current.MainWindow.WindowState =
                    Application.Current.MainWindow.WindowState != WindowState.Maximized
                        ? WindowState.Maximized
                        : WindowState.Normal;
            }
            else Application.Current.MainWindow.DragMove();
        }
    }

    private void Icon_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (App.Current.MainWindow.DataContext is MainVm vm)
        {
            vm.TabsCollapsed = !vm.TabsCollapsed;
        }
    }

    private void NewClose_LeftMouseDown(object sender, MouseButtonEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void NewClose_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }
}