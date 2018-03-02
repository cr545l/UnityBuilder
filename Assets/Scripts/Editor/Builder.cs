using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#region 빌드옵션 예시 및 설명
//BuildOptions opt =	BuildOptions.SymlinkLibraries |			//
//						BuildOptions.AutoRunPlayer |			//
//						BuildOptions.ShowBuiltPlayer |			//
//						BuildOptions.Development |				//
//						BuildOptions.ConnectWithProfiler |		//
//						BuildOptions.AllowDebugging |			//
//						BuildOptions.Development;				//
//
//None									// 옵션 없이 빌드합니다.
//Development							// 개발 버전으로 빌드합니다.
//AutoRunPlayer							// 빌드 후, 어플리케이션을 시작합니다.
//ShowBuiltPlayer						// 빌드 후, 어플리케이션 파일을 표시합니다.
//BuildAdditionalStreamedScenes			// WWW 클래스를 사용하여 씬을 로드하기 위한 압축된 에셋 번들을 만듭니다.
//AcceptExternalModificationsToPlayer	// XCode (iPhone) 또는 Eclipse (Android)용 프로젝트를 생성할 때 사용됩니다.
//WebPlayerOfflineDeployment			// WebPlayer 안에 UnityObject.js를 복사합니다. 이로써 unityObject.js파일은 다운로드가 발생하지 않습니다.
//ConnectWithProfiler					// Start the player with a connection to the profiler in the editor.
//AllowDebugging						// 스크립트 디버거를 사용하여 원격으로 플레이어에 연결합니다.
//SymlinkLibraries						// 심볼릭 링크의 런타임 라이브러리를 iOS의 Xcode 프로젝트 생성시 포함합니다.
//UncompressedAssetBundle				// 에셋 번들을 생성할 때 압축 처리를 하지 않도록 합니다.
//EnableHeadlessMode					// Build headless Linux standalone.
//BuildScriptsOnly						// Build only the scripts of a project.
//ForceEnableAssertions					// Include assertions in the build. By default, the assertions are only included in development builds.
//ForceOptimizeScriptCompilation		// Force full optimizations for script complilation in Development builds.
#endregion

namespace LofleEditor
{
	/// <summary>
	/// 빌드 자동화 기본 클래스
	/// </summary>
	public class Builder
	{
		public class Constant
		{
			public const string _LOFLE = "Lofle";

			public class Path
			{
				/// <summary>
				/// 경로기호
				/// </summary>
				public static string Separator => System.IO.Path.DirectorySeparatorChar.ToString();
				public static string Project => System.IO.Path.GetFullPath( "." ) + Separator;
				public static string Plist => Build.BuildTargetPathIOS + FileName.Plist;
			}

			public class FileName
			{
				public static string Apk => Path.Separator + "build.apk";
				public static string Plist => Path.Separator + "Info.plist";
			}

			public class Menu
			{
				public const string _BUILD = "/Build";
				public const string _IOS = "/iOS";
				public const string _ANDROID = "/Android";
				public const string _BUILD_IOS = _LOFLE + _BUILD + _IOS;
				public const string _BUILD_ANDROID = _LOFLE + _BUILD + _ANDROID;
			}

			public class Build
			{
				public const string _DIRECTORY_NAME_ANDROID = "apk";
				public const string _DIRECTORY_NAME_IOS = "ios";

				public static string BuildTargetPathAndroid => Path.Project + _DIRECTORY_NAME_ANDROID + FileName.Apk;
				public static string BuildTargetPathIOS => Path.Project + _DIRECTORY_NAME_IOS;
			}
		}

		private static string[] FindEnabledEditorScenes()
		{
			List<string> EditorScenes = new List<string>();
			foreach( EditorBuildSettingsScene scene in EditorBuildSettings.scenes )
			{
				if( !scene.enabled )
					continue;
				EditorScenes.Add( scene.path );
			}

			return EditorScenes.ToArray();
		}

		private static void InvokeBuild( string[] scenes, string targetPath, BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, BuildOptions build_options )
		{
			CheckCommandLine();

			EditorUserBuildSettings.SwitchActiveBuildTarget( buildTargetGroup, buildTarget );
			var buildReport = BuildPipeline.BuildPlayer( scenes, targetPath, buildTarget, build_options );
			Debug.Log( buildReport );
			//Debug.Log( $"Result : {buildReport.summary.result}" );
			//
			//StringBuilder log = new StringBuilder();
			//
			//log.AppendLine( "BuildResult.files" );
			//foreach( var i in buildReport.files )
			//{
			//	log.AppendLine( $"	{i.ToString()}" );
			//}
			//log.AppendLine();
			//	
			//log.AppendLine( "BuildResult.steps" );
			//foreach( var i in buildReport.steps )
			//{
			//	log.AppendLine( $"	{i.ToString()}" );
			//}
			//log.AppendLine();
			//
			//log.AppendLine( $"Guid : {buildReport.summary.guid.ToString()}" );
			//log.AppendLine( $"OutputPath : {buildReport.summary.outputPath}" );
			//log.AppendLine( $"TotalSize : {buildReport.summary.totalSize.ToString()}" );
			//log.AppendLine( $"TotalTime : {buildReport.summary.totalTime.ToString()}" );
			//log.AppendLine( $"BuildStartedAt : {buildReport.summary.buildStartedAt.ToLongDateString()}" );
			//log.AppendLine( $"BuildEndedAt : {buildReport.summary.buildEndedAt.ToLongDateString()}" );
			//log.AppendLine( $"TotalErrors : {buildReport.summary.totalErrors.ToString()}" );
			//log.AppendLine( $"TotalWarnings : {buildReport.summary.totalWarnings.ToString()}" );
			//
			//Debug.Log( log );
		}

		private static void CheckCommandLine()
		{
			var args = Environment.GetCommandLineArgs();
			if( null != args )
			{
				for( int i = 0; i < args.Length; i++ )
				{
					string arg = args[i];
					if( i + 1 < args.Length )
					{
						string argValue = args[i + 1];
						switch( arg.ToLower() )
						{
							case "-buildnumber":
								{
									int buildNumber = 0;
									if( int.TryParse( argValue, out buildNumber ) )
									{
										string bundleVersion = PlayerSettings.bundleVersion;

										PlayerSettings.Android.bundleVersionCode = buildNumber;
										PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
										PlayerSettings.bundleVersion = bundleVersion + "." + buildNumber;
									}
								}
								break;

							case "-appledeveloperteamid":
								{
									PlayerSettings.iOS.appleDeveloperTeamID = argValue;
								}
								break;

							case "-iOSManualProvisioningProfileID":
								{
									PlayerSettings.iOS.iOSManualProvisioningProfileID = argValue;
								}
								break;

							case "-keystorepass":
								{
									PlayerSettings.Android.keystorePass = argValue;
								}
								break;

							case "-keyaliaspass":
								{
									PlayerSettings.Android.keyaliasPass = argValue;
								}
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// iOS 빌드용 기능
		/// </summary>
		[MenuItem( Constant.Menu._BUILD_IOS )]
		private static void InvokeBuildIOS()
		{
			// https://developer.apple.com/account/#/membership/r

			// 5.4.3부터 애플 teamID를 설정하여야 함, 안하면 xcode 프로젝트 파일에 teamID가 세팅되어 있지 않음
			BuildOptions option = BuildOptions.None;

			PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

			// https://docs.unity3d.com/550/Documentation/ScriptReference/PlayerSettings.iOS-targetOSVersionString.html
			// 5.5b 부터 targetOSVersionString으로 변경됨. 비워두면 지원하는 최소 OS 버전으로 빌드
			// PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
			PlayerSettings.statusBarHidden = true;

			Directory.CreateDirectory( Constant.Build.BuildTargetPathIOS );

			InvokeBuild( FindEnabledEditorScenes(), Constant.Build.BuildTargetPathIOS, BuildTargetGroup.iOS, BuildTarget.iOS, option );
#if UNITY_IOS
			UnityEditor.iOS.Xcode.PlistDocument plist = new UnityEditor.iOS.Xcode.PlistDocument();

			if( File.Exists( Constant.Path.Plist ) )
			{
				plist.ReadFromFile( Constant.Path.Plist );
			}
			else
			{
				plist.WriteToFile( Constant.Path.Plist );
			}

			// '수출 규정 관련 문서가 누락됨' 방지 코드
			plist.root.SetBoolean( "ITSAppUsesNonExemptEncryption", false );

			plist.WriteToFile( Constant.Path.Plist );
#endif
		}

		/// <summary>
		/// Android 빌드용 기능
		/// </summary>
		[MenuItem( Constant.Menu._BUILD_ANDROID )]
		private static void InvokeBuildAndroid()
		{
			BuildOptions option = BuildOptions.None;

			InvokeBuild( FindEnabledEditorScenes(), Constant.Build.BuildTargetPathAndroid, BuildTargetGroup.Android, BuildTarget.Android, option );
		}
	}
}