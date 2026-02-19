using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Marca este GameObject como exclusivo de un estado de fusión concreto.
    /// Visible + colisionable solo cuando el jugador está en el estado indicado.
    /// Ejemplos: zonas secretas solo accesibles siendo pintura, objetos solo visibles en modo normal.
    /// </summary>
    public class DimensionalObject : MonoBehaviour
    {
        [Header("Estado requerido")]
        [Tooltip("Este objeto solo será visible en este estado de fusión")]
        [SerializeField] private MergeState visibleInState = MergeState.Normal;

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
            MergeState initial = MergeManager.Instance != null
                ? MergeManager.Instance.CurrentState
                : MergeState.Normal;

            ApplyVisibility(initial);
        }

        private void OnEnable()  => MergeManager.OnMergeStateChanged += ApplyVisibility;
        private void OnDisable() => MergeManager.OnMergeStateChanged -= ApplyVisibility;

        private void ApplyVisibility(MergeState state)
        {
            bool visible = (state == visibleInState);
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
