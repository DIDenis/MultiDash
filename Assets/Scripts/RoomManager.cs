using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : NetworkRoomManager
{
    public override void OnGUI()
    {
        base.OnGUI();

        GUILayout.BeginArea(new Rect(Screen.width - 150f, 40f, 140f, 30f));
        if (GUILayout.Button("Quit game"))
            Application.Quit();
        GUILayout.EndArea();
    }
}
