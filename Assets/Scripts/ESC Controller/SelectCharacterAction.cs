using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterAction : IAction
{
    private CharacterData previousCharacter;
    private CharacterData newCharacter;

    public SelectCharacterAction(CharacterData previousCharacter, CharacterData newCharacter)
    {
        this.previousCharacter = previousCharacter;
        this.newCharacter = newCharacter;
    }

    public void Execute()
    {
        // Thực hiện hành động chọn nhân vật
        CharacterSelected.instance.SelectCharacter(newCharacter);
    }

    public void Undo()
    {
        // Hoàn tác, quay lại nhân vật trước đó
        CharacterSelected.instance.SelectCharacter(previousCharacter);
    }
}

