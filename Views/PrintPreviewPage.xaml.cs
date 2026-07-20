namespace HighFidelity.Ui.Views;

public partial class PrintPreviewPage : ContentPage
{
    private readonly string _jobName;

    public PrintPreviewPage(string html, string jobName)
    {
        InitializeComponent();
        _jobName = jobName;
        ReportWebView.Navigated += OnReportLoaded;
        ReportWebView.Source = new HtmlWebViewSource { Html = html };
    }

    private void OnReportLoaded(object? sender, WebNavigatedEventArgs e)
        => PrintButton.IsEnabled = true;

    private async void OnPrintClicked(object? sender, EventArgs e)
    {
        try
        {
#if WINDOWS
            // WebView2 hosts the report; its browser print dialog covers
            // physical printers and Print-to-PDF.
            if (ReportWebView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.WebView2 nativeWebView)
            {
                await nativeWebView.EnsureCoreWebView2Async();
                nativeWebView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            }
#elif ANDROID
            // Standard Android print flow: system PrintManager renders the WebView.
            if (ReportWebView.Handler?.PlatformView is Android.Webkit.WebView nativeWebView &&
                Platform.CurrentActivity?.GetSystemService(Android.Content.Context.PrintService)
                    is Android.Print.PrintManager printManager)
            {
                var adapter = nativeWebView.CreatePrintDocumentAdapter(_jobName);
                var attributes = new Android.Print.PrintAttributes.Builder().Build();
                printManager.Print(_jobName, adapter, attributes);
            }
#else
            await DisplayAlertAsync("Print", "Printing is not supported on this platform yet.", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Print failed", ex.Message, "OK");
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
        => await Navigation.PopModalAsync();
}
