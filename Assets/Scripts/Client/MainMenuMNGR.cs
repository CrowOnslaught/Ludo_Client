using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludo_Client;
using System.Net.Sockets;
using System.Net;
using TMPro;

public class MainMenuMNGR : MonoBehaviour
{

    [SerializeField] private GameObject m_loginPanel = null;
    [SerializeField] private GameObject m_menuPanel = null;
    [SerializeField] private GameObject m_queuePanel = null;
    public TMP_InputField m_userNameInput;
    public TMP_InputField m_passwordInput;
    public TextMeshProUGUI m_errorText;
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

        Screen.SetResolution(540, 960, false);
    }

    #region PanelControllers


    public void ChangeToMenuPanel()
    {
        m_loginPanel.SetActive(false);
        m_queuePanel.SetActive(false);

        m_menuPanel.SetActive(true);
    }
    public void ChangeToLoginPanel()
    {
        m_queuePanel.SetActive(false);
        m_menuPanel.SetActive(false);

        m_loginPanel.SetActive(true);
    }
    public void ChangeToQueuePanel()
    {
        m_loginPanel.SetActive(false);
        m_menuPanel.SetActive(false);

        m_queuePanel.SetActive(true);
    }

    public void ShowErrorText(string text)
    {
        m_errorText.text = text;
        m_errorText.gameObject.SetActive(true);
    }

    #endregion

    #region OnButtonPress
    public void OnLoginSignInButton()
    {
        m_errorText.gameObject.SetActive(false);

        Debug.Log(m_userNameInput.text + "|" + m_passwordInput.text);
        if (m_userNameInput.text == string.Empty || m_passwordInput.text == string.Empty || m_userNameInput.text == null || m_passwordInput.text == null)
            return;

        m_network.m_networkConnection = new NetworkConnection(IPAddress.Loopback, 8130);

        if (m_network.m_networkConnection != null)
        {
            m_network.StartCoroutines();
        }
        else
            ShowErrorText("Could connect to the server. Please try again");
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
