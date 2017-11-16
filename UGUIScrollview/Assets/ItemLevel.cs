using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemLevel : MonoBehaviour {

    public CircleImage m_Icon;
    public Text m_ItemName;
    public Button m_Btn;
    public int m_Id;
    public int m_Key;

	void Start () {
	}
	
    public void ItemInit()
    {
        m_Icon = transform.Find("Icon").GetComponent<CircleImage>();
        m_ItemName = transform.Find("Name").GetComponent<Text>();
        m_Btn = gameObject.AddComponent<Button>();
    }
}
