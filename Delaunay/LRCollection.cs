using System;

namespace csDelaunay
{
	public class LRCollection<T>
	{
		private T left;
		private T right;
		public T this[LR index]
		{
			get
			{
				return index == LR.LEFT ? left : right;
			}
			set
			{
				if (index == LR.LEFT)
				{
					left = value;
				}
				else
				{
					right = value;
				}
			}
		}

		public void Clear()
		{
			left = default(T);
			right = default(T);
		}
	}
}