using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Ads
{
    public class AdsManager : MonoBehaviour
    {
        [Header("AdMob IDs")]
        [SerializeField] private string _androidAppId = "ca-app-pub-3940256099942544~3347511713"; // Тестовый App ID
        [SerializeField] private string _adUnitId = "ca-app-pub-3940256099942544/6300978111"; // Тестовый
        [SerializeField] private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Тестовый
        [SerializeField] private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Тестовый

        private bool _isInitialized;
        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            //Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            MobileAds.Initialize(initStatus =>
            {
                _isInitialized = true;
                Debug.Log("AdMob initialized. SDK: " + initStatus);
                PreloadAds();
            });
        }

        private void PreloadAds()
        {
            LoadRewarded();
        }

        public void ShowBanner()
        {
            if (!_isInitialized) return;

            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

            _bannerView.LoadAd(new AdRequest());
            _bannerView.Hide(); // Скрыт по умолчанию, показывайте когда нужно
        }

        public void ShowBannerAd() => _bannerView?.Show();
        public void HideBannerAd() => _bannerView?.Hide();
        public void DestroyBanner() => _bannerView?.Destroy();

        public void LoadInterstitial()
        {
            InterstitialAd.Load(_interstitialAdUnitId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial load failed: " + error);
                    return;
                }
                _interstitialAd = ad;
                RegisterInterstitialEvents();
                Debug.Log("Interstitial loaded.");
            });
        }

        public void ShowInterstitial()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                _interstitialAd.Show();
                _interstitialAd = null; // Ссылка обнуляется после показа
                LoadInterstitial();     // Сразу загружаем следующую
            }
        }

        private void RegisterInterstitialEvents()
        {
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed.");
                LoadInterstitial(); // Предзагрузка следующей
            };
            _interstitialAd.OnAdFullScreenContentFailed += (error) =>
            {
                Debug.LogError("Interstitial failed: " + error.GetMessage());
                LoadInterstitial();
            };
        }

        public void LoadRewarded()
        {
            RewardedAd.Load(_rewardedAdUnitId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null) return;
                _rewardedAd = ad;
                RegisterRewardedEvents();
            });
        }

        public void ShowRewarded(Action onReward, Action onFailed = null)
        {
            Debug.Log($"[Ads] {_rewardedAd} {_rewardedAd.CanShowAd()}");
            if (_rewardedAd == null || !_rewardedAd.CanShowAd())
            {
                onFailed?.Invoke();
                return;
            }

            // выключаем ввод на время полноэкранной рекламы
            GameEvents.RaiseFullscreenAdActive(true);

            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                GameEvents.RaiseFullscreenAdActive(false);
                LoadRewarded();
            };
            _rewardedAd.OnAdFullScreenContentFailed += (error) =>
            {
                GameEvents.RaiseFullscreenAdActive(false);
                LoadRewarded();
            };

            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Reward granted: {reward.Type} x {reward.Amount}");
                onReward?.Invoke();
            });

            // Обработка ошибок показа
            _rewardedAd.OnAdFullScreenContentFailed += (error) =>
            {
                Debug.LogError($"Rewarded ad failed: {error.GetMessage()}");
                onFailed?.Invoke();
                //LoadRewarded();
            };

        }

        private void RegisterRewardedEvents()
        {
            _rewardedAd.OnAdFullScreenContentClosed += () => LoadRewarded();
            _rewardedAd.OnAdFullScreenContentFailed += (error) => LoadRewarded();
        }
    }
}

