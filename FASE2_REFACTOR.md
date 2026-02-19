# REFACTOR — Mecánica de Fusión con Pared (Zelda: ALBW)

**Rama:** `feature/merge-mechanic`  
**Sustituye:** Sistema dimensional 2D/3D (Fases 2 y 3)

---

## Motivación del Cambio

El sistema de cambio dimensional (ortho ↔ perspectiva) se reemplaza por una mecánica  
más enfocada y visualmente clara: **el jugador puede fusionarse con paredes marcadas**  
(con una grieta), convirtiéndose en una "pintura" que se desliza por su superficie.

Inspiración directa: *The Legend of Zelda: A Link Between Worlds* (Nintendo, 2013).

---

## Arquitectura Nueva

```
MergeManager (Singleton)
    │
    ├── event OnMergeStateChanged ──────┬────────────────────────────┐
    │                                   │                            │
    ▼                                   ▼                            ▼
PlayerController                 DimensionalObject           DimensionalBillboard
(movimiento normal/merged)       (visible por estado)        (billboard en Normal)
        │
        ▼
MergeableWall
(superficie, normal, bounds)
```

---

## Scripts Nuevos / Modificados

### ✅ NUEVO — `Scripts/Core/MergeManager.cs`
- Singleton con `DontDestroyOnLoad`
- Estados: `MergeState.Normal` | `MergeState.Merged`
- Métodos: `MergeIntoWall(wall)`, `Unmerge()`
- Evento estático: `OnMergeStateChanged`
- Referencia a `CurrentWall` (la pared activa)

### ✅ NUEVO — `Scripts/Core/MergeableWall.cs`
- Componente para paredes con grieta
- Expone: `WallNormal`, `WallRight`, `WallUp`
- `GetSurfacePosition(worldPos)` — posición en la superficie de la pared
- `ClampToWallBounds(worldPos)` — evita salirse del muro
- `SetHighlight(bool)` — feedback visual amarillo al acercarse

### ✅ NUEVO — `Scripts/Player/PlayerController.cs`
- Reemplaza `HybridPlayerController.cs`
- **Estado Normal:** WASD en plano XZ (movimiento top-down isométrico)
- **Estado Merged:** WASD a lo largo de `WallRight` y `WallUp`
- Q cerca de una `MergeableWall` → fusionarse (visual: aplastar + tinte azul)
- Q mientras merged → desfusionarse (restaurar escala y color)
- `Physics.OverlapSphere` para detectar paredes cercanas (radio: 1.5u)
- Gizmo amarillo en Editor mostrando el radio de detección

### 🔄 MODIFICADO — `Scripts/Core/DimensionalObject.cs`
- Ahora usa `MergeState` en lugar de `Dimension`
- `visibleInState`: Normal o Merged
- Suscrito a `MergeManager.OnMergeStateChanged`

### 🔄 MODIFICADO — `Scripts/Core/DimensionalBillboard.cs`
- Suscrito a `MergeManager.OnMergeStateChanged`
- Billboard activo en estado `Normal`
- Rotación fija en estado `Merged`

### 🔄 MODIFICADO — `Scripts/Systems/PuzzleKey.cs`
- `requiredState: MergeState.Merged` — la llave se recoge siendo pintura
- Usa `MergeManager.Instance.CurrentState`

### 🔄 MODIFICADO — `Scripts/Systems/PuzzleDoor.cs`
- `openInState: MergeState.Normal` — la puerta se abre en modo normal con llave
- Suscrito a `MergeManager.OnMergeStateChanged`

### ❌ ELIMINADOS
- `DimensionalManager.cs` — reemplazado por `MergeManager`
- `DimensionalCameraController.cs` — cámara ahora es fija isométrica
- `HybridPlayerController.cs` — reemplazado por `PlayerController`

---

## Configuración de Cámara (Isométrica tipo ALBW)

En Unity, selecciona `Main Camera` → Inspector:

| Campo | Valor |
|---|---|
| Position | `(0, 10, -7)` |
| Rotation | `(55, 0, 0)` |
| Projection | `Orthographic` |
| Size | `5` |

Añade `CameraFollow` (del asset Pixel Art) con Target = jugador y Offset = `(0, 10, -7)`.

---

## Diseño del Puzzle con la Nueva Mecánica

```
[Sala Inicio] ──→ [Pared con GRIETA] ──→ (siendo pintura) ──→ [Zona secreta con LLAVE]
                                                                        │
                                                                        ▼
                                                              Recoge llave (estado Merged)
                                                                        │
                                                                        ▼
                                                              Desfusiónate (Q)
                                                                        │
                                                                        ▼
                                                     [PUERTA roja → verde → desaparece]
                                                                        │
                                                                        ▼
                                                                  [Sala Final]
```

---

## Cómo Crear una Pared con Grieta en Unity

1. Crea un Cube → nómbralo `Wall_Mergeable`
2. Colócalo verticalmente (Scale Y alta, Z delgado, ej: `(4, 3, 0.3)`)
3. Añade componente `MergeableWall`
4. **Importante:** el `transform.forward` debe apuntar hacia el jugador
   (rota el cubo si es necesario, o usa el Gizmo de flecha cyan para verificar)
5. Pon una textura de pared con grieta (modifica `TX Tileset Wall` en Photoshop/Aseprite)
6. Layer: `Shared` (siempre visible)

---

## Capas (Layers) — Repropósito

| Layer | Índice | Nuevo uso |
|---|---|---|
| `2D_Only` | 6 | `NormalOnly` — objetos solo visibles en estado Normal |
| `3D_Only` | 7 | `MergeOnly` — objetos solo visibles siendo pintura |
| `Shared` | 8 | Sin cambio — siempre visibles |
| `MergeOnly` | 9 | Zonas de pasillo en la pared (física solo al estar merged) |
