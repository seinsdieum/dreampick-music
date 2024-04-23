namespace dreampick_music.Models;

public class Requests
{
    public static string GetPostsRequest()
    {
        return "select post_date as pdate, post_id as pid, " +
               "post_text as ptext, [USER].user_id as puid, " +
               "[USER].user_name as puname, [USER].user_image as uimg from POST\ninner join [USER] ON [USER].user_id = post_user_fk_id order by pdate desc";
    }

    public static string GetUserInfo(string id)
    {
        return "";
    }

    public static string SetUserImage()
    {
        return "update [USER]\nset user_image = @imgvarbinary where user_id = 'sdfsd'";
    }
}