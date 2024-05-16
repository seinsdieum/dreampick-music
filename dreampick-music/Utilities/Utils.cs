using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using dreampick_music.Models;

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
    
    
    public static string HashPassword(string password)
    {
        using (var sha256 = new SHA256Managed())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length];

            // Concatenate password and salt
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);

            // Hash the concatenated password and salt
            byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

            // Concatenate the salt and hashed password for storage

            return Convert.ToBase64String(hashedBytes);
        }
    }
    
    
    /*public byte[] GenerateSalt()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] salt = new byte[16]; // Adjust the size based on your security requirements
            rng.GetBytes(salt);
            return salt;
        }
    }*/
    
}