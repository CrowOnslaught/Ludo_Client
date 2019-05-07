﻿using Ludo_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class MessageBuilder
{
    public static NetworkMessage Welcome()
    {
        NetworkMessage l_message = new NetworkMessage();
        //Body
        l_message.Write("HOLA D:");
        //
        l_message.Build(MessageType.welcome);
        return l_message;
    }

    public static NetworkMessage LogIn(string userName, string userPassword)
    {
        NetworkMessage l_message = new NetworkMessage();
        //Body
        l_message.Write(userName);
        l_message.Write(userPassword);
        //
        l_message.Build(MessageType.logIn);
        return l_message;
    }

    public static NetworkMessage Ping()
    {
        NetworkMessage l_message = new NetworkMessage();

        l_message.Build(MessageType.ping);
        return l_message;
    }

    public static NetworkMessage JoinNewGame()
    {
        NetworkMessage l_message = new NetworkMessage();

        l_message.Build(MessageType.joinNewGame);
        return l_message;
    }
}