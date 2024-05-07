using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Imaging;
using dreampick_music.Models;
using Microsoft.Data.SqlClient;
using Track = dreampick_music.Models.Track;

namespace dreampick_music.DB;

public class TrackDAO
{

    public static TrackDAO Instance = new TrackDAO();
    public async Task<bool> IsRelatedAsync(string userId, string trackId)
    {
        var res = false;
        var queryString = "if exists(select * from USER_TRACK_RELATIONS where user_id = @uid and track_id = @pid ) " +
                          "select cast(1 as bit) as [statement]" +
                          "else " +
                          "select cast(0 as bit) as [statement]";
        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("uid", userId);
        command.Parameters.AddWithValue("pid", trackId);
        
        var result = command.ExecuteReaderAsync().Result;
        
        while (result.ReadAsync().Result)
        {
            res = result["statement"] is true;
        }

        return res;
    }

    public async Task<ObservableCollection<Track>> Relations(string id)
    {
        return new ObservableCollection<Track>();
    }
    
    public async Task<bool> RelateAsync(string userId, string trackId)
    {
        var res = false;
        var queryString = "if exists(select * from USER_TRACK_RELATIONS where user_id = @uid and track_id = @pid ) " +
                          "delete USER_TRACK_RELATIONS where user_id = @uid and track_id = @pid " +
                          "else " +
                          "insert into USER_TRACK_RELATIONS (user_id, track_id, relation_date) values (@uid, @pid, @date)";
        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("uid", userId);
        command.Parameters.AddWithValue("pid", trackId);
        command.Parameters.AddWithValue("date", DateTime.Now);
        
        await command.ExecuteNonQueryAsync();


        return true;
    }

    public async Task<ObservableCollection<Track>> RelatedAsync(string userId)
    {
        var res = new ObservableCollection<Track>();

        var queryString =
            "select " +
            "p.playlist_image as [pimage], " +
            "t.track_name as [name], " +
            "t.track_lyrics as [lyrics], " +
            "t.TRACK_ID as [id], " +
            "t.TRACK_PATH as [path], " +
            "t.track_playlist_fk_id as [plid], " +
            "p.playlist_name as [pname], " +
            "u.user_id as [uid], " +
            "u.user_name as [uname] " +
            "from [USER_TRACK_RELATIONS] as ut " +
            "inner join [TRACK] as t on t.TRACK_ID = ut.track_id and ut.user_id = @id " +
            "inner join [PLAYLIST] as p on p.playlist_id = t.track_playlist_fk_id " +
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

            var track = new Track()
            {
                Album = album,
                Name = (string)result["name"],
                ID = (string)result["id"],
                Source = new Uri((string)result["path"], UriKind.Absolute),
                Lyrics = (string)result["lyrics"] is string str ? str : ""
            };
            
            res.Add(track);

        }

        await result.CloseAsync();

        await connection.CloseAsync();

        return res;

    }
}