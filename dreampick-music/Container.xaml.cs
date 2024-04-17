using System.Windows;
using System.Windows.Controls;

namespace dreampick_music;

public partial class Container : UserControl
{

    public string Title { get; set; } = "";
    
    /*
    public DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Container));
    */
    public Container()
    {
        InitializeComponent();
        DataContext = this;
    }
}