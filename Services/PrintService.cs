using System.Net;
using System.Text;
using HighFidelity.Ui.Models;
using HighFidelity.Ui.Services.Interfaces;
using HighFidelity.Ui.Views;

namespace HighFidelity.Ui.Services;

public class PrintService : IPrintService
{
    public async Task PrintOrdersAsync(IReadOnlyList<OrderModel> orders, string documentTitle)
    {
        string html = BuildOrdersHtml(orders, documentTitle);
        await Shell.Current.Navigation.PushModalAsync(new PrintPreviewPage(html, documentTitle));
    }

    private static string BuildOrdersHtml(IReadOnlyList<OrderModel> orders, string title)
    {
        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'>");
        sb.Append("<meta name='viewport' content='width=device-width, initial-scale=1'>");
        sb.Append("<title>").Append(WebUtility.HtmlEncode(title)).Append("</title>");
        sb.Append(@"<style>
            body { font-family: 'Segoe UI', Roboto, sans-serif; color: #464E5F; margin: 28px; }
            h1 { font-size: 20px; margin: 0 0 2px 0; }
            .subtitle { color: #8B93A7; font-size: 12px; margin-bottom: 20px; }
            table { width: 100%; border-collapse: collapse; font-size: 12px; }
            thead th { background: #F4F5F8; color: #8B93A7; text-transform: uppercase;
                       font-size: 10px; letter-spacing: .4px; text-align: left; padding: 9px 12px; }
            tbody td { padding: 9px 12px; border-bottom: 1px solid #EDF0F5; }
            .badge { display: inline-block; padding: 3px 10px; border-radius: 3px;
                     color: #fff; font-size: 10px; font-weight: 600; }
            .open { background: #2BC155; } .process { background: #F7284A; } .onhold { background: #6259CE; }
            tfoot td { padding: 12px; font-weight: 600; }
            @media print { body { margin: 0; } }
        </style></head><body>");

        sb.Append("<h1>Order Status</h1>");
        sb.Append("<div class='subtitle'>Overview of Latest Month &bull; Printed ")
          .Append(DateTime.Now.ToString("dd MMM yyyy, HH:mm"))
          .Append("</div>");

        sb.Append("<table><thead><tr><th>Invoice</th><th>Customer</th><th>From</th><th>Price</th><th>Status</th></tr></thead><tbody>");
        foreach (var order in orders)
        {
            string badgeClass = order.Status switch
            {
                "Open" => "open",
                "Process" => "process",
                _ => "onhold"
            };
            sb.Append("<tr>")
              .Append("<td>").Append(order.Invoice).Append("</td>")
              .Append("<td>").Append(WebUtility.HtmlEncode(order.Customer)).Append("</td>")
              .Append("<td>").Append(WebUtility.HtmlEncode(order.Country)).Append("</td>")
              .Append("<td>$").Append(order.Price.ToString("N0")).Append("</td>")
              .Append("<td><span class='badge ").Append(badgeClass).Append("'>")
              .Append(WebUtility.HtmlEncode(order.Status)).Append("</span></td>")
              .Append("</tr>");
        }
        sb.Append("</tbody><tfoot><tr><td colspan='3'>Total: ").Append(orders.Count).Append(" orders</td>")
          .Append("<td colspan='2'>$").Append(orders.Sum(o => o.Price).ToString("N2")).Append("</td></tr></tfoot></table>");
        sb.Append("</body></html>");
        return sb.ToString();
    }
}
