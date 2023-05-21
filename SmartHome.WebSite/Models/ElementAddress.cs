namespace SmartHome.WebSite.Models
{
	public class ElementAddress
	{
		public Guid ClientId { get; set; }
		public int ComponentId { get; set; } = -1;
		public int PortPinId { get; set; } = -1;
	}
}
