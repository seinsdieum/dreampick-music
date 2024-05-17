using System;
using System.Windows;
using Microsoft.Win32;

namespace dreampick_music;

public class TrackVm : HistoryVm
{
    private string trackId;

    public string TrackId
    {
        get => trackId;
        set
        {
            trackId = value;
            NotifyPropertyChanged();
        }
    }
    public string ArtistId { get; private set; }

    private string name = "";

    private Uri source;

    public bool SourceIsLoaded => Source != null;

    [UndoRedo]
    public Uri Source
    {
        get => source;
        set => Set(ref source, value);
    }
    
    
    public ButtonCommand LoadSourceCommand => new ButtonCommand((o =>
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".jpg";
        dialog.Filter = "Image files |*.mp3";
        var result = dialog.ShowDialog();
        if (result != true) return;

        MessageBox.Show("yea");
        string path = dialog.FileName;
        Source = new Uri(path, UriKind.Absolute);
    }));


    

    
    

    [UndoRedo]
    public string Name
    {
        get => name;
        set => Set(ref name, value);
    }

    public TrackVm(string artistId, string trackId)
    {
        ArtistId = artistId;
        TrackId = trackId;
    }
}