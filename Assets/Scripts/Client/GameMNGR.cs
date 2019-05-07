using Ludo_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        {
            m_allPlayersInfo[i] = playerList[i];
        }

        Debug.Log(AllPlayersToString());
        SceneManager.LoadScene("GameScene");
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
