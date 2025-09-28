using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Surface Data", menuName = "Game Data/Tile Surface Data")]
public class TileSurfaceData : ScriptableObject
{
    [System.Serializable]
    public class TileSurfaceMapping
    {
        public TileBase tile; 
        public SurfaceType surfaceType; 
    }

    public List<TileSurfaceMapping> tileMappings;
}
