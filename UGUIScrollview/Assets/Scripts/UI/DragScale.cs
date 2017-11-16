using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class DragScale : MonoBehaviour {

    GridLayoutGroup m_Grid;
    ScrollRect m_Rect;
    UIBook m_UIBook;
    GameObject m_Item;
    public List<CanvasGroup> m_items = new List<CanvasGroup>();
    public List<RectTransform> m_Trans = new List<RectTransform>();        
    float m_alpha = 0;                      
    float m_scale = 0.2f;
    int m_CanSee = 5;
    int m_CurIndex = 0;

    Vector3 scrollSizeData;
    Vector3 gridPos;
    Vector2 gridSize;

    bool m_IsDragging;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    void Start () {
        UIInit();
    }
	void DragStart(GameObject o)
    {
        m_IsDragging = true;
    }
    void DragEnd(GameObject o)
    {
        m_IsDragging = false;
    }
	
	void Update () {
        UpdateScaleAlpha();

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

    public void ClickItem(ItemLevel item)
    {
        if(item.m_Id == m_CurIndex)
        {
            m_UIBook.BookExhibition(item.m_Key);
        }
        else
        {
            float X = -m_Trans[item.m_Id].anchoredPosition.x + 1024;
            m_Grid.transform.DOLocalMoveX(X,0.5f);
        }
    }
    public void UIInit()
    {
        m_Rect = GetComponent<ScrollRect>();
        m_Grid = transform.Find("Grid").GetComponent<GridLayoutGroup>();
        m_UIBook = transform.parent.Find("Book").GetComponent<UIBook>();
        m_Item = transform.Find("Item").gameObject;
        EventTriggerListener.Get(m_Rect.gameObject).onDragStart = DragStart;
        EventTriggerListener.Get(m_Rect.gameObject).onEndDrag = DragEnd;
        m_items.Clear();
        m_Trans.Clear();

        int index = 0;
        foreach(var spr in SpriteMgr.singleton.m_Sprites)
        {
            List<Sprite> sprites = spr.Value;
            GameObject o = GameObject.Instantiate(m_Item);
            o.transform.parent = m_Grid.transform;
            o.transform.localScale = Vector3.one;
            o.transform.localPosition = Vector3.zero;
            ItemLevel item = o.GetComponent<ItemLevel>();
            item.ItemInit();
            item.m_Icon.sprite = sprites[0];
            item.m_Id = index;
            item.m_Key = spr.Key;
            index++;
            item.m_Btn.onClick.AddListener(delegate()
            {
                ClickItem(item);
            });
            o.SetActive(true);
        }

        for (int i = 0; i < m_Grid.transform.childCount; i++)
        {
            m_items.Add(m_Grid.transform.GetChild(i).GetComponent<CanvasGroup>());
            m_Trans.Add(m_Grid.transform.GetChild(i).GetComponent<RectTransform>()); 
        }

        scrollSizeData = m_Rect.transform.GetComponent<RectTransform>().sizeDelta;
        gridPos = m_Grid.transform.localPosition;
        gridSize = m_Grid.cellSize;


    }
    //更新缩放颜色
    void UpdateScaleAlpha()
    {
        UpdateMaxLimit();


        //x值在这个点缩放是1
        float m_foundation = -m_Grid.transform.localPosition.x + 1024;

        for(int i=0;i<m_items.Count;i++)
        {
            //锚点相对位置
            Vector3 vPos = m_Trans[i].anchoredPosition;

            //Item距离中心点
            float x = Mathf.Abs(vPos.x - m_foundation);
            
            float scale = Mathf.Pow((1 - m_scale), Mathf.Abs(x / gridSize.x));
            m_items[i].transform.localScale = Vector3.one * scale;
            m_items[i].alpha = scale;

            //当前那个在中间
            if(scale > 0.92f)
            {
                if (m_CurIndex != i)
                {
                    m_CurIndex = i;
                }
            }

        }

        if(!m_IsDragging)
        {
            if (Mathf.Abs(m_Rect.velocity.x) < 200)
            {
                m_Rect.velocity = Vector2.zero;

                float x = m_Trans[m_CurIndex].anchoredPosition.x;

                float vPos = m_Grid.transform.localPosition.x;

                float distance = -x +1024 - vPos;

                //Debug.Log("x = " + x + "   vpos =" + vPos + "distance =" + distance);

                if (distance > -5 && distance < 5) return;

                if (distance > 0)
                {
                    m_Grid.transform.localPosition += new Vector3(Time.fixedDeltaTime * 300,0, 0);
                }
                else
                {
                    m_Grid.transform.localPosition -= new Vector3(Time.fixedDeltaTime * 300,0, 0);
                }
            }
        }

    }
}
