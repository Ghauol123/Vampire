using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;
    private Stack<IAction> actionStack = new Stack<IAction>();
        void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Gọi khi một hành động được thực hiện
    public void PerformAction(IAction action)
    {
        action.Execute(); // Thực hiện hành động
        actionStack.Push(action); // Đưa hành động vào stack
        Debug.Log($"Hành động đã được lưu: {action.GetType().Name}");
        Debug.Log($"Tổng số hành động đã lưu: {actionStack.Count}");

    }

    // Gọi khi nhấn ESC để hoàn tác hành động cuối cùng
    public void UndoLastAction()
    {
        if (actionStack.Count > 0)
        {
            IAction lastAction = actionStack.Pop();
            lastAction.Undo(); // Hoàn tác hành động
            Debug.Log($"Hành động đã hoàn tác: {lastAction.GetType().Name}");
            Debug.Log($"Tổng số hành động còn lại: {actionStack.Count}");

        }
    }

    void Update()
    {
        // Hoàn tác hành động khi nhấn phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UndoLastAction();
        }
    }
}
