namespace WpfPainter.Model
{
	public class Layer
	{
		public int ZIndex { get; set; }

		public bool IsVisible { get; set; }

		public string Name { get; set; }

		public int X { get; set; }

		public int Y { get; set; }
	}
}