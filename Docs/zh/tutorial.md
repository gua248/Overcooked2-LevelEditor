## 胡闹厨房2 关卡编辑器使用教程

#### 准备工作

1. 下载或克隆本项目。

2. 将游戏目录中 `Overcooked! 2/Overcooked2_Data/StreamingAssets/Windows` 文件夹复制到项目 `Assets/StreamingAssets` 文件夹中。

3. 反编译游戏代码放入 `Assets/Scripts` 中：

  > 1. 下载 Assetripper 反编译工具 https://github.com/assetripper/assetripper。
  > 2. 打开 Assetripper，File - Settings - 勾选 Skip StreamingAssets Folder - 最下面 Save。
  > 3. File - Open Folder - 选择游戏目录中 `Overcooked! 2/Overcooked2_Data` 文件夹。
  > 4. Export - Export All Files - 勾选 Create Subfolder - Select Folder 选择导出目录 - Export Unity Project，等待导出完成。
  > 5. 将导出目录里的 `ExportedProject/Assets/Scripts/Assembly-CSharp` 文件夹拷贝到项目 `Assets/Scripts` 中。
  > 6. 将项目的 `Assembly-CSharp-Patch` 文件夹中的所有内容拷贝到项目 `Assets/Scripts/Assembly-CSharp` 文件夹中，替换。

4. 在 Unity 中打开项目，编辑器版本 2017.4.8f1。

5. 在 Console 查看并修复剩余的编译报错。（应该没有）

6. 打开场景 `LevelSets/oc1_story/scenes/s_oc1_story_1_1`（或其他关卡），点击 Play 即可运行关卡（键位 `W` `A` `S` `D` + `空格` + `G` `H` `J`）。



#### 创建新关卡和新关卡集

假设要创建一个新关卡集 `xxx`，包含 `xxx-1` 这一个关卡。

1. 在 `LevelSets` 目录下新建目录 `xxx/` 以及 `xxx/data/xxx_1/`、`xxx/data/xxx_2/`、`xxx/scenes/`。（参考 `LevelSets/test` ）
2. 在 `xxx/data/xxx_1` 目录右键 Create - LevelEditor - LevelConfigSetupPerPlayerCountSO 创建单人关卡配置，命名为 `config_1p`，填写数据。类似创建 2 / 3 / 4 人关卡配置。[LevelConfigSetupPerPlayerCountSO 说明](reference.md#LevelConfigSetupPerPlayerCountSO)
3. 在 `xxx/data/xxx_1` 目录右键 Create - LevelEditor - LevelInfoSO 创建关卡配置，命名为 `LevelInfo_xxx_1`，填写数据。[LevelInfoSO 说明](reference.md#LevelInfoSO)
4. 在 `xxx/data` 目录右键 Create - LevelEditor - LevelSetInfoSO 创建关卡集配置，__命名为 `LevelSetInfo`（此命名不可更改）__，填写数据。[LevelSetInfoSO 说明](reference.md#LevelSetInfoSO)
5. 多个关卡同理，把所有 `LevelInfo` 添加到 `LevelSetInfo.levelInfos` 即可。
6. 复制模板场景 `Template/s_template` 到 `xxx/scenes` 目录，重命名为 `s_xxx_1`。
7. 打开场景 `s_xxx_1`，修改根物体 `PseudoPrefabManager` 上的 `PseudoPrefabManagerStub.levelInfo` 引用为刚创建的关卡配置 `LevelInfo_xxx_1`。点击 Tools - Reload Pseudo Assets 加载新的关卡配置。
8. [编辑场景](#编辑场景)。点击 Play 可以运行关卡。
9. __点击 Tools - Toggle Prepare For Building 清除临时加载的物体，然后__保存场景。



#### 构建和导出关卡

假设关卡集 `xxx` 包含 `xxx-1` 这一个关卡。

1. 在 Project 面板选中场景文件 `s_xxx_1`，在 Inspector 面板最下方选择 AssetBundle 的第一个选项框，选择 New...，填入 `xxx/s_xxx_1`。
2. 在 Project 面板选中关卡集根目录 `xxx`，在 Inspector 面板最下方选择 AssetBundle 的第一个选项框，选择 New...，填入 `xxx/info_xxx`。
3. 点击 Tools - Build AssetBundles。（第一次构建可能需要较长时间）
4. 在项目目录 `Assets/AssetBundles/xxx` 中可以看到 `info_xxx` 和 `s_xxx_1` 两个文件。将他们拷贝到游戏目录中 `Overcooked! 2/BepInEx/plugins/OC2DIYLevel/levels/xxx` 目录里即可游玩。
5. 现在可以发布你的关卡了！



#### 编辑场景

##### 通用操作

- 大部分游戏物体在场景里有一个占位物体，在编辑和游戏时从游戏原有资源包中临时加载。点击 Tools - Toggle Prepare For Building 可以加载 / 清除临时物体。例如厨师 1 的占位物体是 `Chefs/Player 1`，加载后的临时物体是 `Chefs/Player 1/player`。
- __保存和构建场景时应清除临时物体。__
- __不要操作临时物体。__在场景视图里点击可能会选择到临时物体（或其子物体），需要在 Hierarchy 视图里选中对应的占位物体进行移动等操作。
- __不要将物体放在场景的根物体。__所有物体都需要放在至少有一层父级的位置，如模板里预设的 `Design/Counters` 等位置，也可以新建空物体作为父级。
- 在 Project 面板将 `common01/prefabs` 里的物体拖入场景或 Hierarchy。
- 需要对齐网格的物体可以按住 Ctrl 进行移动。厨房网格的大小为 1.2。
- 如果遇到没有正确加载临时物体或不小心操作了临时物体的情况，点击 Tools - Reload Pseudo Assets 重新加载。

##### 游戏物体

- 桌台

  在 `common01/prefabs/counters` 下。
  
  包含桌台、角落桌台、食材箱、切菜台、垃圾桶、上菜口、脏盘台、洗碗池、灶台、油炸台、搅拌台、烤箱、传送带。
  
  [桌台说明](reference.md#桌台)
  
- 厨房器具

  在 `common01/prefabs/utensils` 下。
  
  包含盘、盘堆、灭火器、煎锅、煮锅、炸锅、蒸笼、搅拌碗。
  
  放在对应桌台位置的上方即可，开局会自动吸附到桌台上。
  
  [厨房器具说明](reference.md#厨房器具)
  
- 游戏机关

  在 `common*/prefabs/mechanisms` 下。
  
  包含食材生成器、摇杆、指示灯、交互按钮、踏板按钮、传送门、地面传送带（`common01`）；火焰发射器（`common02`）。
  
  [游戏机关说明](reference.md#游戏机关)
  
- 玩家

  在 `common01/prefabs/Player`。

  [玩家说明](reference.md#玩家)

##### 场景物体

在 `common01/prefabs/art` 下，按主题分目录。

[场景物体说明](reference.md#场景物体)

##### 相机

- 后处理

  为 `MultiplayerGameCamera/Camera > PostProcessingBehaviour > profile` 字段添加一个后处理配置。预设的后处理配置在 `common01/post_processing` 下，你也可以在你的关卡集目录下创建新的后处理配置。

- `MultiplayerGameCamera`

  调整相机物体 `MultiplayerGameCamera` 的 `Transform` 到合适的位置和角度（查看 Game 视图），同时配合它的 `MultiplayerCamera` 组件的参数，使得游戏时不管玩家如何移动，场景的主要部分总是在镜头中。

  [MultiplayerCamera 说明](reference.md#MultiplayerCamera)

##### 光照

- 光照设置

  在菜单栏 Window - Lighting - Settings 的 Environment 区设置 Skybox 和环境光，参考 `test` 和 `oc1_story` 两个关卡集中的场景。Realtime Global Illumination 和 Baked Global Illumination 不用勾选。

- 在场景 `Art/Lights` 下调整或添加光源。

- 目前，自制关卡全部使用实时光照，单个物体最多接收 4 个实时光源，超过会导致渲染结果异常（亮度变化不连续）。一些场景物体中带有光源，请注意此限制。

##### 音乐和音效

在 `PseudoPrefabManager > PseudoPrefabManagerStub` 组件上：

- `inLevelMusicSO` 字段选择关卡音乐资源，资源位置在 `common*/pseudo_prefab_so/audio/music` 目录。__许多 BGM 所在 bundle 不在游戏默认加载的 bundle 列表中，需要在 `LevelInfoSO.dependencies` 额外添加 BGM 所在 bundle。__

- `inlevelAmbiences` 字段添加关卡氛围音效 tag，参考 `test` 和 `oc1_story` 两个关卡集中的场景。

- `audioDirectorySOs` 字段添加音效集资源，资源位置在 `common02/pseudo_prefab_so/audio/AudioDirectories` 目录。如果氛围音效 tag 或者一些场景物体使用了不在 `audioDirectorySOs` 列表里的 tag，会看到报错

  ```
  ArgumentOutOfRangeException: Argument is out of range.
  Parameter name: index
  System.Collections.Generic.List`1[AudioDirectoryEntry].get_Item (Int32 index) ...
  ```

  一般来说，使用了什么主题的场景物体就添加对应的音效集，或者多添加几个也没关系。

##### 其他设置

- 天花板高度

  `CampaignGameEnvironment/KitchenLoaderManager` 的 `ceilingHeight` 字段为天花板高度。默认为 2，即厨师碰撞体高度。若关卡不全是平地，应该适当增加。例如有一个会抬升 1 的平台，则至少设为 3。

- 动态父物体挂载

  `levelinfo.disabledynamicparenting` 是动态父物体挂载选项。默认勾选。在包含移动、升降平台的关卡，应取消勾选。

- `GridManager`

  `CampaignGameEnvironment/GridManager > QuadGridManager` 组件管理桌台等对齐网格的物体（厨房网格的大小为 1.2）。`size` 字段设置为 `(1.2, 1, 1.2)`，`origin` 字段设置为 0。`gridHalfSize` 为网格半长宽的格数。网格以 `GridManager` 物体所在位置为中心，向两边扩展 `gridHalfSize` 格（例如 `gridHalfSize` 为 `(1, 0, 1)`，网格格数为 `3x1x3`）。应该在保证网格覆盖所有桌台的前提下让 `gridHalfSize` 更小。先将 `GridManager` 物体移动到所有桌台位置的中心的格点，再设置 `gridHalfSize` 让网格覆盖所有桌台。

- Collision

  在 `Design/Collision` 下添加碰撞体。__注意碰撞体需要正确设置 Layer，地面为 `Ground`，墙为 `Walls`。__

- KillPlane

  `CampaignGameEnvironment/KillPlane` 是重生面，玩家或物体碰到了就会重生或消失。设置其位置和 Scale，设置上面的 `RespawnCollider > respawnType` 字段，`Drowning` - 落水，`Fall Death` - 坠落，`Car` - 倒地（`Hit` 废弃）。如果是 `Drowning`，在 `PseudoPrefabManager > PseudoPrefabManagerStub > OnDeathEffectSO` 选择 `WaterSplash_Particle_003_SO`（普通水花）或`WaterSplash_Particle_004_alien_SO`（外星主题的绿水水花），然后点击 Tools - Reload Pseudo Assets 载入。你也可以添加其他 KillPlane，只需要在空物体上添加 `Collider`（勾选 `isTrigger`）和 `Respawn Collider` 组件，参考 `s_oc1_story_6_3 > Design/KillPlanes`。

##### 动画

- 移动地面

  参考 `s_test_level_5 > Design/Platforms`。存在移动地面时，`levelinfo.disabledynamicparenting` 应取消勾选。__每个移动地面都应该添加 `ObjectContainer` 组件，保证移动地面的 `Collider` 在这个物体的层级之下。如果移动地面上包含桌台，应该单独添加 `QuadGridManager` 组件管理这个地面上的网格。__

- 移动桌台

  参考 `s_oc1_story_4_1 > Design/Animated Objects/MovingCountersUp`。__所有移动桌台必须在某个名为 `Animated Objects` 的物体的层级之下。__

- Trigger

  动画需要通过 Trigger 在主客机同步触发。如果是一个很长的动画，应该进行分段，用多个 Trigger 依次触发，防止主客机动画错位。同理循环动画应该在每次循环时通过 Trigger 触发。

  [Trigger 组件说明](reference.md#Trigger-组件)



#### 注意事项

- 保存和构建场景前先点击 Tools - Toggle Prepare For Building 清除临时加载的物体。
- 切换场景或关闭 Unity 前记得保存场景。如果忘了保存，在确认弹窗中点 Cancel 返回，不要点 Save。
- 不要操作临时加载的物体。
- 关卡集配置的命名必须为 `LevelSetInfo`。
- 不要在 `common*` 目录下创建新资源。应该创建在你的关卡集目录下，使得它被打包到你的关卡集 info 包或场景包中。
- 在 `LevelInfoSO.dependencies` 额外添加 BGM 所在 bundle。
- 如果发现桌台靠近时没有高光（桌台外观选 `CounterCampingSO` 会有这一问题），在 `MultiplayerGameCamera/Camera` 添加组件 `FogConfig` 并设置字段 `fogFar` 为 10000，`fogColour` 为纯黑。参考 `s_oc1_story_3_1`。
- 发布关卡集的新版本时，记得修改 `LevelSetInfoSO.version`。

