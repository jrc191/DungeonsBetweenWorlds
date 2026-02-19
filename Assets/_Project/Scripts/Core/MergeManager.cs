using System;
using UnityEngine;

namespace DungeonsBetweenWorlds.Core
{
    public enum MergeState { Normal, Merged }

    /// <summary>
    /// Singleton que gestiona el estado de fusión del jugador (Normal / Merged).
    /// Equivalente a la mecánica de "convertirse en pintura" de Zelda: A Link Between Worlds.
    /// Otros sistemas se suscriben al evento OnMergeStateChanged para reaccionar.
    /// </summary>
    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Instance { get; private set; }

        public MergeState  CurrentState { get; private set; } = MergeState.Normal;
        public MergeableWall CurrentWall  { get; private set; }

        public static event Action<MergeState> OnMergeStateChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>Fusiona al jugador con la pared indicada.</summary>
        public void MergeIntoWall(MergeableWall wall)
        {
            if (CurrentState == MergeState.Merged) return;
            CurrentWall   = wall;
            CurrentState  = MergeState.Merged;
            OnMergeStateChanged?.Invoke(MergeState.Merged);
        }

        /// <summary>Devuelve al jugador a su forma normal.</summary>
        public void Unmerge()
        {
            if (CurrentState == MergeState.Normal) return;
            CurrentWall  = null;
            CurrentState = MergeState.Normal;
            OnMergeStateChanged?.Invoke(MergeState.Normal);
        }
    }
}
