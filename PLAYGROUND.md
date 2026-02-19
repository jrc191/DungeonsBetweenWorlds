# PLAYGROUND — Mapa de Prueba con Puzzle Dimensional

**Rama:** `feature/playground-puzzle`  
**Escena:** `Assets/_Project/Scenes/` → crear `Playground.unity`

---

## Objetivo

Validar el sistema dimensional completo con un puzzle jugable:
el jugador debe cambiar de dimensión para recoger una llave y abrir una puerta.

---

## Scripts del Puzzle

### `Assets/_Project/Scripts/Systems/PuzzleKey.cs`
**Namespace:** `DungeonsBetweenWorlds.Puzzle`

- Ítem flotante recogible (animación de bobbing con Mathf.Sin)
- Solo puede recogerse cuando el jugador está en `requiredDimension` (por defecto: `ThreeD`)
- Al recogerlo dispara `PuzzleKey.OnKeyCollected` (evento estático)
- Se desactiva al recogerse (`gameObject.SetActive(false)`)
- `PuzzleKey.IsCollected` — bool estático consultable por cualquier sistema

### `Assets/_Project/Scripts/Systems/PuzzleDoor.cs`
**Namespace:** `DungeonsBetweenWorlds.Puzzle`

- Se suscribe a `PuzzleKey.OnKeyCollected` y `DimensionalManager.OnDimensionChanged`
- **Rojo** = bloqueada (sin llave) | **Verde** = llave recogida, cambia de dimensión para abrir
- Se abre (desactiva renderers + colliders) cuando:
  1. `PuzzleKey.IsCollected == true`
  2. `CurrentDimension == openInDimension` (por defecto: `TwoD`)

---

## Diseño del Mapa

```
┌─────────────────────────────────────────────────┐
│                                                 │
│  [SALA INICIO]      [SALA LLAVE]   [SALA FINAL] │
│                                                 │
│  ░░░░░░░░░░░░  ██  ░░░░░░░░░░░░   ░░░░░░░░░░░ │
│  ░  Player  ░  ██  ░   KEY(3D) ░   ░  GOAL   ░ │
│  ░░░░░░░░░░░░  ██  ░░░░░░░░░░░░   ░░░░░░░░░░░ │
│               WALL             DOOR             │
│               (2D only)        (2D, needs key)  │
│                                                 │
└─────────────────────────────────────────────────┘

██ = Muro que solo existe en 2D (DimensionalObject → TwoD)
KEY = Llave que solo existe en 3D (DimensionalObject → ThreeD)
DOOR = Puerta que se abre con llave en modo 2D (PuzzleDoor → TwoD)
```

### Flujo del Puzzle

```
1. Jugador en 2D → Muro bloquea el paso
        │
        ▼
2. Jugador pulsa Q → cambia a 3D → Muro desaparece
        │
        ▼
3. Jugador camina hasta la KEY → la recoge (solo en 3D)
   Console: "[PuzzleKey] Llave recogida!"
   La puerta cambia de ROJO → VERDE
        │
        ▼
4. Jugador pulsa Q → vuelve a 2D
        │
        ▼
5. Puerta detecta: llave ✓ + dimensión 2D ✓ → se abre
   Console: "[PuzzleDoor] Puerta abierta!"
        │
        ▼
6. Jugador llega a la Sala Final ✓
```

---

## Cómo Construir el Mapa en Unity

### Paso 1 — Crear la escena
1. `Assets/_Project/Scenes/` → click derecho → **Create → Scene** → nombre `Playground`
2. Doble click para abrirla
3. Añade los GameObjects de la escena anterior:
   - `_GameManager` → `DimensionalManager`
   - `Main Camera` → `DimensionalCameraController`

### Paso 2 — Suelo (Shared)
- Crea varios Planes o usa tiles del Tilemap con el asset Pixel Art
- **Layer:** `Shared`
- Sin `DimensionalObject` (visible siempre)

### Paso 3 — Muro (solo 2D)
```
GameObject: Wall_2D
├── MeshRenderer o SpriteRenderer
├── BoxCollider
├── Layer: 2D_Only
└── Componente: DimensionalObject → visibleInDimension: TwoD
```

### Paso 4 — Llave (solo 3D)
```
GameObject: Key_3D
├── MeshRenderer (Capsule o sprite de llave del asset)
├── BoxCollider (Is Trigger: ✅)
├── Layer: 3D_Only
├── Componente: DimensionalObject → visibleInDimension: ThreeD
└── Componente: PuzzleKey → requiredDimension: ThreeD
```
⚠️ El jugador debe tener el **Tag: "Player"** para que el trigger funcione.

### Paso 5 — Puerta (solo 2D, necesita llave)
```
GameObject: Door_2D
├── MeshRenderer (Cube o sprite de puerta)
├── BoxCollider (Is Trigger: ❌ — bloquea el paso)
├── Layer: 2D_Only
├── Componente: DimensionalObject → visibleInDimension: TwoD
└── Componente: PuzzleDoor → openInDimension: TwoD
```

### Paso 6 — Zona Final
```
GameObject: GoalZone
├── BoxCollider (Is Trigger: ✅)
└── Tag: "Finish" (para futuro GameManager)
```

### Paso 7 — Jugador
```
PF Player en la escena:
├── Tag: "Player"  ← OBLIGATORIO para PuzzleKey.OnTriggerEnter
├── Layer: Shared
├── Rigidbody: Dynamic, Gravity OFF
├── BoxCollider 3D
├── HybridPlayerController (Q para cambiar dimensión)
└── DimensionalBillboard (en el hijo Sprite)
```

---

## Checklist antes de Play

- [ ] Tag "Player" asignado al jugador
- [ ] Layers 2D_Only(6), 3D_Only(7), Shared(8) en Project Settings
- [ ] `_GameManager` con `DimensionalManager` en la escena
- [ ] `Main Camera` con `DimensionalCameraController`
- [ ] Key_3D tiene **Is Trigger: ✅** en el BoxCollider
- [ ] Door_2D tiene **Is Trigger: ❌** en el BoxCollider (debe bloquear físicamente)
- [ ] Active Input Handling: **Both** (Project Settings → Player)

---

## Uso de los Tiles del Asset Pixel Art Top Down - Basic

Para el suelo y paredes del mapa usando el Tilemap:

1. **Window → 2D → Tile Palette** → abre la paleta
2. Selecciona `TP Stone Ground` para el suelo
3. Selecciona `TP Wall` para las paredes
4. Pinta en el Tilemap de la escena

Los tiles del asset ya tienen la escala de 16 PPU configurada en Fase 1.

---

## Notas de Diseño

- La **llave flota** (animación bobbing) para ser fácilmente identificable
- La **puerta cambia de color** (rojo → verde) cuando la llave es recogida — feedback visual sin UI
- El **muro invisible en 3D** no desaparece del todo: el `DimensionalOutlineFeature` lo muestra con contorno fantasmal azul
