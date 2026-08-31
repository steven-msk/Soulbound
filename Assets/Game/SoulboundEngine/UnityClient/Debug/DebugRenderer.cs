namespace SoulboundEngine.UnityClient.Debug {
	using SoulboundEngine.UnityClient.Util;
	using SoulboundEngine.World.Physics;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Rendering;

	public sealed class DebugRenderer {
		private readonly List<Line> lines = new();
		private readonly Material lineMaterial;

		public DebugRenderer() 
			: this(GetDefaultLineMaterial()) {

		}

		public DebugRenderer(Material lineMaterial) {
			this.lineMaterial = lineMaterial;
		}

		public void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) {
			this.lineMaterial.SetPass(0);
			GL.Begin(GL.LINES);
			foreach (Line line in this.GetLines()) {
				GL.Color(line.color);
				GL.Vertex(line.from);
				GL.Vertex(line.to);
			}
			GL.End();
		}

		public void AddLineBox(Vector2 min, Vector2 max, Color color) {
			this.AddLine(new Vector3(min.x, min.y), new Vector3(min.x, max.y), color);
			this.AddLine(new Vector3(min.x, min.y), new Vector3(max.x, min.y), color);
			this.AddLine(new Vector3(max.x, min.y), new Vector3(max.x, max.y), color);
			this.AddLine(new Vector3(min.x, max.y), new Vector3(max.x, max.y), color);
		}

		public void AddLineBox(AABB box, Color color) {
			this.AddLineBox(box.Min.ToVector2(), box.Max.ToVector2(), color);
		}

		public void AddLine(Vector3 from, Vector3 to, Color color) {
			this.lines.Add(new Line(from, to, color));
		}

		public void AddLine(Line line) {
			this.lines.Add(line);
		}

		public void Clear() {
			this.lines.Clear();
		}

		public IEnumerable<Line> GetLines() => this.lines;

		public static Material GetDefaultLineMaterial() {
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			Material lineMaterial = new(shader) { hideFlags = HideFlags.HideAndDontSave };
			lineMaterial.SetInt("_ZWrite", 0);
			return lineMaterial;
		}
	}
}
