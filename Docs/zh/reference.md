## 编辑器物体和组件说明

如果对任何字段的用法有疑问，参考项目中已有的 `test` 和 `oc1_story` 两个关卡集中的资源可能会帮助理解。

### 游戏物体

#### 桌台

在 `common01/prefabs/counters` 下。

- `Counter` - 普通桌台
  - `PseudoPrefabStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/Counter*SO`。
- `CounterCorner` - 角落桌台
  - 不吸附物体。
  - `PseudoPrefabStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/CounterCorner*SO`。
- `Dispenser` - 食材箱
  - `PseudoPrefabDispenserStub > spawnerItemPrefabSO` - 食材箱的食材，资源位置 `common01/food/Ingredients`。修改后点击 Tools - Reload Pseudo Assets 重新加载。
  - `PseudoPrefabDispenserStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/Dispenser*SO`。
- `ChoppingCounter` - 切菜台
  - `PseudoPrefabStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/ChoppingCounter*SO`。
- `Bin` - 垃圾桶
- `ServingStation` - 上菜口
  - 两格宽，如果要对齐其他桌台可能需要移动半格 （0.6）。
  - `PseudoPrefabServingStationStub > plateReturn` - 这个上菜口对应的脏盘台。
  - `PseudoPrefabServingStationStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/ServingStation*SO`。
- `PlateReturn` - 脏盘台
  - `PseudoPrefabPlateReturnStub > returnClean` - 是否返回干净盘子。
- `Sink` - 洗碗池
  - 两格宽，如果要对齐其他桌台可能需要移动半格 （0.6）。
  - `PseudoPrefabStub > pseudoPrefabSO` - 桌台外观，资源位置 `common01/pseudo_prefab_so/counters/Sink*SO`。
- `Cooker` - 灶台
- `FryingStation` - 油炸台
- `Oven` - 烤箱
- `ConveyorStation` - 传送带
  - `PseudoPrefabConveyorStub > conveySpeed` - 传送带速度。

#### 厨房器具

在 `common01/prefabs/utensils` 下。

- `Plate` - 盘
- `CleanPlateStack` - 盘堆
  - `PseudoPrefabCleanPlateStackStub > plateCount` - 盘堆数目。
- `FireExtinguisher` - 灭火器
- `FryPan` - 煎锅，`Pot` - 煮锅，`FrierBasket` - 炸锅，`Steamer` - 蒸笼，`MixerBowl` - 搅拌碗
  - `PseudoPrefabCookingUtensilStub > capacity` - 可容纳食材数目。
  - `PseudoPrefabCookingUtensilStub > allowedIngredientSOs` - 允许烹饪的食材列表，资源位置 `common01/food/Ingredients`。

#### 游戏机关

在 `common01/prefabs/mechanisms` 下。

- `AttachingFoodSpawner` - 食材生成器
  
  - 需连接传送带。
  - `PseudoPrefabAttachingFoodSpawnerStub > spawnInOrder` - 是否顺序生成（或随机生成）。
  - `PseudoPrefabAttachingFoodSpawnerStub > attachmentPrefabSOs` - 生成的食材列表，资源位置 `common01/food/Ingredients`。
  - `PseudoPrefabAttachingFoodSpawnerStub > weights` - 每个食材的权重（随机生成时）。
  - `PseudoPrefabAttachingFoodSpawnerStub > triggerTime` - 生成间隔。
  - `PseudoPrefabAttachingFoodSpawnerStub > triggerAtStart` - 是否在关卡开始时生成一次。
  
- `MultiControlTerminal` - 摇杆（移动平台）

  参考 `s_test_level_3`。

- `ControlTerminal_Marker` - 指示灯

  参考 `s_test_level_5`。

- `Switch` - 交互按钮
  
  - `PseudoPrefabSwitchStub > startEnabled` - 初始是否为按下状态（不勾选为按下）。
  - `PseudoPrefabSwitchStub > activeMaterial` - 未按下时的外观材质，资源位置`common01/pseudo_prefab_so/mechanisms/Switch`。
  - `PseudoPrefabSwitchStub > inactiveMaterial` - 按下时的外观材质，资源位置`common01/pseudo_prefab_so/mechanisms/Switch`。
  - `PseudoPrefabSwitchStub > triggerOnAnimator` - 按钮发送的 trigger（目标为 Animator）。
  - `PseudoPrefabSwitchStub > animatorToTrigger` - 目标 Animator。
  - `PseudoPrefabSwitchStub > triggerOnObject` - 按钮发送的 trigger（目标为物体）。
  - `PseudoPrefabSwitchStub > objectToTrigger` - 目标物体。
  - `TriggerOnObject` 组件不用修改。
  
- `PressureSwitch` - 踏板按钮
  
  - `PseudoPrefabPressureSwitchStub > occupiedMaterialSO` - 按下时的外观材质，资源位置`common01/pseudo_prefab_so/mechanisms/Switch`。
  - `PseudoPrefabPressureSwitchStub > unoccupiedMaterialSO` - 松开时的外观材质，资源位置`common01/pseudo_prefab_so/mechanisms/Switch`。
  - `PseudoPrefabPressureSwitchStub > triggerOnAnimatorEnter` - 按下时发送的 trigger（目标为 Animator）。
  - `PseudoPrefabPressureSwitchStub > triggerOnAnimatorExit` - 松开时发送的 trigger（目标为 Animator）。
  - `PseudoPrefabPressureSwitchStub > animatorToTrigger` - 目标 Animator。
  - `PseudoPrefabPressureSwitchStub > triggerOnObjectEnter` - 按下时发送的 trigger（目标为物体）。
  - `PseudoPrefabPressureSwitchStub > triggerOnObjectExit` - 松开时发送的 trigger（目标为物体）。
  - `PseudoPrefabPressureSwitchStub > objectToTrigger` - 目标物体。
  
- `Teleportal` - 传送门
  
  - `PseudoPrefabTeleportalStub > exitPortal` - 配对的传送门。
  - `PseudoPrefabTeleportalStub > portalColor` - 传送门外观颜色。
  - `PseudoPrefabTeleportalStub > doubleSided` - 是否从两面都能进传送门。
  
- `Travelator` - 地面传送带
  
  - `PseudoPrefabTravelatorStub > speed` - 传送带速度。 

#### 玩家

- `common01/prefabs/Player` - 玩家
  - `PseudoPrefabPlayerStub > playerID` - 玩家编号。



### 场景物体

在 `common01/prefabs/art` 下，按主题分目录。

- 对齐接缝可以把 Shading Mode 改成 Wireframe，选中物体后按住 `v` 键吸附顶点。

#### 特殊物体

- `common01/prefabs/art/npc` - NPC
  - `PseudoPrefabNPCStub > animatorControllerSO` - NPC 动画，资源位置 `common01/pseudo_prefab_so/art/NPC`，可选 `NPC_Diner_Not_Choose_SO, NPC_Diner_Solo_SO, NPC_Waiter_Idle_NotChoose_01_SO, NPC_Walking_01_SO`。
- `common01/prefabs/art/city_sushi/NPC_Walk_Anticlockwise_*s` - 绕圈行走的 NPC
- `common01/prefabs/art/space/Space_Door_Airlock_*` - 开关门
  - 交互按钮用无 Bool 的，踏板按钮用有 Bool 的。
  - 初始时关闭用 Close 的，初始时开启用 Open 的。



### 相机

- <span id="MultiplayerCamera">`MultiplayerCamera`</span> - 相机跟随玩家组件
  - `m_gradientLimit` - 相机跟随时最大移动速度。默认 4 即可。
  - `m_timeToMax` - 相机跟随的加速时间。默认 10 即可。
  - `m_{x}EdgeBuffer` - 游戏视图中最靠 x 方向的玩家距离屏幕边缘希望留出的距离比例。默认 0.4 即可。
  - `m_minDistance` - 相机距离玩家平面的最小距离。当玩家聚在中间时，相机距离会减小。
  - `m_maxDistance` - 相机距离玩家平面的最大距离。当玩家分散时，相机距离会增大。
  - `m_maxUDistance` - 相机前后移动的最大距离。当玩家向同一方向移动时，相机会向那个方向跟随。
  - `m_maxRDistance` - 相机左右移动的最大距离。当玩家向同一方向移动时，相机会向那个方向跟随。



### Trigger 组件

- `TriggerTimer` - 计时器 trigger

  - `m_startTrigger` - 触发这个计时器的 trigger。
  - `m_completeTrigger` - 这个计时器结束后发送的 trigger（目标为自身）。
  - `m_time` - 计时器秒数。
  - `m_startTiming` - 是否在关卡开始时直接开始计时。
  - `m_triggerAtStart` - 是否在关卡开始时直接发送一次。

- `TriggerQueue` - 队列循环 trigger

  - `m_startTrigger` - 触发这个队列循环的 trigger。
  - `m_cancelTrigger` - 取消这个队列循环的 trigger。
  - `m_endTrigger` - 一轮队列结束后发送的 trigger。
  - `m_endTriggerTarget` - 一轮队列结束后发送的目标。
  - `m_startOnAwake` - 是否在关卡开始时直接开始计时。
  - `m_loopWhenFinished` - 是否循环。
  - `m_loopDelay` - 两轮循环之间的间隔秒数。
  - `m_targetType` - 发送 trigger 的目标类型。
  - `m_animator` - 目标 Animator（`m_targetType` 为 `Animator` 时）。
  - `m_targetObject` - 目标物体（`m_targetType` 为 `Object` 时）。
  - `m_finishedTrigger` - 动画结束触发的 trigger（`m_targetType` 为 `Animator` 且 `m_waitForFinished` 为 `true` 时），默认为 `"AnimationFinished"`。
  - `m_waitForFinished` - 是否等待动画结束再开始计时队列下一项（`m_targetType` 为 `Animator` 时）。
  - `m_queue` - `triggers` 和 `delays` 长度相同，为队列每项发送的 trigger 和计时器秒数。

  > - 如果要使用 `m_waitForFinished`，将 Animator 和 `TriggerQueue` 挂在同一个物体上，并在动画的 State 上添加 `SendTriggerToObject` 脚本，`triggerToSend` 字段填 `"AnimationFinished"`，勾选 `orTriggerOnExit`。__注意：添加脚本时可能看到多个同名脚本，添加来自项目代码的脚本（Script 图标为 C#），不要添加来自 bundle 的脚本（Script 图标为白纸）。__
  > - 如果循环，第二次循环在 `m_loopDelay` 后直接发送 `m_queue.m_triggers[0]`，不会再计时 `m_queue.m_delays[0]`。

- `TriggerOnObject` - 物体 trigger 传递器

  - `m_trigger` - 触发这个传递器的 trigger。
  - `m_triggerToFire` - 这个传递器发送的 trigger。
  - `m_targetObject` - 发送的目标物体。
  - `m_targetObjects` - 发送的目标物体列表。

- `TriggerOnAnimator` - Animator trigger 传递器

  - `m_triggerToReceive` - 触发这个传递器的 trigger。
  - `m_triggerToFire` - 这个传递器发送的 trigger。
  - `m_targetAnimator` - 发送的目标 Animator。
  - `m_triggerToFireHash` - 不用填。
  
- `TriggerAdapter` - trigger 转换器

  - `m_inputTrigger` - 触发这个转换器的 trigger。
  - `m_outputTrigger` - 这个转换器发送的 trigger（目标为自身）。




### 数据组件

#### 关卡数据

- <span id="LevelConfigSetupPerPlayerCountSO">`LevelConfigSetupPerPlayerCountSO`</span> - 指定玩家数的关卡配置
  - `orderLifeTime` - 订单超时的时间。
  - `timeBetweenOrders` - 订单生成间隔。
  - `plateReturnTime` - 上菜后到返回盘子的间隔。
  - `survivalTimeMultiplier` - 没用，填 1 即可。
  - `roundTime` - 关卡时长。
  - `m_{x}StarScore` - x 星分数。
  
- <span id="LevelInfoSO">`LevelInfoSO`</span> - 关卡配置
  - `levelName` - 关卡的显示名（英文）。
  - `levelNameZH` - 关卡的显示名（中文）。
  - `screenshot` - 关卡截图。
  - `sceneName` - 关卡场景名，__必须与打包后的场景文件名相同，且不能与任何关卡集的任何场景文件名重复，不要取过于简单的名字__。
  - `recipes` - 关卡所有菜谱。（[可用菜谱](#可用菜谱)）
  - `debugRecipeCount` - 填 0 即可。
  - `optionalRecipeMatchListItems` - 其他允许的菜谱组合。在披萨关，除了订单里的菜谱，另一些菜谱组合也可装盘，需添加在此列表。参考 `LevelInfo_OC1_Story_4_1`。
  - `disableDynamicParenting` - 禁用动态父物体挂载。在包含移动、升降平台的关卡，应取消勾选；在其他关卡，应勾选。
  - `config_{x}p` - 玩家人数 x 的关卡配置。
  - `dependencies` - 依赖的游戏资源包。一般仅添加一项 `bundle47` 即可。若使用木筏主题的 BGM 则需额外添加一项 `bundle11`。

- <span id="LevelSetInfoSO">`LevelSetInfoSO`</span> - 关卡集配置
  - `levelSetName` - 关卡集的显示名（英文）。
  - `levelSetNameZH` - 关卡集的显示名（中文）。
  - `author` - 关卡集作者。
  - `uid` - 关卡集唯一标识符，用于街机大厅搜索等。不用手动修改，在修改 `version` 字段时自动修改。
  - `version` - 关卡集版本号。
  - `levelInfos` - 关卡集包含的所有关卡的配置。


#### 菜谱数据

##### 可用菜谱

- `common01/food/Recipes` 中的主线 34 个菜谱。
- 新菜谱 `Burger_Lettuce_SO, Fry_Fish_And_Chips_SO, Fry_Fish_SO, Pizza_Mushroom_80_SO, Soup_Mushroom_SO, Soup_Onion_SO, Soup_Tomato_SO, Soup_TomatoEgg_SO`。
- 你也可以自定义菜谱（完善中）。

