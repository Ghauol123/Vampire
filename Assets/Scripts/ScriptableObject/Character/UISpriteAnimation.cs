// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class UISpriteAnimation : MonoBehaviour
// {

//     public Image m_Image;

//     public Sprite[] m_SpriteArray;
//     public float m_Speed = .02f;

//     private int m_IndexSprite;
//     Coroutine m_CorotineAnim;
//     bool IsDone;
//     public void Func_PlayUIAnim()
//     {
//         IsDone = false;
//         StartCoroutine(Func_PlayAnimUI());
//     }

//     public void Func_StopUIAnim()
//     {
//         IsDone = true;
//         StopCoroutine(Func_PlayAnimUI());
//     }
//     IEnumerator Func_PlayAnimUI()
//     {
//         yield return new WaitForSeconds(m_Speed);
//         if (m_IndexSprite >= m_SpriteArray.Length)
//         {
//             m_IndexSprite = 0;
//         }
//         m_Image.sprite = m_SpriteArray[m_IndexSprite];
//         m_IndexSprite += 1;
//         if (IsDone == false)
//             m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISpriteAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image m_Image;

    // Reference to the CostumeData ScriptableObject
    public CostumeData costumeData;

    public float m_Speed = .02f;

    private int m_IndexSprite;
    Coroutine m_CorotineAnim;
    bool IsDone;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Func_PlayUIAnim();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Func_StopUIAnim();
    }

    public void Func_PlayUIAnim()
    {
        if (m_CorotineAnim == null && costumeData != null)
        {
            IsDone = false;
            m_IndexSprite = 0; // Reset index when starting animation
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
        }
    }

    public void Func_StopUIAnim()
    {
        IsDone = true;
        if (m_CorotineAnim != null)
        {
            StopCoroutine(m_CorotineAnim);
            m_CorotineAnim = null;
        }
    }

    IEnumerator Func_PlayAnimUI()
    {
        while (!IsDone)
        {
            yield return new WaitForSeconds(m_Speed);

            // Ensure that the sprite array is available
            if (costumeData != null && costumeData.MSpriteArray.Length > 0)
            {
                if (m_IndexSprite >= costumeData.MSpriteArray.Length)
                {
                    m_IndexSprite = 0;
                }
                m_Image.sprite = costumeData.MSpriteArray[m_IndexSprite];
                m_IndexSprite += 1;
            }
        }
    }
}


