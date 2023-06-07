
using System.IO;
using UnityEngine;

public static class Define
{
    
    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }
    
    public enum StageState
    {
        Open,
        Lock,
        Clear
    }
    
    public enum BaseMapSize
    {
        X = 7,
        Y = 32,
    }
    
    public enum TileNames
    {
        grass,
        grassSpawn,
        sand,
        sandSpawn,
        snow,
        snowSpawn,
        road_spring_1,
        road_spring_2,
        road_spring_3,
        road_spring_4,
        road_spring_5,
        road_spring_6,
        road_spring_7,
        road_spring_8,
        road_spring_9,
        road_spring_10,
        road_spring_11,
        road_desert_1,
        road_desert_2,
        road_desert_3,
        road_desert_4,
        road_desert_5,
        road_desert_6,
        road_desert_7,
        road_desert_8,
        road_desert_9,
        road_desert_10,
        road_desert_11,
        road_winter_1,
        road_winter_2,
        road_winter_3,
        road_winter_4,
        road_winter_5,
        road_winter_6,
        road_winter_7,
        road_winter_8,
        road_winter_9,
        road_winter_10,
        road_winter_11,
    }
    public enum EnvironmentTileNames
    {
        cactus_1,
        cactus_2,
        cactus_3,
        cactus_4,
        cactus_5,
        sand_decoration_1,
        sand_decoration_2,
        sand_decoration_3,
        sand_decoration_4,
        sand_decoration_5,
        sand_decoration_6,
        sand_decoration_7,
        sand_decoration_8,
        sand_decoration_9,
        stone_sand_1,
        stone_sand_2,
        stone_sand_3,
        stone_sand_4,
        grass_decoration_1,
        grass_decoration_2,
        grass_decoration_3,
        grass_decoration_4,
        grass_decoration_5,
        grass_decoration_6,
        grass_decoration_7,
        grass_decoration_8,
        grass_decoration_9,
        stone_1,
        stone_2,
        stone_3,
        stone_4,
        stone_ground_1,
        stone_ground_2,
        stone_ground_3,
        stone_ground_4,
        tree_1,
        tree_2,
        snow_decoration_1,
        snow_decoration_2,
        snow_decoration_3,
        snow_decoration_4,
        snow_decoration_5,
        snow_decoration_6,
        snow_decoration_7,
        snow_decoration_8,
        snow_decoration_9,
        tree_winter_1,
        tree_winter_2,
    }
    
    public enum GameFuncTileNames
    {
        SpawnTile
    }

    public const float BASE_SPRITE_SIZE_X = 2.5f;
    public const float BASE_SPRITE_SIZE_Y = 2.35f;

    public static Vector3 MAP_BASE_POS = new Vector3(-8, -11, 1);

    public enum LayerNames
    {
        Environment = 3,
        Tile = 6,
        TowerSpawn = 7,
        Tower = 8,
        SpawnPos = 9,
    }
    
    public enum SortingLayerNum
    {
        Tile = 3,
        Environment = 5,
        Enemy = 10,
        UI = 11,
        Projectile = 12,
    }

    public const string ENEMY_DATA_PATH = "Data/Enemy";
    public const string TOWER_DATA_PATH = "Data/Tower";
    public const string STAGE_DATA_PATH = "Data/Stage";
    public const string CHAPTER_DATA_PATH = "Data/Chapter";
    public const string STAGE_MAP_DATA_PATH = "Data/StageMapData";
    public const string STAGE_CURRENT_DATA_PATH = "Data/StageStatus";
    
    public const string tileMapObjName = "TileMap";
    public const string wayPointObjName = "WayPoint";
    public const string enviroMapObjName = "EnvironmentMap";
    public const string towerSpawnMapObjName = "TowerSpawnMap";

    public const int stageLife = 10;
    public const float minDistanceToDealDamage = 0.1f;

    public const int firstTowerSpawnCost = 10;
    public const int firstStageGold = 10000;

    public static readonly string stageMapDataPath = Path.Combine(Application.dataPath, "Resources/Data/StageMapData");
    public static readonly string stageCurrentDataPath = Path.Combine(Application.dataPath, "Resources/Data/StageStatus");
}
