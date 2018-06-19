using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]

public class EasyAutoGrid : MonoBehaviour {

    public enum Direction
    {
        Horizontal,
        Vertical
    }
    public Direction dir = Direction.Horizontal; 

    List<RectTransform> trans = new List<RectTransform>();
    ScrollRect rect;
    GridLayoutGroup grid;
    Vector2 sizeDelta;
    int curIndex;
    bool m_IsDragging;
    float stepScale = 0.4f;

    System.Action<int> action;

    void Start () {

        Init();   //test
	}
    //初始化调用这个
    public void Init(System.Action<int> _action = null)
    {
        rect = GetComponent<ScrollRect>();

        rect.movementType = ScrollRect.MovementType.Unrestricted;
        sizeDelta = rect.GetComponent<RectTransform>().sizeDelta;
        grid = rect.content.GetComponent<GridLayoutGroup>();
        grid.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        grid.childAlignment = TextAnchor.MiddleCenter;
        if (dir == Direction.Horizontal)
        {
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 1;
            rect.horizontal = true;
            rect.vertical = false;
        }
        else
        {
            grid.startAxis = GridLayoutGroup.Axis.Vertical;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
            rect.horizontal = false;
            rect.vertical = true;
        }

        EventTriggerListener.Get(rect.gameObject).onDragStart = DragStart;
        EventTriggerListener.Get(rect.gameObject).onEndDrag = DragEnd;

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            trans.Add(grid.transform.GetChild(i).GetComponent<RectTransform>());
        }

        action = _action;
    }
    void DragStart(GameObject o)
    {
        m_IsDragging = true;
    }
    void DragEnd(GameObject o)
    {
        m_IsDragging = false;
    }

    void Update()
    {
        if (dir == Direction.Horizontal)
            UpdateScaleHorizontal();
        else
            UpdateScaleVertical();
    }
    //跳到某一个(配合Dotween)
    void MoveToIndexHorizontal(int index)
    {
        curIndex = index;

        float foundation = -trans[index].anchoredPosition.x + sizeDelta.x / 2;
        grid.transform.localPosition = new Vector3(foundation, grid.transform.localPosition.y);

    }
    void MoveToIndexVertical(int index)
    {
        curIndex = index;

        float foundation = -trans[index].anchoredPosition.y + sizeDelta.y / 2;
        grid.transform.localPosition = new Vector3(grid.transform.localPosition.x,foundation);

    }
    //==================横向的===============
    //边界
    void UpdateLitMaxHorizontal()
    {
        float xMax = -trans[0].anchoredPosition.x + sizeDelta.x/2;
        float xMin = -trans[trans.Count - 1].anchoredPosition.x + sizeDelta.x / 2;

        float curX = grid.transform.localPosition.x;

        if (curX > xMax || curX < xMin)
        {

            rect.velocity = Vector2.zero;
            if (curX > xMax)
                grid.transform.localPosition = new Vector3(xMax, grid.transform.localPosition.y, 0);
            else
                grid.transform.localPosition = new Vector3(xMin, grid.transform.localPosition.y, 0);
        }
    }
    //缩放
    void UpdateScaleHorizontal()
    {
        UpdateLitMaxHorizontal();

        //x值在这个点缩放是1
        float m_foundation = -grid.transform.localPosition.x + sizeDelta.x/2;

        for (int i = 0; i < trans.Count; i++)
        {
            //锚点相对位置
            Vector3 vPos = trans[i].anchoredPosition;

            //Item距离中心点
            float x = Mathf.Abs(vPos.x - m_foundation);

            float scale = Mathf.Pow((1 - stepScale), Mathf.Abs(x / sizeDelta.x));
            trans[i].transform.localScale = Vector3.one * scale;

            //当前那个在中间
            if (scale > 0.92f)
            {
                if (curIndex != i)
                {
                    curIndex = i;
                    if (action != null)
                        action(curIndex);
                }
            }

        }
        if (!m_IsDragging)
        {
            if (Mathf.Abs(rect.velocity.x) < 200)
            {
                rect.velocity = Vector2.zero;

                float x = trans[curIndex].anchoredPosition.x;

                float distance = -x + m_foundation;

                //Debug.Log("x = " + x +  "distance =" + distance);

                if (distance > -5 && distance < 5) return;

                if (distance > 0)
                {
                    grid.transform.localPosition += new Vector3(Time.fixedDeltaTime * 300, 0, 0);
                }
                else
                {
                    grid.transform.localPosition -= new Vector3(Time.fixedDeltaTime * 300, 0, 0);
                }
            }
        }

    }
    //===============竖向的==============
    void UpdateLitMaxVertical()
    {
        float yMax = -trans[trans.Count - 1].anchoredPosition.y - sizeDelta.y / 2;
        float yMin = -trans[0].anchoredPosition.y - sizeDelta.y / 2;

        float curY = grid.transform.localPosition.y;

        if (curY > yMax || curY < yMin)
        {

            rect.velocity = Vector2.zero;
            if (curY > yMax)
                grid.transform.localPosition = new Vector3(grid.transform.localPosition.x, yMax, 0);
            else
                grid.transform.localPosition = new Vector3(grid.transform.localPosition.x, yMin, 0);
        }
    }
    void UpdateScaleVertical()
    {
        UpdateLitMaxVertical();

        float m_foundation = -grid.transform.localPosition.y - sizeDelta.y / 2;

        for (int i = 0; i < trans.Count; i++)
        {
            //锚点相对位置
            Vector3 vPos = trans[i].anchoredPosition;

            //Item距离中心点
            float y = Mathf.Abs(vPos.y - m_foundation);

            float scale = Mathf.Pow((1 - stepScale), Mathf.Abs(y / sizeDelta.y));
            trans[i].transform.localScale = Vector3.one * scale;

            //当前那个在中间
            if (scale > 0.92f)
            {
                if (curIndex != i)
                {
                    curIndex = i;
                    if (action != null)
                        action(curIndex);
                }
            }

        }
        if (!m_IsDragging)
        {
            if (Mathf.Abs(rect.velocity.y) < 200)
            {
                rect.velocity = Vector2.zero;

                float y = trans[curIndex].anchoredPosition.y;

                float distance = -y + m_foundation;

                if (distance > -5 && distance < 5) return;

                if (distance > 0)
                {
                    grid.transform.localPosition += new Vector3(0,Time.fixedDeltaTime * 300, 0);
                }
                else
                {
                    grid.transform.localPosition -= new Vector3(0,Time.fixedDeltaTime * 300, 0);
                }
            }
        }

    }


}
