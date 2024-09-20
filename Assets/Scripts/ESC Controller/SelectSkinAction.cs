using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkinAction : IAction
{
    private CostumeData previousCostume;
    private CostumeData newCostume;

    public SelectSkinAction(CostumeData previousCostume, CostumeData newCostume)
    {
        this.previousCostume = previousCostume;
        this.newCostume = newCostume;
    }

    public void Execute()
    {
        // Thực hiện chọn trang phục mới
        // CharacterSelected.instance.SelectSkin(newCostume);
    }

    public void Undo()
    {
        // Hoàn tác quay lại trang phục trước đó
        // CharacterSelected.instance.SelectSkin(previousCostume);
    }
}

