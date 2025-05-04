using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JsonToClass.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [RelayCommand]
    protected virtual void ViewLoaded()
    {
        
    }
}