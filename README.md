# Unity Builder

Eco팀 내부 Mac Pro에서 빌드 자동화를 위해 Jenkins에서 사용하는 소스코드 입니다.

Jenkins 또는 해당 소스코드의 문제점, 또는 개선사항이 있다면 언제든지 Issues에 요청 또는 소스코드를 직접 작성하여 Merge Requests를 요청 해주시기 바랍니다.

|Project|Status|
|--|--|
|BannedWord-apk|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=BannedWord-apk)](http://10.13.0.124:8080/job/BannedWord-apk/)
|BannedWord-ipa|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=BannedWord-ipa)](http://10.13.0.124:8080/job/BannedWord-ipa/)
|DeltaHandlerForUnity-apk|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=DeltaHandlerForUnity-apk)](http://10.13.0.124:8080/job/DeltaHandlerForUnity-apk/)
|DeltaHandlerForUnity-ipa|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=DeltaHandlerForUnity-ipa)](http://10.13.0.124:8080/job/DeltaHandlerForUnity-ipa/)
|HomerunBattle3-apk|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=HomerunBattle3-apk)](http://10.13.0.124:8080/job/HomerunBattle3-apk/)
|HomerunBattle3-ipa|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=HomerunBattle3-ipa)](http://10.13.0.124:8080/job/HomerunBattle3-ipa/)
|RuntimeConsole-ipa|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=RuntimeConsole-ipa)](http://10.13.0.124:8080/job/RuntimeConsole-ipa/)
|UnityAssetAssistDelta-apk|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=UnityAssetAssistDelta-apk)](http://10.13.0.124:8080/job/UnityAssetAssistDelta-apk/)
|UnityAssetAssistDelta-apk|[![Build Status](http://10.13.0.124:8080/buildStatus/icon?job=UnityAssetAssistDelta-apk)](http://10.13.0.124:8080/job/UnityAssetAssistDelta-apk/)

< [Com2us Eco TS - Jenkins](http://10.13.0.124:8080/)에서 관리되고 있는 프로젝트 리스트 >

>## 예제
>
>Windows와 MacOS에서 사용이 가능하도록 두가지 스크립트를 지원하며 사용 예제입니다.
>
>Bash Shell
>
>``` bash
>/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -buildTarget "ios" -projectPath "/Users/LMac/.jenkins/workspace/p171120-ipa" -executeMethod "LofleEditor.Builder.InvokeBuildIOS" -buildNumber 0 -appleDeveloperTeamID XXXXXXXXXX -createPlists
>```
>
>Batch
>
>``` bat
>"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -buildTarget "android" -projectPath "%CD%" -executeMethod "LofleEditor.Builder.InvokeBuildAndroid" -keystorePass KEYSTORE_PASSWORD -keyaliasPass KEYALIAS_PASSWORD -il2cpp
>```

---

## 커맨드 옵션

### -bundleIdentifier `*VALUE*`

프로젝트에 설정된 Bundle ID를 변경하여 빌드합니다.

### -buildNumber `*VALUE*`

iOS의 Build Number와 안드로이드의 Bundle Version Code를 설정합니다.

### -appleDeveloperTeamID `*VALUE*`

iOS 빌드 시 개발자 계정을 수동으로 설정합니다.

### -iOSManualProvisioningProfileID `*VALUE*`

iOS 빌드 시 수동으로 서명 처리를 하는 경우 사용 할 프로비저닝 프로파일의 ID를 수동으로 설정합니다.

### -keystorePass `*VALUE*`

안드로이드 빌드 시 Keystore의 패스워드를 설정합니다.

### -keyaliasPass `*VALUE*`

안드로이드 빌드 시 Alias의 패스워드를 설정합니다.

### -AndroidSdkRoot `*VALUE*`

빌드 시 안드로이드 SDK 대상 경로를 설정합니다.

### -AndroidNdkRoot `*VALUE*`

빌드 시 안드로이드 NDK 대상 경로를 설정합니다.

### -JdkPath `*VALUE*`

빌드 시 안드로이드 JDK 대상 경로를 설정합니다.

---

### -createPlists

iOS 아카이빙 시 필요로 하는 plist를 자동으로 생성합니다.

### -mono

해당 프로젝트에서 Mono 2.6 런타임을 사용하도록 빌드합니다.

### -il2cpp

해당 프로젝트에서 .NET 런타임을 사용하도록 빌드합니다.