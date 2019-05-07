using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Ludo_Client;

public class BoardMNGR : MonoBehaviour
{
    [Header("Game Pieces")]
    public GameObject[] m_redPieces;
    public GameObject[] m_bluePieces, m_greenPieces, m_yellowPieces;
    [Header("Game Info")]
    public TileScript[] m_boardTiles;
    public Enums.Colors m_currentTurn;
    public GameObject[] m_diceFaces;
    public TextMeshProUGUI m_gameText;
    public GameObject m_blackCanvas;

    [Header("Player Info")]
    public TextMeshProUGUI[] m_playerName;
    public GameObject[] m_playerTurn;
    public bool m_isLocalTurn;


    public static BoardMNGR instance;

    private void Awake()
    {
        m_blackCanvas.SetActive(true);
    }
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            SetUpBoard(GameMNGR.instance.m_allPlayersInfo);
        }
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void SetUpBoard(GameMNGR.PlayerInfo[] allPlayers)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            GameMNGR.PlayerInfo l_pi = allPlayers[i];
            for (int j = 0; j < l_pi.m_piecePos.Length; j++)
                MovePiece(0, l_pi.m_piecePos[j], l_pi.m_color);

            m_playerName[i].text = l_pi.m_name;
            if(l_pi.m_currentTurn)
            {
                m_playerTurn[i].SetActive(true);
                m_currentTurn = l_pi.m_color;
            }
            else
                m_playerTurn[i].SetActive(false);

        }

        m_blackCanvas.SetActive(false);
    }

    public void MovePiece(int originTileID, int targetTileID, Enums.Colors pieceColor)
    {
        if (originTileID >= 0)
        {
            GameObject l_selectedPiece = GetPieceByColorAndTile(originTileID, pieceColor);

            if (l_selectedPiece != null)
            {
                //Get Tiles Info
                TileScript l_originTile = GetTileIDByPositionAndColor(originTileID, pieceColor);
                TileScript l_targetTile = GetTileIDByPositionAndColor(targetTileID, pieceColor);

                //Move Piece physically
                l_selectedPiece.transform.SetParent(l_targetTile.transform);
                l_selectedPiece.transform.localPosition = Vector3.zero;

                //Move piece from TileInfoes
                l_originTile.m_childPieces.Remove(l_selectedPiece);
                l_targetTile.m_childPieces.Add(l_selectedPiece);

                //Set Piece Physically if there is other piece on that tile
                if (l_targetTile.m_childPieces.Count == 2)
                {
                    for (int i = 0; i < l_targetTile.m_childPieces.Count; i++)
                    {
                        Transform l_pieceTrans = l_targetTile.m_childPieces[i].transform;
                        l_pieceTrans.localPosition = Vector3.zero + Vector3.right * (-0.2f + 0.4f * i);
                    }
                }
                else if (l_targetTile.m_childPieces.Count >= 3) //Home or LastTile
                    for (int i = 0; i < l_targetTile.m_childPieces.Count; i++)
                    {
                        Transform l_pieceTrans = l_targetTile.m_childPieces[i].transform;
                        l_pieceTrans.localPosition = Vector3.zero + Vector3.right * (-0.2f + 0.4f * i) + Vector3.up * (i%2==0?0.3f : -0.3f);
                    }
            }
            else
                Debug.Log("PIECE NOT FOUND");
        }
        else
            Debug.Log("INVALID MOVEMENT: " + originTileID + "|" + targetTileID);
    }
    public void ChangeTurn(Enums.Colors turnColor, bool isLocalTurn)
    {
        Debug.Log("CHANGE TURN TO " + turnColor.ToString());

        for (int i = 0; i < m_playerTurn.Length; i++)
            m_playerTurn[i].gameObject.SetActive(i == (int)turnColor);

        m_currentTurn = turnColor;
        m_isLocalTurn = isLocalTurn;
        m_gameText.text = "TAP TO ROLL!";
        m_gameText.gameObject.SetActive(isLocalTurn);
    }

#region TileHandlers
    private GameObject GetPieceByColorAndTile(int tile, Enums.Colors pieceColor)
    {
        TileScript l_tile = GetTileIDByPositionAndColor(tile, pieceColor);
        return l_tile.m_childPieces.FirstOrDefault(x => x.name.ToLower().Contains(pieceColor.ToString()));
    }

    private TileScript GetTileIDByPositionAndColor(int position, Enums.Colors color)
    {
        TileScript l_tile = null;
        switch (color)
        {
            case Enums.Colors.red:
                l_tile = m_boardTiles.FirstOrDefault(x => x.m_redTileID == position);
                break;
            case Enums.Colors.green:
                l_tile = m_boardTiles.FirstOrDefault(x => x.m_greenTileID == position);
                break;
            case Enums.Colors.blue:
                l_tile = m_boardTiles.FirstOrDefault(x => x.m_blueTileID == position);
                break;
            case Enums.Colors.yellow:
                l_tile = m_boardTiles.FirstOrDefault(x => x.m_yellowTileID == position);
                break;
        }

        return l_tile;
    }
#endregion
}
