1. If you install from PrimeSDK Toolkit, click Install once. The Toolkit installs External Dependency Manager, Yandex Mobile Ads plugin for Unity, and this API package.

2. If you install manually, open UPM settings by selecting Edit > Project Settings > Package Manager in the Unity menu.

3. Add new scoped registry OpenUPM with URL `https://package.openupm.com` and scopes `com.yandex`, `com.google`.

4. Open UPM by selecting Window -> Package Manager in the Unity menu.

5. Go to My Registries and install `External Dependency Manager for Unity`.

6. Go to My Registries and install `Yandex Mobile Ads plugin for Unity`.

7. Don't forget to do `Force Resolve` and make sure it succeeds before building app!

8. To use mediation, you should also install the `Yandex Mobile Ads Mediation for Unity` package alongside with base package above.

9. With mediation, you should also install mediation modules, here are recommended ones:

```text
AppLovin
BigoAds
ChartBoost
InMobi
IronSource
MyTarget
Pangle
UnityAds
```

10. Add this API package for PrimeSDK after you finished with Yandex Mobile Ads, click Install above and wait for compiler to finish its work.

11. In Toolkit, go to Configurations, select your configuration (e.g. Google Play), go to Ads foldout, click `Use custom`, and select Yandex Mobile Ads as provider for Ads.

12. Enter Ad Unit Ids for each type of ads.

13. To test ads in build, you can also use the following Ad Unit Ids (do not use them in production as those are demo ids): https://ads.yandex.com/helpcenter/ru/dev/unity/demo-blocks

If you publish on RuStore, make sure to mention your email that is used to create Yandex Mobile Ads account in the description of an app. For example, you can write something like: "Support: myemail@example.com".

You can't create Ad Unit Ids for RuStore if you don't have game already published on Google Play. But if you want to, you can contact support for further assistance - this might work.
