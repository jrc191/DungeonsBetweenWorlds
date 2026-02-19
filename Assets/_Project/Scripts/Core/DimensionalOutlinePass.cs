using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Render Pass que dibuja los objetos de la dimensión contraria con un material
    /// de outline semitransparente — efecto "fantasma" de lo que existe en la otra dimensión.
    /// </summary>
    public class DimensionalOutlinePass : ScriptableRenderPass
    {
        private DimensionalOutlineFeature.OutlineSettings settings;

        private readonly List<ShaderTagId> shaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        private static readonly int OutlineColorId = Shader.PropertyToID("_BaseColor");

        public DimensionalOutlinePass(DimensionalOutlineFeature.OutlineSettings settings)
        {
            this.settings = settings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.outlineMaterial == null) return;
            if (MergeManager.Instance == null) return;

            // Resaltar objetos en layer MergeOnly (7) o NormalOnly (6) según estado
            int targetLayer = MergeManager.Instance.CurrentState == MergeState.Normal
                ? 1 << 7   // MergeOnly
                : 1 << 6;  // NormalOnly

            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all, targetLayer);

            // Aplicar color al material del outline
            settings.outlineMaterial.SetColor(OutlineColorId, settings.outlineColor);

            DrawingSettings drawingSettings = CreateDrawingSettings(
                shaderTagIds,
                ref renderingData,
                SortingCriteria.CommonTransparent
            );
            drawingSettings.overrideMaterial = settings.outlineMaterial;

            CommandBuffer cmd = CommandBufferPool.Get("DimensionalOutlinePass");
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
