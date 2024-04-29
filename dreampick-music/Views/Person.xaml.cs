using System.Windows;
using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music;

public partial class Person : Page
{
    public Person(string id)
    {
        
        InitializeComponent();
        if (this.DataContext is PersonVm context)
        {
            context.UserId = id;
        }
    }
}