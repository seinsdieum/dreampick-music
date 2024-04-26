using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class MainVm : INotifyPropertyChanged
{
    private MediaState songState = MediaState.Pause;

    private double songVolume = 0.5;
    private double songValue = 5;


    private CollectionPage collectionPage;
    private Feed feedPage;
    private Settings settingsPage;
    private CreatePost createPostPage;
    private PublishAudio publishAudio;
    private ArtistAlbums artistAlbums;


    private bool tabsCollapsed = true;

    public bool TabsCollapsed
    {
        get { return tabsCollapsed; }
        set
        {
            tabsCollapsed = value;
            OnPropertyChanged(nameof(TabsCollapsed));
        }
    }


    private PlayingQueueVm tracksQueueVm = new PlayingQueueVm();

    public PlayingQueueVm TracksQueueVm
    {
        get { return tracksQueueVm; }
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
                new SingleChoice("LCollection", (() =>
                    {

                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(collectionPage ??= new CollectionPage());
                    }),
                    "ImgQueue"),
                new SingleChoice("LFeed", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                        {
                            service.Navigate(feedPage ??= new Feed());
                        }
                    },
                    "ImgQueue"),
                new SingleChoice("LCreatePost", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(createPostPage ??= new CreatePost());
                    },
                    "ImgQueue"),
                new SingleChoice("LAccount", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service) ;
                    },
                    "ImgQueue"),
                new SingleChoice("LSettings", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(settingsPage ??= new Settings());
                    },
                    "ImgQueue"),
                new SingleChoice("LPublishAudio", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(publishAudio ??= new PublishAudio());
                    },
                    "ImgQueue"),
                new SingleChoice("LAlbum", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(new AlbumPage("PksnTV]lGc"));
                    },
                    "ImgQueue"),
                new SingleChoice("LMissing", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(artistAlbums ??= new ArtistAlbums("sdfsd"));
                    },
                    "ImgPlay"),
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

        var album1 = new Playlist()
        {
            Type = PlaylistType.ALBUM,
            Name = "Suchka",
            Author = artist1
        };


        artist1.Name = "KA$HDAMI";
        artist2.Name = "Distrotronic";


        testTrack1.Name = "Reparations!";
        testTrack1.Source = new Uri("D:/Tracks/124.mp3", UriKind.Absolute);
        testTrack1.Album = album1;

        testTrack2.Name = "Tricky Disco";
        testTrack2.Source = new Uri("D:/Tracks/123.mp3", UriKind.Absolute);
        testTrack2.Album = album1;


        var vm = new PlayingQueueVm();
        vm.Queue = album1;
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