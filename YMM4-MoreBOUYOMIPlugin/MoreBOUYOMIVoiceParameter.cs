using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Plugin.Voice;
using YukkuriMovieMaker.Controls;

namespace MoreBOUYOMIVoicePlugin {
    internal class MoreBOUYOMIVoiceParameter : VoiceParameterBase {
        int speed = 100;
        int pitch = 100;
        int volume = 100;

        [Display(Name = "読み上げ速度", Description = "テキストの読み上げ速度")]
        [TextBoxSlider("F0", "", 50, 300, Delay = -1)]
        [Range(50, 300)]
        [DefaultValue(100)]
        public int Speed { get => speed; set => Set(ref speed, value); }

        [Display(Name = "音程", Description = "合成された音声の音程\n詳しくはYMM4デフォルトの物を参考にしてください。")]
        [TextBoxSlider("F0", "", 50, 200, Delay = -1)]
        [Range(50, 200)]
        [DefaultValue(100)]
        public int Pitch { get => pitch; set => Set(ref pitch, value); }

        [Display(Name = "音量", Description = "合成された音声の音量\n棒読みちゃん側の音量を変更します")]
        [TextBoxSlider("F0", "", 0, 100, Delay = -1)]
        [Range(0, 100)]
        [DefaultValue(100)]
        public int Volume { get => volume; set => Set(ref volume, value); }
}
}