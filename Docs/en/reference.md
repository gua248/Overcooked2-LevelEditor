## Editor Objects and Components Reference

If you have any questions regarding the usage of any fields, it may be helpful to refer to the assets in the two existing level sets, `test` and `oc1_story`.

### Gameplay Objects

#### Counters

In the directory `common01/prefabs/counters`.

- `Counter` - Regular counter
  - `PseudoPrefabStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/Counter*SO`.
- `CounterCorner` - Corner counter
  - Does not attach objects to itself.
  - `PseudoPrefabStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/CounterCorner*SO`.
- `Dispenser` - Ingredient box
  - `PseudoPrefabDispenserStub > spawnerItemPrefabSO` - Ingredient of the dispenser. The assets are located in the directory `common01/food/Ingredients`. After modifying, click Tools - Reload Pseudo Assets to reload.
  - `PseudoPrefabDispenserStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/Dispenser*SO`.
- `ChoppingCounter`
  - `PseudoPrefabStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/ChoppingCounter*SO`.
- `Bin` - Trashbin
- `ServingStation` - Serving station
  - 2 grids wide; may need to be moved half a grid (0.6) to align with other counters.
  - `PseudoPrefabServingStationStub > plateReturn` - The plate return station corresponding to this serving station.
  - `PseudoPrefabServingStationStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/ServingStation*SO`.
- `PlateReturn` - Plate return station
  - `PseudoPrefabPlateReturnStub > returnClean` - Whether clean plates are returned.
- `Sink` - Sink
  - 2 grids wide; may need to be moved half a grid (0.6) to align with other counters.
  - `PseudoPrefabStub > pseudoPrefabSO` - Counter appearance. The assets are located in the directory `common01/pseudo_prefab_so/counters/Sink*SO`.
- `Cooker` - Cooker
- `FryingStation` - Frying station
- `Oven` - Oven
- `ConveyorStation` - Conveyor belt
  - `PseudoPrefabConveyorStub > conveySpeed` - Conveyor belt speed.

#### Utensils

In the directory `common01/prefabs/utensils`.

- `Plate` - Plate
- `CleanPlateStack` - Stack of clean plates
  - `PseudoPrefabCleanPlateStackStub > plateCount` - Number of plates in the stack.
- `FireExtinguisher` - Fire extinguisher
- `FryPan`, `Pot`, `FrierBasket`, `Steamer`, `MixerBowl` - Cooking utensils
  - `PseudoPrefabCookingUtensilStub > capacity` - The number of ingredients that can be placed in the utensil.
  - `PseudoPrefabCookingUtensilStub > allowedIngredientSOs` - The list of ingredients allowed for this utensil. The assets are located in the directory `common01/food/Ingredients`.

#### Mechanisms

In the directory `common01/prefabs/mechanisms`.

- `AttachingFoodSpawner` - Ingredient spawner
  
  - Must be connected to a conveyor belt.
  - `PseudoPrefabAttachingFoodSpawnerStub > spawnInOrder` - Whether to spawn ingredients in order (or randomly).
  - `PseudoPrefabAttachingFoodSpawnerStub > attachmentPrefabSOs` - List of ingredients to be spawned. The assets are located in the directory `common01/food/Ingredients`.
  - `PseudoPrefabAttachingFoodSpawnerStub > weights` - The weight of each ingredient (when spawning randomly).
  - `PseudoPrefabAttachingFoodSpawnerStub > triggerTime` - Spawn interval.
  - `PseudoPrefabAttachingFoodSpawnerStub > triggerAtStart` - Whether to spawn once at the start of the level.
  
- `MultiControlTerminal` - Control stick

  Refer to `s_test_level_3`.

- `ControlTerminal_Marker` - Marker

  Refer to `s_test_level_5`.

- `Switch` - Interactive switch

  - `PseudoPrefabSwitchStub > startEnabled` - Whether the switch is initially in the pressed state (unchecked for pressed).
  - `PseudoPrefabSwitchStub > activeMaterial` - The appearance material when the switch is not pressed. The assets are located in the directory `common01/pseudo_prefab_so/mechanisms/Switch`.
  - `PseudoPrefabSwitchStub > inactiveMaterial` - The appearance material when the switch is pressed. The assets are located in the directory `common01/pseudo_prefab_so/mechanisms/Switch`.
  - `PseudoPrefabSwitchStub > triggerOnAnimator` - Trigger sent by the switch (target is an Animator).
  - `PseudoPrefabSwitchStub > animatorToTrigger` - The target Animator.
  - `PseudoPrefabSwitchStub > triggerOnObject` - Trigger sent by the switch (target is a GameObject).
  - `PseudoPrefabSwitchStub > objectToTrigger` - The target GameObject.
  - Do not modify the `TriggerOnObject` component.
  
- `PressureSwitch` - Pressure switch
  
  - `PseudoPrefabPressureSwitchStub > occupiedMaterialSO` - The appearance material when the switch is pressed. The assets are located in the directory `common01/pseudo_prefab_so/mechanisms/Switch`.
  - `PseudoPrefabPressureSwitchStub > unoccupiedMaterialSO` - The appearance material when the switch is not pressed. The assets are located in the directory `common01/pseudo_prefab_so/mechanisms/Switch`.
  - `PseudoPrefabPressureSwitchStub > triggerOnAnimatorEnter` - Trigger sent by the switch when pressed (target is an Animator).
  - `PseudoPrefabPressureSwitchStub > triggerOnAnimatorExit` - Trigger sent by the switch when released (target is an Animator).
  - `PseudoPrefabPressureSwitchStub > animatorToTrigger` - The target Animator.
  - `PseudoPrefabPressureSwitchStub > triggerOnObjectEnter` - Trigger sent by the switch when pressed (target is a GameObject).
  - `PseudoPrefabPressureSwitchStub > triggerOnObjectExit` - Trigger sent by the switch when released (target is a GameObject).
  - `PseudoPrefabPressureSwitchStub > objectToTrigger` - The target GameObject.
  
- `Teleportal` - Portal
  
  - `PseudoPrefabTeleportalStub > exitPortal` - The paired portal.
  - `PseudoPrefabTeleportalStub > portalColor` - The portal's appearance color.
  - `PseudoPrefabTeleportalStub > doubleSided` - Whether the portal can be entered from both sides.
  
- `Travelator` - Travelator on the ground
  
  - `PseudoPrefabTravelatorStub > speed` - Travelator speed.

#### Player

- `common01/prefabs/Player` - Player
  - `PseudoPrefabPlayerStub > playerID` - Player ID.



### Environment Objects

In the directory `common01/prefabs/art`, organized into subdirectories by theme.

- To align seams, change the Shading Mode to Wireframe, select the object, and hold the `v` key to snap to vertices.

#### Special Objects

- `common01/prefabs/art/npc` - NPC
  - `PseudoPrefabNPCStub > animatorControllerSO` - NPC animator. The assets are located in the directory `common01/pseudo_prefab_so/art/NPC`, options are `NPC_Diner_Not_Choose_SO, NPC_Diner_Solo_SO, NPC_Waiter_Idle_NotChoose_01_SO, NPC_Walking_01_SO`.
- `common01/prefabs/art/city_sushi/NPC_Walk_Anticlockwise_*s` - NPCs walking in a circle
- `common01/prefabs/art/space/Space_Door_Airlock_*` - Toggleable door
  - Use prefabs without "Bool" in the name for interactive switches, and prefabs with "Bool" for pressure switches.
  - Use prefabs with "Close" in the name for initially closed doors, and prefabs with "Open" for initially opened doors.



### Camera

- <span id="MultiplayerCamera">`MultiplayerCamera`</span> - Camera follow component
  - `m_gradientLimit` - Maximum movement speed while following. Defaults to 4.
  - `m_timeToMax` - Acceleration time to reach maximum speed while following. Defaults to 10.
  - `m_{x}EdgeBuffer` - The ratio of the screen size reserved as padding between the x-most player and the screen edge. Defaults to 0.4.
  - `m_minDistance` - The minimum distance between the camera and the player plane. When players cluster in the center, the camera distance decreases.
  - `m_maxDistance` - The maximum distance between the camera and the player plane. When players are spread out, the camera distance increases.
  - `m_maxUDistance` - The maximum distance the camera can move forward or backward. When players move in the same direction, the camera will follow in that direction.
  - `m_maxRDistance` - The maximum distance the camera can move left or right. When players move in the same direction, the camera will follow in that direction.



### Trigger Components

- `TriggerTimer` - Timer trigger

  - `m_startTrigger` - The trigger that starts this timer.
  - `m_completeTrigger` - The trigger sent when this timer ends (target is itself).
  - `m_time` - Timer duration in seconds.
  - `m_startTiming` - Whether to start the timer immediately at the start of the level.
  - `m_triggerAtStart` - Whether to send the trigger once immediately at the start of the level.

- `TriggerQueue` - Looping queue trigger

  - `m_startTrigger` - The trigger that starts this looping queue.
  - `m_cancelTrigger` - The trigger that cancels this looping queue.
  - `m_endTrigger` - The trigger sent when a queue loop ends.
  - `m_endTriggerTarget` - The target to which the end trigger is sent.
  - `m_startOnAwake` - Whether to start the queue immediately at the start of the level.
  - `m_loopWhenFinished` - Whether to loop.
  - `m_loopDelay` - The interval in seconds between two queue loops.
  - `m_targetType` - The target type to which the queue triggers are sent.
  - `m_animator` - The target Animator (when `m_targetType` is `Animator`).
  - `m_targetObject` - The target GameObject (when `m_targetType` is `Object`).
  - `m_finishedTrigger` - The trigger sent when the Animation ends (when `m_targetType` is `Animator` and `m_waitForFinished` is `true`); defaults to `"AnimationFinished"`.
  - `m_waitForFinished` - Whether to wait for the Animation to finish before starting the next item in the queue (when `m_targetType` is `Animator`).
  - `m_queue` - `triggers` and `delays` should be of equal length, representing the triggers sent and the timer durations in seconds for each item in the queue.

  > - To use `m_waitForFinished`, add the Animator and `TriggerQueue` to the same object, and add a `SendTriggerToObject` behaviour to the animation's State. Set the `triggerToSend` field to `"AnimationFinished"` and check the `orTriggerOnExit` checkbox. __Note: When adding the behaviour, you may see multiple behaviours with the same name. Add the behaviour from the project code (the Script icon is C#); do not add the behaviour from the bundle (the Script icon is a blank document icon).__
  > - If looping is enabled, on the second loop, `m_queue.m_triggers[0]` is sent immediately after `m_loopDelay`; `m_queue.m_delays[0]` is no longer timed.

- `TriggerOnObject` - Object trigger forwarder

  - `m_trigger` - The trigger that activates this forwarder.
  - `m_triggerToFire` - The trigger sent by this forwarder.
  - `m_targetObject` - The target GameObject.
  - `m_targetObjects` - The list of target GameObjects.

- `TriggerOnAnimator` - Animator trigger forwarder

  - `m_triggerToReceive` - The trigger that activates this forwarder.
  - `m_triggerToFire` - The trigger sent by this forwarder.
  - `m_targetAnimator` - The target Animator.
  - `m_triggerToFireHash` - Leave blank.
  
- `TriggerAdapter` - Trigger adapter

  - `m_inputTrigger` - The trigger that activates this adapter.
  - `m_outputTrigger` - The trigger sent by this adapter (target is itself).




### Data Components

#### Level Data

- <span id="LevelConfigSetupPerPlayerCountSO">`LevelConfigSetupPerPlayerCountSO`</span> - Level configuration for a specific player count
  - `orderLifeTime` - Order lifeTime.
  - `timeBetweenOrders` - Interval between orders being generated.
  - `plateReturnTime` - Interval between serving a dish and returning the plate.
  - `survivalTimeMultiplier` - Not used; just set it to 1.
  - `roundTime` - Level duration.
  - `m_{x}StarScore` - Score for x stars.
  
- <span id="LevelInfoSO">`LevelInfoSO`</span> - Level configuration
  - `levelName` - Level display name (English).
  - `levelNameZH` - Level display name (Chinese).
  - `screenshot` - Level screenshot.
  - `sceneName` - Level scene name. __It must match the filename of the packaged scene bundle exactly and must not duplicate any scene filename in any level set. Avoid using simple names.__
  - `recipes` - All recipes in the level. ([Available Recipes](#Available-Recipes))
  - `debugRecipeCount` - Set to 0.
  - `optionalRecipeMatchListItems` - Other allowed food combinations. In pizza levels, in addition to the recipes appear in orders, other food combinations can also be plated; these must be added to this list. See `LevelInfo_OC1_Story_4_1` for reference.
  - `disableDynamicParenting` - Dynamic parenting option. In levels containing moving or elevating platforms, this option should be unchecked, otherwise it should be checked.
  - `config_{x}p` - Level configuration for player count x.
  - `dependencies` - Dependent game bundles. Generally, adding just one entry `bundle47` is sufficient. If using the raft-themed BGM (`DownTheRiverSO`), an additional entry `bundle11` must be added.

- <span id="LevelSetInfoSO">`LevelSetInfoSO`</span> - Level set configuration
  - `levelSetName` - Level set display name (English).
  - `levelSetNameZH` - Level set display name (Chinese).
  - `author` - Level set author.
  - `uid` - The level set's unique identifier, used for searching lobbies in the Arcade, etc. Do not modify this manually; it is automatically updated when the `version` field is modified.
  - `version` - The level set's version string.
  - `levelInfos` - The list of the configurations for all levels in the level set.


#### 菜谱数据

##### Available Recipes

- 34 recipes from the story in directory `common01/food/Recipes`.
- New recipes: `Burger_Lettuce_SO, Fry_Fish_And_Chips_SO, Fry_Fish_SO, Pizza_Mushroom_80_SO, Soup_Mushroom_SO, Soup_Onion_SO, Soup_Tomato_SO, Soup_TomatoEgg_SO`.
- You can also customize recipes (under development).

