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

    public void Init(Image image, CostumeData data, float speed)
    {
        m_Image = image;
        costumeData = data;
        m_Speed = speed;
    }
}


