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
                    PublicationDate = (DateTime)result["pdate"],
                    Likes = await RelationsCountAsync((string)result["pid"])
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
}