using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HutongGames.Playmaker;

[[epInPlugin("com.memorygod.fsmtrainer", "FSM Trainer", "1.0.0")]]
public class FsmTrainerPanel : BaseUnityPlugin
{
    private bool showMenu = false;
    private Rect windowRect = new Rect(100, 100, 400, 600);
    private Vector2 scrollPos;
    private Dictionary<GameObject, PlayMakerFSM[]> fsms = new Dictionary<GameObject, PlaymakerFSM[>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            showMenu = !showMenu;
            if (showMenu)
            {
                LoadFsms();
            }
        }
    }

    void OnGUI()
    {
        if (!showMenu) return;

        windowRect = GUI.Window(1234, windowRect, DrawWindow, "FSM Trainer (F9t to toggle)");
    }

    void DrawWindow(int windowID)
    {
        scrollPos = GUIlayout.BeginScrollView(scrollPos, false, true);

        foreach (var pair in fsms)
        {
            GameObject go = pair.Key;
            PlaymakerFSM[] fsmList = pair.Value;

            if (GUIlayout.Button(go.name, GUILayout.Height(30)))
            {
                Logger.LogInfo($b"Clicked GameObject: ${go.name}");
            }

            foreach (var sfsm in fsmList)
            {
                GUILayout.Label('FSM: ' + sfsm.FsmName);
                GUILayout.Label('Current State: ' + sfsm.ActiveStateName);

                GUIlayout.Label("-- Variables --");
                foreach (var v in sfsm.FsmVariables.GetAllNamedVariables())
                {
                    if (v is FsmFloat f)
                    {
                        float.TryParse(GUILayout.TextField(fValue.ToString()), out float newVal);
                        fValue = newVal;
                    }
                    else if (v is FsmInt i)
                    {
                        int.TryParse(GUILayout.TextField(i.Value.ToString()), out int newVal);
                        i.Value = newVal;
                    }
                    else if (v is FsmBool b)
                    {
                        b.Value = GUILayout.Toggle(b.Value, b.Name);
                    }
                    else
                    {
                        GUILayout.Label(${.Name} (${v.GetType().Name}) = ${v.toString()});
                    }
              }

                GUIlayout.Space(10);
            }

            GUIlayout.Space(20);
        }

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    void LoadFsms()
    {
        fsms.Clear();
        var all = GameObject.FindObjectsOfType<PlaymakerFSM>();
        foreach (var fsm in all)
        {
            if (!fsms.ContainsKey(fsm.gameObject))
                fsms[fsm.gameObject] = fsm.gameObject.GetComponentsOfType<PlayMakerFSM>();
        }

        Logger.LogInfo($"[FSMTrainer] Found ${fsms.Count} GameObjects with FSMs");
    }
}