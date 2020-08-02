Shader "Custom/Progress"
{
    Properties
    {
       [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        
        _StencilComp("Stencil Comparison",float) = 8
        _Stencil("Stencil ID",float) = 0
        _StencilOp("Stencil Operation",float) = 0
        _StencilWriteMask("Stencil WriteMask",float) =255
        _StencilReadMask("Stencil ReadMask",float) =255


        _ColorOne("Color One",Color) = (1,1,1,1)
        _ColorTwo("Color Two",Color)= (1,1,1,1)
        _ColorThree("Color Three",Color) = (1,1,1,1)

        _FillAmountOne("FillAmount One",Range(0,1)) = 0
        _FillAmountTwo("FillAmount Two",Range(0,1)) = 0
        _FillAmountThree ("FillAmount Three" ,Range(0,1)) =0

        _FillAppend("FillApend",Range(0,0.05))=0.01

        [Enum(UnityEngine.Rendering.BlendMode)]_BlendScr("BlendSrc",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendDest("BlendDest",float) = 0

        [Enum(UnityEngine.Rendering.BlendMode)]_BlendScrAlpha("BlendScrAlpha",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendDestAlpha("BlendDestAlpha",float) = 0

    }

    SubShader
    {
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
       
        

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            WriteMask [_StencilWriteMask]
            ReadMask [_StencilReadMask]
        }

        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="false"
        }

        CGINCLUDE
            #include "UnityCG.cginc"
            //////*********声明 变量 ***********
            sampler2D _MainTex;
            float4 _MainTex_ST; 
            fixed4 _ColorOne;
            fixed4 _ColorTwo;
            fixed4 _ColorThree;
            fixed _FillAmountOne;
            fixed _FillAmountTwo;
            fixed _FillAmountThree;
            fixed _FillAppend;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };
               struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  TRANSFORM_TEX(v.uv,_MainTex);
                return o;
            }
        ENDCG

    
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            NAME "C1"
            CGPROGRAM
           
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 originCol = tex2D(_MainTex, uv);
                //1111111

                fixed4 outCol;
                 outCol.rgb = originCol.rgb*_ColorOne.rgb;
                
                float alpha = step(uv.x,_FillAmountOne);
                outCol.a = alpha;
                outCol.a*=originCol.a;
                outCol*=outCol.a;

                //222222

                fixed4 outCol_2;
                outCol_2.rgb = originCol.rgb*_ColorTwo.rgb;
                alpha = step(_FillAmountTwo,uv.x);

                fixed alpha_2 = step(_FillAmountOne+_FillAppend,uv.x);
                fixed alpha_3 = 1-step(_FillAmountThree,uv.x);
                fixed alpha_4 = alpha_2*alpha_3;


                outCol_2.a = alpha_4;
                outCol_2.a*=originCol.a;
                outCol_2*=outCol_2.a;
            
                //3333333
                fixed4 outCol_3;
                outCol_3.rgb =  originCol.rgb*_ColorThree.rgb;
                alpha  =step(_FillAmountThree+_FillAppend,uv.x);
                fixed alpha_5 = step()

                outCol_3.a = alpha;
                outCol_3.a*=originCol.a;
                outCol_3*=outCol_3.a;

                return outCol+outCol_2+outCol_3 ;
            }
            ENDCG
        }

    }
    CustomEditor "ProgressShaderGUI"
}
