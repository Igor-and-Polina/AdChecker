using Unity.Services.LevelPlay;
using UnityEngine;

public class AdManager : MonoBehaviour {
  LevelPlayBannerAd _bannerAd;
  LevelPlayInterstitialAd _interstitialAd;

  const string APP_KEY = "21f0d1e95";
  const string BANNER_AD_UNIT_ID = "hsdhoqsp2r3xo9ro";
  const string INTERSTITIAL_AD_UNIT_ID = "tkwhlbr853tx5rpf";

  static void Log(string log) {
    // TODO: make custom log `Debug` function to avoid manually `unity-script:`
    Debug.Log("unity-script: " + log);
  }

  public void Start() {
    if (Debug.isDebugBuild) {
      IronSource.Agent.setMetaData("is_test_suite", "enable");
    }

    Log("IronSource.Agent.validateIntegration");
    IronSource.Agent.validateIntegration();

    Log("unity version: " + IronSource.unityVersion());

    Log("LevelPlay SDK initialization");
    LevelPlay.Init(APP_KEY, adFormats: new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });

    LevelPlay.OnInitSuccess += config => {
      Log("I got SdkInitializationCompletedEvent with config: " + config);
      EnableAds();
    };
    LevelPlay.OnInitFailed += error => {
      Log("I got SdkInitializationFailedEvent with error: " + error);
      //
    };
  }

  void EnableAds() {
    IronSourceRewardedVideoEvents.onAdAvailableEvent += _ => Log("Rewarded video is enabled");

    _bannerAd = new LevelPlayBannerAd(BANNER_AD_UNIT_ID);
    _interstitialAd = new LevelPlayInterstitialAd(INTERSTITIAL_AD_UNIT_ID);

    _bannerAd.OnAdLoaded += _ => { _bannerAd.ShowAd(); };
    _interstitialAd.OnAdLoaded += _ => { _interstitialAd.ShowAd(); };
  }

  void OnApplicationPause(bool isPaused) {
    IronSource.Agent.onApplicationPause(isPaused);
  }

  public void ClickRewarded() {
    Log("Rewarded");
    if (IronSource.Agent.isRewardedVideoAvailable()) {
      IronSource.Agent.showRewardedVideo();
    }
  }

  public void ClickInterstitial() {
    if (_interstitialAd is null) {
      return;
    }

    Log("Interstitial");
    if (_interstitialAd.IsAdReady()) {
      _interstitialAd.ShowAd();
    } else {
      _interstitialAd.LoadAd();
    }
  }

  public void ClickBanner() {
    if (_bannerAd is null) {
      return;
    }

    Log("Banner");
    _bannerAd.LoadAd();
  }
}