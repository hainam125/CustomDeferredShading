using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NamNH.Forward {
    public class SimpleSubRenderer : SimpleBooleanRenderer {
        [SerializeField] private Material m_mask_material;

        protected override void UpdateCommandBuffer(CommandBuffer command) {
            command.Clear();

            int id_backdepth = Shader.PropertyToID("BackDepth");
            command.GetTemporaryRT(id_backdepth, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_backdepth);
            command.ClearRenderTarget(true, true, Color.black, 0.0f);

            command.DrawRenderer(receiver, m_depth_material, 0, 0);
            command.SetGlobalTexture("_BackDepth", id_backdepth);



            var renderer = GetComponent<Renderer>();
            int id_tmpdepth = Shader.PropertyToID("TmpDepth");
            command.GetTemporaryRT(id_tmpdepth, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);
            command.SetRenderTarget(id_tmpdepth);
            command.ClearRenderTarget(true, true, Color.black, 1.0f);
            command.DrawRenderer(receiver, m_depth_material, 0, 1);

            command.DrawRenderer(renderer, m_mask_material, 0, 0);
            command.DrawRenderer(renderer, m_mask_material, 0, 1);
            command.DrawRenderer(renderer, m_mask_material, 0, 2);
            command.DrawRenderer(renderer, m_mask_material, 0, 3);



            command.SetRenderTarget(booleanCamera.rtIdentifier);
            command.SetGlobalTexture("_TmpDepth", id_tmpdepth);
            command.DrawMesh(m_quad, Matrix4x4.identity, m_material_composite);
        }
    }
}