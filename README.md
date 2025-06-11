# EmotionChange_forVRM10
controlling VRM1.0 Expression  for Unity Timeline\
VRM1.0キャラクターの表情アニメーションをTimelineで直感的にコントロール！\
このスクリプトセットは、UnityのTimeline上でVRM1.0モデルのプリセット表情とカスタム表情を簡単に制御できるツールです。\
加えて、自動まばたき制御もサポートしており、よりリアルで自然なアニメーション表現が可能になります。

## Unique Points
・一つのインスペクター上で、VRM1.0モデルのプリセット表情とカスタム表情を簡単に制御 \
・複数の表情の合成ができるので、通常の表情プリセットでは表現しにくい表情も作りやすいです。\
・カスタム表情にも対応していますので、VRMキャラクターにカスタム表情を追加すれば、もっと表現の幅が広がります。\
・クリップごとに自動まばたきのオンオフ・間隔・強度設定が可能\
・カスタム表情名をインスペクター上で検索できるUIを提供

## Environment
Unity2022.3.23f1\
VRM1.0

## User Guide
1.ファイルをダウンロードして、解凍後、UNITYプロジェクトのASSETフォルダに入れる\
2.EmotionChangeBehaviourをVRM10Instanceのオブジェクトにアタッチします。\
（自動的に該当のVrmとSkinned Mesh Rendererが取得されます。）

3.TimeLineで右クリック「Emotion Change Playable Track」を追加。VRM10Instanceオブジェクトをバインドします。\
　TimeLineのTrack上で右クリック「add VRM_Emotionchange_Clip」を配置します。

4.EmotionChangePlayableClipを編集します。\
　項目は上から五つの基本表情を設定。\
 「Preset」は基本の表情以外をホップアップから一つ選んで設定できます。－100%までの減算にも対応。\
 「ここは口を閉じておきたい」など細かい表情作りに便利です。\
 「Custom Expressions」は、VRMキャラクターに個別に設定されたカスタム表情を制御できます。\
 初期設定では二つの「CustomExpression」が設定できます。右下の「＋」ボタンを押せば、カスタム表情の項目をさらに増やすことが可能です。\
 こちらも－100%までの減算に対応。自由度が高い分、表情が崩れやすいので注意してください。\
 カスタム表情リストの検索窓「Search Filter」も一応ついています。
 
「Auto Blink」にチェックをいれるとタイムラインのクリップの期間、自動でまばたきをします。（こちらは編集中は作動せず、Play中のみ作動します）\
「Blink Interval」はまばたきの間隔（初期設定は４秒に１回）、「Blink Max Weight」は目を閉じる最大ウエイトを設定します。\
半目のときは「blinkMaxWeight」のウエイトを小さく、目を見開いたときはウエイトを大きくすると、目の閉じすぎや白目の部分が残るといったトラブルを防げます。\
また、ランダムで「２回連続まばたき」が発生するギミックもあります。\
クリップの終了時直前にまばたきが発生すると、途中でまばたきがキャンセルされることがあるので、その場合は、「Blink Interval」の値を少し変えてください。

## Attention
「CustomExpression」のカスタム表情リストは、シーンにある全てのVRM10Instanceから取得しています。\
そのため、複数のVRMキャラクターがいる場合、設定していないカスタム表情名も表示される場合があります。その表情のウエイトを上げても表情は変わりません。

## Advanced
表情のウエイト範囲は、「EmotionChangePlayableBehaviour」と「EmotionChangeDrawer」で設定。まばたきの設定は「EmotionChangeBehaviour」でしています。
細かくコメントを残していますので、元の設定を変更したい方は手を加えてください。


















