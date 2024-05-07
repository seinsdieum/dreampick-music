using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Threading.Tasks;
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
    #region Navigation Tabs

    private CollectionPage collectionPage;
    private Feed feedPage;
    private Settings settingsPage;
    private CreatePost createPostPage;
    private PublishAudio publishAudio;
    private ArtistAlbums artistAlbums;
    private AccountPage accountPage;
    private TrackCollectionPage trackQueue;


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

    private NotifyTaskCompletion<ObservableCollection<SingleChoice>> tabs;

    public NotifyTaskCompletion<ObservableCollection<SingleChoice>> Tabs
    {
        get => tabs;
        set
        {
            tabs = value;
            OnPropertyChanged(nameof(Tabs));
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
                /*new SingleChoice("LPublishAudio", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(publishAudio ??= new PublishAudio());
                    },
                    "ImgQueue"),*/

                new SingleChoice("LYou", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(accountPage ??= new AccountPage());
                    },
                    "ImgPlay"),
            };
        }
        set
        {
            appTabs = value;
            OnPropertyChanged(nameof(AppTabs));
        }
    }

    #endregion


    private async Task<ObservableCollection<SingleChoice>> LoadTabs()
    {
        var result = await Task.WhenAny(Account.AccountPerson.Task);


        if (NavigationVm.Instance.Navigation is NavigationService service) service.Navigate((feedPage = new Feed()));
        
        if (result.Result is Artist)
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
                new SingleChoice("LSettings", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(settingsPage ??= new Settings());
                    },
                    "ImgQueue"),
                new SingleChoice("LMissing", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(artistAlbums ??= new ArtistAlbums(result.Result.ID));
                    },
                    "ImgQueue"),
                
            };
        }
        if (result.Result is User)
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
                new SingleChoice("LSettings", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(settingsPage ??= new Settings());
                    },
                    "ImgQueue"),
                
            };
        throw new Exception();
    }


    public MainVm()
    {
        Console.WriteLine("sdsd");
        TestTrackVm();

        AccountVm.Instance.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(AccountVm.Instance.IsArtist))
            {
                ClearCurrentUserData();
                Tabs = new NotifyTaskCompletion<ObservableCollection<SingleChoice>>(LoadTabs());
            }
        };
        
    }
    
    private void ClearCurrentUserData()
    {
        while (NavigationVm.Instance.Navigation.CanGoBack) NavigationVm.Instance.Navigation.RemoveBackEntry();
    }

    public AccountVm Account => AccountVm.Instance;

    public ButtonCommand NavigateToAccount => new ButtonCommand((o =>
    {
        if (NavigationVm.Instance.Navigation is NavigationService service && Account.AccountPerson.IsCompleted)
            service.Navigate(accountPage ??= new AccountPage());
    }));

    public ButtonCommand NavigateToQueue => new ButtonCommand(o =>
    {
        trackQueue = new TrackCollectionPage(TrackCollectionType.Queue, "");
        NavigationVm.Instance.Navigate(trackQueue);
    });


    #region Audio Player

    public PlayerVm Player => PlayerVm.Instance;

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

        album1.Tracks.Add(testTrack1);
        album1.Tracks.Add(testTrack2);

        Player.Queue = album1;
    }

    #endregion

    #region NotifyProperty Members

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion
}