using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.Models;
using Microsoft.Data.SqlClient;

namespace dreampick_music.DB;

public class PostDAO
{
    public static PostDAO Instance = new PostDAO();
    
    
    public async Task<ObservableCollection<Post>> CollectionAsync()
    {
        
        await Task.Delay(500);

        var queryString = "select post_date as pdate, " +
                          "post_id as pid, " +
                          "post_text as ptext, " +
                          "[USER].user_id as puid, " +
                          "[USER].user_name as puname, " +
                          "[USER].user_image as uimg, " +
                          "p.playlist_id as plid, " +
                          "p.playlist_image as plimage, " +
                          "p.playlist_name as plname " +
                          "from POST " +
                          "inner join [USER] ON [USER].user_id = post_user_fk_id " +
                          "left join [PLAYLIST] as p ON post_playlist_fk_id = p.playlist_id " +
                          "order by pdate desc";
        
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
                    PublicationDate = (DateTime)result["pdate"],
                    Likes = await RelationsCountAsync((string)result["pid"])
                };

                if (result["plid"] is string id)
                {
                    
                    var playlist = new Playlist()
                    {
                        ID = (string)result["plid"],
                        Name = (string)result["plname"]
                    };
                    
                    if (result["plimage"] is byte[] albumBytes)
                    {
                        playlist.Image = Utils.GetBitmapImage(albumBytes);
                    }
                    

                    post.PostPlaylist = playlist;
                }

                posts.Add(post);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        
        return posts;
    }


    public async Task<bool> IsRelatedAsync(string userId, string postId)
    {
        var res = false;
        var queryString = "if exists(select * from USER_POST_RELATION where user_id = @uid and post_id = @pid ) " +
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
        command.Parameters.AddWithValue("pid", postId);
        
        var result = command.ExecuteReaderAsync().Result;
        
        while (result.ReadAsync().Result)
        {
            res = result["statement"] is true;
        }

        return res;
    }
    
    public async Task<bool> RelateAsync(string userId, string postId)
    {
        var res = false;
        var queryString = "if exists(select * from USER_POST_RELATION where user_id = @uid and post_id = @pid ) " +
                          "delete USER_POST_RELATION where user_id = @uid and post_id = @pid " +
                          "else " +
                          "insert into USER_POST_RELATION (user_id, post_id, relation_date) values (@uid, @pid, @date)";
        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        await using var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("uid", userId);
        command.Parameters.AddWithValue("pid", postId);
        command.Parameters.AddWithValue("date", DateTime.Now);
        
        await command.ExecuteNonQueryAsync();


        return true;
    }
    
    
    public async Task<int> RelationsCountAsync(string id)
    {

        
        int count = 0;

        var queryString =
            "select COUNT(*) as [count] from USER_POST_RELATION\nwhere post_id = @valueid";

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
        
        return count;
    }
    
    public async Task<ObservableCollection<Post>> UserCollectionAsync(string id)
    {
        
        var queryString = "select p.post_text as ptext, " +
                          "u.user_name as puname, " +
                          "p.post_date as pdate, " +
                          "p.post_id as pid, " +
                          "pl.playlist_id as plid, " +
                          "pl.playlist_image as plimage, " +
                          "pl.playlist_name as plname " +
                          "from POST as p " +
                          "inner join [User] u on p.post_user_fk_id = u.user_id " +
                          "left join [PLAYLIST] pl ON post_playlist_fk_id = pl.playlist_id " +
                          "where u.user_id = @valuename " +
                          "order by pdate desc";

        
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
                post.Likes = await RelationsCountAsync((string)result["pid"]);

                
                if (result["plid"] is string idd)
                {
                    
                    var playlist = new Playlist()
                    {
                        ID = idd,
                        Name = (string)result["plname"]
                    };
                    
                    if (result["plimage"] is byte[] albumBytes)
                    {
                        playlist.Image = Utils.GetBitmapImage(albumBytes);
                    }
                    

                    post.PostPlaylist = playlist;
                }

                posts.Add(post);
            }

            await result.CloseAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
        }

        
        return posts;
    }
    
    public async Task<bool> AddAsync(Post post)
    {
        var queryString = "insert into POST(post_id, post_date, post_playlist_fk_id, post_text, post_user_fk_id)" +
                          " values" +
                          " (@postid, @postDate, @postPlaylist, @postText, @postauthor)";


        try
        {
            await Task.Delay(1000);

            await using var connection = new SqlConnection(Config.Instance.DbString);
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();




            var postID = post.ID;
            var postText = post.Description;
            var postAuthor = post.PostAuthor.ID;


            var command =
                new SqlCommand(queryString)
                {
                    Transaction = transaction
                };


            command.Parameters.AddWithValue("@postid", postID);
            command.Parameters.AddWithValue("@postDate", post.PublicationDate);
            command.Parameters.AddWithValue("@postText", postText);
            command.Parameters.AddWithValue("@postauthor", postAuthor);
            command.Parameters.AddWithValue("@postPlaylist", post.PostPlaylist is not null ? post.PostPlaylist.ID  : DBNull.Value);


            command.Connection = connection;
            _ = command.ExecuteNonQueryAsync().Result;

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }

        return true;
    }
    
    
    public async Task<ObservableCollection<Models.Person>> RelationsAsync(string id)
    {
        
        var queryString =
            "select u.user_name as " +
            "[name], u.user_image as [image]" +
            ", u.user_id as [id]" +
            " from USER_POST_RELATION as rel " +
            "inner join [USER] as u on " +
            "rel.user_id = u.user_id " +
            "where rel.post_id = @valueid " +
            "order by rel.relation_date desc";

        await Task.Delay(500);
        var collection = new ObservableCollection<Models.Person>();


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
                Models.Person user = new User();
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


        return collection;
    }
}