using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteMgr : MonoBehaviour {

    static SpriteMgr m_SpriteMgr;
    public Dictionary<int, List<Sprite>> m_Sprites = new Dictionary<int, List<UnityEngine.Sprite>>();
    public List<Sprite> m_EditorSprite = new List<Sprite>();

    public static SpriteMgr singleton
    {
        get
        {
            if (m_SpriteMgr == null)
                m_SpriteMgr = new SpriteMgr();
            return m_SpriteMgr;
        }
    }

	void Awake () {
        m_SpriteMgr = this;
        LoadSprite();
	}
	
    void LoadSprite()
    {
        m_EditorSprite.Clear();
        m_Sprites.Clear();

        for(int i=1; i<=9; i++)
        {
            object[] objs = Resources.LoadAll("animal/" + i +"") as object[];
            List<Sprite> sprs = new List<Sprite>();
            for(int j=0; j<objs.Length;j++)
            {
                Sprite sprite = objs[j] as Sprite;

                if(sprite != null)
                    sprs.Add(sprite);
            }
            m_Sprites.Add(i, sprs);

        }

#if UNITY_EDITOR
        foreach(var sprite in m_Sprites)
        {
            List<Sprite> spr = sprite.Value;

            for(int i=0;i<spr.Count;i++)
            {
                m_EditorSprite.Add(spr[i]);
            }
        }
#endif
    }

    public List<Sprite> GetSprs(int key)
    {
        return m_Sprites[key];
    }

}
