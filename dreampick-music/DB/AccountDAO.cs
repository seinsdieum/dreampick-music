using System;
using System.CodeDom;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;

namespace dreampick_music.Models;

public class AccountDAO
{
    public static AccountDAO Instance = new AccountDAO();
    
    
    public async Task<bool> VerifyUserAsync(string name, string hashedPassword)
    {
        User user = new User()
        {
            ID = "NOTFOUND"
        };
        var queryString =
            "select u.user_id as [id], u.user_password as password from [USER] as u where user_name = @name and user_password = @password";
        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            var command = new SqlCommand(queryString)
            {
                Connection = connection
            };
            

            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("password", hashedPassword);
            


            var result = command.ExecuteReaderAsync().Result;
            
            
            while (result.ReadAsync().Result)
            {
                Console.WriteLine("aha");
                user.ID = result["id"] is string str ? str : "NOTFOUND";
            }

            await result.CloseAsync();

        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
            return false;
        }

        return user.ID != "NOTFOUND";
    }

    public bool CheckUsernameExistingAsync(string name)
    {
        try
        {
            const string queryString = "select count(*) from [USER] as u where user_name = @name";
            using var connection = new SqlConnection(Config.Instance.DbString);


            connection.Open();

            var command = new SqlCommand(queryString)
            {
                Connection = connection
            };


            command.Parameters.AddWithValue("name", name);

            var result = command.ExecuteScalar();

            return result is > 0;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message} {e.Source}");
            return false;
        }
    }

    public async Task<bool> AddUserAsync(AccountModel accountModel)
    {
        var queryString =
            "insert into [USER] (user_id, user_name, user_password, user_email, user_image, user_is_artist) VALUES (@id, @name, @password, @email, NULL, 0)";

        try
        {
            await using var connection = new SqlConnection(Config.Instance.DbString);

            await connection.OpenAsync();

            var command = new SqlCommand(queryString)
            {
                Connection = connection
            };

            command.Parameters.AddWithValue("id", accountModel.ID);
            command.Parameters.AddWithValue("name", accountModel.Name);
            command.Parameters.AddWithValue("password", accountModel.Password);
            command.Parameters.AddWithValue("email", accountModel.Email);

            var result = command.ExecuteNonQueryAsync().Result;
            return result > 0;
        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
            return false;
        }
    }


    public async Task<Person> LoadAccountAsync(string uname)
    {
        await Task.Delay(1200);
        var query =
            "select user_id, user_name, user_image, user_email, user_is_artist from [USER] where user_name = @name";

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
            command.Parameters.AddWithValue("name", uname);

            var result = command.ExecuteReaderAsync().Result;

            while (result.ReadAsync().Result)
            {
                var name = (string)result["user_name"];
                var uid = (string)result["user_id"];
                var image = result["user_image"];
                var isArtist = result["user_is_artist"];
                var email = (string)result["user_email"];

                if (isArtist is true)
                {
                    var artist = new Artist();
                    user = artist;
                }
                else user = new User();

                if (image is byte[] bytes)
                {
                    user.Image = Utils.GetBitmapImage(bytes);
                }

                user.Email = email;
                user.ID = uid;
                user.Name = name;
            }

            await result.CloseAsync();

            await transaction.CommitAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show($"{e.Message}");
            return user;
        }

        return user;
    }

    public async Task<bool> ChangePersonProperty(string id, PersonPropertyChangeType type, object propValue)
    {

        var valueType = Utils.GetPersonChangeType(type);
        var a = typeof(bool);

        if (propValue.GetType() != Utils.GetPersonChangeType(type) ) return false;
        
        var queryString = type switch
        {
            PersonPropertyChangeType.Image => "update [USER] SET user_image = @value where user_id = @id",
            PersonPropertyChangeType.Password => "update [USER] SET user_password = @value where user_id = @id",
            PersonPropertyChangeType.IsArtist => "update [USER] SET user_is_artist = @value where user_id = @id",
            PersonPropertyChangeType.Username => "update [USER] SET user_name = @value where user_id = @id",
            PersonPropertyChangeType.Email => "update [USER] SET user_email = @value where user_id = @id",
            _ => "",
        };

        await using var connection = new SqlConnection(Config.Instance.DbString);

        await connection.OpenAsync();

        var command = new SqlCommand(queryString)
        {
            Connection = connection
        };

        command.Parameters.AddWithValue("id", id);

        command.Parameters.AddWithValue("value", (type switch
        {
            PersonPropertyChangeType.Image => Utils.GetByteArrayFromImage((BitmapImage)propValue),
            PersonPropertyChangeType.Password => (string)propValue,
            PersonPropertyChangeType.IsArtist => (propValue is true ? 1 : 0),
            PersonPropertyChangeType.Username => (string)propValue,
            PersonPropertyChangeType.Email => (string)propValue,
        }));

        var result = command.ExecuteNonQueryAsync().Result;
        
        return result > 0;
    }
}