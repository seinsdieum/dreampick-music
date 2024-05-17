using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
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

    private ObservableCollection<SingleChoice> tabs;

    public ObservableCollection<SingleChoice> Tabs
    {
        get => tabs;
        set
        {
            tabs = value;
            NavigationVm.Instance.Navigate(feedPage ??= new Feed());
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


    private ObservableCollection<SingleChoice> LoadTabs()
    {


        if (NavigationVm.Instance.Navigation is NavigationService service) service.Navigate((feedPage = new Feed()));
        
        if (AccountVm.Instance.AccountPerson.IsArtist)
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
                    "ImgFeed"),
                new SingleChoice("LSettings", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(settingsPage ??= new Settings());
                    },
                    "ImgSettings"),
                new SingleChoice("LPublishAudio", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(artistAlbums ??= new ArtistAlbums());
                    },
                    "ImgArtist"),
                
            };
        }
        
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
                    "ImgFeed"),
                new SingleChoice("LSettings", () =>
                    {
                        if (NavigationVm.Instance.Navigation is NavigationService service)
                            service.Navigate(settingsPage ??= new Settings());
                    },
                    "ImgSettings"),
                
            };
    }


    public MainVm()
    {
        RefreshWindowNavigation();
        
        AccountVm.Instance.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(AccountVm.Instance.AccountPerson)) return;
            RefreshWindowNavigation();
        };
        
    }

    private void RefreshWindowNavigation()
    {
        ClearCurrentUserData();
        Tabs = LoadTabs();
        Tabs[1].ExecuteChoice();
    }
    
    private void ClearCurrentUserData()
    {
        if (NavigationVm.Instance.Navigation is null) return;
        while ( NavigationVm.Instance.Navigation.CanGoBack) NavigationVm.Instance.Navigation.RemoveBackEntry();
    }

    public AccountVm Account => AccountVm.Instance;

    public ButtonCommand NavigateToAccount => new ButtonCommand((o =>
    {
        if (NavigationVm.Instance.Navigation is NavigationService service )
            service.Navigate(accountPage ??= new AccountPage());
    }));

    public ButtonCommand NavigateToQueue => new ButtonCommand(o =>
    {
        trackQueue = new TrackCollectionPage(TrackCollectionType.Queue, "");
        NavigationVm.Instance.Navigate(trackQueue);
    });


    #region Audio Player

    public PlayerVm Player => PlayerVm.Instance;

    /*public void TestTrackVm()
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
    }*/

    #endregion

    #region NotifyProperty Members

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion
}