using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
// class này dùng để có thể chỉnh được giao diện của weapon như là súng, hay rìu hay các vũ khí khác\

// class này chỉ chạy trong môi trường editor không chạy trong môi trường game
[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor {
    // Phương thức này dùng để vẽ được GUI cho custom editor
    public override void OnInspectorGUI() {
        // Draw a dropdown in the spector
        // Tạo một dropdown cho để người dùng có thể chọn các behaviour khác nhau dựa vào việc trong mảng có bao nhiêu behaviour
        selectWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectWeaponSubtype), weaponSubtypes);
        if(selectWeaponSubtype > 0){
            // update the behaviour field
            // nếu được chọn behaviour thì trường behaviour của data sẽ lưu lại giá trị của behaviour được chọn dưới dạng chữ
            weaponData.behaviour = weaponSubtypes[selectWeaponSubtype].ToString();
            // đánh dấu weapondata đã bị thay đổi và cần được lưu lại
            EditorUtility.SetDirty(weaponData); // Marks the object to save
            // Vẽ tất cả các thuộc tính còn lại nếu đã được chọn khác 0
            DrawDefaultInspector();
        }
    }
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectWeaponSubtype;
    private void OnEnable() {
        // target là một đối tượng đặc biệt tham chiếu đến đối tượng ở đây là WeaponData, 
        //và có thể sử dụng được các thông tin của WeaponData thông qua weaponData
        weaponData = (WeaponData)target;

        // xác định kiểu dữ liệu cần tìm ở đây là Weapon
        System.Type baseType = typeof(Weapon);
        // lấy các file có đuôi là dll, đây là các file biên dịch của Unity, 
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            // trả về một mảng các loại type khác nhau
            .SelectMany(s => s.GetTypes())
            // chỉ lấy type nào là con của weaponData và không phải là weaponData
            .Where(p=> baseType.IsAssignableFrom(p) && p!= baseType)
            .ToList();

        // thêm lựa chọn none vào list chọn
        List<string> subTypeString = subTypes.Select(t=>t.Name).ToList();
        subTypeString.Insert(0,"None");
        weaponSubtypes = subTypeString.ToArray();

        // gán chỉ số tương ứng của weapondata đã chọn cho selectWeaponSubtype, đảm bảo rằng chỉ số ít nhất luôn là 0
        selectWeaponSubtype = Math.Max(0,Array.IndexOf(weaponSubtypes,weaponData.behaviour));
    }
}