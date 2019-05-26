using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingPlayerContainer : MonoBehaviour
{
    public TextMeshProUGUI m_playernameText, m_rankingText, m_scoreText;

    public void SetUp(string name, string ranking, string score)
    {
        m_playernameText.text = name;
        m_rankingText.text = ranking;
        m_scoreText.text = score;
    }
}
