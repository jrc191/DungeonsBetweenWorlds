using System;
using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Puzzle
{
    /// <summary>
    /// Item recogible solo en un estado de fusion concreto (Normal o Merged).
    /// Nuevo puzzle: la llave esta en una zona solo accesible siendo pintura (Merged).
    /// </summary>
    public class PuzzleKey : MonoBehaviour
    {
        [Header("Estado requerido para recoger")]
        [SerializeField] private MergeState requiredState = MergeState.Merged;

        [Header("Feedback visual")]
        [SerializeField] private float bobSpeed  = 2f;
        [SerializeField] private float bobHeight = 0.2f;

        public static event Action OnKeyCollected;
        public static bool IsCollected { get; private set; }

        private Vector3 startPosition;

        private void Awake() { IsCollected = false; startPosition = transform.position; }

        private void Update()
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (MergeManager.Instance == null) return;
            if (MergeManager.Instance.CurrentState != requiredState)
            {
                Debug.Log($"[PuzzleKey] Necesitas estar en estado {requiredState} para recoger la llave.");
                return;
            }
            IsCollected = true;
            OnKeyCollected?.Invoke();
            Debug.Log("[PuzzleKey] Llave recogida!");
            gameObject.SetActive(false);
        }

        private void OnDestroy() => IsCollected = false;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
#endif
    }
}
