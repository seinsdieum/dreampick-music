using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace dreampick_music.Models;

public class PlatformDAO : IDatabaseRequests
{
    public static PlatformDAO Instance = new PlatformDAO();

    public ObservableCollection<Post> LoadPosts()
    {
        var posts = new ObservableCollection<Post>();

        using (var connection = new SqlConnection(Config.Instance.DbString))
        {
            connection.Open();
            var command =
                new SqlCommand(
                    Requests.GetPostsRequest());
            command.Connection = connection;
            var result = command.ExecuteReader();


            while (result.Read())
            {
                var user = new User();
                user.ID = (string)result["puid"];
                user.Name = (string)result["puname"];

                var imgRes = result["uimg"];
                if (imgRes is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                var post = new Post((string)result["pid"], (string)result["ptext"]);
                post.PostAuthor = user;
                post.PublicationDate = (DateTime)result["pdate"];

                posts.Add(post);
            }

            result.Close();
        }

        return posts;
    }

    public Person LoadPerson(string id)
    {
        var query = "select user_id, user_name, user_artist_fk_id, user_image from [USER] where user_id = @uid";

        Person user = new User();

        using (var connection = new SqlConnection(Config.Instance.DbString))
        {
            connection.Open();
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("uid", id);

            var result = command.ExecuteReader();

            while (result.Read())
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
        }

        return user;
    }

    public int LoadUserSubscribers(string id)
    {
        int count = 0;

        using var connection = new SqlConnection(Config.Instance.DbString);
        connection.Open();
        var command =
            new SqlCommand("select COUNT(*) as count from SUBSCRIPTIONS\nwhere subscription_user_subscribed_fk_id = @valueid");

        command.Parameters.AddWithValue("valueid", id);
        command.Connection = connection;
        var result = command.ExecuteReader();

        while (result.Read())
        {
            count = (int)result["count"];
        }

        return count;
    }
    
    public int LoadUserSubscriptions(string id)
    {
        int count = 0;

        using var connection = new SqlConnection(Config.Instance.DbString);
        connection.Open();
        var command =
            new SqlCommand("select COUNT(*) as count from SUBSCRIPTIONS\nwhere subscription_subscriber_fk_id = @valueid");

        command.Parameters.AddWithValue("valueid", id);
        command.Connection = connection;
        var result = command.ExecuteReader();

        while (result.Read())
        {
            count = (int)result["count"];
        }

        return count;
    }


    public void AddPost(Post post)
    {
        using var connection = new SqlConnection(Config.Instance.DbString);
        connection.Open();

        var testUserID = "basebashit";


        var postID = post.ID;
        var postDate = Utils.FormatDateToSQL(post.PublicationDate);
        var postText = post.Description;
        var postAuthor = testUserID;


        var command =
            new SqlCommand(
                "insert into POST(post_id, post_date, post_playlist_fk_id, post_text, post_user_fk_id)" +
                " values" +
                " (@postid, @postDate, NULL, @postText, @postauthor)");


        command.Parameters.AddWithValue("@postid", postID);
        command.Parameters.AddWithValue("@postDate", post.PublicationDate);
        command.Parameters.AddWithValue("@postText", postText);
        command.Parameters.AddWithValue("@postauthor", postAuthor);


        command.Connection = connection;
        command.ExecuteNonQuery();
    }
}