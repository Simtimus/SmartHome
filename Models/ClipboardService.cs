using Microsoft.JSInterop;

namespace SmartHome.Models
{
    public class ClipboardService
    {
        public async Task CopyToClipboard(IJSRuntime js, string text)
        {
            await js.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
