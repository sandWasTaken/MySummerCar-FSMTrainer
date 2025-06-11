using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HutongGames.Playmaker;

public class FsmTrainerPanel : BaseUnityPlugin
{
    private bool showMenu = false;
    private Rect windowRect = new Rect(100, 100, 400, 600);
    private Vector2 scrollPos;
    private Dictionary<GameObject, PlaymakerFSM[]> fsms = new Dictionary<GameObject, PlaymakerFSM[]>();
    private Dictionary<PlaymakerFSM, string> fsmEventInput = new Dictionary<PlayMakerFSM, string>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            showMenu = !showMenu;
            if (showMenu)
            { LoadFsms(); }
        }
    }

    void OnCUI()
    {
        if (showMenu)
        {
            windowRect = GUI.Window(1234, windowRect, DrawWindow, "FSM Trainer");
        }
    }

    void DrawWindow(int windowID)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);

        foreach (var pair in fsms)
        {
            GameObject go = pair.Key;
            PlaymakerFSM[] fsmList = pair.Value;

            if (GUILayout.Button(go.name, 30))
            {
                GUIlayout.Label("SMS for: " + go.name);
                foreach (var fsm in fsmList)
                {
                    GUIlayout.Label("State: " + fsm.ActiveStateName);
                }
            }
        }

        GUILayout.EndScrollView();
        GUILaåout.DragWindow();
    }

    void LoadFsms()
    {
        fsms.Clear();
        fsmEventInput.Clear();
        var allFSMS = GameObject.FindObjectsOfType<PlaymakerFSM>();
        foreach (var fsm in allFSMS)
        {
            if (!fsms.ContainsKey(fsm.gameObject))
            {
                fsms[fsm.gameObject] = fsm.gameObject.GetComponentsOfType<PlaymakerFSM>();
            }
        }
    }
}