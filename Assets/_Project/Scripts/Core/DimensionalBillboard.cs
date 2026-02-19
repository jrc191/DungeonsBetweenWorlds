using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Hace que el sprite del objeto siempre mire a la cámara en modo 3D (billboarding).
    /// En modo 2D se fija a una rotación estática configurable.
    /// Inspirado en MMBillboard (MoreMountains.Tools) pero integrado con el sistema dimensional.
    /// </summary>
    public class DimensionalBillboard : MonoBehaviour
    {
        [Header("Billboard 3D")]
        [Tooltip("Bloquea el eje Y para que el sprite no se incline — mantiene la verticalidad")]
        [SerializeField] private bool lockYAxis = true;

        [Header("Rotación fija en modo 2D")]
        [SerializeField] private Vector3 rotation2D = new Vector3(90f, 0f, 0f);

        private Camera mainCamera;
        private Dimension currentDimension = Dimension.TwoD;

        private void Start()
        {
            mainCamera = Camera.main;

            Dimension initial = DimensionalManager.Instance != null
                ? DimensionalManager.Instance.CurrentDimension
                : Dimension.TwoD;

            currentDimension = initial;
            ApplyStaticRotation(initial);
        }

        private void OnEnable()  => DimensionalManager.OnDimensionChanged += OnDimensionChanged;
        private void OnDisable() => DimensionalManager.OnDimensionChanged -= OnDimensionChanged;

        private void LateUpdate()
        {
            if (currentDimension != Dimension.ThreeD || mainCamera == null) return;

            // Dirección hacia la cámara
            Vector3 dirToCamera = mainCamera.transform.position - transform.position;

            // Opcional: bloquar el eje Y para que el sprite no se tumbe
            if (lockYAxis) dirToCamera.y = 0f;

            if (dirToCamera.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(-dirToCamera);
        }

        private void OnDimensionChanged(Dimension dimension)
        {
            currentDimension = dimension;
            if (dimension == Dimension.TwoD)
                ApplyStaticRotation(dimension);
        }

        private void ApplyStaticRotation(Dimension dimension)
        {
            if (dimension == Dimension.TwoD)
                transform.rotation = Quaternion.Euler(rotation2D);
        }
    }
}
