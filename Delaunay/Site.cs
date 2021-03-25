using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace csDelaunay {

	public class Site : ICoord {

		private static Queue<Site> pool = new Queue<Site>();

		public static Site Create(Vector2 p, int index, float weigth) {
			if (pool.Count > 0) {
				return pool.Dequeue().Init(p, index, weigth);
			} else {
				return new Site(p, index, weigth);
			}
		}

		public static void SortSites(List<Site> sites) {
			sites.Sort(delegate(Site s0, Site s1) {
				int returnValue = Voronoi.CompareByYThenX(s0,s1);
				
				int tempIndex;
				
				if (returnValue == -1) {
					if (s0.siteIndex > s1.SiteIndex) {
						tempIndex = s0.SiteIndex;
						s0.SiteIndex = s1.SiteIndex;
						s1.SiteIndex = tempIndex;
					}
				} else if (returnValue == 1) {
					if (s1.SiteIndex > s0.SiteIndex) {
						tempIndex = s1.SiteIndex;
						s1.SiteIndex = s0.SiteIndex;
						s0.SiteIndex = tempIndex;
					}
				}
				
				return returnValue;
			});
		}

		public int Compare(Site s1, Site s2) {
			return s1.CompareTo(s2);
		}

		public int CompareTo(Site s1) {
			int returnValue = Voronoi.CompareByYThenX(this,s1);

			int tempIndex;

			if (returnValue == -1) {
				if (this.siteIndex > s1.SiteIndex) {
					tempIndex = this.SiteIndex;
					this.SiteIndex = s1.SiteIndex;
					s1.SiteIndex = tempIndex;
				}
			} else if (returnValue == 1) {
				if (s1.SiteIndex > this.SiteIndex) {
					tempIndex = s1.SiteIndex;
					s1.SiteIndex = this.SiteIndex;
					this.SiteIndex = tempIndex;
				}
			}

			return returnValue;
		}
		
		private const float EPSILON = 0.005f;
		private static bool CloseEnough(Vector2 p0, Vector2 p1) {
			return (p0-p1).magnitude < EPSILON;
		}
		
		private int siteIndex;
		public int SiteIndex {get{return siteIndex;} set{siteIndex=value;}}
		
		private Vector2 coord;
		public Vector2 Coord {get{return coord;}set{coord=value;}}

		public float X {get{return coord.x;}}
		public float Y {get{return coord.y;}}

		private float weigth;
		public float Weigth {get{return weigth;}}

		// The edges that define this Site's Voronoi region:
		private List<Edge> edges;
		public List<Edge> Edges {get{return edges;}}
		// which end of each edge hooks up with the previous edge in edges:
		private List<LR> edgeOrientations;
		// ordered list of points that define the region clipped to bounds:
		private List<Vector2> region;

		public Site(Vector2 p, int index, float weigth) {
			Init(p, index, weigth);
		}

		private Site Init(Vector2 p, int index, float weigth) {
			coord = p;
			siteIndex = index;
			this.weigth = weigth;
			edges = new List<Edge>();
			region = null;

			return this;
		}

		public override string ToString() {
			return "Site " + siteIndex + ": " + coord;
		}

		private void Move(Vector2 p) {
			Clear();
			coord = p;
		}

		public void Dispose() {
			Clear();
			pool.Enqueue(this);
		}

		private void Clear() {
			if (edges != null) {
				edges.Clear();
				edges = null;
			}
			if (edgeOrientations != null) {
				edgeOrientations.Clear();
				edgeOrientations = null;
			}
			if (region != null) {
				region.Clear();
				region = null;
			}
		}

		public void AddEdge(Edge edge) {
			edges.Add(edge);
		}

		public Edge NearestEdge() {
			edges.Sort(Edge.CompareSitesDistances);
			return edges[0];
		}

		public List<Site> NeighborSites() {
			if (edges == null || edges.Count == 0) {
				return new List<Site>();
			}
			if (edgeOrientations == null) {
				ReorderEdges();
			}
			List<Site> list = new List<Site>();
			foreach (Edge edge in edges) {
				list.Add(NeighborSite(edge));
			}
			return list;
		}

		private Site NeighborSite(Edge edge) {
			if (this == edge.LeftSite) {
				return edge.RightSite;
			}
			if (this == edge.RightSite) {
				return edge.LeftSite;
			}
			return null;
		}

		public List<Vector2> Region(Rect clippingBounds) {
			if (edges == null || edges.Count == 0) {
				return new List<Vector2>();
			}
			if (edgeOrientations == null) {
				ReorderEdges();
				region = ClipToBounds(clippingBounds);
				if ((new Polygon(region)).PolyWinding() == Winding.CLOCKWISE) {
					region.Reverse();
				}
			}
			return region;
		}

		private void ReorderEdges() {
			EdgeReorderer reorderer = new EdgeReorderer(edges, typeof(Vertex));
			edges = reorderer.Edges;
			edgeOrientations = reorderer.EdgeOrientations;
			reorderer.Dispose();
		}

		private List<Vector2> ClipToBounds(Rect bounds) {
			List<Vector2> points = new List<Vector2>();
			int n = edges.Count;
			int i = 0;
			Edge edge;

			while (i < n && !edges[i].Visible()) {
				i++;
			}

			if (i == n) {
				// No edges visible
				return new List<Vector2>();
			}
			edge = edges[i];
			LR orientation = edgeOrientations[i];
			points.Add(edge.ClippedVertices[orientation]);
			points.Add(edge.ClippedVertices[LR.Other(orientation)]);

			for (int j = i + 1; j < n; j++) {
				edge = edges[j];
				if (!edge.Visible()) {
					continue;
				}
				Connect(ref points, j, bounds);
			}
			// Close up the polygon by adding another corner point of the bounds if needed:
			Connect(ref points, i, bounds, true);

			return points;
		}

		private void Connect(ref List<Vector2> points, int j, Rect bounds, bool closingUp = false) {
			Vector2 rightPoint = points[points.Count-1];
			Edge newEdge = edges[j];
			LR newOrientation = edgeOrientations[j];

			// The point that must be conected to rightPoint:
			Vector2 newPoint = newEdge.ClippedVertices[newOrientation];

			if (!CloseEnough(rightPoint, newPoint)) {
				// The points do not coincide, so they must have been clipped at the bounds;
				// see if they are on the same border of the bounds:
				if (rightPoint.x != newPoint.x && rightPoint.y != newPoint.y) {
					// They are on different borders of the bounds;
					// insert one or two corners of bounds as needed to hook them up:
					// (NOTE this will not be correct if the region should take up more than
					// half of the bounds rect, for then we will have gone the wrong way
					// around the bounds and included the smaller part rather than the larger)
					int rightCheck = BoundsCheck.Check(rightPoint, bounds);
					int newCheck = BoundsCheck.Check(newPoint, bounds);
					float px, py;
					if ((rightCheck & BoundsCheck.RIGHT) != 0) {
						px = bounds.xMax;

						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.yMax;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.yMin;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							if (rightPoint.y - bounds.y + newPoint.y - bounds.y < bounds.height) {
								py = bounds.yMin;
							} else {
								py = bounds.yMax;
							}
							points.Add(new Vector2(px,py));
							points.Add(new Vector2(bounds.xMin, py));
						}
					} else if ((rightCheck & BoundsCheck.LEFT) != 0) {
						px = bounds.xMin;

						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.yMax;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.yMin;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.RIGHT) != 0) {
							if (rightPoint.y - bounds.y + newPoint.y - bounds.y < bounds.height) {
								py = bounds.yMin;
							} else {
								py = bounds.yMax;
							}
							points.Add(new Vector2(px,py));
							points.Add(new Vector2(bounds.xMax, py));
						}
					} else if ((rightCheck & BoundsCheck.TOP) != 0) {
						py = bounds.yMin;

						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.xMax;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.xMin;
							points.Add(new Vector2(px,py));

						} else if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width) {
								px = bounds.xMin;
							} else {
								px = bounds.xMax;
							}
							points.Add(new Vector2(px,py));
							points.Add(new Vector2(px, bounds.yMax));
						}
					} else if ((rightCheck & BoundsCheck.BOTTOM) != 0) {
						py = bounds.yMax;
						
						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.xMax;
							points.Add(new Vector2(px,py));
							
						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.xMin;
							points.Add(new Vector2(px,py));
							
						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width) {
								px = bounds.xMin;
							} else {
								px = bounds.xMax;
							}
							points.Add(new Vector2(px,py));
							points.Add(new Vector2(px, bounds.yMin));
						}
					}
				}
				if (closingUp) {
					// newEdge's ends have already been added
					return;
				}
				points.Add(newPoint);
			}
			Vector2 newRightPoint = newEdge.ClippedVertices[LR.Other(newOrientation)];
			if (!CloseEnough(points[0], newRightPoint)) {
				points.Add(newRightPoint);
			}
		}

		public float Dist(ICoord p) {
			return (this.Coord - p.Coord).magnitude;
		}
	}

	public class BoundsCheck {
		public const int TOP = 1;
		public const int BOTTOM = 2;
		public const int LEFT = 4;
		public const int RIGHT = 8;

		/*
		 * 
		 * @param point
		 * @param bounds
		 * @return an int with the appropriate bits set if the Point lies on the corresponding bounds lines
		 */
		public static int Check(Vector2 point, Rect bounds) {
			int value = 0;
			if (point.x == bounds.xMin) {
				value |= LEFT;
			}
			if (point.x == bounds.xMax) {
				value |= RIGHT;
			}
			if (point.y == bounds.yMin) {
				value |= TOP;
			}
			if (point.y == bounds.yMax) {
				value |= BOTTOM;
			}

			return value;
		}
	}
}
