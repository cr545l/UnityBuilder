SET UNITY_PATH="C:\Program Files\Unity 2018.1.0b2\Editor\"
cd %UNITY_PATH%
Unity.exe -quit -batchmode -buildTarget "android" -projectPath "%CD%" -executeMethod "LofleEditor.Builder.InvokeBuildAndroid"