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
    public async Task<ObservableCollection<Models.Person>> PostRelationsAsync(string id)
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