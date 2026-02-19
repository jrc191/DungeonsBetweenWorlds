using UnityEngine;
using DungeonsBetweenWorlds.Core;
using DungeonsBetweenWorlds.Puzzle;

namespace DungeonsBetweenWorlds.Puzzle
{
    /// <summary>
    /// Puerta que se abre cuando el jugador tiene la llave y esta en el estado requerido.
    /// </summary>
    public class PuzzleDoor : MonoBehaviour
    {
        [Header("Estado requerido para abrir")]
        [SerializeField] private MergeState openInState = MergeState.Normal;

        [Header("Feedback de color")]
        [SerializeField] private Color lockedColor   = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color unlockedColor = new Color(0.3f, 1f, 0.3f, 1f);

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
            PuzzleKey.OnKeyCollected              += HandleKeyCollected;
            MergeManager.OnMergeStateChanged      += HandleStateChanged;
        }

        private void OnDisable()
        {
            PuzzleKey.OnKeyCollected              -= HandleKeyCollected;
            MergeManager.OnMergeStateChanged      -= HandleStateChanged;
        }

        private void Start()
        {
            if (PuzzleKey.IsCollected) { keyCollected = true; SetColor(unlockedColor); }
            TryOpen();
        }

        private void HandleKeyCollected()
        {
            keyCollected = true;
            SetColor(unlockedColor);
            Debug.Log("[PuzzleDoor] Llave detectada. Vuelve a modo Normal para abrir.");
            TryOpen();
        }

        private void HandleStateChanged(MergeState state) => TryOpen();

        private void TryOpen()
        {
            if (!keyCollected) return;
            if (MergeManager.Instance == null) return;
            if (MergeManager.Instance.CurrentState != openInState) return;
            Debug.Log("[PuzzleDoor] Puerta abierta!");
            foreach (var r in doorRenderers) r.enabled = false;
            foreach (var c in doorColliders) c.enabled = false;
        }

        private void SetColor(Color color)
        {
            foreach (var r in doorRenderers)
                if (r.material != null) r.material.color = color;
        }
    }
}
