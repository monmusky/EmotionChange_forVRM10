using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;
using static EmotionChangePlayableBehaviour;
[RequireComponent(typeof(Vrm10Instance))]
public class EmotionChangeBehaviour : MonoBehaviour
{
   public Vrm10Instance vrm;
   public SkinnedMeshRenderer skinnedMeshRenderer;

    //アタッチ時にゲームオブジェクトを自動取得
    void Reset()
    {
        vrm = GetComponent<Vrm10Instance>();
        // Vrm10Instance 内の "Face" オブジェクトの SkinnedMeshRenderer を取得
        if (vrm != null)
        {           
            skinnedMeshRenderer = vrm.transform.Find("Face")?.GetComponent<SkinnedMeshRenderer>();
        }
    }

    // まばたき制御の変数
    private bool isAutoBlink;
    private float blinkWeight = 0.0f;
    private float blinkWaitTime = 0f;
    private float blinkTimer = 0f;
    private bool isBlinking = false;

    // まばたきパラメータ初期値
    private float timelineBlinkInterval = 4f;   // まばたき間隔（秒）
    private float timelineBlinkMaxWeight = 1f;    // まばたき時の最大ウェイト

    private bool previousCycleWasConsecutive = false; // 「連続まばたき」が直前に起こったかどうかを管理するフラグ

    void LateUpdate()
    {
        if (isAutoBlink)
        {
            if (!isBlinking)
            {
                if (blinkWaitTime <= 0f)
                {
                    isBlinking = true;
                    blinkTimer = 0f;
                }
                else
                {
                    blinkWaitTime -= Time.deltaTime;
                }
            }
            else
            {
                blinkTimer += Time.deltaTime;
                // 各フェーズの時間（秒）
                float closeDuration = 0.09f;
                float closedDuration = 0.09f;
                float openDuration = 0.12f;
                float totalDuration = closeDuration + closedDuration + openDuration;

                if (blinkTimer <= closeDuration)
                {
                    // 閉じる動作（イーズイン）
                    float t = blinkTimer / closeDuration;
                    blinkWeight = (t * t) * timelineBlinkMaxWeight;
                }
                else if (blinkTimer <= closeDuration + closedDuration)
                {
                    // 完全に閉じた状態
                    blinkWeight = timelineBlinkMaxWeight;
                }
                else if (blinkTimer <= totalDuration)
                {
                    // 開く動作（イーズアウト）
                    float t = (blinkTimer - closeDuration - closedDuration) / openDuration;
                    blinkWeight = (1f - t) * (1f - t) * timelineBlinkMaxWeight;
                }
                else
                {
                    // 瞬きのサイクルを終了
                    isBlinking = false;
                    blinkWeight = 0f;
                    SetWeight(ExpressionKey.Blink, blinkWeight);

                    // 「連続まばたき」のための条件
                    if (!previousCycleWasConsecutive && Random.value < 0.25f)   // 前回が「連続まばたき」でない、かつ25%の確率で連続まばたき
                    {
                        // 連続まばたきの待機時間（0.06秒）
                        blinkWaitTime = 0.06f;
                        previousCycleWasConsecutive = true; //「連続まばたき」が次発生しない
                    }
                    else
                    {
                        blinkWaitTime = timelineBlinkInterval;
                        previousCycleWasConsecutive = false;
                    }
                    return;
                }
                SetWeight(ExpressionKey.Blink, blinkWeight);

            }
        }
    }

    //各表情の取得。EmotionChangePlayableMixerで辞書みたいに使用
    public void SetWeight(ExpressionKey key,float weight){
        vrm.Runtime.Expression.SetWeight(key,weight);
    }
    public  void SetWeight(ExpressionPreset preset, float weight)
    {
        if (Application.isPlaying)
        {
            List<ExpressionKey> expList = vrm.Runtime.Expression.ExpressionKeys.ToList();
            var idx =  expList.FindIndex(k => k.Preset == preset);
            if (idx < 0) return;
            var key = expList[idx];
            vrm.Runtime.Expression.SetWeight(key, weight);           
        }
        else{
            // Editor
            var clip = vrm.Vrm.Expression.Clips.FirstOrDefault(c => c.Preset == preset);
            if (clip == default) return;
            foreach(var binding in clip.Clip.MorphTargetBindings){
                skinnedMeshRenderer.SetBlendShapeWeight(binding.Index,binding.Weight*weight * 100);
            }
        }
    }
    // カスタム表情名でのSetWeight
    public void SetWeight(string expressionName, float weight)
    {
        if (Application.isPlaying)
        {
            var key = vrm.Runtime.Expression.ExpressionKeys.FirstOrDefault(k => k.Name == expressionName);
            if (string.IsNullOrEmpty(key.Name)) return;
            vrm.Runtime.Expression.SetWeight(key, weight);
        }
        else
        {
            var clip = vrm.Vrm.Expression.Clips.FirstOrDefault(c => c.Clip.name == expressionName);
            if (clip == default) return;
            foreach (var binding in clip.Clip.MorphTargetBindings)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(binding.Index, binding.Weight * weight * 100);
            }
        }
    }
    //まばたきパラメータ更新用
    public void SetBlinkSettings(bool isBlinkActive, float interval, float maxWeight)
    {
        this.isAutoBlink = isBlinkActive;
        this.timelineBlinkInterval = interval;
        this.timelineBlinkMaxWeight = maxWeight;
    }
}
