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

public class PlatformDAO
{

    private SemaphoreSlim sem;
    
    public static PlatformDAO Instance = new PlatformDAO();

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
        var query = "select user_id, user_name, user_is_artist, user_image from [USER] where user_id = @uid";

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
                var isArtist = result["user_is_artist"] is true;
                var image = result["user_image"];

                if (isArtist)
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

        
        await Task.Delay(500);


        var queryString =
            "select sub.user_name as [name], " +
            "sub.user_image as [image], sub.user_id as [id] " +
            "from SUBSCRIPTIONS as subs " +
            "inner join [USER] as sub on subs.subscription_subscriber_fk_id = " +
            "sub.user_id " +
            "where subs.subscription_user_subscribed_fk_id = @valueid " +
            "order by subs.relation_date desc";

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
            "where subs.subscription_subscriber_fk_id = @valueid " +
            "order by subs.relation_date desc";

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


            command.Connection = connection;
            _ = command.ExecuteNonQueryAsync().Result;

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
        finally
        {
            sem.Release();
        }

        return true;
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