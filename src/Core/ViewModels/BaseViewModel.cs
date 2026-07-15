using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiHighFidelityDashboard.Core.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;
}
