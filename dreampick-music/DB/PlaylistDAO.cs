using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using dreampick_music.Models;
using Microsoft.Data.SqlClient;

namespace dreampick_music.DB;

public class PlaylistDAO
{
    public static PlaylistDAO Instance = new PlaylistDAO();
    
    
    public async Task AddAsync(Playlist playlist)
    {
        
        var queryString =
            "insert into PLAYLIST(playlist_id, user_fk_id, playlist_type_fk_id, playlist_name, playlist_description, playlist_image, release_date) " +
            "values " +
            "(@plid, @uid, @pltid, @plname, @pld, @pli, @pldate)";

        var tracksQueryString =
            "insert into TRACK(TRACK_ID, TRACK_PATH, track_playlist_fk_id, track_lyrics, track_name, release_date) values";
        
        try
        {
            if (string.IsNullOrEmpty(playlist.ID) || string.IsNullOrEmpty(playlist.Name) ||
                playlist.Author is not Models.Person person
                || playlist.Tracks.Count == 0
                || playlist.Tracks.Any(track =>
                    string.IsNullOrEmpty(track.Name) ||
                    string.IsNullOrEmpty(track.ID) || track.Source is not Uri source)
               ) return;


            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var command = new SqlCommand(queryString)
            {
                Transaction = transaction
            };

            command.Parameters.AddWithValue("plid", playlist.ID);
            command.Parameters.AddWithValue("uid", playlist.Author.ID);
            command.Parameters.AddWithValue("pltid",
                playlist is { Type: PlaylistType.ALBUM, Author: Artist a } ? 1 : 0);
            command.Parameters.AddWithValue("plname", playlist.Name);
            command.Parameters.AddWithValue("pld",
                string.IsNullOrEmpty(playlist.Description) ? "" : playlist.Description);

            command.Parameters.AddWithValue("pli",
                playlist.Image is BitmapImage image ? Utils.GetByteArrayFromImage(image) : DBNull.Value);
            command.Parameters.AddWithValue("pldate", playlist.ReleaseDate);

            command.Connection = connection;

            await command.ExecuteNonQueryAsync();

            var tracks = playlist.Tracks.OrderBy(t => t.ReleaseDate).ToList();

            for (int i = 0; i < tracks.Count; i++)
            {
                tracksQueryString += $" (@tid{i}, @tpath{i}, @tplid{i}, @tlyrics{i}, @tname{i}, @tdate{i})";
                if (i != playlist.Tracks.Count() - 1) tracksQueryString += ",";
            }

            var tracksCommand = new SqlCommand(tracksQueryString)
            {
                Transaction = transaction
            };

            for (int i = 0; i < tracks.Count; i++)
            {
                tracksCommand.Parameters.AddWithValue($"tid{i}", playlist.Tracks[i].ID);
                tracksCommand.Parameters.AddWithValue($"@tpath{i}", playlist.Tracks[i].Source.AbsolutePath);
                tracksCommand.Parameters.AddWithValue($"@tplid{i}", playlist.ID);
                tracksCommand.Parameters.AddWithValue($"@tlyrics{i}",
                    string.IsNullOrEmpty(playlist.Tracks[i].Lyrics) ? "" : playlist.Tracks[i].Lyrics);
                tracksCommand.Parameters.AddWithValue($"@tname{i}", playlist.Tracks[i].Name);
                tracksCommand.Parameters.AddWithValue($"@tdate{i}", playlist.Tracks[i].ReleaseDate);
            }

            tracksCommand.Connection = connection;

            await tracksCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }

    }
    
    
     public async Task UpdateAsync(Playlist playlist, ObservableCollection<Track> oldTracks)
    {
        
        var queryString =
            "update playlist set playlist_description = @pld, playlist_type_fk_id = @pltid, playlist_name = @plname, playlist_image = @pli where playlist_id = @plid";

        var tracksQueryString =
            "delete from track where TRACK_ID in (";

        var tracksUpdateQueryString =
            "update track set track_lyrics = @tlyrics, track_name = @tname, TRACK_PATH = @tpath\nwhere TRACK_ID = @tid";

        

        
        try
        {
            if (string.IsNullOrEmpty(playlist.ID) || string.IsNullOrEmpty(playlist.Name) ||
                playlist.Author is not Models.Person person
                || playlist.Tracks.Count == 0
                || playlist.Tracks.Any(track =>
                    string.IsNullOrEmpty(track.Name) ||
                    string.IsNullOrEmpty(track.ID) || track.Source is not Uri source)
               ) return;

            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var command = new SqlCommand(queryString)
            {
                Transaction = transaction
            };

            command.Parameters.AddWithValue("plid", playlist.ID);
            command.Parameters.AddWithValue("pltid",
                playlist is { Type: PlaylistType.ALBUM, Author: Artist a } ? 1 : 0);
            command.Parameters.AddWithValue("plname", playlist.Name);
            command.Parameters.AddWithValue("pld",
                string.IsNullOrEmpty(playlist.Description) ? "" : playlist.Description);

            command.Parameters.AddWithValue("pli",
                playlist.Image is BitmapImage image ? Utils.GetByteArrayFromImage(image) : DBNull.Value);

            command.Connection = connection;

            await command.ExecuteNonQueryAsync();

            //////////////////////////////////////////
            //////////////////////////////////////////
            //////////////////////////////////////////



            foreach (var track in playlist.Tracks)
            {
                var updateCommand = new SqlCommand(tracksUpdateQueryString)
                {
                    Transaction = transaction
                };
                updateCommand.Parameters.AddWithValue("tlyrics",
                    string.IsNullOrEmpty(track.Lyrics) ? DBNull.Value : track.Lyrics);
                updateCommand.Parameters.AddWithValue("tname", track.Name);
                updateCommand.Parameters.AddWithValue("tid", track.ID);
                updateCommand.Parameters.AddWithValue("tpath", track.Source.AbsolutePath);
                updateCommand.Connection = connection;
                await updateCommand.ExecuteNonQueryAsync();
            }


            //////////////////////////////////////////
            //////////////////////////////////////////
            //////////////////////////////////////////

            var removalTracks = new List<Track>();
            foreach (var t1 in oldTracks)
            {
                if (!playlist.Tracks.Any(t => t.ID == t1.ID))
                {
                    removalTracks.Add(t1);
                }
            }


            if (removalTracks.Any())
            {
                for (var i = 0; i < removalTracks.Count; i++)
                {
                    tracksQueryString += $" @tid{i}";
                    if (i != removalTracks.Count - 1) tracksQueryString += ",";
                }

                tracksQueryString += ")";

                var tracksCommand = new SqlCommand(tracksQueryString)
                {
                    Transaction = transaction
                };

                for (var i = 0; i < removalTracks.Count; i++)
                {
                    tracksCommand.Parameters.AddWithValue($"tid{i}", playlist.Tracks[i].ID);
                }

                tracksCommand.Connection = connection;

                await tracksCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

        }
        
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }


    public async Task<Playlist> GetAsync(string id)
    {

        var playlist = new Playlist();

        
        const string tracksQuery = "select t.TRACK_ID as id, t.track_lyrics as lyrics, t.track_name as [name], t.TRACK_PATH as [tpath], t.release_date as [tdate] from TRACK as t where t.track_playlist_fk_id = @playlistid order by t.release_date";
        var playlistQuery = "select p.playlist_id as id, p.playlist_description as [description], p.playlist_image as [image], p.playlist_name as [name], p.playlist_type_fk_id as [type], p.user_fk_id as [uid], p.release_date as [date] from PLAYLIST as p where p.playlist_id = @playlistid";
        var userQuery = "select u.user_id as [id], u.user_name as [name] from [USER] as u\ninner join PLAYLIST as p on p.user_fk_id = u.user_id and p.playlist_id = @playlistid";


        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var tracksCommand = new SqlCommand(tracksQuery)
            {
                Transaction = transaction
            };
            tracksCommand.Parameters.AddWithValue("playlistid", id);
            tracksCommand.Connection = connection;
            var result1 = tracksCommand.ExecuteReaderAsync().Result;

            var tracks = new List<Track>();
            var artist = new Artist();

            while (result1.ReadAsync().Result)
            {

                var track = new Track();

                track.ID = (string)result1["id"];
                track.Name = (string)result1["name"];
                track.Lyrics = result1["lyrics"] is string tlyrics ? tlyrics : "";
                track.Source = result1["tpath"] is string tpath ? new Uri(tpath, UriKind.Absolute) : null;
                track.Album = playlist;
                track.ReleaseDate = (DateTime)result1["tdate"];


                tracks.Add(track);
            }

            playlist.Tracks = new ObservableCollection<Track>(tracks);

            await result1.CloseAsync();


            ///////////////////////////////////////////////
            ///////////////////////////////////////////////
            ///////////////////////////////////////////////

            var userCommand = new SqlCommand(userQuery)
            {
                Transaction = transaction
            };

            userCommand.Parameters.AddWithValue("playlistid", id);
            userCommand.Connection = connection;

            var result2 = userCommand.ExecuteReaderAsync().Result;

            while (result2.ReadAsync().Result)
            {
                artist.ID = (string)result2["id"];
                artist.Name = (string)result2["name"];
            }

            playlist.Author = artist;


            await result2.CloseAsync();


            ///////////////////////////////////////////////
            ///////////////////////////////////////////////
            ///////////////////////////////////////////////


            var playlistCommand = new SqlCommand(playlistQuery)
            {
                Transaction = transaction
            };

            playlistCommand.Parameters.AddWithValue("playlistid", id);
            playlistCommand.Connection = connection;
            var result = playlistCommand.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                playlist.Image = result["image"] is byte[] playlistBytes ? Utils.GetBitmapImage(playlistBytes) : null;
                playlist.Description = result["description"] is string desc ? desc : "";
                playlist.ID = id;
                playlist.Type = PlaylistType.ALBUM;
                playlist.Name = (string)result["name"];
                playlist.ReleaseDate = (DateTime)result["date"];
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }

        return playlist;
    }
    
    public async Task<Playlist> GetInfoAsync(string id)
    {

        var playlist = new Playlist();


        try
        {
            var playlistQuery =
                "select p.playlist_id as id, p.playlist_description as [description], p.playlist_image as [image], p.playlist_name as [name], p.playlist_type_fk_id as [type], p.user_fk_id as [uid] from PLAYLIST as p where p.playlist_id = @playlistid";
            var userQuery =
                "select u.user_id as [id], u.user_name as [name] from [USER] as u\ninner join PLAYLIST as p on p.user_fk_id = u.user_id and p.playlist_id = @playlistid";

            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var artist = new Artist();


            ///////////////////////////////////////////////
            ///////////////////////////////////////////////
            ///////////////////////////////////////////////

            var userCommand = new SqlCommand(userQuery)
            {
                Transaction = transaction
            };

            userCommand.Parameters.AddWithValue("playlistid", id);
            userCommand.Connection = connection;

            var result2 = userCommand.ExecuteReaderAsync().Result;

            while (result2.ReadAsync().Result)
            {
                artist.ID = (string)result2["id"];
                artist.Name = (string)result2["name"];
            }

            playlist.Author = artist;


            await result2.CloseAsync();


            ///////////////////////////////////////////////
            ///////////////////////////////////////////////
            ///////////////////////////////////////////////


            var playlistCommand = new SqlCommand(playlistQuery)
            {
                Transaction = transaction
            };

            playlistCommand.Parameters.AddWithValue("playlistid", id);
            playlistCommand.Connection = connection;
            var result = playlistCommand.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                playlist.Image = result["image"] is byte[] playlistBytes ? Utils.GetBitmapImage(playlistBytes) : null;
                playlist.Description = result["description"] is string desc ? desc : "";
                playlist.ID = id;
                playlist.Type = PlaylistType.ALBUM;
                playlist.Name = (string)result["name"];
            }

            await result.CloseAsync();

            await transaction.CommitAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        return playlist;
    }
    
    
    public async Task<ObservableCollection<Playlist>> LastCollection()
    {

        var query =
            "select p.playlist_id as id, p.playlist_description as [description], p.playlist_image as [image], p.playlist_name as [name], p.playlist_type_fk_id as [type], u.user_id as [uid], u.user_name as [uname] from PLAYLIST as p inner join [USER] as u on u.user_id = p.user_fk_id order by p.release_date desc";
        var collection = new ObservableCollection<Playlist>();


        try
        {
            
            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();



            ///////////////////////////////////////////////
            ///////////////////////////////////////////////
            ///////////////////////////////////////////////

            var command = new SqlCommand(query)
            {
                Transaction = transaction
            };

            command.Connection = connection;

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                var author = new Artist()
                {
                    Name = (string)result["uname"],
                    ID = (string)result["uid"]
                };
                
                var p = new Playlist()
                {
                    Author = author,
                    Name = (string)result["name"],
                    Description = result["description"] is string str ? str : "",
                    ID = (string)result["id"],
                    Image = result["image"] is byte[] bytes ? Utils.GetBitmapImage(bytes) : null,
                    Type = result["type"] == "1" ? PlaylistType.ALBUM : PlaylistType.PLAYLIST,
                };
                collection.Add(p);
            }

            await result.CloseAsync();
            


            await transaction.CommitAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        return collection;
    }


    public async Task<ObservableCollection<Playlist>> GetAlbumsInfo(List<string> albumsID)
    {
        var collection = new ObservableCollection<Playlist>();
        try
        {
            foreach (var id in albumsID)
            {
                var item = await GetInfoAsync(id);
                collection.Add(item);
            }

        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
        return collection;
    }

    public async Task<bool> UserRelated(string userId, string playlistId)
    {
        var queryString = "if exists(select * from USER_PLAYLIST_RELATION where playlist_id = @plid and user_id = @uid) " +
                          "select cast(1 as bit) as [statement] " +
                          "else " +
                          "select cast(0 as bit) as [statement]";

        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };
        command.Parameters.AddWithValue("plid", playlistId);
        command.Parameters.AddWithValue("uid", userId);

        var result = command.ExecuteReaderAsync().Result;

        while (result.ReadAsync().Result)
        {
            return result["statement"] is true;
        }

        await result.CloseAsync();

        return false;
    }
    
    
    public async Task<bool> RelateUser(string userId, string playlistId)
    {
        var queryString = "if exists(select * from [USER_PLAYLIST_RELATION] where playlist_id = @plid and user_id = @uid) " +
                          "delete USER_PLAYLIST_RELATION where playlist_id = @plid and user_id = @uid " +
                          "else " +
                          "insert into USER_PLAYLIST_RELATION (user_id, playlist_id, relation_date) " +
                          "values (@uid, @plid, @date)";

        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };
        command.Parameters.AddWithValue("plid", playlistId);
        command.Parameters.AddWithValue("uid", userId);
        command.Parameters.AddWithValue("date", DateTime.Now);

        var result = command.ExecuteReaderAsync().Result;

        while (result.ReadAsync().Result)
        {
            return result["statement"] is true;
        }

        return false;
    }
    

    public async Task<List<string>> GetUserPlaylistRelations(string userId)
    {
        var queryString = "select p.playlist_id as [id] from PLAYLIST as p\nwhere p.user_fk_id = @uid";
        var list = new List<string>();

        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var command = new SqlCommand(queryString)
            {
                Transaction = transaction
            };

            command.Parameters.AddWithValue("uid", userId);
            command.Connection = connection;

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                if (result["id"] is string str) list.Add(str);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        return list;
    }
    
    
    public async Task<ObservableCollection<Playlist>> RelatedPlaylists(string userId)
    {
        var res = new ObservableCollection<Playlist>();

        var queryString =
            "select " +
            "p.playlist_image as [pimage], " +
            "p.playlist_id as [plid], " +
            "p.playlist_name as [pname], " +
            "u.user_id as [uid], " +
            "u.user_name as [uname] " +
            "from [USER_PLAYLIST_RELATION] as ut " +
            "inner join [PLAYLIST] as p on ut.playlist_id = p.playlist_id " +
            "inner join [USER] as u on p.user_fk_id = u.user_id " +
            "order by ut.relation_date desc";

        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();
        
        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("id", userId);

        var result = await command.ExecuteReaderAsync();

        while (await result.ReadAsync())
        {
            var author = new Artist()
            {
                Name = (string)result["uname"],
                ID = (string)result["uid"]
            };


            var album = new Playlist()
            {
                Author = author,
                Name = (string)result["pname"],
                ID = (string)result["plid"],
            };
            if (result["pimage"] is byte[] bytes)
            {
                album.Image = Utils.GetBitmapImage(bytes);
            }

            res.Add(album);
        }

        await result.CloseAsync();

        await connection.CloseAsync();

        return res;

    }
    
    
}