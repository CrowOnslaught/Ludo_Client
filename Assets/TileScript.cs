using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [Header ("TileID by Color")]
    public int m_redTileID;
    public int m_blueTileID;
    public int m_yellowTileID;
    public int m_greenTileID;

    [Header("Tile Info")]
    public bool m_safeTile = false;
    public List<GameObject> m_childPieces = new List<GameObject>();
    public bool IsEmpty { get { return m_childPieces.Count == 0; } }
}
