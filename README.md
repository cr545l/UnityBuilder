# Unity Builder

>## 예제
>
>Windows와 MacOS에서 사용이 가능하도록 두가지 스크립트를 지원하며 사용 예제입니다.
>
>Bash Shell
>
>``` bash
>/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -buildTarget "ios" -projectPath >"/Users/LMac/.jenkins/workspace/p171120-ipa" -executeMethod "LofleEditor.Builder.InvokeBuildIOS" -buildNumber 0 >-appleDeveloperTeamID XXXXXXXXXX -createPlists
>```
>
>Batch
>
>``` bat
>"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -buildTarget "android" -projectPath "%CD%" >-executeMethod "LofleEditor.Builder.InvokeBuildAndroid" -keystorePass KEYSTORE_PASSWORD -keyaliasPass >KEYALIAS_PASSWORD -il2cpp
>```