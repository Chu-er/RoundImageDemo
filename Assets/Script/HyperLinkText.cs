using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//标签类型
public enum RichTextTagType
{
    None,
    Underline,      //下划线
}

public class HyperLinkText : Text,IPointerClickHandler
{

    #region 超链接相关
    /// <summary>
    /// 超链接的信息类
    /// </summary>
    private class HyperLinkInfo
    {
        public int startIndex;
        public int endIndex;
        public string name;
        public readonly List<Rect> boxes = new List<Rect>();
    }
    /// <summary>
    /// 解析玩的最终文本
    /// </summary>
    private string m_OutputText;

    /// <summary>
    /// 超连接信息类、
    /// </summary>
    private readonly List<HyperLinkInfo> m_HrefInfo = new List<HyperLinkInfo>();
    /// <summary>
    /// 文本构造器
    /// </summary>
    private readonly StringBuilder m_TextBuilder = new StringBuilder();

    [Header("超链接的颜色")]
    public string hyperLinkColor = "red";
    /// <summary>
    /// 点击事件
    /// </summary>
    public UnityAction<string> m_OnHrefClick;

    /// <summary>
    /// 匹配超链接的正则表达式 [^>\n\s] 匹配除了 > 换行 空格的   这里只匹配单行
    /// </summary>
    private static readonly Regex m_HrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

    #endregion

    #region 图片相关

    private static readonly string m_ReplaceStr = "\u00A0";//不间断空格\u00A0,主要用在office中,让一个单词在结尾处不会换行显示,
    private static readonly Regex m_ImageTagRegex = new Regex(@"<iocn name=([^>\s]+)([^>]*)/>");//匹配ICon的名字属性
    private static readonly Regex m_ImageParaRegex = new Regex(@"(\w+)=([^\s]+)");//key = value
    private List<RichTextImageInfo> m_ImageInfoList = new List<RichTextImageInfo>();
    private bool m_IsImageDirty = false;
    #endregion


    #region 下划线相关

    [DllImport("unitys")]
    public static extern int MessageBox();
    #endregion




    private HyperLinkText m_HyperLinkText;

    protected override void Start()
    {

        base.Start();
        m_HyperLinkText = GetComponent<HyperLinkText>();
        m_OnHrefClick = OnTextClick;
    }

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabAssetType(this) == UnityEditor.PrefabAssetType.Regular)
        {
            return;
        }
#endif
        //<a href=www.baidu.com>[百度]</a>
        m_OutputText = GetOutputText(text);
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        var orignText = m_Text;
        m_Text = m_OutputText;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;
        CaculateBounds(toFill);
       
    }

    /// <summary>
    /// 计算包围框
    /// </summary>
    private void CaculateBounds( VertexHelper toFill)
    {
        UIVertex vert = new UIVertex();
        //处理超链接的包围框
        foreach (var hrefInfo in m_HrefInfo)//有几个超链接就遍历几次
        {
            hrefInfo.boxes.Clear();//清理Rect 数据
            if (hrefInfo.startIndex >= toFill.currentVertCount)
            {
                continue;
            }
            //获取索引为2的顶点数据  保存在verts中
            toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
            var pos = vert.position;

            var bound = new Bounds(pos, Vector3.zero);
            
            for (int i = hrefInfo.startIndex, j = hrefInfo.endIndex; i < j; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }
                toFill.PopulateUIVertex(ref vert, i);
                pos = vert.position;

                
                if (pos.x < bound.min.x)//换行重新添加包围框 
                {
                    hrefInfo.boxes.Add(new Rect(bound.min, bound.size));
                    bound = new Bounds(pos, Vector3.zero);
                }
                else
                {
                    bound.Encapsulate(pos);//重新计算最大最小点
                }
            }
            hrefInfo.boxes.Add(new Rect(bound.min, bound.size));
            Debug.Log(bound.size);
        }
    }

    /// <summary>
    /// 获取超链接
    /// </summary>
    /// <returns></returns>
    private string GetOutputText(string outputText)
    {
        m_TextBuilder.Length = 0;
        m_HrefInfo.Clear();
        int indexText = 0;

        //MatchCollection matchs = m_HrefRegex.Matches(outputText);
        //Debug.Log(matchs[0].Groups[1]); // Group 索引是零的 话 是全部的字符串输出 = match[0].Value 

        foreach (Match match in m_HrefRegex.Matches(outputText))
        {
            m_TextBuilder.Append(outputText.Substring(indexText, match.Index - indexText));//matrch.index 就是在整个字符串中的索引
            m_TextBuilder.AppendFormat("<color={0:color}>",hyperLinkColor);//添加在可点击文字前的颜色Xml
            var group = match.Groups[1];

            //Debug.Log(m_TextBuilder.Length);

            var hrefInfo = new HyperLinkInfo
            {
                startIndex = m_TextBuilder.Length * 4, //超链接的文本起始顶点索引
                endIndex = (m_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                name = group.Value,
            };
            m_HrefInfo.Add(hrefInfo);
            m_TextBuilder.Append(match.Groups[2].Value);
            m_TextBuilder.Append("</color>");

            indexText = match.Index + match.Length;
          //  Debug.Log(match.Index+"And"+match.Value);///这里只匹配了一项 也就是这个遍历只执行一次  match.Index=0
            //，match。Value 完整的Text
        }
        m_TextBuilder.Append(outputText.Substring(indexText, outputText.Length-indexText));//把与超链接最后的无关的文本也加进来
        string outStr = m_TextBuilder.ToString();
        //Debug.Log("OutPurString:"+ outStr);
        return outStr;
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_OnHrefClick == null) return;
        Vector2 localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera,out localPos);

        foreach (var hrefInfo in m_HrefInfo)
        {
            var box = hrefInfo.boxes;
            for (int i = 0; i < box.Count; i++)
            {
                if (box[i].Contains(localPos))
                {
                    m_OnHrefClick.Invoke(hrefInfo.name);
                    return;
                }
            }

        }
    }

    private void OnTextClick(string texts)
    {
        Debug.Log(texts);
    }

}

public class RichTextImageInfo
{
    public string name;       //名字(路径)
    public Vector2 size;      //宽高
    public Vector2 position;  //位置
    public int startVertex;   //起始顶点
    public int vertexLength;  //占据顶点数
    public Color color;       //颜色

    //标签属性
    public float widthScale = 1f;              //宽度缩放
    public float heightScale = 1f;             //高度缩放
    public string eventName;                   //事件名
    public string eventParameter;              //事件参数
    public int count = 0;                      //帧数

    public void SetValue(string key, string value)
    {
        switch (key)
        {
            case "w":
                {
                    float.TryParse(value, out widthScale);
                    break;
                }
            case "h":
                {
                    float.TryParse(value, out heightScale);
                    break;
                }
            case "n":
                {
                    eventName = value;
                    break;
                }
            case "p":
                {
                    eventParameter = value;
                    break;
                }
            case "c":
                {
                    int.TryParse(value, out count);
                    break;
                }
            default:
                break;
        }
    }
}






