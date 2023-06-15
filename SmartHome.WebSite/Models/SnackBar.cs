namespace SmartHome.WebSite.Models
{
	public class SnackBar
	{
		public SnackBar() { }
		public string Message { get; private set; } = String.Empty;
		public async Task ShowMessage(string message)
		{
			Message = message;
			await Task.Run(async () => { await Task.Delay(3000); Message = String.Empty; });
		}
	}
}
