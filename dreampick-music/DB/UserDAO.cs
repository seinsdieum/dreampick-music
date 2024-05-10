using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using dreampick_music.Models;
using Microsoft.Data.SqlClient;

namespace dreampick_music.DB;

public class UserDAO
{

    public static UserDAO Instance = new UserDAO();    
    
    public async Task<Models.Person> GetAsync(string id)
    {

        
        await Task.Delay(800);
        var query = "select user_id, user_name, user_is_artist, user_image from [USER] where user_id = @uid";

        Models.Person user = new User();

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
        
        return user;
    }
    
    
    
    public async Task<ObservableCollection<Models.Person>> SubscribersAsync(string id)
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

    public async Task<ObservableCollection<Models.Person>> SubscriptionsAsync(string id)
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
    
    
    public async Task<int> SubscribersCountAsync(string id)
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

    public async Task<int> SubscriptionsCountAsync(string id)
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

        return count;
    }

    public async Task SubscribeAsync(string subscriberId, string subscribedId)
    {
        var queryString = "if exists(select * from SUBSCRIPTIONS where subscription_subscriber_fk_id = @subid and subscription_user_subscribed_fk_id = @subbedid ) " +
                          "delete SUBSCRIPTIONS where subscription_subscriber_fk_id = @subid and subscription_user_subscribed_fk_id = @subbedid " +
                          "else " +
                          "insert into SUBSCRIPTIONS (subscription_subscriber_fk_id, subscription_user_subscribed_fk_id, relation_date) values (@subid, @subbedid, @date)";
        
        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("subid", subscriberId);
        command.Parameters.AddWithValue("subbedid", subscribedId);
        command.Parameters.AddWithValue("date", DateTime.Now);

        await command.ExecuteNonQueryAsync();

        await connection.CloseAsync();
    }

    public async Task<bool> IsSubscribedAsync(string subscriberId, string subscribedId)
    {
        var statement = false;
        
        var queryString = "if exists(select * from SUBSCRIPTIONS where subscription_subscriber_fk_id = @subid and subscription_user_subscribed_fk_id = @subbedid ) " +
                          "select cast(1 as bit) as [statement] " +
                          "else " +
                          "select cast(0 as bit) as [statement]";

        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("subid", subscriberId);
        command.Parameters.AddWithValue("subbedid", subscribedId);

        var result = await command.ExecuteReaderAsync();

        while (await result.ReadAsync())
        {
            statement = result["statement"] is true;
        }

        
        return statement;
    }
    
    
}