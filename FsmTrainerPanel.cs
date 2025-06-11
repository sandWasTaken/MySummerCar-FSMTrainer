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
    private Dictionary<PlayMakerFSM, string> fsmEventInput = new Dictionary<PlayMakerFSM, string>();

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

    void OnCUI()
    {
        if (!showMenu) return;
        windowRect = GUI.Window(1234, windowRect, DrawWindow, "FSM Trainer (F9to toggle)");
    }

    void DrawWindow(int windowID)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);

        foreach (var pair in fsms)
        {
            GameObject go = pair.Key;
            PlayMakerFSM[] fsmList = pair.Value;

            if (GUILayout.Button(go.name, GUILayout.Options().Height(30)))
            {
                Logger.LogInfo("?Clicked GameObject: {go.name}");
            }

            foreach (var fsm in fsmList)
            {
                GUILayout.Label("SML: " + fsm.FsmName);
                GUILayout.Label("Current State: " + fsm.ActiveStateName);

                GUILayout.Label("-- Variables --");

                foreach (var v in fsm.FsmVariables.GetAllNamedVariables())
                {
                    if (v is FsmFloat f)
                    {
                        float newVal;
                        float.TryParse(GUILayout.TextField(f.Value.ToString()), out newVal);
                        f.Value = newVal;
                    }
                    else if (v is FsmInt i)
                    {
                        int newVal;
                        int.TryParse(GUIlayout.TextField(i.Value.ToString()), out newVal);
                        i.Value = newVal;
                    }
                    else if (v is FsmBool b)
                    {
                        b.Value = GUIlayout.Toggle(b.Value, b.Name);
                    }
                    else
                    {
                        GUILayout.Label("${v.Name} (${v.GetType().Name}) = ${v.toString()}");
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
        fsmEventInput.Clear();
        var all = GameObject.FindObjectsOfType<PlayMakerFSM>();
        foreach (var fsm in all)
        {
            if (!fsms.ContainsKey(fsm.gameObject))
            {
                fsms[fsm.gameObject] = fsm.gameObject.GetComponentsOfType<PlayMakerFSM>();
            }
        }
        Logger.LogInfo("\FSMTrainer] Found " + fsms.Count + " GameObjects with FSMs");
    }
}