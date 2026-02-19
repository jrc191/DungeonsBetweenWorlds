using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Player
{
    /// <summary>
    /// Controlador de jugador con mecánica de fusión con paredes (inspirada en Zelda: ALBW).
    ///
    /// Estado NORMAL:  movimiento top-down isométrico en plano XZ (WASD).
    /// Estado MERGED:  jugador aplastado sobre la superficie de la pared,
    ///                 movimiento a lo largo de los ejes de la pared (WASD).
    ///
    /// Q → intentar fusionarse / desfusionarse.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private KeyCode mergeKey = KeyCode.Q;

        [Header("Velocidades")]
        [SerializeField] private float normalSpeed = 5f;
        [SerializeField] private float mergedSpeed = 3f;

        [Header("Rango de detección de pared")]
        [SerializeField] private float mergeDetectionRadius = 1.5f;

        [Header("Visual — modo pintura")]
        [SerializeField] private float  mergedScaleZ  = 0.05f;
        [SerializeField] private Color  mergedTint    = new Color(0.55f, 0.78f, 1f, 0.85f);

        private Rigidbody rb;
        private Renderer  playerRenderer;
        private Vector3   originalScale;
        private Color     originalColor;
        private MergeableWall lastHighlightedWall;

        private void Awake()
        {
            rb             = GetComponent<Rigidbody>();
            playerRenderer = GetComponentInChildren<Renderer>();
            originalScale  = transform.localScale;
            originalColor  = playerRenderer != null ? playerRenderer.material.color : Color.white;

            rb.useGravity  = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            if (MergeManager.Instance == null) return;

            HighlightNearbyWall();

            if (!Input.GetKeyDown(mergeKey)) return;

            if (MergeManager.Instance.CurrentState == MergeState.Normal)
            {
                MergeableWall wall = FindNearestWall();
                if (wall != null) Merge(wall);
            }
            else
            {
                Unmerge();
            }
        }

        private void FixedUpdate()
        {
            if (MergeManager.Instance == null) return;

            if (MergeManager.Instance.CurrentState == MergeState.Normal)
                MoveNormal();
            else
                MoveMerged();
        }

        // ── Movimiento ────────────────────────────────────────────────────────

        private void MoveNormal()
        {
            float   h   = Input.GetAxisRaw("Horizontal");
            float   v   = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(h, 0f, v).normalized;
            rb.MovePosition(rb.position + dir * normalSpeed * Time.fixedDeltaTime);
        }

        private void MoveMerged()
        {
            MergeableWall wall = MergeManager.Instance.CurrentWall;
            if (wall == null) return;

            float   h   = Input.GetAxisRaw("Horizontal");
            float   v   = Input.GetAxisRaw("Vertical");
            Vector3 dir = (wall.WallRight * h + wall.WallUp * v).normalized;

            Vector3 newPos = rb.position + dir * mergedSpeed * Time.fixedDeltaTime;
            newPos = wall.GetSurfacePosition(newPos);
            newPos = wall.ClampToWallBounds(newPos);

            rb.MovePosition(newPos);
        }

        // ── Merge / Unmerge ──────────────────────────────────────────────────

        private void Merge(MergeableWall wall)
        {
            // Fijar Rigidbody — el movimiento es manual sobre la superficie
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.linearVelocity     = Vector3.zero;

            // Snap a la superficie de la pared
            rb.position = wall.GetSurfacePosition(transform.position);

            // Visual: aplastar y teñir
            transform.localScale = new Vector3(originalScale.x, originalScale.y, mergedScaleZ);
            if (playerRenderer != null)
                playerRenderer.material.color = mergedTint;

            wall.SetHighlight(false);
            MergeManager.Instance.MergeIntoWall(wall);
        }

        private void Unmerge()
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            // Restaurar visual
            transform.localScale = originalScale;
            if (playerRenderer != null)
                playerRenderer.material.color = originalColor;

            MergeManager.Instance.Unmerge();
        }

        // ── Detección de pared ───────────────────────────────────────────────

        private MergeableWall FindNearestWall()
        {
            Collider[]   hits     = Physics.OverlapSphere(transform.position, mergeDetectionRadius);
            MergeableWall nearest = null;
            float         minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                MergeableWall wall = hit.GetComponent<MergeableWall>();
                if (wall == null) continue;
                float dist = Vector3.Distance(transform.position, wall.transform.position);
                if (dist < minDist) { minDist = dist; nearest = wall; }
            }
            return nearest;
        }

        private void HighlightNearbyWall()
        {
            MergeableWall nearest = MergeManager.Instance.CurrentState == MergeState.Normal
                ? FindNearestWall()
                : null;

            if (nearest != lastHighlightedWall)
            {
                if (lastHighlightedWall != null) lastHighlightedWall.SetHighlight(false);
                if (nearest != null)             nearest.SetHighlight(true);
                lastHighlightedWall = nearest;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, mergeDetectionRadius);
        }
#endif
    }
}
