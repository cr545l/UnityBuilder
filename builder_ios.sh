#!/bin/bash
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -buildTarget "ios" -projectPath "$WORKSPACE" -executeMethod "LofleEditor.Builder.InvokeBuildIOS"
