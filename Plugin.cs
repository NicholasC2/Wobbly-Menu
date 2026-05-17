using System.Collections;
using System.Collections.Generic;
using BepInEx;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WobblyMenu.UI;

namespace WobblyMenu
{
    [BepInPlugin("com.wobblymenu.plugin", "Wobbly Menu", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private Rect spawnMenuWindowRect;
        private Rect itemEditorWindowRect;
        private Rect variableMenuWindowRect;

        private Vector2 variableScrollPosition;
        private Vector2 itemEditorScrollPosition;
        private Vector2 itemSpawnScrollPosition;

        private const int SPAWN_WINDOW_ID = 0;
        private const int VARIABLE_WINDOW_ID = 1;
        private const int ITEM_EDITOR_WINDOW_ID = 2;

        private string currentlySelectedItem = null;
        private bool isItemEditorOpen = false;
        private int itemSpawnAmount = 1;
        private bool isMenuVisible = true;

        private string itemSearch = "";
        private string itemEditorSearch = "";
        private string variableSearch = "";

        private GUIStyle leftButtonStyle;
        private bool guiInit;

        private List<string> assets = new List<string>();

        private List<string> cachedComponentNames = new List<string>();
        private bool componentsLoaded = false;

        void Awake()
        {
            Debug.Log("PLUGIN: Awake");
            ModRegistry.RegisterAll();
            StartCoroutine(WaitThenDump());
        }

        IEnumerator WaitThenDump()
        {
            yield return new WaitForSeconds(1f);

            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    string parsedKey = key.ToString().ToLower();

                    if (parsedKey.EndsWith(".prefab") || parsedKey.EndsWith(".asset"))
                        assets.Add(key.ToString());
                }
            }

            Debug.Log($"Collected {assets.Count} addressable keys");
        }

        void Update()
        {
            spawnMenuWindowRect = new Rect(20, 20, 200, Screen.height - 40);
            itemEditorWindowRect = new Rect(250, 20, 250, 400);
            variableMenuWindowRect = new Rect(Screen.width - 220, 20, 200, Screen.height - 40);

            if (Input.GetKeyDown(KeyCode.Insert))
                isMenuVisible = !isMenuVisible;
        }

        void InitGUI()
        {
            if (guiInit) return;

            leftButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };

            guiInit = true;
        }

        void LoadSelectedItemComponents()
        {
            cachedComponentNames.Clear();
            componentsLoaded = false;

            if (string.IsNullOrEmpty(currentlySelectedItem))
                return;

            Addressables.LoadAssetAsync<GameObject>(currentlySelectedItem).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    Debug.LogError("Failed to load: " + currentlySelectedItem);
                    componentsLoaded = true;
                    return;
                }

                GameObject prefab = handle.Result;

                var comps = prefab.GetComponents<Component>();
                foreach (var c in comps)
                {
                    if (c == null) continue;
                    cachedComponentNames.Add(c.GetType().Name);
                }

                componentsLoaded = true;

                Addressables.Release(handle);
            };
        }

        void OnGUI()
        {
            InitGUI();
            if (!isMenuVisible) return;

            GUI.Window(SPAWN_WINDOW_ID, spawnMenuWindowRect, DrawSpawnMenuWindow, "Spawn Menu");
            GUI.Window(VARIABLE_WINDOW_ID, variableMenuWindowRect, DrawVariableWindow, "Variable Menu");

            if (isItemEditorOpen && currentlySelectedItem != null)
            {
                GUI.Window(ITEM_EDITOR_WINDOW_ID, itemEditorWindowRect, DrawItemEditorWindow,
                    "Item Spawn Config: " + currentlySelectedItem);
            }
        }

        void DrawVariableWindow(int id)
        {
            GUILayout.Label("Search:");
            variableSearch = GUILayout.TextField(variableSearch);

            variableScrollPosition = GUILayout.BeginScrollView(variableScrollPosition);

            foreach (var variable in VariableRegistry.Variables)
            {
                var value = variable.GetValue();
                if (value != null)
                    Drawer.DrawObject(value, variableSearch);
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        void DrawItemEditorWindow(int id)
        {
            GUILayout.Label("Search:");
            itemEditorSearch = GUILayout.TextField(itemEditorSearch);

            itemEditorScrollPosition = GUILayout.BeginScrollView(itemEditorScrollPosition);

            if (!componentsLoaded)
            {
                GUILayout.Label("Loading components...");
            }
            else
            {
                foreach (var name in cachedComponentNames)
                {
                    GUILayout.Label(name);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.Label("Spawn Amount:");
            string input = GUILayout.TextField(itemSpawnAmount.ToString());
            int.TryParse(input, out itemSpawnAmount);

            if (GUILayout.Button("Spawn"))
            {
                var players = UnitySingleton<GameInstance>.Instance.GetLocalPlayerControllers();

                foreach (PlayerController player in players)
                {
                    if (!player) continue;

                    Vector3 pos = player.GetPlayerTransform().position;

                    for (int i = 0; i < itemSpawnAmount; i++)
                    {
                        Vector3 spawnPos = pos + new Vector3(0, 1f + i * 0.5f, 0);

                        NetworkPrefab.SpawnNetworkPrefab(
                            currentlySelectedItem,
                            obj =>
                            {
                                if (!obj) return;

                                foreach (var c in obj.GetComponents<Component>())
                                {
                                    if (c) Debug.Log(c.GetType().Name);
                                }
                            },
                            spawnPos,
                            Quaternion.identity
                        );
                    }
                }
            }

            if (GUILayout.Button("Close"))
            {
                isItemEditorOpen = false;
                currentlySelectedItem = null;
                cachedComponentNames.Clear();
                componentsLoaded = false;
            }

            GUI.DragWindow();
        }

        void DrawSpawnMenuWindow(int id)
        {
            GUILayout.Label("Search:");
            itemSearch = GUILayout.TextField(itemSearch);

            itemSpawnScrollPosition = GUILayout.BeginScrollView(itemSpawnScrollPosition);

            string search = itemSearch.ToLower();

            int maxDraw = 200;
            int drawn = 0;

            foreach (string key in assets)
            {
                if (drawn >= maxDraw) break;

                if (string.IsNullOrEmpty(key))
                    continue;

                if (!key.ToLower().Contains(search))
                    continue;

                if (GUILayout.Button(key, leftButtonStyle))
                {
                    currentlySelectedItem = key;
                    isItemEditorOpen = true;

                    LoadSelectedItemComponents();
                }

                drawn++;
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}