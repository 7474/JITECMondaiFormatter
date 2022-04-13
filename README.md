# JITECKakomon

[情報処理技術者試験の過去問題](https://www.jitec.ipa.go.jp/1_04hanni_sukiru/_index_mondai.html)に関する遊びリポジトリ。

## JITECEntity

各プロジェクトで共通して使う型。

## JITECKakomonViewer

過去問題を閲覧する静的サイトのジェネレータ。

https://www.statiq.dev/ を用いている。

https://7474.github.io/JITECKakomon/


## JITECKakomonFunctions

過去問題をTwitterに投稿するBotのAzure Funcsions。

### IPA過去問Bot

情報処理技術者試験の過去問題の画像とアンケートをセットでつぶやくBot。

スケジュールなどはLogic Apps（コード未管理）。

https://twitter.com/ipa_kakomon

> ![image](https://user-images.githubusercontent.com/4744735/162548806-8142e216-dbe3-4f2e-bb44-1b1a78d12bd0.png)


## JITECMondaiFormatter

情報処理技術者試験の過去問題のPDFファイルを読み込んで1問ずつの画像にして出力するツール。

応用処理技術者試験の令和3年秋問題を何となく処理できる状態。

### 処理フロー

1. PDFium でPDFを画像に変換
2. Windows組み込みのOCRで画像から `問 xx` を探してそこで分割

構成の都合上特定のバージョンのWindows10でしか実行できません。


### 例

![q002](https://user-images.githubusercontent.com/4744735/155316832-9d7f3e42-bf1c-40c2-8257-601f10005074.png)

出典：令和3年度 秋期 応用情報技術者試験 午前 問2
