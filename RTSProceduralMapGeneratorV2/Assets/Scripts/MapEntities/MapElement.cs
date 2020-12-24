using UnityEngine;

namespace MapEntities
{
    public class MapElement
    {
        private TileType _type;
        private Vector2Int _position;

        public MapElement(TileType type, Vector2Int position)
        {
            this._type = type;
            this._position = position;
        }

        public TileType Type
        {
            get => _type;
            set => _type = value;
        }

        public Vector2Int Position
        {
            get => _position;
            set => _position = value;
        }
    }
}
