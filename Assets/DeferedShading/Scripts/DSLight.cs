using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]
public class DSLight : MonoBehaviour {
	public static HashSet<DSLight> instances = new HashSet<DSLight>();

	[SerializeField] private bool castShadow = true;
	[SerializeField] private int shadowSteps = 10;
	[SerializeField] private Material mat;
	[SerializeField] private Mesh mesh;
	private Light lit;

	private void OnEnable() {
		instances.Add(this);
	}

	private void OnDisable() {
		instances.Remove(this);
	}

	private void Start() {
		lit = GetComponent<Light>();
	}

	public static void RenderLights(RenderTexture[] mrtTex) {
		foreach (DSLight l in instances) {
			l.mat.SetTexture("_NormalBuffer", mrtTex[0]);
			l.mat.SetTexture("_PositionBuffer", mrtTex[1]);
			l.mat.SetTexture("_ColorBuffer", mrtTex[2]);
			l.mat.SetTexture("_GlowBuffer", mrtTex[3]);

			Vector4 color = l.lit.color * l.lit.intensity;
			l.mat.SetVector("_LightColor", color);
			Vector4 shadow = Vector4.zero;
			shadow.x = l.castShadow ? 1.0f : 0.0f;
			shadow.y = (float)l.shadowSteps;
			l.mat.SetVector("_ShadowParams", shadow);
			if (l.lit.type == LightType.Point) {
				Matrix4x4 trans = Matrix4x4.TRS(l.transform.position, Quaternion.identity, Vector3.one);
				Vector4 range = Vector4.zero;
				range.x = l.lit.range;
				range.y = 1.0f / range.x;
				l.mat.SetVector("_LightPosition", l.transform.position);
				l.mat.SetVector("_LightRange", range);
				l.mat.SetPass(0);
				Graphics.DrawMeshNow(l.mesh, trans);
			}
			else if (l.lit.type == LightType.Directional) {
				l.mat.SetVector("_LightDir", l.transform.forward);
				l.mat.SetPass(0);
				DSCamera.DrawFullscreenQuad();
			}
		}
	}
}
