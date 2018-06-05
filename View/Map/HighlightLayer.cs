using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Utilities;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class HighlightLayer : Pod
	{
		readonly List<Highlight> _Highlights = new List<Highlight>();
		bool _DirtyBuffer;

		readonly TransparentArrayList<Vertex> _VertexBuffer = new TransparentArrayList<Vertex>();

		public void Clear()
		{
			_Highlights.Clear();
			_DirtyBuffer = true;
		}

		public void AddHighlight(Highlight Highlight)
		{
			_Highlights.Add(Highlight);
			_DirtyBuffer = true;
		}

		public void RemoveHighlight(Highlight Highlight)
		{
			_Highlights.Remove(Highlight);
			_DirtyBuffer = true;
		}

		void UpdateVertexBuffer()
		{
			_VertexBuffer.Clear();
			var highlighted = new HashSet<Tile>();
			foreach (Highlight h in _Highlights)
			{
				foreach (Tuple<Tile, Color> t in h.Highlights)
				{
					if (!highlighted.Contains(t.Item1))
					{
						highlighted.Add(t.Item1);
						HighlightTile(t.Item1, t.Item2);
					}
				}
			}
		}

		void HighlightTile(Tile Tile, Color Color)
		{
			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				_VertexBuffer.Add(new Vertex(Tile.Bounds[i].Point, Color));
				_VertexBuffer.Add(new Vertex(Tile.Bounds[(i + 1) % Tile.Bounds.Length].Point, Color));
				_VertexBuffer.Add(new Vertex(Tile.Center, Color));
			}
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (_DirtyBuffer)
			{
				UpdateVertexBuffer();
				_DirtyBuffer = false;
			}
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(
				_VertexBuffer.Values,
				0,
				(uint)_VertexBuffer.Length,
				PrimitiveType.Triangles,
				new RenderStates(Transform));
		}
	}
}
