using Godot;
using System;
using GolfGame;

public class Players : Node
{
    Player _player1;
    Player _player2;
    Player _player3;
    Player _player4;
    Player[] _players;

    public override void _Ready()
    {
        _player1 = GetNode<Player>("Player1");
        _player2 = GetNode<Player>("Player2");
        _player3 = GetNode<Player>("Player3");
        _player4 = GetNode<Player>("Player4");

        _player1.Visible = false;
        _player2.Visible = false;
        _player3.Visible = false;
        _player4.Visible = false;

        _players = new[]
        {
            _player1,
            _player3,
            _player2,
            _player4,
        };

        GameCore.Player[] corePlayers = GameCore.GameManager.Instance.GetPlayers();
        for (int i = 0; i < corePlayers.Length; i++)
        {
            _players[i].SetPlayer(corePlayers[i]);
            _players[i].Visible = true;
        }

    }
}
