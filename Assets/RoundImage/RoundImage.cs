using System.Collections;
using System.Collections.Generic;
using UnityEngine.Sprites;
using UnityEngine;
using UnityEngine.UI;
/*
 * 首先，我们将一张图分成6个三角形和四个90°的扇形。每个扇形用若干个三角形来模拟。这样我们就将一个圆角矩形，
 * 划分成了GPU能认识的三角形了。
 * 我们以扇形的半径，构成扇形的三角形的数量作为变量，就可以算出每个我们需要的顶点的坐标了。具体的实现见代码。
*/

public class RoundImage : Image
{
    /// <summary>
    /// 每个角
    /// </summary>
    const int MaxTriangleNmu = 20;
    const int MinTriangleNum = 1;
    /// <summary>
    /// 每个角填充的三角形数量
    /// </summary>
    [Range(MinTriangleNum, MaxTriangleNmu)]
    public int TrinagleNum = 5;

    /// <summary>
    /// 扇形的半径取值
    /// </summary>
    public float Radius =35 ;

    Vector3[] vectors;


    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        Vector4 v=  GetDrawingDimensions(preserveAspect);
        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        Color32 color32 = color;
        toFill.Clear();

        //对扇形范围限制 不能大于我们绘制矩形的 一半
        float radius = Radius;
        if (radius > (v.z - v.x) / 2) radius = (v.z - v.x) / 2;
        if (radius > (v.w - v.y) / 2) radius = (v.w - v.y) / 2;
        if (radius < 0) radius = 0;

        //计算UV对应的半径值  宽高的比例  因为UV 0-1  所以算的是 比例
        float uvRadiusX = radius / (v.z - v.x);
        float uvRadiusY = radius / (v.w - v.y);

        //先构造左右中间  三个矩形的顶点  UV Trangle
         vectors = new Vector3[]
        {
             new Vector3(v.x, v.w - radius),//0
             new Vector3(v.x,v.y+radius),//1
             new Vector3(v.x+radius,v.w),//2
             new Vector3(v.x+radius,v.w-radius),//3
             new Vector3(v.x+radius,v.y+radius),//4
             new Vector3(v.x+radius,v.y),//5
             new Vector3(v.z-radius,v.w),//6
             new Vector3(v.z-radius,v.w-radius),//7
             new Vector3(v.z-radius,v.y+radius),//8 
             new Vector3(v.z-radius,v.y),//9
             new Vector3(v.z,v.w-radius),//10
             new Vector3(v.z,v.y+radius),//11
        };
        Vector2[] uvs = new Vector2[] {
             new Vector3(uv.x, uv.w - uvRadiusY),//0
             new Vector2(uv.x,uv.y+uvRadiusY),
             new Vector2(uv.x+uvRadiusX,uv.w),
             new Vector2(uv.x+uvRadiusX,uv.w-uvRadiusY),
             new Vector2(uv.x+uvRadiusX,uv.y+uvRadiusY),//4
             new Vector2(uv.x+uvRadiusX,uv.y),//5
             new Vector2(uv.z-uvRadiusX,uv.w),
             new Vector2(uv.z-uvRadiusX,uv.w-uvRadiusY),//7
             new Vector2(uv.z-uvRadiusX,uv.y+uvRadiusY),//8
             new Vector2(uv.z-uvRadiusX,uv.y),//9
             new Vector2(uv.z,uv.w-uvRadiusY),//10
             new Vector2(uv.z,uv.y+uvRadiusY),//11
         };
        if (uvs.Length== vectors.Length)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                toFill.AddVert(vectors[i], color32, uvs[i]);
                //Debug.Log(vectors[i]);

            }
            //Debug.Log(toFill.currentIndexCount+"IndexCount");
            //Debug.Log(toFill.currentVertCount+"VerCount");
        }

        //左边的矩形
        toFill.AddTriangle(1,0,3);
        toFill.AddTriangle(1, 3, 4);
        //中间的矩形
        toFill.AddTriangle(5, 2, 6);
        toFill.AddTriangle(5, 6, 9);

        //右边的矩形
        toFill.AddTriangle(8,7,10);
        toFill.AddTriangle(8, 10, 11);

        //开始构造四个角
        List<Vector2> vCenterList = new List<Vector2>();
        List<Vector2> uvCenterList = new List<Vector2>();
        List<int> vCenterVertList = new List<int>();

        //右上角  第七个点
        int rightTop = 7;
        vCenterList.Add(vectors[rightTop]);
        uvCenterList.Add(uvs[rightTop]);
        vCenterVertList.Add(rightTop);

        //左上角
        int leftTop = 3;
        vCenterList.Add(vectors[leftTop]);
        uvCenterList.Add(uvs[leftTop]);
        vCenterVertList.Add(leftTop);

        //左下角
        int leftBottom = 4;
        vCenterList.Add(vectors[leftBottom]);
        uvCenterList.Add(uvs[leftBottom]);
        vCenterVertList.Add(leftBottom);

        //右下角
        int rightBottom = 8;
        vCenterList.Add(vectors[rightBottom]);
        uvCenterList.Add(uvs[rightBottom]);
        vCenterVertList.Add(rightBottom);


        //计算每个角的弧度
        float radian = (float)(Mathf.PI / 2 / TrinagleNum);
        //当前角度 这是递增的
        float curRadian = 0;

        for (int i = 0; i < vCenterList.Count; i++)
        {
            int preVertNum = toFill.currentVertCount;
            for (int j = 0; j <= TrinagleNum; j++)
            {
                float cosA = Mathf.Cos(curRadian);//求邻边与对边的比值  
                float sinA = Mathf.Sin(curRadian);//求对边与邻边的比值

                float adijacentSide = cosA * radius; //邻边的长度
                float opposite = sinA * radius;// 对边的长度

                float uvAdijacent = cosA * uvRadiusX;
                float uvOpppsite = sinA * uvRadiusY;

                Vector3 vPos = new Vector3(vCenterList[i].x + adijacentSide, vCenterList[i].y + opposite);
                Vector2 uvPos = new Vector2(uvCenterList[i].x + uvAdijacent, uvCenterList[i].y + uvOpppsite);
                //Vector3 vPos = new Vector3(vCenterList[i].x + opposite, vCenterList[i].y + adijacentSide);
                //Vector2 uvPos = new Vector2(uvCenterList[i].x + uvOpppsite, uvCenterList[i].y + uvAdijacent);
                toFill.AddVert(vPos, color32, uvPos);
                Debug.Log(Mathf.Rad2Deg* curRadian);
                curRadian += radian;
            }
            curRadian -= radian;

            Debug.Log(Mathf.Rad2Deg * curRadian);
            for (int j = 0; j < TrinagleNum; j++)
            {
                toFill.AddTriangle(vCenterVertList[i], preVertNum+j+1, preVertNum + j);
             }
        }




    }




    /// <summary>
    /// 计算 精灵的包围框  通过GetPixelAdjustedRect
    /// </summary>
    Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
    {
        //获取精外框 透明区域的尺寸 x左边的透明宽度 y底部透明尺寸 z右变的透明宽度 w顶部的透明高度
        //这个Demo的 x: 5.1 y: 8.1  z: 7.1 w: 5.1
        Vector4 padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
        //获取当前Graphic矩形绘制范围 r.x  =-(r.width* pivot.x)  r.y  = -(pivot.y *r.height)
        Rect r = GetPixelAdjustedRect();
        //得到精灵的 尺寸
        Vector2 size = overrideSprite == null ? new Vector2(r.width, r.height) : new Vector2( overrideSprite.rect.width, overrideSprite.rect.height);

        int spriteW = Mathf.RoundToInt(size.x);
        int spriteH = Mathf.RoundToInt(size.y);

        //Debug.Log("PaddIng" + padding);
        //Debug.Log("Rect" + r);
        //Debug.Log("size" + size);


        if (shouldPreserveAspect && size.sqrMagnitude>0)
        {
            float spriteRadio = size.x / size.y;
            float rectRadio = r.width / r.height;

            if (spriteRadio>rectRadio) //一般都是相等吧
            {
                float oldHeight = r.height;
                r.height = 1 / (r.width * spriteRadio);
                r.y += (oldHeight - r.height) * rectTransform.pivot.y;
            }
            else //重新计算r. x 和 宽度
            {
                float oldWidth = r.width;
                r.width = r.height * spriteRadio;
                r.x += (oldWidth - r.width) * rectTransform.pivot.x;
            }
        }
        //获取空白区域占的宽高比例
        Vector4 v = new Vector4()
        {
            x = padding.x / spriteW,//左边
            y = padding.y / spriteH,//下边
            z = (spriteW - padding.z) / spriteW,//右边
            w = (spriteH -  padding.w) / spriteH,//上边
        };

        Vector4 outRect = new Vector4()
        {
            x = r.x + r.width * v.x,
            y = r.y + r.height * v.y,
            z = r.x + r.width * v.z,
            w = r.y + r.height * v.w,
        };
        return outRect;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (vectors.Length == 0) return ;
        Gizmos.color = Color.red;


        //RectTransformUtility.ScreenPointToWorldPointInRectangle

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(vectors[i], 20);
        }

    }
#endif



}
