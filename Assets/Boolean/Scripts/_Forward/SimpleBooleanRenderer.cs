using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NamNH.Forward {
    public abstract class SimpleBooleanRenderer : MonoBehaviour {
        [SerializeField] protected Renderer receiver;
        [SerializeField] protected Shader m_shader_composite;
        [SerializeField] protected Material m_depth_material;

        protected Mesh m_quad;

        protected Material m_material_composite;

        private CommandBuffer commandBuffer;
        private HashSet<Camera> cameras = new HashSet<Camera>();
        private bool hasAdded;
        protected BooleanCamera booleanCamera;

        private void Start() {
            booleanCamera = FindObjectOfType<BooleanCamera>();
        }


        private void OnWillRenderObject() {
            var cam = Camera.current;
            if (!hasAdded) {
                hasAdded = true;

                if (m_material_composite == null) m_material_composite = new Material(m_shader_composite);
                if (m_quad == null) m_quad = Ist.MeshUtils.GenerateQuad();
                if (commandBuffer == null) commandBuffer = new CommandBuffer() { name = typeof(SimpleAndRenderer).ToString() };

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
            camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, command);
        }

        protected abstract void UpdateCommandBuffer(CommandBuffer command);
    }
}