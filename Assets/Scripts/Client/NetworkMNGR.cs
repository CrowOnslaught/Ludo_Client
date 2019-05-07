using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludo_Client;
using System.Net.Sockets;
using System.Net;
using TMPro;

public class NetworkMNGR : MonoBehaviour
{
    private TcpClient m_tcpClient;
    private Client m_thisClient;
    public NetworkConnection m_networkConnection;
    [SerializeField]private GameObject m_GameMngrPref = null;

    public static NetworkMNGR instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
            Destroy(this.gameObject);
        
    }

    public void StartCoroutines()
    {
        StartCoroutine(ReadMessages());
        StartCoroutine(ConstantPingCoroutine());
    }

    public void StartMatch(int roomID, List<GameMNGR.PlayerInfo> playerList)
    {
        GameMNGR l_gameManager = GameObject.Instantiate(m_GameMngrPref).GetComponent<GameMNGR>();
        l_gameManager.m_roomID = roomID;
        l_gameManager.SetUp(playerList);
    }

    #region ConstantThreads
    private IEnumerator ReadMessages()
    {
        while (m_networkConnection != null)
        {
            if (m_networkConnection.m_client != null)
            {
                m_networkConnection.m_client.Reading();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator ConstantPingCoroutine()
    {
        NetworkMessage l_ping = MessageBuilder.Ping();
        while (m_networkConnection != null)
        {
            m_networkConnection.Send(l_ping);
            yield return new WaitForSeconds(2);
        }
    }
#endregion

}
