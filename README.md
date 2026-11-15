# YMM4-MoreBOUYOMIPlugin
これは棒読みちゃんをどうしても使いたい人が作成したPluginです。
## 使い方
### 前提ソフトのインストール
使用するには[棒読みちゃん](https://chi.usamimi.info/Program/Application/BouyomiChan/)と自作のこちらの[棒読みちゃん用のPlugin](https://github.com/taisan11/Plugin_HTTPAPICreateWav)が必要です。
まず棒読みちゃんをダウンロードし、任意のディレクトリに展開!!そして、中にある実行ファイルと同じ場所に棒読みちゃん用のPluginを入れます。
### このPluginのインストール
リリースページにある`.ymme`をダウンロードしてYMM4にドラッグアンドドロップすればOKです。
そしたら他の声質と同じように使えます。

### 棒読みちゃんの自動起動
YMM4の「設定 → ボイス → 棒読みちゃん」で「棒読みちゃんの実行ファイル」に`BouyomiChan.exe` を指定してください。
### rawモード（既定で有効）
YMM4側でセリフをAquesTalk音声記号列に変換します。

- **AquesTalk系の声専用**です。それ以外の声では、YMM4の「設定 → ボイス → 棒読みちゃん」で
  「rawモードを使用する」を無効にしてください。
- 音程と音量は変更できません。ご了承ください。
## 開発(こっからほぼAI Slopですがお許しください。)

### 必要な環境
- Windows x64
- .NET SDK 10.0以降
- .NET 10版のYMM4 / YMM4 Lite

### YMM4のDLL参照

YMM4_PATHにPATHを通してください。

### ビルドと配布パッケージ
リポジトリのルートで実行します。

```powershell
dotnet build -c Release
dotnet publish YMM4-MoreBOUYOMIPlugin/YMM4-MoreBOUYOMIPlugin.csproj -c Release
```

- DLL: `YMM4-MoreBOUYOMIPlugin/bin/Release/net10.0-windows10.0.19041.0/YMM4-MoreBOUYOMIPlugin.dll`
- インストール用パッケージ: `artifacts/YMM4-MoreBOUYOMIPlugin.ymme`
- パッケージにはプラグイン本体、README、ライセンスを含めます。

開発用YMM4へ直接配置したい場合だけ、**YMM4を終了してから**次を実行してください。

```powershell
dotnet build -c Debug -p:DeployToYmm4=true
```

配置先は `$(YMM4_PATH)/user/plugin/YMM4-MoreBOUYOMIPlugin/` です。
既存フォルダの削除や設定ファイルの上書きは行いません。

### 設定の保持
- 設定画面と音声生成は同じ `MoreBOUYOMIVoiceSettings.Default` を参照します。
- HTTPプラグインのポート番号、棒読みちゃんの実行ファイル、rawモードの有効・無効、話者キャッシュをYMM4の `SettingsBase` で保存します。
  ポートの変更後はYMM4を通常終了してください。
- 保存先はYMM4が管理する `user/setting/<YMM4のバージョン>/MoreBOUYOMIVoicePlugin.MoreBOUYOMIVoiceSettings.json` です。
- 従来の `WavPortNumber` はそのまま使用します。本体用の `PortNumber` は使用しません。それ以前の形式の設定移行は行わないため、必要に応じて再設定してください。
- 話者一覧は取得に成功した場合だけ更新・保存し、取得失敗時は既存のキャッシュを保持します。
  HTTPプラグインのポートを変更した場合は、必要に応じて声質一覧を再読み込みしてください。

### 参考
- [ymme-vscode-boilerplate](https://github.com/InuInu2022/ymme-vscode-boilerplate):
  `YMM4_PATH` によるDLL参照、CLIタスク、`.ymme` 作成
- [YMM4plugin_template](https://github.com/leftcontroller0518/YMM4plugin_template):
  .NET 10 / WPFのプロジェクト構成、設定クラスへの直接バインド

## 裏話
### 名前の理由
もっと棒読みちゃんとの互換性を増やしたい!!っていう意味です。
