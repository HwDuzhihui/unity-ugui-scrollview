using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BookItem : MonoBehaviour {

    public Button m_Btn;
    public Image m_Icon;
    public Image m_BG;
    public int m_Id;
	void Start () {
	
	}
	
    public void ItemInit()
    {
        m_Btn = gameObject.AddComponent<Button>();
        m_Icon = transform.Find("Icon").GetComponent<Image>();
        m_BG = transform.Find("BG").GetComponent<Image>();
    }
}
