using UnityEngine;
using DungeonsBetweenWorlds.Core;
using DungeonsBetweenWorlds.Puzzle;

namespace DungeonsBetweenWorlds.Puzzle
{
    /// <summary>
    /// Puerta bloqueada que se abre cuando:
    ///   1. El jugador tiene la llave (PuzzleKey.IsCollected == true)
    ///   2. El jugador está en la dimensión requerida
    /// Cuando ambas condiciones se cumplen, desactiva renderers y colliders (puerta abierta).
    /// </summary>
    public class PuzzleDoor : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private Dimension openInDimension = Dimension.TwoD;

        [Header("Feedback — color cuando falta la llave")]
        [SerializeField] private Color lockedColor   = new Color(1f, 0.3f, 0.3f, 1f); // Rojo
        [SerializeField] private Color unlockedColor = new Color(0.3f, 1f, 0.3f, 1f); // Verde

        private bool keyCollected = false;
        private Renderer[] doorRenderers;
        private Collider[]  doorColliders;

        private void Awake()
        {
            doorRenderers = GetComponentsInChildren<Renderer>();
            doorColliders = GetComponentsInChildren<Collider>();
            SetColor(lockedColor);
        }

        private void OnEnable()
        {
            PuzzleKey.OnKeyCollected               += HandleKeyCollected;
            DimensionalManager.OnDimensionChanged  += HandleDimensionChanged;
        }

        private void OnDisable()
        {
            PuzzleKey.OnKeyCollected               -= HandleKeyCollected;
            DimensionalManager.OnDimensionChanged  -= HandleDimensionChanged;
        }

        private void Start()
        {
            // Por si la llave ya estaba recogida antes de cargar la puerta
            if (PuzzleKey.IsCollected)
            {
                keyCollected = true;
                SetColor(unlockedColor);
            }
            TryOpen();
        }

        private void HandleKeyCollected()
        {
            keyCollected = true;
            SetColor(unlockedColor);
            Debug.Log("[PuzzleDoor] Llave detectada. Cambia a la dimension correcta para abrir.");
            TryOpen();
        }

        private void HandleDimensionChanged(Dimension dimension)
        {
            TryOpen();
        }

        private void TryOpen()
        {
            if (!keyCollected) return;
            if (DimensionalManager.Instance == null) return;
            if (DimensionalManager.Instance.CurrentDimension != openInDimension) return;

            OpenDoor();
        }

        private void OpenDoor()
        {
            Debug.Log("[PuzzleDoor] Puerta abierta!");
            foreach (var r in doorRenderers) r.enabled = false;
            foreach (var c in doorColliders) c.enabled = false;
        }

        private void SetColor(Color color)
        {
            foreach (var r in doorRenderers)
            {
                if (r.material != null)
                    r.material.color = color;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
#endif
    }
}
