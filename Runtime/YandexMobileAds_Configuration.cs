using PrimeGames.SDK.Common;
using UnityEngine;

namespace PrimeGames.SDK.YandexMobileAds {

    [ProviderConfiguration(typeof(YandexMobileAds))]
    public class YandexMobileAds_Configuration : PropertyGroup {

        public override string Name => nameof(YandexMobileAds);

        [field: SerializeField] public string InterstitialAdUnitIdAndroid { get; private set; } = "demo-interstitial-yandex";
        [field: SerializeField] public string InterstitialAdUnitIdIOS { get; private set; } = "demo-interstitial-yandex";
        [field: SerializeField] public string RewardedAdUnitIdAndroid { get; private set; } = "demo-rewarded-yandex";
        [field: SerializeField] public string RewardedAdUnitIdIOS { get; private set; } = "demo-rewarded-yandex";

        public override StringProperty[] GetStringProperties() {
            return new StringProperty[] {
                new(
                    "Interstitial UID Android",
                    getter: () => { return InterstitialAdUnitIdAndroid; },
                    setter: (value) => { InterstitialAdUnitIdAndroid = value; }
                ),
                new(
                    "Interstitial UID iOS",
                    getter: () => { return InterstitialAdUnitIdIOS; },
                    setter: (value) => { InterstitialAdUnitIdIOS = value; }
                ),
                new(
                    "Rewarded UID Android",
                    getter: () => { return RewardedAdUnitIdAndroid; },
                    setter: (value) => { RewardedAdUnitIdAndroid = value; }
                ),
                new(
                    "Rewarded UID iOS",
                    getter: () => { return RewardedAdUnitIdIOS; },
                    setter: (value) => { RewardedAdUnitIdIOS = value; }
                )
            };
        }

    }

}
