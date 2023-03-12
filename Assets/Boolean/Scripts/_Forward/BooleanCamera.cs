using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NamNH.Forward {
	public class BooleanCamera : MonoBehaviour {
		[SerializeField] Material matCombine;
		[SerializeField] RenderTexture rtComposite;
		[SerializeField] RenderTexture mrtTex;
		private RenderBuffer mrtRB;
		private Camera cam;

		public RenderTargetIdentifier rtIdentifier;

		private void Start() {
			cam = GetComponent<Camera>();
			mrtTex = new RenderTexture((int)cam.pixelWidth, (int)cam.pixelHeight, 32, RenderTextureFormat.ARGBFloat);
			mrtTex.name = "Boolean_Camera";
			rtIdentifier = new RenderTargetIdentifier(mrtTex);
			mrtRB = mrtTex.colorBuffer;
			rtComposite = new RenderTexture((int)cam.pixelWidth, (int)cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat);
		}

		private void OnPreRender() {
			mrtRB = mrtTex.colorBuffer;
			cam.SetTargetBuffers(mrtRB, mrtTex.depthBuffer);
		}

		private void OnPostRender() {
			//draw to screen
			Graphics.SetRenderTarget(null);
			//setup material to draw
			matCombine.SetTexture("_MainTex", mrtTex);
			matCombine.SetPass(0);
			//draw a quad on screen
			DrawFullscreenQuad();
		}

		public static void DrawFullscreenQuad(float z = 1.0f) {
			GL.Begin(GL.QUADS);
			GL.Vertex3(-1.0f, -1.0f, z);
			GL.Vertex3(1.0f, -1.0f, z);
			GL.Vertex3(1.0f, 1.0f, z);
			GL.Vertex3(-1.0f, 1.0f, z);

			GL.Vertex3(-1.0f, 1.0f, z);
			GL.Vertex3(1.0f, 1.0f, z);
			GL.Vertex3(1.0f, -1.0f, z);
			GL.Vertex3(-1.0f, -1.0f, z);
			GL.End();
		}
	}
}