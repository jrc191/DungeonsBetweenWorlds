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
            if (DimensionalManager.Instance == null) return;

            // Determinar qué capa renderizar: la de la dimensión OPUESTA
            int oppositeLayer = DimensionalManager.Instance.CurrentDimension == Dimension.TwoD
                ? 1 << DimensionalManager.LAYER_3D_ONLY
                : 1 << DimensionalManager.LAYER_2D_ONLY;

            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all, oppositeLayer);

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
