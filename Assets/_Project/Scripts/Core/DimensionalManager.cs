using System;
using UnityEngine;

namespace DungeonsBetweenWorlds.Core
{
    public enum Dimension { TwoD, ThreeD }

    /// <summary>
    /// Singleton central del sistema dimensional.
    /// Gestiona qué capas son visibles/colisionables según la dimensión activa.
    /// Layer 6 = 2D_Only | Layer 7 = 3D_Only | Layer 8 = Shared
    /// </summary>
    public class DimensionalManager : MonoBehaviour
    {
        public static DimensionalManager Instance { get; private set; }

        public const int LAYER_2D_ONLY = 6;
        public const int LAYER_3D_ONLY = 7;
        public const int LAYER_SHARED  = 8;

        [Header("Estado Inicial")]
        [SerializeField] private Dimension startDimension = Dimension.TwoD;

        public Dimension CurrentDimension { get; private set; }

        // Otros sistemas se suscriben aquí para reaccionar al cambio dimensional
        public static event Action<Dimension> OnDimensionChanged;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CurrentDimension = startDimension;
            ApplyDimension(startDimension);
        }

        /// <summary>Alterna entre 2D y 3D.</summary>
        public void SwitchDimension()
        {
            SetDimension(CurrentDimension == Dimension.TwoD ? Dimension.ThreeD : Dimension.TwoD);
        }

        /// <summary>Establece una dimensión concreta.</summary>
        public void SetDimension(Dimension dimension)
        {
            if (CurrentDimension == dimension) return;
            CurrentDimension = dimension;
            ApplyDimension(dimension);
            OnDimensionChanged?.Invoke(dimension);
        }

        private void ApplyDimension(Dimension dimension)
        {
            if (dimension == Dimension.TwoD)
            {
                // Objetos 3D_Only quedan fuera de colisión
                Physics.IgnoreLayerCollision(LAYER_2D_ONLY, LAYER_3D_ONLY, true);
                Physics.IgnoreLayerCollision(LAYER_SHARED,  LAYER_3D_ONLY, true);
                Physics.IgnoreLayerCollision(LAYER_SHARED,  LAYER_2D_ONLY, false);
            }
            else
            {
                // Objetos 2D_Only quedan fuera de colisión
                Physics.IgnoreLayerCollision(LAYER_3D_ONLY, LAYER_2D_ONLY, true);
                Physics.IgnoreLayerCollision(LAYER_SHARED,  LAYER_2D_ONLY, true);
                Physics.IgnoreLayerCollision(LAYER_SHARED,  LAYER_3D_ONLY, false);
            }

            UpdateCameraCulling(dimension);
        }

        private void UpdateCameraCulling(Dimension dimension)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            int all    = ~0;
            int only2D = 1 << LAYER_2D_ONLY;
            int only3D = 1 << LAYER_3D_ONLY;

            // Ocultar la capa exclusiva de la dimensión contraria
            mainCam.cullingMask = dimension == Dimension.TwoD
                ? all & ~only3D
                : all & ~only2D;
        }
    }
}
