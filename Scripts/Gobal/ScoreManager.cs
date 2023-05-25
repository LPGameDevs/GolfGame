using System.Collections.Generic;
using GameCore;
using GameCore.Players;
using GameCore.States;
using Godot;

namespace GolfGame
{
    public class ScoreManager : Node
    {
        private List<GameCore.Players.Player> _players = new List<GameCore.Players.Player>();

        public int GetPlayerScore(PlayerId playerId)
        {
            foreach (var player in _players)
            {
                if (player.Id == playerId)
                {
                    return player.Score();
                }
            }

            throw new PlayerNotFoundException();
        }

        private void OnFetchPlayers(List<GameCore.Players.Player> playerList)
        {
            _players = playerList;
        }

        public override void _EnterTree()
        {
            PlayerManager.OnFetchPlayers += OnFetchPlayers;
        }

        public override void _ExitTree()
        {
            PlayerManager.OnFetchPlayers -= OnFetchPlayers;
        }
    }
}
