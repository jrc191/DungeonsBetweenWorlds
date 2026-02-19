using System;
using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Puzzle
{
    /// <summary>
    /// Ítem recogible que solo puede recogerse en una dimensión concreta.
    /// Al recogerlo dispara el evento estático OnKeyCollected para que
    /// PuzzleDoor (y cualquier otro sistema) reaccione.
    /// </summary>
    public class PuzzleKey : MonoBehaviour
    {
        [Header("Dimensión requerida para recoger")]
        [SerializeField] private Dimension requiredDimension = Dimension.ThreeD;

        [Header("Feedback visual")]
        [SerializeField] private float bobSpeed    = 2f;
        [SerializeField] private float bobHeight   = 0.2f;

        // Cualquier sistema puede suscribirse para saber que la llave fue recogida
        public static event Action OnKeyCollected;

        public static bool IsCollected { get; private set; }

        private Vector3 startPosition;

        private void Awake()
        {
            IsCollected   = false;
            startPosition = transform.position;
        }

        private void Update()
        {
            // Animación de flotado para que sea visualmente identificable
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (DimensionalManager.Instance == null) return;

            // Solo puede recogerse en la dimensión correcta
            if (DimensionalManager.Instance.CurrentDimension != requiredDimension)
            {
                Debug.Log($"[PuzzleKey] Necesitas estar en dimension {requiredDimension} para recoger la llave.");
                return;
            }

            Collect();
        }

        private void Collect()
        {
            IsCollected = true;
            OnKeyCollected?.Invoke();
            Debug.Log("[PuzzleKey] Llave recogida!");
            gameObject.SetActive(false);
        }

        // Resetea el estado al descargar la escena
        private void OnDestroy()
        {
            IsCollected = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
#endif
    }
}
