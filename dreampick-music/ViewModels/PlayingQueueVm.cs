using System.ComponentModel;
using dreampick_music.Models;

namespace dreampick_music;

public class PlayingQueueVm : INotifyPropertyChanged
{
    private int trackIndex = 0;

    private Playlist queue = new Playlist();

    public Playlist Queue
    {
        get
        {
            return queue;
        }
        set
        {
            queue = value; OnPropertyChanged(nameof(Queue));
        }
    }




    public Track CurrentTrack
    {
        get
        {
            if (trackIndex < 0)
            {
                return new Track();
            }
            else
            {
                return Queue.Tracks[trackIndex];
            }
        }
    }
    
    public ButtonCommand NextTrackCommand 
    {
        get
        {
            return new ButtonCommand(o =>
            {
                trackIndex++;
                if (queue.HasTrack(trackIndex))
                {
                    OnPropertyChanged(nameof(CurrentTrack));
                }
                else
                {
                    trackIndex = 0;
                    OnPropertyChanged(nameof(CurrentTrack));
                }
            });
        }
    }
    
    public ButtonCommand PrevTrackCommand 
    {
        get
        {
            return new ButtonCommand(o =>
            {
                trackIndex--;
                if (queue.HasTrack(trackIndex))
                {
                    OnPropertyChanged(nameof(CurrentTrack));
                }
                else
                {
                    trackIndex = 0;
                    OnPropertyChanged(nameof(CurrentTrack));
                }
            });
        }
    }

    public PlayingQueueVm()
    {
        
    }
    
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}