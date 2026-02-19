using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// URP ScriptableRendererFeature que dibuja un contorno fantasmal sobre los objetos
    /// que pertenecen a la dimensión opuesta a la activa. Añadir en el URP Renderer Asset.
    /// </summary>
    public class DimensionalOutlineFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class OutlineSettings
        {
            [Tooltip("Material con shader de outline (URP Unlit + color semitransparente)")]
            public Material outlineMaterial;

            [ColorUsage(true, true)]
            [Tooltip("Color HDR del contorno fantasmal")]
            public Color outlineColor = new Color(0.4f, 0.8f, 1f, 0.35f);

            public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public OutlineSettings settings = new OutlineSettings();

        private DimensionalOutlinePass outlinePass;

        public override void Create()
        {
            outlinePass = new DimensionalOutlinePass(settings)
            {
                renderPassEvent = settings.passEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.outlineMaterial == null) return;
            renderer.EnqueuePass(outlinePass);
        }
    }
}
