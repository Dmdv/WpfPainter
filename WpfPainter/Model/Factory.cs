using System.Collections.Generic;

namespace WpfPainter.Model
{
	internal static class Factory
	{
		public static IEnumerable<Layer> CreateLayers()
		{
			return new[]
			{
				new Layer
				{
					ZIndex = 0,
					Name = "Layer 1",
					IsVisible = true
				},
				new Layer
				{
					ZIndex = 1,
					Name = "Layer 2",
					IsVisible = true,
					X = 100,
					Y = 100
				},
			};
		}
	}
}