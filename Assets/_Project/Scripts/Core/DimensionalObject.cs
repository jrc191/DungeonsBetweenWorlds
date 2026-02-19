using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Marca este GameObject como exclusivo de una dimensión concreta.
    /// Se activa (visible + colisionable) solo cuando la dimensión activa coincide.
    /// Añadir este componente a cualquier objeto del escenario que deba existir solo en 2D o solo en 3D.
    /// </summary>
    public class DimensionalObject : MonoBehaviour
    {
        [Header("Dimensión")]
        [Tooltip("Este objeto solo será visible y colisionable en esta dimensión")]
        [SerializeField] private Dimension visibleInDimension = Dimension.TwoD;

        [Tooltip("Si true, afecta también a todos los hijos del GameObject")]
        [SerializeField] private bool affectChildren = true;

        private Renderer[] renderers;
        private Collider[]  colliders;

        private void Awake()
        {
            renderers = affectChildren
                ? GetComponentsInChildren<Renderer>(includeInactive: true)
                : GetComponents<Renderer>();

            colliders = affectChildren
                ? GetComponentsInChildren<Collider>(includeInactive: true)
                : GetComponents<Collider>();
        }

        private void Start()
        {
            Dimension initial = DimensionalManager.Instance != null
                ? DimensionalManager.Instance.CurrentDimension
                : Dimension.TwoD;

            ApplyVisibility(initial);
        }

        private void OnEnable()  => DimensionalManager.OnDimensionChanged += ApplyVisibility;
        private void OnDisable() => DimensionalManager.OnDimensionChanged -= ApplyVisibility;

        private void ApplyVisibility(Dimension dimension)
        {
            bool visible = (dimension == visibleInDimension);
            foreach (var r in renderers) r.enabled = visible;
            foreach (var c in colliders) c.enabled = visible;
        }

#if UNITY_EDITOR
        // Gizmo para identificar visualmente la dimensión en el editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = visibleInDimension == Dimension.TwoD
                ? new Color(0.2f, 0.6f, 1f, 0.5f)   // Azul = 2D
                : new Color(1f,   0.4f, 0.1f, 0.5f); // Naranja = 3D

            Bounds bounds = new Bounds(transform.position, Vector3.one);
            foreach (var r in GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(r.bounds);

            Gizmos.DrawWireCube(bounds.center, bounds.size * 1.05f);
        }
#endif
    }
}
