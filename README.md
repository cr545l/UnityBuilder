# Unity Builder

Bash Shell

``` bash
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -buildTarget "ios" -projectPath "/Users/LMac/.jenkins/workspace/p171120-ipa" -executeMethod "LofleEditor.Builder.InvokeBuildIOS" -buildNumber 0 -appleDeveloperTeamID XXXXXXXXXX -createPlists
```

Batch

``` bat
"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -buildTarget "android" -projectPath "%CD%" -executeMethod "LofleEditor.Builder.InvokeBuildAndroid" -keystorePass KEYSTORE_PASSWORD -keyaliasPass KEYALIAS_PASSWORD
```