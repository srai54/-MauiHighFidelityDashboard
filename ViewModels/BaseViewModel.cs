using CommunityToolkit.Mvvm.ComponentModel;

namespace HighFidelity.Ui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
#pragma warning disable MVVMTK0045 // ObservableProperty fields not AOT-compatible on WinRT
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;
#pragma warning restore MVVMTK0045
}
