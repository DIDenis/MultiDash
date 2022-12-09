using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Player : NetworkBehaviour
{
    [SyncVar] int score;
    public int Score => score;
    PlayerMovement player;

    public override void OnStartClient()
    {
        player = GetComponent<PlayerMovement>();
        if (isLocalPlayer)
            gameObject.name = "Player " + netId + " (You)";
        else
            gameObject.name = "Player " + netId;
        StartCoroutine(WaitingServerStart());
    }

    IEnumerator WaitingServerStart()
    {
        yield return new WaitWhile(() => !NetworkServer.active);
        Referee.instance.SetupPlayer(this);
    }

    [ServerCallback]
    public void OnTriggerEnterInChildren(Collider other)
    {
        if (other.CompareTag("Player"))
            if (other.GetComponent<ColorChanger>().Hit())
                SetScore(score + 1);
    }

    [Server]
    public void SetScore(int newValue)
    {
        score = newValue;
    }

    [Server]
    public void StopMatch()
    {
        player.RpcStopMatch();
    }
    [Server]
    public void RestartPlayer(Vector3 position)
    {
        player.RpcRestartPlayer(position);
    }
}
