using Ludo_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameMNGR : MonoBehaviour
{
    public struct PlayerInfo
    {
        public int m_id;
        public string m_name;
        public Enums.Colors m_color;

        public int[] m_piecePos;
        public bool m_currentTurn;
    }

    public static GameMNGR instance;

    public Client m_localClient;
    public PlayerInfo[] m_allPlayersInfo;

    [Header("Game Info")]
    public int m_roomID = -1;
    public bool m_isLocalTurn = false;
    public TurnState m_turnState = TurnState.None;
    public Vector2 m_mouseOver;

    public void SetUp(List<PlayerInfo> playerList)
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
            Destroy(this.gameObject);


        m_allPlayersInfo = new PlayerInfo[playerList.Count];
        for (int i = 0; i < playerList.Count; i++)
            m_allPlayersInfo[i] = playerList[i];

        Debug.Log(AllPlayersToString());
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            m_localClient.CloseClient();

        if (!m_isLocalTurn)
            return;

        if (Input.touchCount > 0)
        {
            Touch l_touch = Input.GetTouch(0);
            if (l_touch.phase == TouchPhase.Began)
                PressScreen(l_touch.position);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
            PressScreen(Input.mousePosition);
    }
    private void PressScreen(Vector3 clickPos)
    {
        switch (m_turnState)
        {
            case TurnState.RollDice:
                NetworkMNGR.instance.m_networkConnection.Send(MessageBuilder.RollDice(m_roomID));
                m_turnState = TurnState.None;
                BoardMNGR.instance.m_gameText.text = "";
                break;
            case TurnState.SelectPiece:
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPos), out hit, 25))
                {
                    GameObject l_hitObject = hit.collider.gameObject;
                    Debug.Log(l_hitObject.name);
                    if (l_hitObject.CompareTag("Ludo_Piece"))
                        if (l_hitObject.name.ToLower().Contains(BoardMNGR.instance.m_currentTurn.ToString()))
                        { 
                            NetworkMessage l_message = MessageBuilder.ChoosePiece(m_roomID, BoardMNGR.instance.GetTileIdByPieceAndColor(l_hitObject, BoardMNGR.instance.m_currentTurn));
                            NetworkMNGR.instance.m_networkConnection.Send(l_message);

                            m_turnState = TurnState.None;
                            BoardMNGR.instance.m_gameText.text = "";
                        }
                }
                else
                    Debug.Log("RAYCAST DIDNT HIT D:");
                break;
            case TurnState.None:
                break;
        }
    }
    public string AllPlayersToString()
    {
        string l_result = "";
        for (int i = 0; i < m_allPlayersInfo.Length; i++)
        {
            l_result += m_allPlayersInfo[i].m_name + "|" +
                      m_allPlayersInfo[i].m_id + "|" +
                      m_allPlayersInfo[i].m_color + "|" +
                      m_allPlayersInfo[i].m_piecePos[0] + " " + m_allPlayersInfo[i].m_piecePos[1] + " " + m_allPlayersInfo[i].m_piecePos[2] + " " + m_allPlayersInfo[i].m_piecePos[3] + "\n";

        }

        return l_result;
    }
}
