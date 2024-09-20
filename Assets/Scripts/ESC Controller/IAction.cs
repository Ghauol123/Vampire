using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    void Execute(); // Thực hiện hành động
    void Undo();    // Hoàn tác hành động
}
