# EmotionChange_forVRM10
controlling VRM1.0 Expression  for Unity Timeline\
VRM1.0のプリセット表情とカスタム表情をUnityのTimelineで制御します。

## Unique Points
一つのインスペクター上で、表情の重ね合わせや減算を一括でコントロールできます。通常の表情プリセットでは表現しにくい表情も作りやすいです。\
カスタム表情の制御にも対応していますので、VRMキャラクターにカスタム表情を追加すれば、もっと表現の幅が広がります。\
クリップごとに、オートまばたきの設定も可能です。

## Environment
Unity2022.3.23f1\
VRM1.0

## User Guide
1.EmotionChangeBehaviourをVRM10Instanceのオブジェクトにアタッチします。\
（自動的に該当のVrmとSkinned Mesh Rendererが取得されます。）

2.TimeLineで右クリック「Add 〓PlayableTrack」を追加。VRM10Instanceオブジェクトをバインドします。\
　TimeLineのTrack上で右クリック「add VRM_Emotionchange_Clip」を配置します。

3.EmotionChangePlayableClipを編集します。\
　項目は上から五つの基本表情「ExpressionPreset〓」を設定。\
 「ExpressionPreset」は基本の表情以外をホップアップから一つ選んで設定できます。－100%までの減算にも対応。\
 「ここは口を閉じておきたい」など細かい表情作りに便利です。\
 「CustomExpression」は、VRMキャラクターに個別に設定されたカスタム表情を制御できます。\
 初期設定では二つの「CustomExpression」が設定できます。右下の「＋」ボタンを押せば、カスタム表情の項目をさらに増やすことが可能です。\
 こちらも－100%までの減算に対応。自由度が高い分、表情が崩れやすいので注意してください。
 
 「AutoBlink」にチェックをいれるとタイムラインのクリップの期間、自動でまばたきをします。（こちらは編集中は作動せず、Play中のみ作動します）\
 「blinkInterval」はまばたきの間隔（初期設定は４秒に１回）、「blinkMaxWeight」は目を閉じる最大ウエイトを設定します。\
 半目のときは「blinkMaxWeight」のウエイトを小さく、目を見開いたときはウエイトを大きくすると、目の閉じすぎや白目の部分が残るといったトラブルを防げます。\
また、ランダムで「２回連続まばたき」が発生するギミックもあります。\
クリップの終了時直前にまばたきが発生すると、途中でまばたきがキャンセルされることがあるので、その場合は、「blinkInterval」の値を少し変えてください。

## Attention
「CustomExpression」のカスタム表情リストは、シーンにある全てのVRM10Instanceから取得しています。\
そのため、複数のVRMキャラクターがいる場合、設定していないカスタム表情も表示される場合があります。その表情のウエイトを上げても表情は変わりません。\
（例えば、VRM「A」と「B」がいるシーンで、「A」だけに設定したカスタム表情が、「B」のカスタム表情リストにも表示されますが、「B」でその表情のウエイトをあげても表情は変化しません）

## Advanced
表情のウエイト範囲は、「EmotionChangePlayableBehaviour」と「EmotionChangeDrawer」で設定。まばたきの設定は「VrmEmotionChangeBehaviour」でしています。\
細かくコメントを残していますので、元の設定を変更したい方は手を加えてください。

## Install
ファイルをダウンロードして、解凍後、UNITYプロジェクトの任意のASSETフォルダなどに入れてご使用ください。



















