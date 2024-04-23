using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using dreampick_music.Models;

namespace dreampick_music;

public class MainVm : INotifyPropertyChanged
{
    private MediaState songState = MediaState.Pause;

    private double songVolume = 0.5;
    private double songValue = 5;



    private PlayingQueueVm tracksQueueVm = new PlayingQueueVm();

    public PlayingQueueVm TracksQueueVm
    {
        get
        {
            return tracksQueueVm;
        }
        set
        {
            tracksQueueVm = value;
            OnPropertyChanged(nameof(TracksQueueVm));
        }
    }

    public double SongVolume
    {
        get { return songVolume; }
        set
        {
            songVolume = value;
            OnPropertyChanged(nameof(SongVolume));
        }
    }

    public MediaState SongState
    {
        get { return songState; }
        set
        {
            songState = value;
            OnPropertyChanged(nameof(SongState));
        }
    }

    public ButtonCommand PlayCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                SongState = songState == MediaState.Play ? MediaState.Pause : MediaState.Play;
            });
        }
    }
    
    public ButtonCommand NextCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {

                var state = songState;
                SongState = MediaState.Pause;
                TracksQueueVm.NextTrackCommand.Execute(o);
                var timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 600);
                timer.Tick += ((sender, args) =>
                {
                    SongState = state;
                    timer.Stop();
                });
                timer.Start();
                
            });
        }
    }
    
    public ButtonCommand PrevCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                var state = songState;
                SongState = MediaState.Pause;
                TracksQueueVm.PrevTrackCommand.Execute(o);
                var timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 600);
                timer.Tick += ((sender, args) =>
                {
                    SongState = state;
                    timer.Stop();
                });
                timer.Start();
            });
        }
    }



    private ObservableCollection<SingleChoice> appTabs;

    public ObservableCollection<SingleChoice> AppTabs
    {
        get
        {
            return new ObservableCollection<SingleChoice>()
            {
                new SingleChoice("LCollection", (() => { PageSource = new Uri("Collection.xaml", UriKind.Relative); }),
                    new Uri("Assets/AppIcon.svg", UriKind.Relative)),
                new SingleChoice("LFeed", () => { PageSource = new Uri("Feed.xaml", UriKind.Relative); },
                    new Uri("Assets/AppIcon.svg", UriKind.Relative)),
                new SingleChoice("LCreatePost", () => { PageSource = new Uri("CreatePost.xaml", UriKind.Relative); },
                    new Uri("Assets/AppIcon.svg", UriKind.Relative)),
                new SingleChoice("LAccount", () => { PageSource = new Uri("Account.xaml", UriKind.Relative); },
                    new Uri("Assets/AppIcon.svg", UriKind.Relative)),
                new SingleChoice("LSettings", () => { PageSource = new Uri("Settings.xaml", UriKind.Relative); },
                    new Uri("Assets/AppIcon.svg", UriKind.Relative))
            };
        }
    }

    private Uri pageSource = new Uri("Settings.xaml", UriKind.Relative);

    public Uri PageSource
    {
        get { return pageSource; }
        set
        {
            pageSource = value;
            OnPropertyChanged(nameof(PageSource));
        }
    }


    public MainVm()
    {
        TestTrackVm();
    }


    public void TestTrackVm()
    {
        var artist1 = new Artist();
        var artist2 = new Artist();
        
        var testTrack1 = new Track();
        var testTrack2 = new Track();

        var album1 = new Playlist();
        album1.Type = PlaylistType.ALBUM;
        album1.Name = "Suchka";

        var img = new Blob();
        
        
        album1.Image = new Blob();
        

        artist1.Name = "KA$HDAMI";
        artist2.Name = "Distrotronic";

        
        testTrack1.Name = "Reparations!";
        testTrack1.Source = new Uri("D:/Tracks/124.mp3", UriKind.Absolute);
        testTrack1.Artist = artist1;
        
        testTrack2.Name = "Tricky Disco";
        testTrack2.Source = new Uri("D:/Tracks/123.mp3", UriKind.Absolute);
        testTrack2.Artist = artist2;
        
        
        
        var vm = new PlayingQueueVm();
        vm.Queue = new Playlist();
        vm.Queue.Tracks.Add(testTrack1);
        vm.Queue.Tracks.Add(testTrack2);
        TracksQueueVm = vm;
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
    
    
    
}