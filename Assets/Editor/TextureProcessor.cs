using System.IO;
using UnityEditor;
using UnityEngine;


public class TextureProcessorEditor : Editor
{
    // --- 功能 1：处理打包贴图 (R:Rough G:Met B:AO A:Emis) ---
    [MenuItem("Assets/RMAO->MS+AO", false, 100)]
    public static void ProcessRMAOTexture()
    {
        Texture2D source = GetSelectedTexture();
        if (source == null) return;

        string path = AssetDatabase.GetAssetPath(source);
        PrepareTexture(path); // 确保原图可读

        int w = source.width;
        int h = source.height;
        Color[] p = source.GetPixels();

        // 创建各通道容器
        Color[] msP = new Color[p.Length];
        Color[] aoP = new Color[p.Length];
        Color[] emP = new Color[p.Length];

        for (int i = 0; i < p.Length; i++)
        {
            // 原始布局: R=Rough, G=Met, B=AO, A=Emi
            float rough = p[i].r;
            float metal = p[i].g;
            float ao = p[i].b;
            //float emi = p[i].a;

            // Unity标准 MetallicSmoothness: R=Metal, A=1-Rough
            msP[i] = new Color(metal, 0, 0, 1.0f - rough);
            // AO: 纯B通道
            aoP[i] = new Color(ao, ao, ao, 1.0f);
            // Emission: 纯A通道
            //emP[i] = new Color(emi, emi, emi, 1.0f);
        }

        // 保存并自动设置导入参数 (关闭sRGB)
        SaveAndSetImport(msP, w, h, TextureFormat.RGBA32, path, "_MetSmooth.png", false);
        SaveAndSetImport(aoP, w, h, TextureFormat.RGB24, path, "_AO.png", false);
        //SaveAndSetImport(emP, w, h, path, "_Emission.png", false, false);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", "贴图已分离并生成到原文件夹！", "好的");
    }

    // --- 功能 2：修复红色法线贴图 ---
    [MenuItem("Assets/Fix Normal Map (R2B)", false, 101)]
    public static void FixNormalMap()
    {
        Texture2D source = GetSelectedTexture();
        if (source == null) return;

        string path = AssetDatabase.GetAssetPath(source);
        PrepareTexture(path);

        Color[] p = source.GetPixels();
        Color[] outP = new Color[p.Length];

        for (int i = 0; i < p.Length; i++)
        {
            float x = p[i].a;
            float y = p[i].g;
            float z = Mathf.Sqrt(Mathf.Max(0, 1.0f - x * x - y * y));
            outP[i] = new Color(x, y, z, 1.0f);
        }

        // 法线贴图在设置类型后，Unity会自动处理空间，无需手动设sRGB
        SaveAndSetImport(outP, source.width, source.height, TextureFormat.RGB24, path, "_FixedNormal.png", true);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", "法线修复成功！", "好的");
    }

    // --- 核心辅助函数：保存并设置导入属性 ---
    private static void SaveAndSetImport(Color[] pixels, int w, int h, TextureFormat format, string origPath, string suffix, bool isNormal)
    {
        Texture2D tex = new Texture2D(w, h, format, false);
        tex.SetPixels(pixels);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string dir = Path.GetDirectoryName(origPath);
        string name = Path.GetFileNameWithoutExtension(origPath);
        string newPath = dir + "/" + name + suffix;

        File.WriteAllBytes(newPath, bytes);
        AssetDatabase.ImportAsset(newPath); // 强制让Unity发现新文件

        // 获取新生成的贴图导入器
        TextureImporter importer = AssetImporter.GetAtPath(newPath) as TextureImporter;
        if (importer != null)
        {
            if (isNormal)
            {
                importer.textureType = TextureImporterType.NormalMap;
            }
            else
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = false; // <<< 关键：取消勾选 sRGB
            }
            importer.SaveAndReimport();
        }
    }

    private static void PrepareTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (!importer.isReadable || importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    private static Texture2D GetSelectedTexture()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex == null) EditorUtility.DisplayDialog("错误", "请先选中一张贴图！", "确定");
        return tex;
    }
}