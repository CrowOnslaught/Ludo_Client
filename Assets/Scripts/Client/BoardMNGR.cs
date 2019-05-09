using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Ludo_Client;
using static Enums;

public class BoardMNGR : MonoBehaviour
{
    [Header("Game Pieces")]
    public GameObject[] m_redPieces;
    public GameObject[] m_bluePieces, m_greenPieces, m_yellowPieces;
    [Header("Game Info")]
    public TileScript[] m_boardTiles;
    public Colors m_currentTurn;
    public GameObject[] m_diceFaces;
    public TextMeshProUGUI m_gameText;
    public GameObject m_blackCanvas;

    [Header("Player Info")]
    public TextMeshProUGUI[] m_playerName;
    public GameObject[] m_playerTurn;


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

    public void MovePiece(int originTileID, int targetTileID, Colors pieceColor)
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

                if(targetTileID == 72)
                    Destroy(l_selectedPiece.GetComponent<SphereCollider>());
            }
            else
                Debug.Log("PIECE NOT FOUND");
        }
        else
            Debug.Log("INVALID MOVEMENT: " + originTileID + "|" + targetTileID);
    }
    public void ChangeTurn(Colors turnColor, bool isLocalTurn)
    {
        Debug.Log("CHANGE TURN TO " + turnColor.ToString());
        for (int i = 0; i < m_playerTurn.Length; i++)
            m_playerTurn[i].gameObject.SetActive(i == (int)turnColor);

        for (int i = 0; i < m_diceFaces.Length; i++)
            m_diceFaces[i].SetActive(false);

        m_currentTurn = turnColor;
        GameMNGR.instance.m_isLocalTurn = isLocalTurn;
        GameMNGR.instance.m_turnState = isLocalTurn ? TurnState.RollDice : TurnState.None;
        m_gameText.gameObject.SetActive(isLocalTurn);
        m_gameText.text = "TAP TO ROLL!";
    }
    public void OnDiceRolledMessage(int diceResult)
    {
        m_diceFaces[diceResult - 1].SetActive(true);
    }
    public void OnChosePieceMessage()
    {
        GameMNGR.instance.m_turnState = GameMNGR.instance.m_isLocalTurn ? TurnState.SelectPiece : TurnState.None;
        m_gameText.gameObject.SetActive(GameMNGR.instance.m_isLocalTurn);
        m_gameText.text = "SELECT A PIECE!";
    }

#region TileHandlers
    public GameObject GetPieceByColorAndTile(int tile, Enums.Colors pieceColor)
    {
        TileScript l_tile = GetTileIDByPositionAndColor(tile, pieceColor);
        return l_tile.m_childPieces.FirstOrDefault(x => x.name.ToLower().Contains(pieceColor.ToString()));
    }
    public TileScript GetTileIDByPositionAndColor(int position, Enums.Colors color)
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
    public TileScript GetTileByPiece(GameObject piece)
    {
        return m_boardTiles.FirstOrDefault(x => x.m_childPieces.Contains(piece));
    }
    public int GetTileIdByPieceAndColor(GameObject piece, Colors pieceColor)
    {
        TileScript l_tile = GetTileByPiece(piece);
        int l_tileId = -1;
        switch (pieceColor)
        {
            case Colors.red:
                l_tileId = l_tile.m_redTileID;
                break;
            case Colors.blue:
                l_tileId = l_tile.m_blueTileID;
                break;
            case Colors.green:
                l_tileId = l_tile.m_greenTileID;
                break;
            case Colors.yellow:
                l_tileId = l_tile.m_yellowTileID;
                break;

        }

        return l_tileId;
    }

#endregion
}
