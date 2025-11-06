using System.Collections.Generic;
using System.Linq;

public class PlayerRegistry
{
    private readonly List<PlayerInfoState> _players = new();

    public void AddPlayer(PlayerInfoState player) => _players.Add(player);
    public IEnumerable<PlayerInfoState> GetAllPlayers() => _players;

    public PlayerInfoState GetPlayerById(string userId) => _players.FirstOrDefault(p => p.UserId == userId);

    public IEnumerable<PlayerInfoState> GetTeam(string teamName) =>
        _players.Where(p => p.TeamName == teamName);

    public IEnumerable<PlayerInfoState> GetOpponents(string myTeamName) =>
        _players.Where(p => p.TeamName != myTeamName);
}
