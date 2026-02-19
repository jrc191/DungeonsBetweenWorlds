# Plan: Dungeons Between Worlds — TopDown Engine

## Contexto del Proyecto

**Juego:** "Dungeons Between Worlds" — dungeon explorer híbrido 2D/3D
**Motor base:** TopDown Engine v4.4 en `D:\UNITY_DEFINITIVO\My project`
**Referencia técnica:** PDF "Desarrollo de Juego Híbrido 2D_3D.pdf"

---

## Ventajas del TopDown Engine (qué ya existe)

| Sistema necesario | Lo que ya tiene el TopDown Engine |
|---|---|
| Movimiento grid 2D | `CharacterGridMovement.cs` (snapping a tiles) |
| Movimiento libre 3D | `TopDownController3D.cs` + `CharacterMovement.cs` |
| Gestión de capas | `LayerManager.cs` |
| Grid manager | `GridManager.cs` |
| Demo con grid 3D | `MinimalGrid3D` y `Minimal2DGrid` — escenas listas |
| Armas y combate | 50+ scripts de weapons listos |
| Input System | `InputManager.cs` + New Input System ya configurado |
| Cámara Cinemachine | Integración nativa |
| Game/Level managers | `GameManager.cs`, `LevelManager.cs` |

---

## Qué hay que construir encima

### Concepto central: Cambio Dimensional
El jugador puede alternar entre **Modo 2D** (cámara ortográfica top-down, movimiento por tiles) y **Modo 3D** (cámara perspectiva top-down ligeramente inclinada, movimiento libre). Esto abre puzzles donde objetos solo existen en una dimensión.

---

## Fases de Implementación

### Fase 1: Configurar Proyecto Base
- Estructura de carpetas `Assets/_Project/` (Art, Scripts, Prefabs, ScriptableObjects, Shaders)
- Color Space → Linear (Project Settings)
- Verificar Render Graph activo en URP
- Configurar Git LFS + .gitignore para Unity
- Importar assets: KayKit Dungeon Remastered (modular, grid-compatible)

### Fase 2: Sistema de Cámara y Movimiento Dual
- **DimensionalCameraController.cs**: interpola Camera.projectionMatrix entre ortográfica y perspectiva con `SmoothStep`. Integra Dolly Zoom para mantener tamaño aparente del personaje.
- **DimensionalManager.cs**: controla Layer culling (Layer 6=2D_Only, 7=3D_Only, 8=Shared) y Physics.IgnoreLayerCollision.
- **HybridPlayerController.cs**: wraper que activa `CharacterGridMovement` en modo 2D y `TopDownController3D` en modo 3D. Input via dos Action Maps (Exploration2D / Exploration3D).

### Fase 3: Renderizado Híbrido y Mecánicas
- Billboarding en sprites del personaje (siempre miran a la cámara)
- Shader URP/Lit para sprites + normal maps
- Render Feature personalizado: Outline Pass sobre objetos de la dimensión opuesta (efecto fantasmal)
- **DimensionalObject.cs**: componente para objetos que solo existen en una dimensión
- Sala de prueba: puente visible solo en 3D / muro solo en 2D
- Puzzle básico de cambio dimensional

### Fase 4: Backend Firebase
- Firebase SDK para Unity
- Autenticación anónima silenciosa
- Firestore schema:
  ```
  players/{userId}:
    stats: { playTime, dungeonsCompleted, enemiesDefeated }
    inventory: [itemIds]
    checkpoints: { lastScene, position{x,y,z}, dimension }
  leaderboards/{scoreId}:
    { username, score, timestamp }
  ```
- Guardado async/await en checkpoints (modo offline con caché local)
- UI de leaderboard top-20

---

## Escenas de Referencia del TopDown Engine a Usar

- `MinimalGrid3D` → base para modo 3D con grid
- `Minimal2DGrid` → base para modo 2D con tiles
- `KoalaDungeon` → referencia visual de dungeon 2D

---

## Archivos Clave a Crear

```
Assets/_Project/Scripts/Core/
  DimensionalManager.cs       ← gestiona el cambio dimensional
  DimensionalCameraController.cs ← transición de matrices
  DimensionalObject.cs        ← objetos por dimensión

Assets/_Project/Scripts/Player/
  HybridPlayerController.cs   ← wraper sobre TopDown Engine

Assets/_Project/Scripts/Systems/
  FirebaseSaveSystem.cs       ← async/await Firestore
  LeaderboardSystem.cs

Assets/_Project/Shaders/
  BillboardSprite.shadergraph
  DimensionalOutline.shadergraph
```

---

## Notas Técnicas

- **NO reescribir el TopDown Engine** — extender/heredar sus clases
- Usar `Character.cs` como base para el jugador híbrido
- Escena playground de prueba: colocar en `Assets/_Project/Scenes/Playground.unity`
- Referencia: `Assets/Project/Art/Script/DimensionalCamera.cs` en DBW_Definitivo como punto de partida para la transición de cámara
