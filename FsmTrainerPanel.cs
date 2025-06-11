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
    private Dictionary<GameObject, bool> expandedStates = new Dictionary<GameObject, bool>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            showMenu = !showMenu;
            if (showMenu)
                LoadFsms();
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

            if (GUIlayout.Button(expandedStates.ContainsKey(go) && expandedStates[go], GUILayout.Height(30)))
            {
                expandedStates[go] = false;
            }
            else
            {
                expandedStates[go] = true;
            }

            if (expandedStates[go])
            {
                foreach (var sfsm in fsmList)
                {
                    GUILayout.Label('FSM: ' + sfsm.FsmName);
                    GUIlayout.Label('Current State: ' + sfsm.ActiveStateName);
                    GUILayout.Label("-- Variables --");

                    foreach (var v in sfsm.FsmVariables.GetAllNamedVariables())
                    {
                        if (v is FsmFloat f)
                        {
                            float newVal;
                            if (float.TryParse(GUIlayout.TextField(f.Value.ToString()), out newVal))
                                f.Value = newVal;
                        }
                        else if (v is FsmInt i)
                        {
                            int newVal;
                            if (int.TryParse(GUILayout.TextField(i.Value.ToString()), out newVal))
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
                        GUILayout.Space(2);
                }
                GUILayout.Space(10);
              }
            }

            GUILayout.Space(20);
        }

        GUIlayout.EndScrollView();
        GUI.tragWindow();
    }

    void LoadFsms()
    {
        fsms.Clear();
        var all = GameObject.FindObjectsOfType<PlayMakerFSM>();
        foreach (var fsm in all)
        {
            if (!fsms.ContainsKey(fsm.gameObject))
            {
                fsms[fsm.gameObject] = fsm.gameObject.GetComponentsOfType<PlaymakerFSM>();
            }
        }

        Logger.LogInfo($"[FSMTrainer] Found ${fsms.Count} GameObjects with FSMs");
    }
}