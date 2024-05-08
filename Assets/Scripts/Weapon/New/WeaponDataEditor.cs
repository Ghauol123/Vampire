using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor {
    public override void OnInspectorGUI() {
        // Draw a dropdown in the spector
        selectWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectWeaponSubtype), weaponSubtypes);
        if(selectWeaponSubtype > 0){
            // update the behaviour field
            weaponData.behaviour = weaponSubtypes[selectWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData); // Marks the object to save
            DrawDefaultInspector();
        }
    }
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectWeaponSubtype;
    private void OnEnable() {
        // lưu trữ giá trị dữ liệu vũ khí
        weaponData = (WeaponData)target;

        // Truy xuất tất cả các loại vũ khí và lưu trữ nó
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p=> baseType.IsAssignableFrom(p) && p!= baseType)
            .ToList();

        // thêm lựa chọn none vào list chọn
        List<string> subTypeString = subTypes.Select(t=>t.Name).ToList();
        subTypeString.Insert(0,"None");
        weaponSubtypes = subTypeString.ToArray();

        // Đảm bảo rằng chúng tôi đang sử dụng đúng loại vũ khí phụ
        selectWeaponSubtype = Math.Max(0,Array.IndexOf(weaponSubtypes,weaponData.behaviour));
    }
}