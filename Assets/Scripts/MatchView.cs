using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

public class MatchView : NetworkBehaviour
{
    [SerializeField] GameObject playersColumn;
    [SerializeField] GameObject scoresColumn;
    [SerializeField] Canvas winnerWindow;
    [SerializeField] Canvas scoreTable; 
    [SerializeField] Text winnerInfo;
    [SerializeField] Text restartTimer;
    List<Text> playerLines;
    List<Text> scoreLines;

    void Awake()
    {
        playerLines = new List<Text>();
        scoreLines = new List<Text>();
        winnerWindow.enabled = false;
        scoreTable.enabled = true;
    }

    [ClientRpc]
    public void RpcCreateLines(List<Player> players)
    {
        DestroyLine(playerLines);
        DestroyLine(scoreLines);
        for (int i = 0; i < players.Count; i++)
        {
            GameObject pColumn = CreateLine(playersColumn, i);
            GameObject sColumn = CreateLine(scoresColumn, i);
            playerLines.Add(pColumn.GetComponent<Text>());
            scoreLines.Add(sColumn.GetComponent<Text>());
            playerLines[i].text = players[i].name;
            scoreLines[i].text = "0";
        }
    }

    [ClientRpc]
    public void RpcUpdateScore(Player player, int index)
    {
        scoreLines[index].text = player.Score.ToString();
    }

    [ClientRpc]
    public void RpcWinner(Player player)
    {
        winnerWindow.enabled = true;
        scoreTable.enabled = false;
        winnerInfo.text = $"{player.name} is winner!";
    }
    [ClientRpc]
    public void RpcUpdateTimer(int time)
    {
        restartTimer.text = $"Restart {time}";
    }
    [ClientRpc]
    public void RpcCloseWinnerWindow()
    {
        winnerWindow.enabled = false;
        scoreTable.enabled = true;
    }

    GameObject CreateLine(GameObject original, int index)
    {
        GameObject line = Instantiate(original, original.transform.parent);
        Rect rect = original.GetComponent<RectTransform>().rect;
        Vector3 position = original.transform.localPosition;
        position.y = position.y - (rect.height * (index + 1));
        line.transform.localPosition = position;
        return line;
    }

    void DestroyLine(List<Text> list)
    {
        for (int i = 0; i < list.Count; i++)
            Destroy(list[i].gameObject);
        list.Clear();
    }
}
