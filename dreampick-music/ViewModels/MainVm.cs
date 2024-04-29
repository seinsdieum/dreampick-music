﻿using System;
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



    #region Navigation Tabs

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

    #endregion


    public MainVm()
    {
        TestTrackVm();
    }



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