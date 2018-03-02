using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif

#region 빌드옵션 예시 및 설명
//BuildOptions.None									// 옵션 없이 빌드합니다.
//BuildOptions.Development							// 개발 버전으로 빌드합니다.
//BuildOptions.AutoRunPlayer						// 빌드 후, 어플리케이션을 시작합니다.
//BuildOptions.ShowBuiltPlayer						// 빌드 후, 어플리케이션 파일을 표시합니다.
//BuildOptions.BuildAdditionalStreamedScenes		// WWW 클래스를 사용하여 씬을 로드하기 위한 압축된 에셋 번들을 만듭니다.
//BuildOptions.AcceptExternalModificationsToPlayer	// XCode (iPhone) 또는 Eclipse (Android)용 프로젝트를 생성할 때 사용됩니다.
//BuildOptions.WebPlayerOfflineDeployment			// WebPlayer 안에 UnityObject.js를 복사합니다. 이로써 unityObject.js파일은 다운로드가 발생하지 않습니다.
//BuildOptions.ConnectWithProfiler					// Start the player with a connection to the profiler in the editor.
//BuildOptions.AllowDebugging						// 스크립트 디버거를 사용하여 원격으로 플레이어에 연결합니다.
//BuildOptions.SymlinkLibraries						// 심볼릭 링크의 런타임 라이브러리를 iOS의 Xcode 프로젝트 생성시 포함합니다.
//BuildOptions.UncompressedAssetBundle				// 에셋 번들을 생성할 때 압축 처리를 하지 않도록 합니다.
//BuildOptions.EnableHeadlessMode					// Build headless Linux standalone.
//BuildOptions.BuildScriptsOnly						// Build only the scripts of a project.
//BuildOptions.ForceEnableAssertions				// Include assertions in the build. By default, the assertions are only included in development builds.
//BuildOptions.ForceOptimizeScriptCompilation		// Force full optimizations for script complilation in Development builds.
#endregion

namespace Com2usEditor
{
	/// <summary>
	/// 빌드 자동화 기본 클래스
	/// </summary>
	public class Builder
	{
		public class Constant
		{
			public const string COM2US = "COM2US";

			public class Path
			{
				/// <summary>
				/// 경로기호
				/// </summary>
				public static string Separator { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
				public static string Project { get { return System.IO.Path.GetFullPath( "." ) + Separator; } }
				public static string Plist { get { return Build.BuildTargetPathIOS + FileName.Plist; } }
			}

			public class FileName
			{
				private const string _APK = "build.apk";
				private const string _PLIST = "Info.plist";

				public static string Apk { get { return Path.Separator + _APK; } }
				public static string Plist { get { return Path.Separator + _PLIST; } }
			}

			public class Menu
			{
				private const string _BUILD = "/Build";
				private const string _IOS = "/iOS";
				private const string _ANDROID = "/Android";

				public const string BUILD_IOS = COM2US + _BUILD + _IOS;
				public const string BUILD_ANDROID = COM2US + _BUILD + _ANDROID;
			}

			public class Build
			{
				private const string _DIRECTORY_NAME_ANDROID = "apk";
				private const string _DIRECTORY_NAME_IOS = "ios";

				public static string BuildTargetPathAndroid { get { return Path.Project + _DIRECTORY_NAME_ANDROID + FileName.Apk; } }
				public static string BuildTargetPathIOS { get { return Path.Project + _DIRECTORY_NAME_IOS; } }

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

					// 2018부터 BuildReport로 리턴하도록 변경 됨
					var buildReport = BuildPipeline.BuildPlayer( scenes, targetPath, buildTarget, build_options );
#if UNITY_2018
					Debug.LogFormat( "Result : {0}", buildReport.summary.result );

					StringBuilder log = new StringBuilder();

					log.AppendLine( "BuildResult.files" );
					foreach( var i in buildReport.files )
					{
						log.AppendFormat( "	{0}\n", i.ToString() );
					}
					log.AppendLine();

					log.AppendLine( "BuildResult.steps" );
					foreach( var i in buildReport.steps )
					{
						log.AppendFormat( "	{0}\n", i.ToString() );
					}
					log.AppendLine();

					log.AppendFormat( "Guid : {0}\n", buildReport.summary.guid.ToString() );
					log.AppendFormat( "OutputPath : {0}\n", buildReport.summary.outputPath );
					log.AppendFormat( "TotalSize : {0}\n", buildReport.summary.totalSize.ToString() );
					log.AppendFormat( "TotalTime : {0}\n", buildReport.summary.totalTime.ToString() );
					log.AppendFormat( "BuildStartedAt : {0}\n", buildReport.summary.buildStartedAt.ToLongDateString() );
					log.AppendFormat( "BuildEndedAt : {0}\n", buildReport.summary.buildEndedAt.ToLongDateString() );
					log.AppendFormat( "TotalErrors : {0}\n", buildReport.summary.totalErrors.ToString() );
					log.AppendFormat( "TotalWarnings : {0}\n", buildReport.summary.totalWarnings.ToString() );
					Debug.Log( log );
#else
					Debug.LogFormat( "Result : {0}", buildReport );
#endif
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
												PlayerSettings.bundleVersion = string.Format( "{0}.{1}", bundleVersion, buildNumber );
											}
										}
										break;

									case "-appledeveloperteamid":
										{
											// https://developer.apple.com/account/#/membership/r

											// 5.4.3부터 애플 teamID를 설정하여야 함, 안하면 xcode 프로젝트 파일에 teamID가 세팅되어 있지 않음
											PlayerSettings.iOS.appleDeveloperTeamID = argValue;
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
				[MenuItem( Constant.Menu.BUILD_IOS )]
				private static void InvokeBuildIOS()
				{
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
				[MenuItem( Constant.Menu.BUILD_ANDROID )]
				private static void InvokeBuildAndroid()
				{
					BuildOptions option = BuildOptions.None;

					InvokeBuild( FindEnabledEditorScenes(), Constant.Build.BuildTargetPathAndroid, BuildTargetGroup.Android, BuildTarget.Android, option );
				}
			}
		}
	}
}