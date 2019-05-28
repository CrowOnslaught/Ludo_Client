using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludo_Client;
using System.Net.Sockets;
using System.Net;
using TMPro;

public class MainMenuMNGR : MonoBehaviour
{

    [Header("Containers")]
    [SerializeField] private GameObject m_loginPanel = null;
    [SerializeField] private GameObject m_menuPanel = null;
    [SerializeField] private GameObject m_queuePanel = null;
    [SerializeField] private GameObject m_rankingPanel = null;
    [SerializeField] private GameObject m_rejoinGameScrollContainer = null;
    [SerializeField] private GameObject m_rankingScrollContainer = null;

    [Header("Texts")]
    public TMP_InputField m_userNameInput;
    public TMP_InputField m_passwordInput;
    public TMP_InputField m_ipInput;
    public TextMeshProUGUI m_errorText;
    public TextMeshProUGUI m_scoreText;

    [Header("Prefabs")]
    public GameObject m_buttonPref;
    public GameObject m_rankingContainerPref;

    private NetworkMNGR m_network;

    public static MainMenuMNGR instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetUp();
        }
        else if(instance != this)
            Destroy(this.gameObject);
        
    }

    private void SetUp()
    {
        ChangeToLoginPanel();
        m_network = NetworkMNGR.instance;

        m_ipInput.placeholder.GetComponent<TextMeshProUGUI>().text = m_network.m_connectionIp;

#if UNITY_STANDALONE_WIN
        Screen.SetResolution(540, 960, false);
#endif
    }

    public void SetUpCurrentGameButtons(NetworkMessage message)
    {
        int l_amount = message.ReadInt();
        for (int i = 0; i < l_amount; i++)
        {
            GameObject l_button = GameObject.Instantiate(m_buttonPref, m_rejoinGameScrollContainer.transform);
            l_button.GetComponent<RejoinGameButton>().SetUp(message.ReadInt(), message.ReadByte() > 0, message.ReadString(), message.ReadString(), message.ReadString());
        }
    }

    public void SeUpRanking(NetworkMessage message)
    {
        int l_amount = message.ReadInt();
        for (int i = 0; i < l_amount; i++)
        {
            string l_name = message.ReadString();
            string l_score = message.ReadString();
            string l_pos = message.ReadString();

            GameObject l_ranking = Instantiate(m_rankingContainerPref, m_rankingScrollContainer.transform);
            l_ranking.GetComponent<RankingPlayerContainer>().SetUp(l_name, l_pos, l_score);
        }

        ChangeToRankingPanel();
    }

    #region PanelControllers


    public void ChangeToMenuPanel()
    {
        m_loginPanel.SetActive(false);
        m_queuePanel.SetActive(false);
        m_rankingPanel.SetActive(false);

        m_menuPanel.SetActive(true);
    }
    public void ChangeToLoginPanel()
    {
        m_queuePanel.SetActive(false);
        m_menuPanel.SetActive(false);
        m_rankingPanel.SetActive(false);


        m_loginPanel.SetActive(true);
    }
    public void ChangeToQueuePanel()
    {
        m_loginPanel.SetActive(false);
        m_menuPanel.SetActive(false);
        m_rankingPanel.SetActive(false);


        m_queuePanel.SetActive(true);
    }

    public void ChangeToRankingPanel()
    {
        m_loginPanel.SetActive(false);
        m_menuPanel.SetActive(false);
        m_queuePanel.SetActive(false);

        m_rankingPanel.SetActive(true);
    }

    public void ShowErrorText(string text)
    {
        m_errorText.text = text;
        m_errorText.gameObject.SetActive(true);
    }

    public void HideAllPanels()
    {
        m_queuePanel.SetActive(false);
        m_menuPanel.SetActive(false);
        m_loginPanel.SetActive(false);
    }

    #endregion

    #region OnButtonPress
    public void OnLoginSignInButton()
    {
        m_errorText.gameObject.SetActive(false);

        Debug.Log(m_userNameInput.text + "|" + m_passwordInput.text);
        if (m_userNameInput.text == string.Empty || m_passwordInput.text == string.Empty || m_userNameInput.text == null || m_passwordInput.text == null)
            return;

        m_network.m_networkConnection = new NetworkConnection(m_network.m_modifiedConnectionIp != ""? m_network.m_modifiedConnectionIp: m_network.m_connectionIp, 8130);

        if (m_network.m_networkConnection != null)
        {
            m_network.StartCoroutines();
        }
        else
            ShowErrorText("Could connect to the server. Please try again");
    }

    public void OnRefreshButton()
    {
        for (int i = 0; i < m_rejoinGameScrollContainer.transform.childCount; i++)
            Destroy(m_rejoinGameScrollContainer.transform.GetChild(i).gameObject);

        NetworkMNGR.instance.m_networkConnection.Send(MessageBuilder.RefreshCurrentGames());
    }

    public void OnRankingButton()
    {
        for (int i = 1; i < m_rankingScrollContainer.transform.childCount; i++)
            Destroy(m_rankingScrollContainer.transform.GetChild(i).gameObject);

        NetworkMNGR.instance.m_networkConnection.Send(MessageBuilder.Ranking());
    }

    public void OnJoinNewGameButton()
    {
        ChangeToQueuePanel();

        NetworkMessage l_message = MessageBuilder.JoinNewGame();
        m_network.m_networkConnection.m_client.Send(l_message);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnQuitQueueButton()
    {
        ChangeToMenuPanel();

        NetworkMessage l_message = MessageBuilder.QuitQueue();
        m_network.m_networkConnection.m_client.Send(l_message);
    }
    #endregion


}
