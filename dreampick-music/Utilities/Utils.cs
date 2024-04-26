using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
            str += (char)rand.Next(70,120);
        }

        return str;
    }  
    
    public static BitmapImage GetBitmapImage(byte[] imageBytes)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = new MemoryStream(imageBytes);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public static byte[] GetByteArrayFromImage(BitmapImage image)
    {
        byte[] data;
        JpegBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(image));
        using (var stream = new MemoryStream())
        {
            encoder.Save(stream);
            data = stream.ToArray();
        }

        return data;
    }
}