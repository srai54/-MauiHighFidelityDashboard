using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Services.Interfaces;

public interface IPrintService
{
    /// <summary>
    /// Renders the given orders as a printable report and opens the
    /// platform print flow (print preview + OS print dialog).
    /// </summary>
    Task PrintOrdersAsync(IReadOnlyList<OrderModel> orders, string documentTitle);
}
