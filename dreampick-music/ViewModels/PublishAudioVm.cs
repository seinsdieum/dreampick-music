using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using dreampick_music.Models;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace dreampick_music;

public class PublishAudioVm : HistoryVm
{
    private MainVm mainVm;

    public MainVm MainVm
    {
        set { mainVm = value; }
    }

    private string albumName;

    [UndoRedo]
    public string AlbumName
    {
        get => albumName;
        set => Set(ref albumName, value);
    }

    private ObservableCollection<TrackVm> tracks = new ObservableCollection<TrackVm>();

    private Uri imageSource;

    [UndoRedo]
    public Uri ImageSource
    {
        get => imageSource;
        set => Set(ref imageSource, value);
    }

    [UndoRedo]
    public ObservableCollection<TrackVm> Tracks
    {
        get => tracks;
        set => Set(ref tracks, value);
    }

    public ButtonCommand LoadImageCommand => new ButtonCommand((o =>
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".jpg";
        dialog.Filter = "Image files |*.jpg;*.png";
        var result = dialog.ShowDialog();
        if (result != true) return;

        string path = dialog.FileName;
        ImageSource = new Uri(path, UriKind.Absolute);
    }));

    public ButtonCommand AddTrackCommand => new ButtonCommand((o =>
    {
        var tracks = Tracks;
        tracks.Add(new TrackVm("sdfsd", Utils.GenerateRandomString(10)));
        Set(ref this.tracks, tracks);
    }));

    public ButtonCommand PlayQueueCommand => new ButtonCommand((o =>
    {
        if (o is not string id) return;
        var tracksPlaylist = CreatePlaylist();
        PlayerVm.Instance.PlayNewQueue(tracksPlaylist, id);
    }));


    private List<Track> VmToTracks(Playlist tracksPlaylist)
    {
        return tracks
            .Where(track => track.SourceIsLoaded && !string.IsNullOrEmpty(track.Name))
            .Select(track => new Track()
            {
                Name = track.Name,
                Source = track.Source,
                Album = tracksPlaylist,
                ID = track.TrackId,
            }).ToList();
    }
    
    private List<Track> VmToTracks(Playlist tracksPlaylist, out bool hasProblems)
    {
        var t = tracks
            .Where(track => track.SourceIsLoaded && !string.IsNullOrEmpty(track.Name))
            .Select(track => new Track()
            {
                Name = track.Name,
                Source = track.Source,
                Album = tracksPlaylist,
                ID = track.TrackId,
            }).ToList();

        hasProblems = (t.Count < tracks.Count);
        
        
        return t;
    }

    private Playlist CreatePlaylist()
    {
        var playlist = new Playlist()
        {
            Name = AlbumName,
            Type = PlaylistType.ALBUM,
            Image = ImageSource is null ? null : new BitmapImage(ImageSource),
            // TODO account binding
            Author = new Artist()
            {
                Name = "alexellipse",
                ID = "sdfsd",
            }
        };
        playlist.Tracks = new ObservableCollection<Track>(VmToTracks(playlist));
        return playlist;
    }
    
    private Playlist CreatePlaylist(out bool hasProblems)
    {
        var playlist = new Playlist()
        {
            ID = Utils.GenerateRandomString(10),
            Name = AlbumName,
            Type = PlaylistType.ALBUM,
            Image = ImageSource is null ? null : new BitmapImage(ImageSource),

            // TODO account binding
            Author = new Artist()
            {
                Name = "alexellipse",
                ID = "sdfsd",
            }
        };
        playlist.Tracks = new ObservableCollection<Track>(VmToTracks(playlist, out hasProblems));
        return playlist;
    }
    

    private void PublishPlaylist()
    {
        var playlist = CreatePlaylist(out var hasProblems);
        
        if(hasProblems) return;
        
        _ = PlatformDAO.Instance.AddPlaylist(playlist);
    }

    private void RefreshFields()
    {
        
    }


    public ButtonCommand PublishCommand => new ButtonCommand((o =>
    {
        PublishPlaylist();
    }));


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}