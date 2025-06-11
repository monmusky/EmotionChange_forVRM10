using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UniVRM10;

public class EmotionChangePlayableMixer : PlayableBehaviour
{
    public TimelineClip[] Clips { get; set; }
    public PlayableDirector Director { get; set; }

    // フレーム間で状態を保持する変数
    private ExpressionPreset? lastPreset = null;
    private float lastWeight = 0f;
    private float lastHappyWeight = 0f;
    private float lastAngryWeight = 0f;
    private float lastSadWeight = 0f;
    private float lastRelaxedWeight = 0f;
    private float lastSurprisedWeight = 0f;
    private bool lastBlinkState = false;

    // カスタム表情をフレーム間で状態を保持する変数
    private HashSet<string> lastCustomExpressionKeys = new HashSet<string>();

    // カスタム表情のウェイトを保持する辞書
    private Dictionary<string, float> lastCustomWeights = new Dictionary<string, float>();

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Clips.Length == 0) return;

        var vrmExp = playerData as EmotionChangeBehaviour;
        if (vrmExp == null)
        {
            return;
        }

        double timelineTime = Director.time; // 現在のタイムライン時間
        ExpressionPreset[] presets = (ExpressionPreset[])System.Enum.GetValues(typeof(ExpressionPreset));

        // カスタム表情の辞書を用意
        Dictionary<string, float> customExpWeights = new Dictionary<string, float>();

        // 各プリセットのウェイトを初期化
        Dictionary<ExpressionPreset, float> presetWeights = presets.ToDictionary(p => p, p => 0f);
        bool isBlinkActive = false;

        //カスタム表情のウエイトを初期化
        List<string> allExpressionKeys;
        if (Application.isPlaying)
        {
            // 実行時は Runtime の ExpressionKeys を利用
            allExpressionKeys = vrmExp.vrm.Runtime.Expression.ExpressionKeys
                .Where(k => k.Preset == ExpressionPreset.custom)
                .Select(k => k.Name)
                .ToList();
        }
        else
        {
            // エディタ中は、代替として Vrm.Expression.Clips から取得
            allExpressionKeys = vrmExp.vrm.Vrm.Expression.Clips
                .Where(c => c.Preset == ExpressionPreset.custom)
                .Select(c => c.Clip.name)
                .ToList();
        }

        foreach (var key in allExpressionKeys)
        {
            if (!customExpWeights.ContainsKey(key))
            {
                vrmExp.SetWeight(key, 0f);
            }
        }

        // まばたきパラメータの初期化
        float aggregatedBlinkInterval = 0f;
        float aggregatedBlinkMaxWeight = 0f;

        bool isAnyClipActive = false; // 現在アクティブなクリップがあるかを追跡

        //EmotionChangePlayableClipの呼び出し
        for (int i = 0; i < Clips.Length; i++)
        {
            var clip = Clips[i];
            var clipAsset = clip.asset as EmotionChangePlayableClip;
            var behaviour = clipAsset.behaviour;

            if (behaviour == null)
            {
                continue;
            }

            // クリップの進行状況を計算
            float clipStart = (float)clip.start;
            float clipDuration = (float)clip.duration;
            float clipProgress = (float)((timelineTime - clipStart) / clipDuration);

            // クリップがアクティブかどうかを確認
            if (clipProgress >= 0f && clipProgress <= 1f)
            {
                isAnyClipActive = true;

                float inputWeight = playable.GetInputWeight(i);
               float combinedWeight = inputWeight * behaviour.expWeight;
               presetWeights[behaviour.preset] += combinedWeight;

                // 個別のウェイトをプリセットウェイトに統合
                presetWeights[ExpressionPreset.happy] += inputWeight * behaviour.happyWeight;
                presetWeights[ExpressionPreset.angry] += inputWeight * behaviour.angryWeight;
                presetWeights[ExpressionPreset.sad] += inputWeight * behaviour.sadWeight;
                presetWeights[ExpressionPreset.relaxed] += inputWeight * behaviour.relaxedWeight;
                presetWeights[ExpressionPreset.surprised] += inputWeight * behaviour.surprisedWeight;

                // カスタム表情の加算処理
                foreach (var customExp in behaviour.customExpressions)
                {
                    if (string.IsNullOrEmpty(customExp.customExpressionName)) continue;
                    float combinedWeightoo = inputWeight * customExp.weight;
                    if (customExpWeights.ContainsKey(customExp.customExpressionName))
                    {
                        customExpWeights[customExp.customExpressionName] += combinedWeightoo;
                    }
                    else
                    {
                        customExpWeights.Add(customExp.customExpressionName, combinedWeightoo);
                    }
                }
                
                // 自動瞬きを有効化
                if (behaviour.AutoBlink)
                {
                     aggregatedBlinkInterval += inputWeight * behaviour.blinkInterval;
                    aggregatedBlinkMaxWeight += inputWeight * behaviour.blinkMaxWeight;
                    isBlinkActive = true;
           　　 }
               
                // このクリップを「直前のクリップ」として記録
                lastPreset = behaviour.preset; //プリセットリスト型の表情
                lastWeight = combinedWeight; //プリセットリスト型のウエイト
                lastHappyWeight = presetWeights[ExpressionPreset.happy];
                lastAngryWeight = presetWeights[ExpressionPreset.angry];
                lastSadWeight = presetWeights[ExpressionPreset.sad];
                lastRelaxedWeight = presetWeights[ExpressionPreset.relaxed];
                lastSurprisedWeight = presetWeights[ExpressionPreset.surprised];
                lastBlinkState = behaviour.AutoBlink;
            }
        }
        
        // クリップがアクティブでない場合、直前の状態を適用
        if (!isAnyClipActive && lastPreset.HasValue)
        {
            presetWeights[lastPreset.Value] = lastWeight; //プリセットリスト型のウエイト
            presetWeights[ExpressionPreset.happy] = lastHappyWeight;
            presetWeights[ExpressionPreset.angry] = lastAngryWeight;
            presetWeights[ExpressionPreset.sad] = lastSadWeight;
            presetWeights[ExpressionPreset.relaxed] = lastRelaxedWeight;
            presetWeights[ExpressionPreset.surprised] = lastSurprisedWeight;
            isBlinkActive = lastBlinkState;

            // カスタム表情も直前の状態を適用
            foreach (var kvp in lastCustomWeights)
            {
                customExpWeights[kvp.Key] = kvp.Value;
            }
        }

        // 計算されたウェイトをすべてのプリセットに適用
        foreach (var preset in presets)
        {
            vrmExp.SetWeight(preset, presetWeights[preset]);            
        }
        // カスタム表情を適用
        foreach (var kvp in customExpWeights)
        {
            vrmExp.SetWeight(kvp.Key, kvp.Value);
        }

        // カスタム表情とウェイトを保存
        lastCustomExpressionKeys = new HashSet<string>(customExpWeights.Keys);       
        lastCustomWeights = new Dictionary<string, float>(customExpWeights);

        // まばたきパラメータを設定
        vrmExp.SetBlinkSettings(isBlinkActive, aggregatedBlinkInterval, aggregatedBlinkMaxWeight);
    }
} 
