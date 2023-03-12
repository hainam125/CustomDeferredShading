using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DSCamera : MonoBehaviour {
	[SerializeField] Material matCombine;
	[SerializeField] RenderTexture rtComposite;
	[SerializeField] RenderTexture[] mrtTex = new RenderTexture[4];
	private RenderBuffer[] mrtRB = new RenderBuffer[4];
	private Camera cam;

	private void Start() {
		cam = GetComponent<Camera>();
		for (int i = 0; i < mrtTex.Length; ++i) {
			var depth = i == 0 ? 32 : 0;
			mrtTex[i] = new RenderTexture((int)cam.pixelWidth, (int)cam.pixelHeight, depth, RenderTextureFormat.ARGBFloat);
			mrtRB[i] = mrtTex[i].colorBuffer;
		}
		rtComposite = new RenderTexture((int)cam.pixelWidth, (int)cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat);
	}

	private void OnPreRender() {
		for (int i = 0; i < mrtTex.Length; ++i) {
			mrtRB[i] = mrtTex[i].colorBuffer;
		}
		cam.SetTargetBuffers(mrtRB, mrtTex[0].depthBuffer);
	}

	private void OnPostRender() {
		//clear rtComposite
		Graphics.SetRenderTarget(rtComposite);
		GL.Clear(true, true, Color.black);

		//setup current target to draw (with back color and depth of mrtTex[0])
		//depth is used to compare in point light
		Graphics.SetRenderTarget(rtComposite.colorBuffer, mrtTex[0].depthBuffer);

		//draw light
		DSLight.RenderLights(mrtTex);

		//draw to screen
		Graphics.SetRenderTarget(null);
		//setup material to draw
		matCombine.SetTexture("_MainTex", rtComposite);
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
