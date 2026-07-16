using System.ComponentModel;

namespace MauiHighFidelityDashboard.Domain.Models;

public class OrderModel : INotifyPropertyChanged
{
    public int Invoice { get; init; }
    public string Customer { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Status { get; init; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
