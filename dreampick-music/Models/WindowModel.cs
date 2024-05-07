namespace dreampick_music.Models;

public class WindowModel
{
    public static void SwitchToMainWindow()
    {
        if (App.Current.MainWindow is not AuthenticationWindow window) return;
        MainWindow mainWindow = new()
        {
            ShowActivated = true,
            Topmost = true,
        };
        App.Current.MainWindow = mainWindow;
        App.Current.MainWindow.Show();
        mainWindow.Topmost = false;
        window.Close();
    }
    
    public static void SwitchToLoginWIndow()
    {
        if (App.Current.MainWindow is not MainWindow window) return;
        AuthenticationWindow mainWindow = new()
        {
            ShowActivated = true,
            Topmost = true,
        };
        
        App.Current.MainWindow = mainWindow;
        App.Current.MainWindow.Show();
        mainWindow.Topmost = false;
        window.Close();
    }
}