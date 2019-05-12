using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System;
using Ludo_Client;
using static Enums;

namespace Ludo_Client
{
    public class NetworkConnection
    {

        public int m_currentPackageID;

        ConnectionState m_state;
        TcpClient m_TcpClient = new TcpClient();
        public Client m_client;

        NetworkStream m_stream;
        Thread m_readThread;
        int m_readInterval = 500;

        public bool m_isConnected { get { return m_state == ConnectionState.Connected; } }

        internal NetworkConnection(IPAddress ip, int port)
        {
            IAsyncResult l_connection = m_TcpClient.BeginConnect(ip, port, OnConnected, null);
            m_state = ConnectionState.Connecting;
            m_currentPackageID = 0;
        }


        void OnConnected(IAsyncResult result)
        {
            try
            {
                m_TcpClient.EndConnect(result);
                m_stream = m_TcpClient.GetStream();
                m_client = new Client(m_TcpClient);
                Debug.Log("CONNECTED! :D");

                m_state = ConnectionState.Connected;
                m_readThread = new Thread(ReadLoop);
                m_readThread.Start();

                m_currentPackageID = 0;

                NetworkMessage l_message = MessageBuilder.LogIn(MainMenuMNGR.instance.m_userNameInput.text, MainMenuMNGR.instance.m_passwordInput.text);
                Send(l_message);
            }
            catch
            {
                Debug.LogError("Could not connect to the server|" + result.AsyncState.ToString());
            }
        }

        void ReadLoop()
        {
            while (m_isConnected)
            {
                while (m_stream.DataAvailable)
                {
                    ProcessIncomingData();
                }
                Thread.Sleep(m_readInterval);
            }
        }

        void ProcessIncomingData()
        {
            try
            {
                bool l_fastForward = false;
                byte[] l_bMessage = ReadFullMessage(out l_fastForward);
                if (l_bMessage.Length > 0)
                {

                    NetworkMessage l_netMessage = new NetworkMessage(new Client(m_TcpClient), l_bMessage);
                }
            }
            catch (Exception error)
            {
                Debug.Log(error.ToString());
            }
        }

        byte[] ReadFullMessage(out bool fastForward)
        {
            fastForward = false;
            int newPackageID = m_currentPackageID + 1;

            //Debug.Log("Checking ID - " + newPackageID + " - current: " + m_currentPackageID);

            int l_size = ReadSize();
            byte[] l_read = new byte[l_size];
            int l_totalRead = 0;

            while (l_totalRead < l_size)
            {
                byte[] l_tempByte = new byte[l_size - l_totalRead];
                int l_bytesRead = m_stream.Read(l_tempByte, 0, l_tempByte.Length);
                Array.Copy(l_tempByte, 0, l_read, l_totalRead, l_bytesRead);

                l_totalRead += l_bytesRead;

                if (l_totalRead < l_size)
                    Thread.Sleep(m_readInterval);
            }
            m_currentPackageID = newPackageID;
            return l_read;
       
        }

        int ReadSize()
        {
            byte[] bSize = new byte[4];
            m_stream.Read(bSize, 0, 4);
            int size = BitConverter.ToInt32(bSize, 0);
            return size;
        }

        int ReadPackageID()
        {
            byte[] bID = new byte[4];
            m_stream.Read(bID, 0, 4);
            return BitConverter.ToInt32(bID, 0);
        }

        public bool Send(NetworkMessage message)
        {

            try
            {
                if(m_client == null)
                    m_client = new Client(m_TcpClient);

                m_client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            if (m_TcpClient != null)
                m_TcpClient.Close();
            if (m_stream != null)
                m_stream.Close();
            m_state = ConnectionState.Disconnected;

            Debug.Log("DISCONNECTED D:");
        }


    }
}
