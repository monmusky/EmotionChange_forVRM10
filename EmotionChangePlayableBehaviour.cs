using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniVRM10;

/// <summary>
/// EmotionChangeの各Clipのインスペクター表示を決定
/// </summary>
[System.Serializable]

public class EmotionChangePlayableBehaviour : PlayableBehaviour
{
    // 各プリセット表情とウェイト
    [SerializeField, Range(0f, 1f)] public float happyWeight = 0f;
    [Range(0f, 1f)] public float angryWeight = 0f;
    [Range(0f, 1f)] public float sadWeight = 0f;
    [Range(0f, 1f)] public float relaxedWeight = 0f;
    [Range(0f, 1f)] public float surprisedWeight = 0f;

    [Space] // スペース追加
    [Space] 

    //基本表情のリスト型
    public ExpressionPreset preset = ExpressionPreset.neutral;
    [SerializeField, Range(-1f, 1f)] public float expWeight = 0f;

    [Space] 
    [Space]

    // カスタム表情用辞書を追加.EmotionChangeDrawerで対応
    [SerializeField]
    public List<EmotionChangeData> customExpressions = new List<EmotionChangeData>();

    //カスタム表情の初期項目を２に設定
    public EmotionChangePlayableBehaviour()
    {
        InitializeDefaultExpressions();
    }
    private void InitializeDefaultExpressions()
    {
        if (customExpressions == null || customExpressions.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                customExpressions.Add(new EmotionChangeData
                {
                    customExpressionName = "",
                    weight = 0f
                });
            }
        }
    }

    //カスタム表情の初期項目を２に設定。Clone時にも初期化しておくと安全
    public override object Clone()
    {
        var clone = (EmotionChangePlayableBehaviour)base.Clone();
        clone.customExpressions = new List<EmotionChangeData>();
        for (int i = 0; i < 2; i++)
        {
            clone.customExpressions.Add(new EmotionChangeData
            {
                customExpressionName = "",
                weight = 0f
            });
        }
        return clone;
    }

    // 描画ロジックを追加
    public class CustomExpressionAttribute : PropertyAttribute { }
    [System.Serializable]
    public struct EmotionChangeData
    {
        [CustomExpression] 
        public string customExpressionName; // カスタム表情名
        public float weight; // カスタム表情の重み
    }

    //まばたき制御
    [Header("Blink Settings")]
    public bool AutoBlink;  // まばたき動作を有効にするか？
    [Range(0f, 8f)]
     public float blinkInterval = 4f;  //まばたきの間隔(秒)
    [SerializeField, Range(0f, 1.4f)]
    public float blinkMaxWeight = 1f;  //まばたき時の最大ウエイト   
}




