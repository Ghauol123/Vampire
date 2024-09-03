using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI; // Đảm bảo bạn đã thêm namespace này để sử dụng Image component


public class UIStatDisplay : MonoBehaviour
{
    public PlayerStats playerStats;
    TextMeshProUGUI statName, statValue;
    UnityEngine.UI.Image icon;
    public bool updateInEditor = false;
    void Start(){
        playerStats = FindAnyObjectByType<PlayerStats>();

    }
    private void OnEnable() {
        UpdateStatField();
    }
    void OnDrawGizmosSelected(){
        if(updateInEditor) UpdateStatField();
    }
    public void UpdateStatField(){
        if(!playerStats) return;
        //Get a reference to both Text objects to render stats names and stat values
        if(!statName) statName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(!statValue) statValue = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        // if(!icon) spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        icon = transform.GetChild(2).GetComponent<UnityEngine.UI.Image>();
        icon.sprite = playerStats.cst.title_Character;
        //Render all stats names and values
        // Use StringBuilder so that the strig manipulation runs faster
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(FieldInfo field in fields){
            //Render Stats Name
            names.AppendLine(field.Name);

            //Get the Stat value
            object val = field.GetValue(playerStats.Stats);
            float fval = val is int ? (int)val : (float)val;
            // values.Append(fval).Append("\n");

            PropertyAttribute attribute = (PropertyAttribute)ProductAttribute.GetCustomAttribute(field,typeof(PropertyAttribute));
            if(attribute != null && field.FieldType == typeof(float)){
                float percentage = Mathf.Round(fval*100-100);
                if(Mathf.Approximately(percentage,0)){
                    values.Append('-').Append('\n');
                }
                else{
                    if(percentage > 0){
                        values.Append('+');
                    }
                    else values.Append('-');
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append("\n");
            }

            //Update the field with the string we build
            statName.text = PrettifyName(names);
            statValue.text = values.ToString();
        }
    }
    public static string PrettifyName(StringBuilder input){
        if(input.Length <= 0) return string.Empty;
        StringBuilder result = new StringBuilder();
        char last = '\0';
        for(int i = 0; i<input.Length; i++){
            char c = input[i];

            if(last == '\0' || char.IsWhiteSpace(last)){
                c = char.ToUpper(c);
            }
            else if(char.IsUpper(c)){
                result.Append(' ');
            }
            result.Append(c);
            last = c;
        }
        return result.ToString();
    }
    private void Reset() {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }
}
