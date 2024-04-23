using System;
using System.Windows;

namespace dreampick_music;

public class Utils
{
    public static string GetLocalizedName(string localName)
    {
        return (string)Application.Current.Resources[localName];
    }

    public static string FormatDateToSQL(DateTime date)
    {
        return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    public static string GenerateRandomString(int length)
    {
        var str = "";
        var rand = new Random();
        for (int i = 1; i <= length; i++)
        {
            str += (char)rand.Next(1000);
        }

        return str;
    }  
}