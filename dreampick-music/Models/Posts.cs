using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace dreampick_music.Models;

public class Posts
{


    public static ObservableCollection<Post> LoadPosts()
    {
        var posts = new ObservableCollection<Post>();
        
        using (var connection = new SqlConnection(Config.Instance.DbString))
        {
            connection.Open();
            var command =
                new SqlCommand(
                    Requests.GetPostsRequest());
            command.Connection = connection;
            var result =  command.ExecuteReader();


            while (result.Read())
            {

                var user = new User();
                user.ID = (string)result["puid"];
                user.Name = (string)result["puname"];
                
                var post = new Post((string)result["pid"], (string)result["ptext"]);
                post.PostAuthor = user;
                post.PublicationDate = (DateTime)result["pdate"];
                
                posts.Add(post);
            }
            
            result.Close();
        }

        return posts;
    }

    public static void AddPost(Post post)
    {
        using (var connection = new SqlConnection(Config.Instance.DbString))
        {
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

    static Posts()
    {
        LoadPosts();
    }
}