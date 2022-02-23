# JITECMondaiFormatter

情報処理技術者試験の過去問題のPDFファイルを読み込んで1問ずつの画像にして出力するツール。

https://www.jitec.ipa.go.jp/1_04hanni_sukiru/_index_mondai.html

応用処理技術者試験の令和3年秋問題を何となく処理できる状態。

## 処理フロー

1. PDFium でPDFを画像に変換
2. Windows組み込みのOCRで画像から `問 xx` を探してそこで分割

構成の都合上特定のバージョンのWindows10でしか実行できません。


## 例

![q002](https://user-images.githubusercontent.com/4744735/155316832-9d7f3e42-bf1c-40c2-8257-601f10005074.png)

出典：令和3年度 秋期 応用情報技術者試験 午前 問2
