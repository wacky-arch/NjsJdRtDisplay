# NjsJdRtDisplay

Beat Saber 1.40.8 向け HUD拡張MODです。  
譜面プレイ中にリアルタイムの **NJS（譜面速度）**、**JD（ジャンプ距離）**、**RT（反応時間/ms）** を空間内のフローティングパネルに表示します。

---

## 🎯 特徴
- **見やすい配色とレイアウト**:
  - 上段中央: **NJS**（白色）
  - 下段左: **JD**（視認性の高いエメラルドグリーン #40FF66）
  - 下段右: **RT**（視認性の高い明るい紫色 #D978FF）
  - 文字サイズは 6.5 に統一、JDとRTの間隔を約2.5ユニットに寄せて視線移動を最適化。
- **ゲーム内設定対応**:
  - Mod Settings メニューからHUDの表示位置（X, Y, Z）や有効/無効を自由に変更可能（UserData/NjsJdRtDisplay.json に自動保存）。

---

## 🔧 前提MOD
- [BSIPA](https://github.com/beat-saber-modding-group/BeatSaber-IPA-Reloaded) (4.3.0+)
- [SiraUtil](https://github.com/Auros/SiraUtil) (3.1.0+)
- [BeatSaberMarkupLanguage](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) (1.11.0+)

---

## 📦 インストール方法
1. 最新リリースの NjsJdRtDisplay.dll をダウンロードします。
2. Beat Saber のインストール先にある Plugins フォルダに配置します。
3. ゲームを起動すると自動的に読み込まれます。

---

## 🛠️ 開発・ビルド環境
- .NET Framework 4.7.2
- C# 8.0+
- [BeatSaberModdingTools](https://github.com/Zingabopp/BeatSaberModdingTools)
