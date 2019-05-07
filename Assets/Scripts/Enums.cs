using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
    public enum Colors {red = 0, blue, green, yellow }
    public enum ConnectionState { Disconnected, Connecting, Connected }
    public enum TurnState {RollDice, SelectPiece, None }

    public enum MessageType : byte
    {
        welcome = 0x00,
        ping = 0x01,
        logIn = 0x02,
        joinNewGame = 0x03,
        startNewGame = 0x04,
        loginFailed = 0x05,
        changeTurn = 0x06,
        movePiece = 0x07,
        rollDice = 0x08,
        choosePiece = 0x09,
    }

}
