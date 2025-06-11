using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

[DisplayName("VRM_Emotionchange_Clip")] // 表示名を「"VRM_Emotionchange_Clip」に変更
public class EmotionChangePlayableClip : PlayableAsset
{
    public EmotionChangePlayableBehaviour behaviour = new EmotionChangePlayableBehaviour();

    public override double duration => 1.0; // 初期のクリップの長さ決定（秒）

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EmotionChangePlayableBehaviour>.Create(graph, behaviour);

        // 【修正】PlayableBehaviourに値をコピー
        var behaviourInstance = playable.GetBehaviour();
        behaviourInstance.preset = behaviour.preset;
        behaviourInstance.expWeight = behaviour.expWeight;

        // 【修正1】追加したウェイトの値をコピー
        behaviourInstance.happyWeight = behaviour.happyWeight;
        behaviourInstance.angryWeight = behaviour.angryWeight;
        behaviourInstance.sadWeight = behaviour.sadWeight;
        behaviourInstance.relaxedWeight = behaviour.relaxedWeight;
        behaviourInstance.surprisedWeight = behaviour.surprisedWeight;

        // 【修正】カスタム表情をコピー
        behaviourInstance.customExpressions = new List<EmotionChangePlayableBehaviour.EmotionChangeData>(behaviour.customExpressions);

        // まばたきパラメータのコピー
        behaviourInstance.AutoBlink = behaviour.AutoBlink;
        behaviourInstance.blinkInterval = behaviour.blinkInterval;
        behaviourInstance.blinkMaxWeight = behaviour.blinkMaxWeight;

        return playable;
    }
}
