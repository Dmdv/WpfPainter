using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using WpfPainter.Primitives;

namespace WpfPainter.Controls
{
	public class VisualCanvas : Canvas
	{
		protected override int VisualChildrenCount
		{
			get { return _children.Count; }
		}

		protected void AddVisual(Visual visual)
		{
			_children.Add(visual);
			//AddLogicalChild(visual);
		}

		protected void DeleteVisual(Visual visual)
		{
			_children.Remove(visual);
			//RemoveLogicalChild(visual);
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index < 0 || index >= _children.Count)
			{
				throw new ArgumentOutOfRangeException();
			}

			return _children[index];
		}

		protected DrawingObject[] GetVisuals(Geometry region)
		{
			_hits.Clear();
			var parameters = new GeometryHitTestParameters(region);
			HitTestResultCallback callback = HitTestCallback;
			VisualTreeHelper.HitTest(this, null, callback, parameters);
			return _hits.OfType<DrawingObject>().ToArray();
		}

		private HitTestResultBehavior HitTestCallback(HitTestResult result)
		{
			var geometryResult = (GeometryHitTestResult) result;
			var visual = result.VisualHit as DrawingVisual;
			if (visual != null &&
			    geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
			{
				_hits.Add(visual);
			}
			return HitTestResultBehavior.Continue;
		}

		protected VisualCanvas()
		{
			_children = new VisualCollection(this);
		}

		private readonly VisualCollection _children;
		private readonly List<DrawingVisual> _hits = new List<DrawingVisual>();
	}
}