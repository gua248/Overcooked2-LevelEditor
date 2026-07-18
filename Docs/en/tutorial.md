## Overcooked! 2 Level Editor Tutorial

#### Setup

1. Download or clone the project。

2. Copy the `Overcooked! 2/Overcooked2_Data/StreamingAssets/Windows` folder from the game directory into the project's `Assets/StreamingAssets` folder.

3. Decompile the game code and place it into `Assets/Scripts`:

  > 1. Download AssetRipper from https://github.com/assetripper/assetripper.
  > 2. Open AssetRipper, File - Settings - check Skip StreamingAssets Folder - click Save at the bottom.
  > 3. File - Open Folder - select the `Overcooked! 2/Overcooked2_Data` folder from the game directory.
  > 4. Export - Export All Files - check Create Subfolder - Select Folder - choose your export directory - Export Unity Project. Wait for the export to complete.
  > 5. Copy the `ExportedProject/Assets/Scripts/Assembly-CSharp` folder from your export directory into the project's `Assets/Scripts` folder.
  > 6. Copy all contents from the project's `Assembly-CSharp-Patch` folder into the `Assets/Scripts/Assembly-CSharp` folder, replace the existing files.

4. Open the project in Unity. The editor version is 2017.4.8f1.

5. Check the Console panel and fix any remaining compilation errors (there should not be any).

6. Open the scene `LevelSets/oc1_story/scenes/s_oc1_story_1_1` (or any other level scene). Click Play to run the level (controls: `W` `A` `S` `D` + `space` + `G` `H` `J`).



#### Creating New Levels and Level Sets

Assume you want to create a new level set named `xxx`, which contains a single level named `xxx-1`.

1. Create a new folder named `xxx/` in the `LevelSets` directory, as well as `xxx/data/xxx_1/`, `xxx/data/xxx_2/` and `xxx/scenes/`. See `LevelSets/test` for reference.
2. Right-click the `xxx/data/xxx_1` directory and select Create – LevelEditor – LevelConfigSetupPerPlayerCountSO to create a single-player level configuration. Name it `config_1p` and fill in the data. Follow the same procedure to create configurations for 2, 3 and 4 players. [LevelConfigSetupPerPlayerCountSO reference](reference.md#LevelConfigSetupPerPlayerCountSO)
3. Right-click the `xxx/data/xxx_1` directory and select Create – LevelEditor – LevelInfoSO to create a level configuration. Name it `LevelInfo_xxx_1` and fill in the data. [LevelInfoSO reference](reference.md#LevelInfoSO)
4. Right-click the `xxx/data` directory and select Create – LevelEditor – LevelSetInfoSO to create a level set configuration. __Name it `LevelSetInfo` (this name cannot be changed)__ and fill in the data. [LevelSetInfoSO reference](reference.md#LevelSetInfoSO)
5. For multiple levels, follow the same procedure and simply add all `LevelInfo` to `LevelSetInfo.levelInfos`.
6. Duplicate the template scene `Template/s_template` to the `xxx/scenes` directory and rename it to `s_xxx_1`.
7. Open the scene `s_xxx_1` and __modify the `PseudoPrefabManagerStub.levelInfo` reference on the root object `PseudoPrefabManager` to the newly created level configuration `LevelInfo_xxx_1`__. Click Tools – Reload Pseudo Assets to load the level configuration.
8. [Edit the scene](#Editing-the-Scene). Click Play to run the level.
9. __Click Tools – Toggle Prepare For Building to clear temporary objects, then__ save the scene.



#### Building and Exporting Levels

Assume that the level set `xxx` contains a single level, `xxx-1`.

1. In the Project panel, select the scene file `s_xxx_1`. At the very bottom of the Inspector panel, select the first dropdown menu beside AssetBundle, choose New..., and enter `xxx/s_xxx_1`.
2. In the Project panel, select the level set root directory `xxx`. At the very bottom of the Inspector panel, select the first dropdown menu beside AssetBundle, choose New..., and enter `xxx/info_xxx`.
3. Click Tools – Build AssetBundles. (The first build may take some time.)
4. You will see two files, `info_xxx` and `s_xxx_1`, in the project‘s folder `Assets/AssetBundles/xxx`. Copy them to the game directory `Overcooked! 2/BepInEx/plugins/OC2DIYLevel/levels/xxx`, and you can play the level in the game.
5. You can now publish your level!



#### Editing the Scene

##### General Instructions

- Most gameobjects have a placeholder in the scene, and the actual object is temporarily loaded from the game's original bundles in the editor and the game. Click Tools – Toggle Prepare For Building to load or clear temporary objects. For example, the placeholder for Chef 1 is `Chefs/Player 1`, and the loaded temporary object is `Chefs/Player 1/player`.
- __Clear temporary objects when saving and building the scene.__
- __Do not modify temporary objects.__ Clicking on them in the Scene view may select the temporary object (or its child objects); you need to select the corresponding placeholder object in the Hierarchy view to perform actions such as moving or rotating.
- __Do not place objects in the scene root.__ All objects must be placed under a parent object, such as `Design/Counters` in the template, or you may create an empty object to act as the parent.
- Drag objects from `common01/prefabs` in the Project panel into the scene or the Hierarchy.
- To align objects to the grid, hold down Ctrl while moving them. The game's grid size is 1.2.
- If temporary objects are not loaded correctly or if you have accidentally modified them, click Tools – Reload Pseudo Assets to reload them.

##### Gameplay Objects

- Counters

  In the directory `common01/prefabs/counters`.
  
  [Counters reference](reference.md#Counters)
  
- Utensils

  In the directory `common01/prefabs/utensils`.
  
  Place them above the corresponding counter; it will automatically attach to the counter at the start of the level.
  
  [Utensils reference](reference.md#Utensils)
  
- Mechanisms

  In the directory `common*/prefabs/mechanisms`.
  
  [Mechanisms reference](reference.md#Mechanisms)
  
- Player

  `common01/prefabs/Player`。

  [Player reference](reference.md#Player)

##### Environment Objects

In the directory `common*/prefabs/art`, organized into subdirectories by theme.

[Environment Objects reference](reference.md#Environment-Objects)

##### Camera

- Post Processing

  Add a post-processing profile to the `MultiplayerGameCamera/Camera > PostProcessingBehaviour > profile` field. The preset post-processing profiles are located in `common01/post_processing`; you can also create a new post-processing profile in your level set directory.

- `MultiplayerGameCamera`

  Adjust the `Transform` of the `MultiplayerGameCamera` object to the appropriate position and rotation (check the Game view), while setting the parameters of its `MultiplayerCamera` component, so that the main part of the scene remains within the Game view regardless of how the players move.

  [MultiplayerCamera reference](reference.md#MultiplayerCamera)

##### Lighting

- Lighting Settings

  Set up Skybox and Environment Lighting in the Environment section under Window - Lighting - Settings. See the scenes in the `test` and `oc1_story` level sets for reference. Leave Realtime Global Illumination and Baked Global Illumination unchecked.

- Adjust or add lights under `Art/Lights` in the scene.

- Currently, custom levels use fully real-time lighting. A single object can receive a maximum of 4 real-time lights; exceeding this limit will cause rendering artifacts, such as discontinuous brightness changes. Some environment objects contain lights; please keep this limit in mind.

##### Music and Sound Effects

On the `PseudoPrefabManager > PseudoPrefabManagerStub` component:

- The `inLevelMusicSO` field is used to set the level music. The assets are located in the `common*/pseudo_prefab_so/audio/music` directory. __Many BGMs use bundles that are not loaded by the game by default, so you need to add the BGM's bundle to `LevelInfoSO.dependencies`.__

- The `inlevelAmbiences` field is used to add level ambient sound effect tags. See the scenes in the `test` and `oc1_story` level sets for reference.

- The `audioDirectorySOs` field is used to add sound effect directories. The assets are located in the `common02/pseudo_prefab_so/audio/AudioDirectories` directory. If an ambient sound effect tag or certain environment objects use a tag not listed in `audioDirectorySOs`, you will see the following error in the Console panel:

  ```
  ArgumentOutOfRangeException: Argument is out of range.
  Parameter name: index
  System.Collections.Generic.List`1[AudioDirectoryEntry].get_Item (Int32 index) ...
  ```

  Generally, you should add the sound effect directories corresponding to the theme of the environment objects, or it's fine to add a few extra ones as well.

##### Other Settings

- Ceiling Height

  The `ceilingHeight` field in `CampaignGameEnvironment/KitchenLoaderManager` is the ceiling height. The default value is 2, which corresponds to the height of the chef's collider. If the level is not entirely flat, this value should be increased appropriately. For example, if there is a platform that raises the height by 1, it should be set to at least 3.

- Dynamic Parenting

  The `levelinfo.disabledynamicparenting` field is the dynamic parenting option. It is checked by default. In levels containing moving or elevating platforms, this option should be unchecked.

- `GridManager`

  The `CampaignGameEnvironment/GridManager > QuadGridManager` component manages objects that are aligned to the grid such as counters (the game's grid size is 1.2). Set the `size` field to `(1.2, 1, 1.2)`, and the `origin` field to 0. The `gridHalfSize` field represents the number of grid cells for the half-width/half-length of the grid. The grid centers on the position of the `GridManager` object and extends `gridHalfSize` cells in each direction (for example, if `gridHalfSize` is `(1, 0, 1)`, the grid size is `3x1x3`). You should keep `gridHalfSize` as small as possible while ensuring the grid covers all counters. First, move the `GridManager` object to the grid cell at the center of all counters, then set `gridHalfSize` so that the grid covers all counters.

- Collision

  Add colliders under `Design/Collision`. __Please note that the colliders must be assigned to the correct object Layer: the floor should be `Ground` and the walls should be `Walls`.__

- KillPlane

  `CampaignGameEnvironment/KillPlane` is the respawn plane; players or objects that touch it will respawn or be destroyed. Set its position and scale, and set the `RespawnCollider > respawnType` field as follows: `Drowning` – drowning, `Fall Death` – falling, `Car` – knocked down (`Hit` is deprecated). If set to `Drowning`, select `WaterSplash_Particle_003_SO` (standard water splash) or `WaterSplash_Particle_004_alien_SO` (alien-themed green water splash) in the `PseudoPrefabManager > PseudoPrefabManagerStub > OnDeathEffectSO` field, then click Tools – Reload Pseudo Assets to load them. You can also add other KillPlanes by adding a `Collider` (check its `isTrigger` checkbox) and a `Respawn Collider` component to an empty object. See `s_oc1_story_6_3 > Design/KillPlanes` for reference.

##### Animation

- Moving Ground

  See `s_test_level_5 > Design/Platforms` for reference. When there are moving grounds in the level, the `levelinfo.disabledynamicparenting` checkbox should be unchecked. __Each moving ground should have an `ObjectContainer` component added to a container object, ensuring that the ground's `Collider` is under its hierarchy. If a moving ground has counters on it, a separate `QuadGridManager` component should be added to the container object to manage the grid on that ground.__

- Moving Counters

  See `s_oc1_story_4_1 > Design/Animated Objects/MovingCountersUp` for reference. __All moving counters must be under the hierarchy of an object named `Animated Objects`.

- Trigger

  Animations must be triggered simultaneously on the host and the client via Triggers. If an animation is very long, it should be divided into segments and triggered sequentially using multiple Triggers to prevent the animations on the host and the client from becoming out of sync. Similarly, looping animations should be triggered via Triggers at the start of each loop.

  [Trigger Components reference](reference.md#Trigger-Components)



#### Important Notes

- When duplicating a scene, remember to modify the `PseudoPrefabManagerStub.levelInfo` reference on the root object `PseudoPrefabManager`.
- Click Tools - Toggle Prepare For Building to clear temporary objects when saving and building the scene.
- Remember to save the scene before switching scenes or closing Unity. If you forget to save, click Cancel in the confirmation dialogue to return; do not click Save.
- Do not modify temporary objects.
- The name of the level set configuration must be `LevelSetInfo`.
- Do not create new assets in the `common*` directory. They should be created within your level set directory so that they are packaged into your level set info bundle or scene bundle.
- Add the BGM's bundle to `LevelInfoSO.dependencies`.
- If you find that a counter does not highlight when the player approaches it (this issue occurs when the counter appearance is set to `CounterCampingSO`), add a `FogConfig` component to `MultiplayerGameCamera/Camera` and set the `fogFar` field to 10000 and the `fogColour` field to pure black. See `s_oc1_story_3_1` for reference.
- When releasing a new version of the level set, remember to update `LevelSetInfoSO.version`.

