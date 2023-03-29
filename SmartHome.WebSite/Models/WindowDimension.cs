using Microsoft.JSInterop;

namespace SmartHome.WebSite.Models
{
    public class WindowDimension
    {
        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;

        public async Task GetDimensions(IJSRuntime js)
        {
            var dimension = await js.InvokeAsync<WindowDimension>("getDimensions");
            Width = dimension.Width;
            Height = dimension.Height;
        }
    }
}
