using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NamNH.Deferred {
    [ExecuteInEditMode]
    public class SimpleSubRenderer : MonoBehaviour {
        [SerializeField] private Renderer receiver;
        [SerializeField] private Shader m_shader_composite;
        [SerializeField] private Material m_depth_material, m_mask_material;

        private Mesh m_quad;

        private Material m_material_composite;

        private CommandBuffer commandBuffer;
        private HashSet<Camera> cameras = new HashSet<Camera>();
        private bool hasAdded;

        private void OnWillRenderObject() {
            var cam = Camera.current;
            if (!hasAdded) {
                hasAdded = true;

                if (m_material_composite == null) m_material_composite = new Material(m_shader_composite);
                if (m_quad == null) m_quad = Ist.MeshUtils.GenerateQuad();
                if (commandBuffer == null) commandBuffer = new CommandBuffer() { name = typeof(SimpleSubRenderer).ToString() };

                UpdateCommandBuffer(commandBuffer);
            }
            if (cameras.Add(cam)) {
                AddCommandBuffer(cam, commandBuffer);
            }
        }

        private void OnRenderObject() {
            hasAdded = false;
        }

        private void AddCommandBuffer(Camera camera, CommandBuffer command) {
            camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, command);
        }

        private void UpdateCommandBuffer(CommandBuffer command) {
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



            command.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            command.SetGlobalTexture("_TmpDepth", id_tmpdepth);
            command.DrawMesh(m_quad, Matrix4x4.identity, m_material_composite);
        }

    }
}