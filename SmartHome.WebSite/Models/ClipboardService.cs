using Microsoft.JSInterop;

namespace SmartHome.WebSite.Models
{
 public class ClipboardService
 {
  public async Task CopyToClipboard(IJSRuntime js, string text)
  {
   await js.InvokeVoidAsync("navigator.clipboard.writeText", text);
  }
 }
}
