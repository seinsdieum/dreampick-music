namespace dreampick_music.Models;

public class Requests
{

    public static string GetUserInfo(string id)
    {
        return "";
    }

    public static string SetUserImage()
    {
        return "update [USER]\nset user_image = @imgvarbinary where user_id = 'sdfsd'";
    }
}