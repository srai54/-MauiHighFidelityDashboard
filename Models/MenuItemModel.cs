namespace HighFidelity.Ui.Models;

public class MenuItemModel
{
    public string Label { get; init; } = string.Empty;
    public string Icon { get; init; } = "\u25C7";
    public bool IsActive { get; init; }
    public string Route { get; init; } = string.Empty;
}
