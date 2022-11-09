using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace csDelaunay {

	public class LineSegment {

		public static List<LineSegment> VisibleLineSegments(List<Edge> edges) {
			List<LineSegment> segments = new List<LineSegment>();

			foreach (Edge edge in edges) {
				if (edge.Visible()) {
					Vector2 p1 = edge.ClippedEnds[LR.LEFT];
					Vector2 p2 = edge.ClippedEnds[LR.RIGHT];
					segments.Add(new LineSegment(p1,p2));
				}
			}

			return segments;
		}

		public static float CompareLengths_MAX(LineSegment segment0, LineSegment segment1) {
			float length0 = (segment0.p0 - segment0.p1).magnitude;
			float length1 = (segment1.p0 - segment1.p1).magnitude;
			if (length0 < length1) {
				return 1;
			}
			if (length0 > length1) {
				return -1;
			}
			return 0;
		}

		public static float CompareLengths(LineSegment edge0, LineSegment edge1) {
			return - CompareLengths_MAX(edge0, edge1);
		}

		public Vector2 p0;
		public Vector2 p1;

		public LineSegment (Vector2 p0, Vector2 p1) {
			this.p0 = p0;
			this.p1 = p1;
		}
	}
}