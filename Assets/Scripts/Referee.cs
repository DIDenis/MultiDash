using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using System.Collections;

public class Referee : NetworkBehaviour
{
    public static Referee instance { get; private set; }

    [SerializeField] MatchView view;
    [SerializeField] int timeForRestart;
    List<Player> players;

    void Awake()
    {
        instance = this;
        players = new List<Player>();
    }

    [Server]
    public void SetupPlayer(Player player)
    {
        players.Add(player);
        view.RpcCreateLines(players);
    }

    [ServerCallback]
    void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            view.RpcUpdateScore(players[i], i);
            if (players[i].Score >= 3)
                StartCoroutine(RestartMatch(players[i]));
        }
    }

    [Server]
    IEnumerator RestartMatch(Player player)
    {
        this.enabled = false;

        view.RpcWinner(player);
        for (int i = 0; i < players.Count; i++)
            players[i].StopMatch();

        int time = timeForRestart;
        while (time > 0)
        {
            view.RpcUpdateTimer(time);
            yield return new WaitForSeconds(1);
            time--;
        }

        view.RpcCloseWinnerWindow();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetScore(0);
            Transform point = RoomManager.singleton.GetStartPosition();
            players[i].RestartPlayer(point.position);
        }

        this.enabled = true;
    }
}
