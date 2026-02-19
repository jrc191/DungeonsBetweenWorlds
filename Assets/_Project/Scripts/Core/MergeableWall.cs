using UnityEngine;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Marca una pared como fusionable (tiene una grieta).
    /// Expone la normal y ejes de movimiento de la superficie para que
    /// el PlayerController pueda mover al jugador a lo largo del muro.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MergeableWall : MonoBehaviour
    {
        [Header("Visual feedback")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.92f, 0.4f, 1f);
        [SerializeField] private Color normalColor    = Color.white;

        // Ejes de la superficie de la pared
        public Vector3 WallNormal => transform.forward;  // dirección que apunta hacia fuera
        public Vector3 WallRight  => transform.right;    // eje horizontal al moverse sobre la pared
        public Vector3 WallUp     => transform.up;       // eje vertical al moverse sobre la pared

        private Renderer wallRenderer;
        private MaterialPropertyBlock propBlock;

        private void Awake()
        {
            wallRenderer = GetComponentInChildren<Renderer>();
            propBlock    = new MaterialPropertyBlock();
        }

        /// <summary>Activa/desactiva el brillo de "pared cercana".</summary>
        public void SetHighlight(bool active)
        {
            if (wallRenderer == null) return;
            wallRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", active ? highlightColor : normalColor);
            wallRenderer.SetPropertyBlock(propBlock);
        }

        /// <summary>
        /// Devuelve la posición en la superficie de la pared más cercana al punto dado.
        /// El offset de 0.05 evita z-fighting con el mesh del muro.
        /// </summary>
        public Vector3 GetSurfacePosition(Vector3 worldPos)
        {
            Vector3 toPoint        = worldPos - transform.position;
            float   distAlongNormal = Vector3.Dot(toPoint, WallNormal);
            return worldPos - distAlongNormal * WallNormal + WallNormal * 0.05f;
        }

        /// <summary>Restringe una posición a los límites del collider de la pared.</summary>
        public Vector3 ClampToWallBounds(Vector3 worldPos)
        {
            Bounds bounds = GetComponent<Collider>().bounds;
            return new Vector3(
                Mathf.Clamp(worldPos.x, bounds.min.x + 0.1f, bounds.max.x - 0.1f),
                Mathf.Clamp(worldPos.y, bounds.min.y + 0.1f, bounds.max.y - 0.1f),
                Mathf.Clamp(worldPos.z, bounds.min.z + 0.1f, bounds.max.z - 0.1f)
            );
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
            Gizmos.DrawCube(transform.position, transform.localScale);
            // Flecha indicando la normal (dirección de entrada del jugador)
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * 1.2f);
        }
#endif
    }
}
