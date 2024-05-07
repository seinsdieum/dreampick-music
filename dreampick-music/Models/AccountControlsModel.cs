using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using dreampick_music.Models;

namespace dreampick_music;


public enum AccountTab
{
    Feed = 1,
    Collection,
    Settings,
    CreatePost,
    ArtistAlbums,
    AccountPage,
}


public class AccountControlsModel
{
    public async Task<ObservableCollection<AccountTab>> LoadAccountTabsAsync()
    {

        await AccountVm.Instance.AccountPerson.Task.WaitAsync(TimeSpan.FromSeconds(10));

        if (!AccountVm.Instance.AccountPerson.IsSuccessfullyCompleted) throw new Exception();
        if (AccountVm.Instance.AccountPerson.Result is Artist)
        {
            return new ObservableCollection<AccountTab>()
            {
                AccountTab.Collection,
                AccountTab.Feed,
                AccountTab.CreatePost,
                AccountTab.Settings,
                AccountTab.AccountPage,
            };
        }

        return new ObservableCollection<AccountTab>()
        {
            AccountTab.Collection,
            AccountTab.Feed,
            AccountTab.CreatePost,
            AccountTab.Settings,
            AccountTab.ArtistAlbums,
            AccountTab.AccountPage,
        };

    }
}