using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RejoinGameButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_currentTurnText = null;
    [SerializeField] private TextMeshProUGUI m_player2NameText = null;
    [SerializeField] private TextMeshProUGUI m_player3NameText = null;
    [SerializeField] private TextMeshProUGUI m_player4NameText = null;
    private int m_roomID = -1;

    public void OnPress()
    {
        //Send Message To Server
    }

    public void SetUp(int roomID, bool currentTurn, string player2, string player3, string player4)
    {
        m_currentTurnText.text = currentTurn ? "Your Turn" : "Opponent Turn";
        m_currentTurnText.color = currentTurn ? Color.green : Color.red;

        m_player2NameText.text = player2;
        m_player3NameText.text = player3;
        m_player4NameText.text = player4;

        m_roomID = roomID;
    }
}
