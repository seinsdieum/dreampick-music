using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace dreampick_music;

public class FrameController
{
    private readonly string FramesPath = "";

    public static Action Settings(Frame frame)
    {
        return (() =>
        {
            frame.Source = new Uri(Config.Instance.PageSources[0], UriKind.Relative);
        });
    }
    
    public static Action AccountPage(Frame frame)
    {
        return (() =>
        {
            frame.Source = new Uri(Config.Instance.PageSources[2], UriKind.Relative);
        });
    }
    public static Action CollectionPage(Frame frame)
    {
        return (() =>
        {
            frame.Source = new Uri(Config.Instance.PageSources[1], UriKind.Relative);
        });
    }
    public static Action FeedPage(Frame frame)
    {
        return (() =>
        {
            
            frame.Source = new Uri(Config.Instance.PageSources[3], UriKind.Relative);
        });
    }
    public static Action AuthPage(Frame frame)
    {
        return (() =>
        {
            frame.Source = new Uri(Config.Instance.PageSources[4], UriKind.Relative);
        });
    }


    public static ObservableCollection<SingleChoice> FrameList = new ObservableCollection<SingleChoice>()
    {
        new SingleChoice("LBye", () => { Application.Current.MainWindow.WindowState = WindowState.Minimized; }),
        new SingleChoice("LHello", () => { Application.Current.MainWindow.WindowState = WindowState.Maximized; })
    };
}