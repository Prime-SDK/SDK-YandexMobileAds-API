using PrimeGames.SDK.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YandexPlugin = global::YandexMobileAds;
using YandexPluginBase = global::YandexMobileAds.Base;
using Logger = PrimeGames.SDK.Common.Logger;

namespace PrimeGames.SDK.YandexMobileAds {

    [Provider(typeof(IAds))]
    public class YandexMobileAds : CommonAds {

        private readonly YandexMobileAds_Configuration configuration;

        // Interstitial fields
        private YandexPlugin.InterstitialAdLoader _interstitialLoader;
        private YandexPlugin.Interstitial _interstitial;
        private Action _currentInterstitialOnOpen;
        private Action<bool> _currentInterstitialOnClose;

        // Rewarded fields
        private YandexPlugin.RewardedAdLoader _rewardedLoader;
        private YandexPlugin.RewardedAd _rewarded;
        private Action _currentRewardedOnOpen;
        private Action<bool> _currentRewardedOnClose;
        private bool _rewardGranted;

        public YandexMobileAds(YandexMobileAds_Configuration configuration, IEventAggregator eventAggregator) : base(eventAggregator) {
            this.configuration = configuration;
            SetInitialized();
        }

        public override bool IsInterstitialAvailable => true;
        public override bool IsRewardedAvailable => true;

        private string GetInterstitialAdUnitId() {
#if UNITY_IOS
            return configuration.InterstitialAdUnitIdIOS;
#elif UNITY_ANDROID
            return configuration.InterstitialAdUnitIdAndroid;
#else
            Logger.Warning(this, "Unsupported platform for Yandex Ads. Using Android Ad Unit ID by default.");
            return configuration.InterstitialAdUnitIdAndroid;
#endif
        }

        private string GetRewardedAdUnitId() {
#if UNITY_IOS
            return configuration.RewardedAdUnitIdIOS;
#elif UNITY_ANDROID
            return configuration.RewardedAdUnitIdAndroid;
#else
            Logger.Warning(this, "Unsupported platform for Yandex Ads. Using Android Ad Unit ID by default.");
            return configuration.RewardedAdUnitIdAndroid;
#endif
        }

        // Banner is not supported in Yandex Mobile Ads SDK hence these methods just log warnings

        protected override void InvokeBannerImpl() {
            Logger.NotAvailableWarning(this, nameof(InvokeBannerImpl));
        }

        protected override void RefreshBannerImpl() {
            Logger.NotAvailableWarning(this, nameof(RefreshBannerImpl));
        }

        protected override void DisableBannerImpl() {
            Logger.NotAvailableWarning(this, nameof(DisableBannerImpl));
        }

        protected override void InvokeInterstitialImpl(InterstitialParameters parameters, Action onOpen, Action<bool> onClose) {
            string adUnitId = GetInterstitialAdUnitId();
            if (string.IsNullOrEmpty(adUnitId)) {
                onClose?.Invoke(false);
                return;
            }

            _currentInterstitialOnOpen = onOpen;
            _currentInterstitialOnClose = onClose;

            if (_interstitial != null) {
                AttachInterstitialHandlers();
                _interstitial.Show();
            }
            else {
                if (_interstitialLoader == null) {
                    _interstitialLoader = new YandexPlugin.InterstitialAdLoader();
                    _interstitialLoader.OnAdLoaded += HandleInterstitialLoaded;
                    _interstitialLoader.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
                }

                var config = new YandexPluginBase.AdRequestConfiguration.Builder(adUnitId).Build();
                try {
                    _interstitialLoader.LoadAd(config);
                }
                catch (Exception ex) {
                    Logger.CreateError(this, $"Failed to load interstitial ad configuration: {ex.Message}");
                    _currentInterstitialOnClose?.Invoke(false);
                    _currentInterstitialOnOpen = null;
                    _currentInterstitialOnClose = null;
                }
            }
        }

        private void HandleInterstitialLoaded(object sender, YandexPluginBase.InterstitialAdLoadedEventArgs args) {
            _interstitial = args.Interstitial;
            AttachInterstitialHandlers();
            _interstitial.Show();
        }

        private void HandleInterstitialFailedToLoad(object sender, YandexPluginBase.AdFailedToLoadEventArgs args) {
            Logger.CreateError(this, $"Interstitial ad failed to load: {args.Message}");
            _currentInterstitialOnClose?.Invoke(false);
            _currentInterstitialOnOpen = null;
            _currentInterstitialOnClose = null;
        }

        private void AttachInterstitialHandlers() {
            if (_interstitial == null) return;

            _interstitial.OnAdShown += HandleInterstitialShown;
            _interstitial.OnAdDismissed += HandleInterstitialDismissed;
            _interstitial.OnAdFailedToShow += HandleInterstitialFailedToShow;
        }

        private void HandleInterstitialShown(object sender, EventArgs e) {
            _currentInterstitialOnOpen?.Invoke();
        }

        private void HandleInterstitialDismissed(object sender, EventArgs e) {
            _currentInterstitialOnClose?.Invoke(true);
            CleanupInterstitial();
            _currentInterstitialOnOpen = null;
            _currentInterstitialOnClose = null;
        }

        private void HandleInterstitialFailedToShow(object sender, YandexPluginBase.AdFailureEventArgs e) {
            Logger.CreateError(this, $"Interstitial ad failed to show: {e.Message}");
            _currentInterstitialOnClose?.Invoke(false);
            CleanupInterstitial();
            _currentInterstitialOnOpen = null;
            _currentInterstitialOnClose = null;
        }

        private void CleanupInterstitial() {
            if (_interstitial != null) {
                _interstitial.OnAdShown -= HandleInterstitialShown;
                _interstitial.OnAdDismissed -= HandleInterstitialDismissed;
                _interstitial.OnAdFailedToShow -= HandleInterstitialFailedToShow;
                _interstitial.Destroy();
                _interstitial = null;
            }
        }

        protected override void InvokeRewardedImpl(RewardedParameters parameters, Action onOpen, Action<bool> onClose) {
            // rewardTag is ignored as per Yandex SDK; rewards are handled generically
            string adUnitId = GetRewardedAdUnitId();
            if (string.IsNullOrEmpty(adUnitId)) {
                onClose?.Invoke(false);
                return;
            }

            _currentRewardedOnOpen = onOpen;
            _currentRewardedOnClose = onClose;
            _rewardGranted = false;

            if (_rewarded != null) {
                AttachRewardedHandlers();
                _rewarded.Show();
            }
            else {
                if (_rewardedLoader == null) {
                    _rewardedLoader = new YandexPlugin.RewardedAdLoader();
                    _rewardedLoader.OnAdLoaded += HandleRewardedLoaded;
                    _rewardedLoader.OnAdFailedToLoad += HandleRewardedFailedToLoad;
                }

                var config = new YandexPluginBase.AdRequestConfiguration.Builder(adUnitId).Build();
                try {
                    _rewardedLoader.LoadAd(config);
                }
                catch (Exception ex) {
                    Logger.CreateError(this, $"Failed to load rewarded ad configuration: {ex.Message}");
                    _currentRewardedOnClose?.Invoke(false);
                    _currentRewardedOnOpen = null;
                    _currentRewardedOnClose = null;
                }
            }
        }

        private void HandleRewardedLoaded(object sender, YandexPluginBase.RewardedAdLoadedEventArgs args) {
            _rewarded = args.RewardedAd;
            _rewardGranted = false;
            AttachRewardedHandlers();
            _rewarded.Show();
        }

        private void HandleRewardedFailedToLoad(object sender, YandexPluginBase.AdFailedToLoadEventArgs args) {
            Logger.CreateError(this, $"Rewarded ad failed to load: {args.Message}");
            _currentRewardedOnClose?.Invoke(false);
            _currentRewardedOnOpen = null;
            _currentRewardedOnClose = null;
        }

        private void AttachRewardedHandlers() {
            if (_rewarded == null) return;

            _rewarded.OnAdShown += HandleRewardedShown;
            _rewarded.OnAdDismissed += HandleRewardedDismissed;
            _rewarded.OnAdFailedToShow += HandleRewardedFailedToShow;
            _rewarded.OnRewarded += HandleRewardedRewarded;
        }

        private void HandleRewardedShown(object sender, EventArgs e) {
            _currentRewardedOnOpen?.Invoke();
        }

        private void HandleRewardedDismissed(object sender, EventArgs e) {
            _currentRewardedOnClose?.Invoke(_rewardGranted);
            CleanupRewarded();
            _currentRewardedOnOpen = null;
            _currentRewardedOnClose = null;
        }

        private void HandleRewardedRewarded(object sender, YandexPluginBase.Reward reward) {
            _rewardGranted = true;
            // Reward details (amount, type) are available in 'reward' but ignored per contract; bool indicates grant
        }

        private void HandleRewardedFailedToShow(object sender, YandexPluginBase.AdFailureEventArgs e) {
            Logger.CreateError(this, $"Rewarded ad failed to show: {e.Message}");
            _currentRewardedOnClose?.Invoke(false);
            CleanupRewarded();
            _currentRewardedOnOpen = null;
            _currentRewardedOnClose = null;
        }

        private void CleanupRewarded() {
            if (_rewarded != null) {
                _rewarded.OnAdShown -= HandleRewardedShown;
                _rewarded.OnAdDismissed -= HandleRewardedDismissed;
                _rewarded.OnAdFailedToShow -= HandleRewardedFailedToShow;
                _rewarded.OnRewarded -= HandleRewardedRewarded;
                _rewarded.Destroy();
                _rewarded = null;
            }
        }

    }

}
