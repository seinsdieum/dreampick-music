using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Quic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using MessageBox = System.Windows.Forms.MessageBox;

namespace dreampick_music.Models;

public class PlatformDAO : IDatabaseRequests
{

    private SemaphoreSlim sem;
    
    public static PlatformDAO Instance = new PlatformDAO();

    public async Task<ObservableCollection<Post>> LoadPostsAsync()
    {

        await sem.WaitAsync();
        
        await Task.Delay(500);


        var queryString = "select post_date as pdate, post_id as pid, " +
                          "post_text as ptext, [USER].user_id as puid, " +
                          "[USER].user_name as puname, [USER].user_image as uimg from POST\ninner join [USER] ON [USER].user_id = post_user_fk_id order by pdate desc";


        var posts = new ObservableCollection<Post>();

        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            var command =
                new SqlCommand(
                    queryString)
                {
                    Transaction = transaction
                };




            command.Connection = connection;
            var result = command.ExecuteReaderAsync().Result;


            while (result.ReadAsync().Result)
            {
                var user = new User
                {
                    ID = (string)result["puid"],
                    Name = (string)result["puname"]
                };

                var imgRes = result["uimg"];
                if (imgRes is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                var post = new Post((string)result["pid"], (string)result["ptext"])
                {
                    PostAuthor = user,
                    PublicationDate = (DateTime)result["pdate"]
                };

                posts.Add(post);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();

        }
        
        return posts;
    }

    public async Task<ObservableCollection<Post>> LoadUserPostsAsync(string id)
    {
        
        var queryString = "select p.post_text as ptext, " +
                          "u.user_name as puname, " +
                          "p.post_date as pdate, " +
                          "p.post_id as pid " +
                          "from POST as p " +
                          "inner join [User] u on p.post_user_fk_id = u.user_id " +
                          "where u.user_id = @valuename " +
                          "order by pdate desc";

        await sem.WaitAsync();
        
        await Task.Delay(200);

        var posts = new ObservableCollection<Post>();

        try
        {

            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();




            var command =
                new SqlCommand(
                    queryString)
                {
                    Transaction = transaction
                };


            command.Parameters.AddWithValue("valuename", id);

            command.Connection = connection;


            var result = command.ExecuteReaderAsync().Result;


            while (result.ReadAsync().Result)
            {
                var user = new User();
                user.Name = (string)result["puname"];

                var post = new Post((string)result["pid"], (string)result["ptext"]);
                post.PostAuthor = user;
                post.PublicationDate = (DateTime)result["pdate"];

                posts.Add(post);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
        finally
        {
            sem.Release();

        }
        
        return posts;
    }

    public async Task<Person> LoadPersonAsync(string id)
    {

        await sem.WaitAsync();
        
        await Task.Delay(800);
        var query = "select user_id, user_name, user_artist_fk_id, user_image from [USER] where user_id = @uid";

        Person user = new User();

        try
        {

            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();


            var command = new SqlCommand(query, connection)
            {
                Transaction = transaction
            };
            command.Parameters.AddWithValue("uid", id);

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                var name = (string)result["user_name"];
                var uid = (string)result["user_id"];
                var artistId = result["user_artist_fk_id"];
                var image = result["user_image"];

                if (artistId is string)
                {
                    var artist = new Artist();
                    user = artist;
                }
                else user = new User();

                if (image is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                user.ID = uid;
                user.Name = name;
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
        }
        
        return user;
    }

    public async Task<int> LoadUserSubscribersAsync(string id)
    {

        
        int count = 0;

        var queryString =
            "select COUNT(*) as [count] from SUBSCRIPTIONS\nwhere subscription_user_subscribed_fk_id = @valueid";

        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();
            
            await using var transaction = connection.BeginTransaction();
            

            var command =
                new SqlCommand(
                    queryString)
                {
                    Transaction = transaction
                };

            command.Parameters.AddWithValue("valueid", id);
            command.Connection = connection;
            
            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                count = (int)result["count"];
            }
            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
        finally
        {
        }

        
        return count;
    }

    public async Task<int> LoadUserSubscriptionsAsync(string id)
    {
        var queryString = "select COUNT(*) as count from SUBSCRIPTIONS\nwhere subscription_subscriber_fk_id = @valueid";

        int count = 0;


        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();



            var command =
                new SqlCommand(queryString)
                {
                    Transaction = transaction
                };

            command.Parameters.AddWithValue("valueid", id);
            command.Connection = connection;
            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                count = (int)result["count"];
            }

            await result.CloseAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
        }

        return count;
    }

    public async Task<ObservableCollection<Person>> LoadSubscribersAsync(string id)
    {

        await sem.WaitAsync();
        
        await Task.Delay(500);


        var queryString =
            "select sub.user_name as [name], " +
            "sub.user_image as [image], sub.user_id as [id] " +
            "from SUBSCRIPTIONS as subs " +
            "inner join [USER] as sub on subs.subscription_subscriber_fk_id = " +
            "sub.user_id " +
            "where subs.subscription_user_subscribed_fk_id = @valueid";

        var collection = new ObservableCollection<Person>();

        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);
            
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();


            var command = new SqlCommand(queryString)
            {
                Transaction = transaction
            };

            command.Connection = connection;

            command.Parameters.AddWithValue("valueid", id);

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                Person user = new User();
                var uid = (string)result["id"];
                var image = result["image"];
                var uname = (string)result["name"];

                if (image is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                user.Name = uname;
                user.ID = uid;
                collection.Add(user);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();

        }

        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
        }

        return collection;
    }

    public async Task<ObservableCollection<Person>> LoadSubscriptionsAsync(string id)
    {
        
        var queryString =
            "select sub.user_name as " +
            "[name], sub.user_image as [image]" +
            ", sub.user_id as [id]" +
            " from SUBSCRIPTIONS as subs " +
            "inner join [USER] as sub on " +
            "subs.subscription_user_subscribed_fk_id = sub.user_id " +
            "where subs.subscription_subscriber_fk_id = @valueid";

        await sem.WaitAsync();
        await Task.Delay(500);
        var collection = new ObservableCollection<Person>();


        try
        {

            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();



            var command = new SqlCommand(queryString)
            {
                Transaction = transaction
            };

            command.Connection = connection;

            command.Parameters.AddWithValue("valueid", id);

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                Person user = new User();
                var uid = (string)result["id"];
                var image = result["image"];
                var uname = (string)result["name"];

                if (image is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                user.Name = uname;
                user.ID = uid;
                collection.Add(user);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
            
        }


        return collection;
    }


    public async Task<bool> AddPost(Post post)
    {
        var queryString = "insert into POST(post_id, post_date, post_playlist_fk_id, post_text, post_user_fk_id)" +
                          " values" +
                          " (@postid, @postDate, NULL, @postText, @postauthor)";

        await sem.WaitAsync();

        try
        {
            await Task.Delay(3000);

            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();


            // TODO account binding and remove this from fucking here
            var testUserID = "basebashit";


            var postID = post.ID;
            var postText = post.Description;
            var postAuthor = testUserID;


            var command =
                new SqlCommand(queryString)
                {
                    Transaction = transaction
                };


            command.Parameters.AddWithValue("@postid", postID);
            command.Parameters.AddWithValue("@postDate", post.PublicationDate);
            command.Parameters.AddWithValue("@postText", postText);
            command.Parameters.AddWithValue("@postauthor", postAuthor);


            command.Connection = connection;
            _ = command.ExecuteNonQueryAsync().Result;

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
        }

        return true;
    }

    public async Task AddPlaylist(Playlist playlist)
    {
        
        var queryString =
            "insert into PLAYLIST(playlist_id, user_fk_id, playlist_type_fk_id, playlist_name, playlist_description, playlist_image) " +
            "values " +
            "(@plid, @uid, @pltid, @plname, @pld, @pli)";

        var tracksQueryString =
            "insert into TRACK(TRACK_ID, TRACK_PATH, track_playlist_fk_id, track_lyrics, track_name) values";
        
        
        
        await sem.WaitAsync();

        try
        {
            if (string.IsNullOrEmpty(playlist.ID) || string.IsNullOrEmpty(playlist.Name) ||
                playlist.Author is not Person person
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

            command.Connection = connection;

            await command.ExecuteNonQueryAsync();


            for (int i = 0; i < playlist.Tracks.Count(); i++)
            {
                tracksQueryString += $" (@tid{i}, @tpath{i}, @tplid{i}, @tlyrics{i}, @tname{i})";
                if (i != playlist.Tracks.Count() - 1) tracksQueryString += ",";
            }

            var tracksCommand = new SqlCommand(tracksQueryString)
            {
                Transaction = transaction
            };

            for (int i = 0; i < playlist.Tracks.Count(); i++)
            {
                tracksCommand.Parameters.AddWithValue($"tid{i}", playlist.Tracks[i].ID);
                tracksCommand.Parameters.AddWithValue($"@tpath{i}", playlist.Tracks[i].Source.AbsolutePath);
                tracksCommand.Parameters.AddWithValue($"@tplid{i}", playlist.ID);
                tracksCommand.Parameters.AddWithValue($"@tlyrics{i}",
                    string.IsNullOrEmpty(playlist.Tracks[i].Lyrics) ? "" : playlist.Tracks[i].Lyrics);
                tracksCommand.Parameters.AddWithValue($"@tname{i}", playlist.Tracks[i].Name);
            }

            tracksCommand.Connection = connection;

            await tracksCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
        }

    }
    
    
     public async Task UpdatePlaylistAsync(Playlist playlist, ObservableCollection<Track> oldTracks)
    {
        
        var queryString =
            "update playlist set playlist_description = @pld, playlist_type_fk_id = @pltid, playlist_name = @plname, playlist_image = @pli where playlist_id = @plid";

        var tracksQueryString =
            "delete from track where TRACK_ID in (";

        var tracksUpdateQueryString =
            "update track set track_lyrics = @tlyrics, track_name = @tname, TRACK_PATH = @tpath\nwhere TRACK_ID = @tid";

        
        await sem.WaitAsync();

        
        try
        {
            if (string.IsNullOrEmpty(playlist.ID) || string.IsNullOrEmpty(playlist.Name) ||
                playlist.Author is not Person person
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
        finally
        {
            sem.Release();
        }
    }


    public async Task<Playlist> LoadAlbumAsync(string id)
    {

        var playlist = new Playlist();

        
        const string tracksQuery = "select t.TRACK_ID as id, t.track_lyrics as lyrics, t.track_name as [name], t.TRACK_PATH as [tpath] from TRACK as t where t.track_playlist_fk_id = @playlistid";
        var playlistQuery = "select p.playlist_id as id, p.playlist_description as [description], p.playlist_image as [image], p.playlist_name as [name], p.playlist_type_fk_id as [type], p.user_fk_id as [uid] from PLAYLIST as p where p.playlist_id = @playlistid";
        var userQuery = "select u.user_id as [id], u.user_name as [name] from [USER] as u\ninner join PLAYLIST as p on p.user_fk_id = u.user_id and p.playlist_id = @playlistid";

        await sem.WaitAsync();

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
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            sem.Release();
        }

        return playlist;
    }
    
    
    
    
    
    
    public async Task<Playlist> LoadAlbumInfoAsync(string id)
    {

        await sem.WaitAsync();
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
        finally
        {
            sem.Release();
        }
        return playlist;
    }
    
    
    public async Task<ObservableCollection<Playlist>> LoadAlbumsInfoAsync()
    {

        var query =
            "select p.playlist_id as id, p.playlist_description as [description], p.playlist_image as [image], p.playlist_name as [name], p.playlist_type_fk_id as [type], u.user_id as [uid], u.user_name as [uname] from PLAYLIST as p inner join [USER] as u on u.user_id = p.user_fk_id";
        await sem.WaitAsync();
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
        finally
        {
            sem.Release();
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
                var item = await LoadAlbumInfoAsync(id);
                collection.Add(item);
            }

        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }
        return collection;
    }

    public async Task<List<string>> GetUserPlaylistRelations(string userId)
    {
        var queryString = "select p.playlist_id as [id] from PLAYLIST as p\nwhere p.user_fk_id = @uid";
        var list = new List<string>();
        await sem.WaitAsync();

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
        finally
        {
            sem.Release();
            
        }
        return list;
    }
    
    
    
    
    


    public PlatformDAO()
    {
        sem = new SemaphoreSlim(1);
    }

    ~PlatformDAO()
    {
        sem.Dispose();
    }
}