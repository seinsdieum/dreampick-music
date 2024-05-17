using System.Collections.Generic;

namespace dreampick_music.DbContexts;

public class UserPlaylistTracks
{
    public Playlist UserPlaylist { get; set; }
    public Track UserTrack { get; set; }
    
    public string UserPlaylistId { get; set; }
    public string UserTrackId { get; set; }
}