﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music;

public enum AudioRepeatType
{
    NO_REPEAT = 1,
    REPEAT_QUEUE,
    REPEAT_TRACK,
}

public class PlayerVm : INotifyPropertyChanged
{
    private MediaState songState = MediaState.Pause;

    private double songVolume = 0.5;


    private Playlist queue = new Playlist();
    private bool TrackIsAvailable => Queue.HasTrack(currentIndex);
    private int currentIndex = 0;


    private bool isShuffled = false;
    private AudioRepeatType repeatType = AudioRepeatType.NO_REPEAT;

    public Playlist Queue
    {
        get => queue;
        set
        {
            queue = value;
            OnPropertyChanged(nameof(Queue));
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

    public AudioRepeatType AudioRepeat
    {
        get => repeatType;
        set
        {
            repeatType = value;
            OnPropertyChanged(nameof(AudioRepeat));
        }
    }
    
    public int CurrentIndex
    {
        set
        {
            currentIndex = value;
            OnPropertyChanged(nameof(CurrentTrack));
        }
    }


    public Track CurrentTrack
    {
        get
        {
            try
            {
                return Queue.Tracks[currentIndex];
            }
            catch
            {
                return new Track();
            }
        }
    }

    public bool IsShuffled
    {
        get => isShuffled;
        set
        {
            isShuffled = value;
            OnPropertyChanged(nameof(IsShuffled));
        }
    }

    public ButtonCommand NextTrackCommand => new ButtonCommand(o =>
    {
        var rand = new Random();
        
        var delta = isShuffled ? rand.Next(0, Queue.Tracks.Count-1)  : currentIndex + 1;

        if (repeatType == AudioRepeatType.REPEAT_TRACK)
        {
            var a = currentIndex;
            CurrentIndex = -1;
            CurrentIndex = a;
        }
        else
        {
            if (queue.HasTrack(delta)) CurrentIndex = delta;
            else if (repeatType == AudioRepeatType.REPEAT_QUEUE) CurrentIndex = 0;
            else
            {
                CurrentIndex = 0;
                SongState = MediaState.Pause;
            }
        }
    });

    public ButtonCommand PlayTrackCommand => new ButtonCommand(o => { SwitchSongState(); });

    public ButtonCommand PrevTrackCommand => new ButtonCommand(o =>
    {
        var rand = new Random();
        
        var delta = isShuffled ? rand.Next(0, Queue.Tracks.Count-1)  : currentIndex - 1;


        if (repeatType == AudioRepeatType.REPEAT_TRACK)
        {
            var a = currentIndex;
            CurrentIndex = -1;
            CurrentIndex = a;
        }
        else
        {
            if (queue.HasTrack(delta)) CurrentIndex = delta;
            else if (repeatType == AudioRepeatType.REPEAT_QUEUE) CurrentIndex = 0;
            else
            {
                SongState = MediaState.Pause;
            }
        }
    });


    public ButtonCommand ToggleRepeatCommand => new ButtonCommand(o =>
    {
        AudioRepeat = repeatType switch
        {
            AudioRepeatType.NO_REPEAT => AudioRepeatType.REPEAT_QUEUE,
            AudioRepeatType.REPEAT_QUEUE => AudioRepeatType.REPEAT_TRACK,
            _ => AudioRepeatType.NO_REPEAT
        };
    });

    public ButtonCommand ShuffleQueueCommand => new ButtonCommand(o =>
    {
        IsShuffled = !isShuffled;
    });


    public void PlayNewQueue(Playlist queuePlay, string trackId)
    {
        if (Queue.ID == queuePlay.ID && CurrentTrack.ID == trackId)
        {
            SwitchSongState();
            return;
        }


        if (queuePlay.Tracks.Count <= 0) return;

        Queue = queuePlay;
        CurrentIndex = AudioPlayerModel.Instance.GetQueueIndex(queuePlay, trackId);
        SongState = MediaState.Play;
    }

    public void PlayNewQueue(Playlist queuePlay, int index = 0)
    {
        if (Queue.ID == queuePlay.ID)
        {
            SwitchSongState();
            return;
        }
        
        
        if (queuePlay.Tracks.Count <= 0) return;

        index = index > queuePlay.Tracks.Count - 1 ? 0 : index;
        Queue = queuePlay;
        CurrentIndex = index;
        SongState = MediaState.Play;
    }

    private void SwitchSongState()
    {
        SongState = songState == MediaState.Play ? MediaState.Pause : MediaState.Play;
    }


    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public static PlayerVm Instance = new PlayerVm();
}