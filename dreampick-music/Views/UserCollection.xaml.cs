using System.Windows.Controls;
using dreampick_music.Models;

namespace dreampick_music.Views;

public partial class UserCollection : Page
{
    public UserCollection(string id, UserCollectionType type = UserCollectionType.NoType)
    {
        InitializeComponent();
        if (DataContext is not UserCollectionVm vm) return;
        
        
        vm.CollectionType = type;
        vm.ReferenceId = id;
    }
    
    
}