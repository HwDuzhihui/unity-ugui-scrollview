using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class UIBook : MonoBehaviour {

    public ScrollRect m_Rect;
    public GridLayoutGroup m_Grid;
    public GameObject m_Item;
    public List<RectTransform> m_Trans = new List<RectTransform>();
    public int m_CurIndex;
    public bool m_IsDrag;

	void Awake () {

        m_Rect = transform.Find("Rect").GetComponent<ScrollRect>();
        m_Grid = transform.Find("Rect/Grid").GetComponent<GridLayoutGroup>();
        m_Item = transform.Find("BookItem").gameObject;
        EventTriggerListener.Get(m_Rect.gameObject).onDragStart = DragStart;
        EventTriggerListener.Get(m_Rect.gameObject).onEndDrag = DragEnd;
    }

	void Update()
    {
        UpdateDrag();
    }
    void UpdateMaxLimit()
    {
        float xMax = -m_Trans[0].anchoredPosition.x + 1024;
        float xMin = -m_Trans[m_Trans.Count - 1].anchoredPosition.x + 1024;

        float curX = m_Grid.transform.localPosition.x;

        if (curX > xMax || curX < xMin)
        {

            m_Rect.velocity = Vector2.zero;
            if (curX > xMax)
                m_Grid.transform.localPosition = new Vector3(xMax, m_Grid.transform.localPosition.y, 0);
            else
                m_Grid.transform.localPosition = new Vector3(xMin, m_Grid.transform.localPosition.y, 0);
        }
    }
    void UpdateDrag()
    {
        UpdateMaxLimit();

        float m_foundation = -m_Grid.transform.localPosition.x + 1024;

        for (int i = 0; i < m_Trans.Count; i++)
        {
            Vector3 vPos = m_Trans[i].anchoredPosition;
            float x = Mathf.Abs(vPos.x - m_foundation);

            if (x < 1024)
            {
                if (m_CurIndex != i)
                    m_CurIndex = i;
            }
        }

        if (!m_IsDrag)
        {
            if(m_Rect.velocity.x < 200)
            {
                m_Rect.velocity = Vector2.zero;
                float x = m_Trans[m_CurIndex].anchoredPosition.x;

                float vPos = m_Grid.transform.localPosition.x;

                float distance = -x + 1024 - vPos;

                if (distance > -20 && distance < 20)
                {

                    return;
                }
                
                if (distance > 0)
                {
                    m_Grid.transform.localPosition += new Vector3(Time.fixedDeltaTime * 1000, 0, 0);
                }
                else
                {
                    m_Grid.transform.localPosition -= new Vector3(Time.fixedDeltaTime * 1000, 0, 0);
                }
            }
        }
    }

    void DragStart(GameObject o)
    {
        m_IsDrag = true;
    }
    void DragEnd(GameObject o)
    {
        m_IsDrag = false;
    }

    public void BookExhibition(int key)
    {
        gameObject.SetActive(true);

        m_Trans.Clear();

        while(m_Grid.transform.childCount>0)
        {
            DestroyImmediate(m_Grid.transform.GetChild(0).gameObject);
        }

        Texture2D texture = null;

        List<Sprite> sprites = SpriteMgr.singleton.GetSprs(key);
        for(int i=0;i<sprites.Count;i++)
        {
            GameObject o = GameObject.Instantiate(m_Item);
            o.transform.parent = m_Grid.transform;
            o.transform.localScale = Vector3.one;
            o.transform.localPosition = Vector3.zero;
            o.gameObject.SetActive(true);
            m_Trans.Add(o.GetComponent<RectTransform>());
            BookItem item = o.GetComponent<BookItem>();
            item.ItemInit();
            item.m_Id = i;
            item.m_Icon.sprite = sprites[i];
            item.m_Icon.SetNativeSize();
            item.m_Btn.onClick.AddListener(delegate (){ ClickItem(item); });

            Vector2 size = item.m_Icon.GetComponent<RectTransform>().sizeDelta;
            float xScale = size.x / 2048.0f;
            float yScale = size.y / 1536.0f;
            if(xScale > yScale)
            {
                float scale = size.x / 2048.0f;
                item.m_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, size.y / scale);
            }
            else
            {
                float scale = size.y / 1536.0f;
                item.m_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x / scale,1536);
            }
            //string path = "animal/" + key + "/" + (i+1);
            //texture = (Texture2D)Resources.Load(path);
            //item.m_BG.color = texture.GetPixel(0, (int)texture.height);
            item.m_BG.color = Color.black;

        }
        //m_Grid.CalculateLayoutInputHorizontal();

        Vector3 vPos = new Vector3(-m_Trans[0].anchoredPosition.x + 1024,0,0);
        m_Grid.transform.localPosition = vPos;
        m_CurIndex = 0;

    }

    void ClickItem(BookItem item)
    {
        if (item.m_Id == m_CurIndex)
        {
            if(m_CurIndex < m_Grid.transform.childCount - 1)
            {
                float X = -m_Trans[m_CurIndex + 1].anchoredPosition.x + 1024;
                m_Grid.transform.DOLocalMoveX(X, 0.8f);
            }
            else
            {
                item.transform.DOScale(0.3f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     gameObject.SetActive(false);
                 }).OnStart(()=>
                 {
                     item.m_BG.DOFade(0, 0.2f);
                     item.m_Icon.DOFade(0, 0.2f);
                 });
            }
        }
        else
        {
            float X = -m_Trans[item.m_Id].anchoredPosition.x + 1024;
            m_Grid.transform.DOLocalMoveX(X, 0.5f);
        }
    }
}
