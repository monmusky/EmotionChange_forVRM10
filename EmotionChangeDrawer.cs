using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniVRM10;


[CustomPropertyDrawer(typeof(EmotionChangePlayableBehaviour.EmotionChangeData))]
public class EmotionChangeDrawer : PropertyDrawer
{
    // 検索クエリはstaticで全体共有
    private static string searchQuery = "";
    private static SerializedObject lastSerializedObject = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // クリップ切り替え時に検索文字列をクリア
        if (lastSerializedObject != property.serializedObject)
        {
            searchQuery = "";
            lastSerializedObject = property.serializedObject;
        }

    
        var nameProp = property.FindPropertyRelative("customExpressionName");
        var weightProp = property.FindPropertyRelative("weight");
        if (nameProp == null || weightProp == null)
        {
            EditorGUI.LabelField(position, label.text, "Invalid EmotionChangeData.");
            return;
        }

        // リストの最初の要素だけ検索窓を描画
        bool isFirstElement = property.propertyPath.EndsWith(".Array.data[0]");

        float y = position.y;
        if (isFirstElement)
        {
            var searchRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            searchQuery = EditorGUI.TextField(searchRect, "Search Filter", searchQuery);
            y += EditorGUIUtility.singleLineHeight + 2;
        }

        // カスタム表情リスト取得（例: シーン内のVrm10Instanceから取得）
        var allVrm = GameObject.FindObjectsOfType<Vrm10Instance>();
        var expressionKeys = new HashSet<string>();
        foreach (var vrm in allVrm)
        {
            if (vrm?.Vrm?.Expression?.Clips == null) continue;
            foreach (var clip in vrm.Vrm.Expression.Clips)
            {
                if (clip.Preset == ExpressionPreset.custom)
                {
                    expressionKeys.Add(clip.Clip.name);
                }
            }
        }
        string[] customNames = expressionKeys.ToArray();

        // 検索フィルタ
        var filteredNames = customNames
            .Where(name => string.IsNullOrEmpty(searchQuery) || name.ToLower().Contains(searchQuery.ToLower()))
            .ToArray();

        // ↓ ここでreturnせず、必ずドロップダウンを表示する
        var dropdownRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
        string currentValue = nameProp.stringValue;
        List<string> popupList = filteredNames.ToList();
        int currentIndex = popupList.IndexOf(currentValue);

        // 新規クリップ時（currentValueが空）の場合は先頭を選択
        if (string.IsNullOrEmpty(currentValue) && popupList.Count > 0)
        {
            currentIndex = 0;
            nameProp.stringValue = popupList[0];
        }
        else if (currentIndex == -1 && !string.IsNullOrEmpty(currentValue))
        {
            popupList.Insert(0, currentValue);
            currentIndex = 0;
        }

        // 選択肢が1つもない場合は空のPopupを表示
        if (popupList.Count == 0)
        {
            EditorGUI.LabelField(dropdownRect, "No expressions found.");
        }
        else
        {
            int newIndex = EditorGUI.Popup(dropdownRect, "Expression", currentIndex, popupList.ToArray());
            nameProp.stringValue = popupList[newIndex];
        }

        var sliderRect = new Rect(position.x, y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        weightProp.floatValue = EditorGUI.Slider(sliderRect, "Weight", weightProp.floatValue, 0f, 1f);  //カスタム表情のweightの範囲[Range(0f, 1f)]を決定
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 最初の要素だけ検索窓分を追加
        bool isFirstElement = property.propertyPath.EndsWith(".Array.data[0]");
        int lineCount = isFirstElement ? 3 : 2;
        return EditorGUIUtility.singleLineHeight * lineCount + (lineCount - 1) * 2;
    }
}
