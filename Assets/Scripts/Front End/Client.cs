using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
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
                Close();
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

        public void Send(NetworkMessage message)
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
                Close();
                Debug.Log("Error 001: Sending Message Error");
            }
        }

        public void Execute(NetworkMessage message)
        {
            Debug.Log("Message Received from Server of type "+ message.m_type);
            switch (message.m_type)
            {
                case MessageType.welcome:
                    break;
                case MessageType.logIn:
                    Debug.Log("LogIn success!");
                    MainMenuMNGR.instance.ChangeToMenuPanel();
                    break;
                case MessageType.loginFailed:
                    MainMenuMNGR.instance.ShowErrorText("Wrong password or new account already exists");
                    Debug.Log("LogIn Failed...");
                    break;
                case MessageType.startNewGame:
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

                    NetworkMNGR.instance.StartMatch(l_allPlayerInfo);
                    break;
                case MessageType.changeTurn:
                    Colors l_turnColor = (Colors)message.ReadInt();
                    bool l_localTurn = message.ReadByte() > 0;
                    if (BoardMNGR.instance != null)
                        BoardMNGR.instance.ChangeTurn(l_turnColor, l_localTurn);
                    break;
                default:
                    Debug.LogError("Error 002: Unknown type of Message");
                    break;
            }
        }

        private void Close()
        {
            MainMenuMNGR.instance.ChangeToLoginPanel();
            m_tcpClient.Close();
        }
    }
}
