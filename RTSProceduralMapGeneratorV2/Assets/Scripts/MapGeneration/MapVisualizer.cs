using MapEntities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGeneration
{
    public class MapVisualizer : MonoBehaviour
    {
        public Tilemap BaseMap;
        public Tilemap TreeMap;
        public Tilemap ResourcesMap;
        public Tile MountainTile;
        public Tile WaterTile;
        public Tile GrassTile;
        public Tile TreeTile;
        public Tile SpawnTile;
        public Tile BCopperTile;
        public Tile BGoldTile;
        public Tile EmptyTile;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void DrawMap(Map map)
        {
            BaseMap.ClearAllTiles();
            TreeMap.ClearAllTiles();
            ResourcesMap.ClearAllTiles();
            foreach (var mapTile in map.Map1.Values)
            {
                if (mapTile.Type == TileType.Empty)
                {
                    BaseMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0), GrassTile);
                }
                else if (mapTile.Type == TileType.Mountain)
                {
                    BaseMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0),
                        MountainTile);
                }
                else if (mapTile.Type == TileType.Water)
                {
                    BaseMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0), WaterTile);
                }
                else if (mapTile.Type == TileType.Tree)
                {
                    TreeMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0), TreeTile);
                }
                else if (mapTile.Type == TileType.Gold)
                {
                    ResourcesMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0),
                        BGoldTile);
                }
                else if (mapTile.Type == TileType.Copper)
                {
                    ResourcesMap.SetTile(new Vector3Int(mapTile.Position.x, map.Height - mapTile.Position.y, 0),
                        BCopperTile);
                }
            }

            foreach (Player p in map.Players)
            {
                BaseMap.SetTile(new Vector3Int(p.StartingPosition.x, map.Height - p.StartingPosition.y, 0), SpawnTile);
            }
        }

        public void ClearGridMap()
        {
            
            BaseMap.ClearAllTiles();
            TreeMap.ClearAllTiles();
            ResourcesMap.ClearAllTiles();
            BaseMap.RefreshAllTiles();
            TreeMap.RefreshAllTiles();
            ResourcesMap.RefreshAllTiles();
        }
    }
}