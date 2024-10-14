using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindManager : MonoBehaviour
{

    [SerializeField] private GameObject[] keyBindButton;
    public Dictionary<string, KeyCode> keyBinds { get; private set; }

    private string BindName;

    private void Awake() {
        keyBindButton = GameObject.FindGameObjectsWithTag("KeyBind");
    }

    private void Start() {
        keyBinds = new Dictionary<string, KeyCode>();

        LoadKeyBinds(); // Load the key binds from PlayerPrefs
    }

    public void ChangeKeyBind(string keyName, KeyCode key) {
        if (!keyBinds.ContainsKey(keyName)) {
            keyBinds.Add(keyName, key);
        } else {
            keyBinds[keyName] = key;
        }

        UpdateKeyText(keyName, key);
        SaveKeyBinds(); // Save the key bind to PlayerPrefs
        BindName = string.Empty;
    }

    public void UpdateKeyText(string keyName, KeyCode key) {
        TextMeshProUGUI tmp = Array.Find(keyBindButton, x => x.name == keyName).GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = key.ToString();
    }

    public void ChangeKeyOnClick(string keyName) {
        BindName = keyName;
    }

    private void OnGUI() {
        if (BindName != string.Empty) {
            Event e = Event.current;
            if (e.isKey) {
                ChangeKeyBind(BindName, e.keyCode);
            }
        }
    }

    private void SaveKeyBinds() {
        foreach (var keyBind in keyBinds) {
            PlayerPrefs.SetString(keyBind.Key, keyBind.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    private void LoadKeyBinds() {
        ChangeKeyBind("Up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W")));
        ChangeKeyBind("Left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "A")));
        ChangeKeyBind("Down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "S")));
        ChangeKeyBind("Right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D")));
    }

    public void ResetKeyBindsToDefault() {
        ChangeKeyBind("Up", KeyCode.W);
        ChangeKeyBind("Left", KeyCode.A);
        ChangeKeyBind("Down", KeyCode.S);
        ChangeKeyBind("Right", KeyCode.D);

        SaveKeyBinds(); // Save the default key binds to PlayerPrefs
    }
}
