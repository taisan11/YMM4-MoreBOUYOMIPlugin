using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Plugin.Voice;

namespace MoreBOUYOMIVoicePlugin {
    public record MoreBOUYOMIVoiceLicense : IVoiceLicense {
        //規約をどこに表示するか
        public VoiceLicenseDisplayLocation SummaryLocation
            => VoiceLicenseDisplayLocation.CharacterEditor;
        //規約概要。なくてもよい
        public string? Summary { get; } = "棒読みちゃんは様々な声が使用できるのでそれぞれを確認してください。\n公式サイトが参考になるかと思われます";
        public bool IsTermsAgreed { get; set; } = true;
        //ここにメッセージがあるとリンク文字列をクリックすると承認ダイアログ表示
        public string? Terms { get; }
        //TermsがnullならこちらのURLへ飛ぶようになる
        public string? TermsURL { get; }
            = "https://chi.usamimi.info/Program/Application/BouyomiChan/";

        public MoreBOUYOMIVoiceLicense(
            string? voiceTermsUrl = null
        )
        {
            if (voiceTermsUrl is not null)
            {
                TermsURL = voiceTermsUrl;
            }
        }

        public ValueTask<bool> ValidateLicenseAsync()
        {
            return new ValueTask<bool>(true);
        }
    }
}
