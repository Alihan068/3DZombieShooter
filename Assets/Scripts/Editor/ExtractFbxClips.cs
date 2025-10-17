#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class ExtractFbxClips {
    [MenuItem("Tools/Animation/Extract Clips From Selected FBX...")]
    public static void ExtractFromSelection() {
        var selection = Selection.objects;
        if (selection == null || selection.Length == 0) {
            EditorUtility.DisplayDialog("Extract Clips", "Select one or more FBX assets first.", "OK");
            return;
        }

        var outDirAbs = EditorUtility.SaveFolderPanel("Choose output folder for .anim files", "Assets", "");
        if (string.IsNullOrEmpty(outDirAbs)) return;

        var projectRoot = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        if (!outDirAbs.Replace('\\', '/').StartsWith(projectRoot.Replace('\\', '/'))) {
            EditorUtility.DisplayDialog("Extract Clips", "Folder must be inside this Unity project.", "OK");
            return;
        }

        var outDirRel = outDirAbs.Substring(projectRoot.Length).Replace("\\", "/");

        AssetDatabase.StartAssetEditing();
        try {
            foreach (var obj in selection) {
                var fbxPath = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(fbxPath) || !fbxPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var baseName = Path.GetFileNameWithoutExtension(fbxPath);

                foreach (var a in AssetDatabase.LoadAllAssetsAtPath(fbxPath)) {
                    var srcClip = a as AnimationClip;
                    if (srcClip == null) continue;
                    if (srcClip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase)) continue;

                    var newClip = Object.Instantiate(srcClip);

                    var cleanBase = SanitizeName(baseName);
                    var cleanClip = SanitizeName(srcClip.name);

                    newClip.name = cleanClip;

                    var combined = CombineNonEmpty(cleanBase, cleanClip);     // Uses a single space
                    if (string.IsNullOrWhiteSpace(combined)) combined = "Clip";

                    var safeFile = MakeSafeFileName(combined + ".anim");
                    var targetPath = AssetDatabase.GenerateUniqueAssetPath($"{outDirRel}/{safeFile}");

                    AssetDatabase.CreateAsset(newClip, targetPath);
                }
            }
        }
        finally {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Extract Clips", "Done extracting", "OK");
    }

    static string SanitizeName(string s) {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;

        // Remove "mixamo.com" (any case)
        var result = Regex.Replace(s, "mixamo\\.com", "", RegexOptions.IgnoreCase);

        // Remove underscores entirely
        result = result.Replace("_", "");

        // Tidy common separators and whitespace
        result = result.Replace(" - ", " ");
        while (result.Contains("  ")) result = result.Replace("  ", " ");

        // Trim leftover punctuation/spaces
        result = result.Trim(' ', '-', '_', '.');

        return result;
    }

    static string CombineNonEmpty(string a, string b) {
        if (string.IsNullOrWhiteSpace(a)) return b ?? string.Empty;
        if (string.IsNullOrWhiteSpace(b)) return a ?? string.Empty;
        return (a + " " + b).Trim();
    }

    static string MakeSafeFileName(string s) {
        foreach (var ch in Path.GetInvalidFileNameChars())
            s = s.Replace(ch.ToString(), "");

        // Ensure no underscores (already removed in SanitizeName, but keep consistent)
        s = s.Replace("_", "");

        // Collapse spaces, trim trailing dots/spaces
        while (s.Contains("  ")) s = s.Replace("  ", " ");
        s = s.Trim().TrimEnd('.');

        return s;
    }
}
#endif