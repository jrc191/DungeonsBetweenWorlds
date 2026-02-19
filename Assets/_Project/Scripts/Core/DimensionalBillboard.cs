using UnityEngine;
using DungeonsBetweenWorlds.Core;

namespace DungeonsBetweenWorlds.Core
{
    /// <summary>
    /// Hace que el sprite del jugador siempre mire a la cámara en estado Normal.
    /// En estado Merged se fija a una rotación estática (aplastado sobre la pared).
    /// </summary>
    public class DimensionalBillboard : MonoBehaviour
    {
        [Header("Billboard (estado Normal)")]
        [SerializeField] private bool lockYAxis = true;

        [Header("Rotación fija en estado Merged")]
        [SerializeField] private Vector3 rotationMerged = new Vector3(90f, 0f, 0f);

        private Camera mainCamera;
        private MergeState currentState = MergeState.Normal;

        private void Start()
        {
            mainCamera   = Camera.main;
            currentState = MergeManager.Instance != null
                ? MergeManager.Instance.CurrentState
                : MergeState.Normal;
        }

        private void OnEnable()  => MergeManager.OnMergeStateChanged += OnMergeStateChanged;
        private void OnDisable() => MergeManager.OnMergeStateChanged -= OnMergeStateChanged;

        private void LateUpdate()
        {
            if (currentState != MergeState.Normal || mainCamera == null) return;

            Vector3 dir = mainCamera.transform.position - transform.position;
            if (lockYAxis) dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(-dir);
        }

        private void OnMergeStateChanged(MergeState state)
        {
            currentState = state;
            if (state == MergeState.Merged)
                transform.rotation = Quaternion.Euler(rotationMerged);
        }
    }
}
