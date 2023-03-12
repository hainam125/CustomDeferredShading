using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NamNH.Forward {
    public class SimpleAndRenderer : SimpleBooleanRenderer {
        protected override void UpdateCommandBuffer(CommandBuffer command) {
            var renderer = GetComponent<Renderer>();
            command.Clear();

            int id_backdepth = Shader.PropertyToID("BackDepth");
            int id_frontdepth = Shader.PropertyToID("TmpDepth"); // reuse SubtractionRenderer's buffer
            int id_backdepth2 = Shader.PropertyToID("BackDepth2");
            int id_frontdepth2 = Shader.PropertyToID("TmpDepth2");

            // back depth - receivers
            command.GetTemporaryRT(id_backdepth, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_backdepth);
            command.ClearRenderTarget(true, true, Color.black, 0.0f);
            command.DrawRenderer(receiver, m_depth_material, 0, 0);//back depth -> clear at 0
            command.SetGlobalTexture("_BackDepth", id_backdepth);

            // back depth - operators
            command.GetTemporaryRT(id_backdepth2, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_backdepth2);
            command.ClearRenderTarget(true, true, Color.black, 0.0f);
            command.DrawRenderer(renderer, m_depth_material, 0, 0);
            command.SetGlobalTexture("_BackDepth2", id_backdepth2);


            // front depth - receivers
            command.GetTemporaryRT(id_frontdepth, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_frontdepth);
            command.ClearRenderTarget(true, true, Color.black, 1.0f);//front depth -> clear at 1
            command.DrawRenderer(receiver, m_depth_material, 0, 1);
            command.SetGlobalTexture("_FrontDepth", id_frontdepth);

            // front depth - operators
            command.GetTemporaryRT(id_frontdepth2, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_frontdepth2);
            command.ClearRenderTarget(true, true, Color.black, 1.0f);
            command.DrawRenderer(renderer, m_depth_material, 0, 1);
            command.SetGlobalTexture("_FrontDepth2", id_frontdepth2);


            // output depth
            command.SetRenderTarget(booleanCamera.rtIdentifier);
            command.DrawMesh(m_quad, Matrix4x4.identity, m_material_composite);
        }

    }
}
