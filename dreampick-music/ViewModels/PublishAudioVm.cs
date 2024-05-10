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
using dreampick_music.DB;
using dreampick_music.Models;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace dreampick_music;

public class PublishAudioVm : HistoryVm
{
    

    private string albumName;

    private string albumDescription;
    private ObservableCollection<TrackVm> tracks = new ObservableCollection<TrackVm>();

    private Uri imageSource;

    [UndoRedo]
    public string AlbumName
    {
        get => albumName;
        set => Set(ref albumName, value);
    }

    [UndoRedo]
    public string AlbumDescription
    {
        get => albumDescription;
        set => Set(ref albumDescription, value);
    }
    

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
    
    public ButtonCommand RemoveTrackCommand => new ButtonCommand(o =>
    {
        if (o is string id)
        {
            Tracks.Remove(Tracks.Single(track => track.TrackId == id));
        }
    });
    
    public ButtonCommand PublishCommand => new ButtonCommand((o =>
    {
        PublishPlaylist();
    }));

    public ButtonCommand BackCommand => new ButtonCommand((o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    }));

    private void DestroyObjects()
    {
        // TODO implement
    }


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
                ReleaseDate = DateTime.Now
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
                ReleaseDate = DateTime.Now
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
            Author = AccountVm.Instance.AccountPerson.Result,
            Description = AlbumDescription,
            ReleaseDate = DateTime.Now,
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
            Description = AlbumDescription,
            ReleaseDate = DateTime.Now,

            Author = AccountVm.Instance.AccountPerson.Result
        };
        playlist.Tracks = new ObservableCollection<Track>(VmToTracks(playlist, out hasProblems));
        return playlist;
    }
    

    private void PublishPlaylist()
    {
        var playlist = CreatePlaylist(out var hasProblems);
        
        if(hasProblems) return;
        
        _ = PlaylistDAO.Instance.AddAsync(playlist);
        
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);

    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}