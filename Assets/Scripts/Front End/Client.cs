using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

namespace Ludo_Client
{
    public class Client
    {
        private TcpClient m_tcpClient;
        private NetworkStream m_clientStream;

        public Client(TcpClient tcpClient)
        {
            if (m_clientStream == null)
            {
                m_tcpClient = tcpClient;
                m_clientStream = m_tcpClient.GetStream(); //Canal de entrada de datos
            }
            else
                CloseClient();
        }

        public void Reading() //Lectura de paquetes
        {
            if (m_clientStream.DataAvailable)
            {
                byte[] l_size = new byte[4];
                m_clientStream.Read(l_size, 0, 4);

                byte[] l_messageRecived = new byte[BitConverter.ToUInt16(l_size, 0)];
                m_clientStream.Read(l_messageRecived, 0, l_messageRecived.Length);

                NetworkMessage l_message = new NetworkMessage(this, l_messageRecived);
                Execute(l_message);
            }
        }

        public void Send(NetworkMessage message) //Enviar paquete
        {
            byte[] l_messageSize;
            byte[] l_messageResult = new byte[message.m_raw.Length + 4];

            try
            {
                l_messageSize = BitConverter.GetBytes(message.m_raw.Length);
                Array.Copy(l_messageSize, 0, l_messageResult, 0, 4);
                Array.Copy(message.m_raw, 0, l_messageResult, 4, message.m_raw.Length);

                m_clientStream.Write(l_messageResult, 0, l_messageResult.Length);
            }
            catch
            {
                CloseClient();
                Debug.Log("Error 001: Sending Message Error");
            }
        }

        public void Execute(NetworkMessage message) //Ejecutar paquete recivido
        {
            Debug.Log("Message:"+ message.m_type);
            switch (message.m_type)
            {
                case MessageType.welcome:
                    break;
                case MessageType.logIn:
                    Debug.Log("LogIn success!");
                    MainMenuMNGR.instance.ChangeToMenuPanel();
                    MainMenuMNGR.instance.m_scoreText.text = "Score: " + message.ReadInt().ToString();
                    break;
                case MessageType.loginFailed:
                    MainMenuMNGR.instance.ShowErrorText("Wrong password or new account already exists");
                    break;
                case MessageType.startNewGame:
                    int l_roomID = message.ReadInt();
                    Debug.Log("Recieved GameID:" + l_roomID);
                    List<GameMNGR.PlayerInfo> l_allPlayerInfo = new List<GameMNGR.PlayerInfo>();
                    for (int i = 0; i < 4; i++)
                    {
                        GameMNGR.PlayerInfo l_pi = new GameMNGR.PlayerInfo();
                        l_pi.m_id = message.ReadInt();
                        l_pi.m_name = message.ReadString();
                        l_pi.m_color = (Colors)message.ReadInt();
                        l_pi.m_currentTurn = message.ReadByte() > 0;

                        l_pi.m_piecePos = new int[4];
                        for (int j = 0; j < 4; j++)
                            l_pi.m_piecePos[j] = message.ReadInt();

                        l_allPlayerInfo.Add(l_pi);
                    }

                    NetworkMNGR.instance.StartMatch(l_roomID, l_allPlayerInfo);
                    break;
                case MessageType.changeTurn:
                    Colors l_turnColor = (Colors)message.ReadInt();
                    bool l_localTurn = message.ReadByte() > 0;
                    if (BoardMNGR.instance != null)
                        BoardMNGR.instance.ChangeTurn(l_turnColor, l_localTurn);
                    break;
                case MessageType.rollDice:
                    int l_diceResult = message.ReadInt();
                    BoardMNGR.instance.OnDiceRolledMessage(l_diceResult);
                    break;
                case MessageType.choosePiece:
                    BoardMNGR.instance.OnChosePieceMessage();
                    break;
                case MessageType.movePiece:
                    Colors l_color = (Colors)message.ReadInt();
                    int l_originID = message.ReadInt();
                    int l_destID = message.ReadInt();

                    BoardMNGR.instance.MovePiece(l_originID, l_destID, l_color);
                    break;
                case MessageType.currentGames:
                    MainMenuMNGR.instance.SetUpCurrentGameButtons(message);
                    break;
                case MessageType.endMatch:
                    int l_position = message.ReadInt();
                    break;
                case MessageType.ranking:
                    MainMenuMNGR.instance.SeUpRanking(message);
                    break;
                default:
                    Debug.LogError("Error 002: Unknown type of Message");
                    break;
            }
        }

        public void CloseClient()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
            else
                MainMenuMNGR.instance.ChangeToLoginPanel();

            NetworkMNGR.instance.m_networkConnection = null;
        }

        private void Close()
        { 
            m_tcpClient.Close();
        }
    }
}
