using System;
using System.Collections.Generic;
using System.Numerics;

namespace csDelaunay {
	public class Polygon {

		private List<Vector2> vertices;

		public Polygon(List<Vector2> vertices) {
			this.vertices = vertices;
		}

		public float Area() {
			return Math.Abs(SignedDoubleArea() * 0.5f);
		}

		public Winding PolyWinding() {
			float signedDoubleArea = SignedDoubleArea();
			if (signedDoubleArea < 0) {
				return Winding.CLOCKWISE;
			}
			if (signedDoubleArea > 0) {
				return Winding.COUNTERCLOCKWISE;
			}
			return Winding.NONE;
		}

		private float SignedDoubleArea() {
			int index, nextIndex;
			int n = vertices.Count;
			Vector2 point, next;
			float signedDoubleArea = 0;

			for (index = 0; index < n; index++) {
				nextIndex = (index+1) % n;
				point = vertices[index];
				next = vertices[nextIndex];
				signedDoubleArea += point.X * next.Y - next.X * point.Y;
			}

			return signedDoubleArea;
		}
	}
}
