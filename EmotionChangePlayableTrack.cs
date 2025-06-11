using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UniVRM10;

[TrackBindingType(typeof(EmotionChangeBehaviour))] // コントロールする対象の型
[TrackColor(0, 0.5f, 0.5f)] // トラックの色
[TrackClipType(typeof(EmotionChangePlayableClip))] // 設定できるクリップの型（複数指定可能）
public class EmotionChangePlayableTrack : TrackAsset // TrackAssetを継承する
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Mixer
        var mixer = ScriptPlayable<EmotionChangePlayableMixer>.Create(graph, inputCount);
        var mixerBehaviour = mixer.GetBehaviour();
        mixerBehaviour.Clips = GetClips().ToArray();
        mixerBehaviour.Director = go.GetComponent<PlayableDirector>();

        //クリップ名の決定（ウエイトを比較して最大のものを選択）
        foreach (TimelineClip clip in mixerBehaviour.Clips)
        {
            var playableAsset = clip.asset as EmotionChangePlayableClip;
            if (playableAsset != null)
            {
                var behaviour = playableAsset.behaviour;

                Dictionary<string, float> weights = new Dictionary<string, float>();

                // カスタム表情
                foreach (var custom in behaviour.customExpressions)
                {
                    if (!string.IsNullOrEmpty(custom.customExpressionName))
                    {
                        weights[custom.customExpressionName] = custom.weight;
                    }
                }

                // Preset表情
                weights["Happy"] = behaviour.happyWeight;
                weights["Angry"] = behaviour.angryWeight;
                weights["Sad"] = behaviour.sadWeight;
                weights["Relaxed"] = behaviour.relaxedWeight;
                weights["Surprised"] = behaviour.surprisedWeight;

                // Behaviour自体のpreset
                if (behaviour.expWeight > 0f)
                {
                    weights[behaviour.preset.ToString()] = behaviour.expWeight;
                }

                // 最大ウェイトを取得
                float maxWeight = weights.Values.Max();

                // 最大ウェイトの項目だけ抽出
                var maxItems = weights
                    .Where(kv => Mathf.Approximately(kv.Value, maxWeight) && maxWeight > 0f)
                    .Select(kv => kv.Key)
                    .ToList();

                // 優先度: カスタム > Preset > その他
                string selectedName = maxItems
                    .OrderBy(name =>
                    {
                        bool isCustom = behaviour.customExpressions.Any(c => c.customExpressionName == name);
                        bool isPreset = System.Enum.TryParse<ExpressionPreset>(name, out _);
                        return isCustom ? 0 : isPreset ? 1 : 2;
                    })
                    .FirstOrDefault();

                // clip名を設定（見つからなければpreset名）
                clip.displayName = !string.IsNullOrEmpty(selectedName)
                    ? selectedName
                    : behaviour.preset.ToString();
            }
        }
        return mixer;
    }
}
