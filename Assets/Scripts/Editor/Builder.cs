using UnityEngine;
using UnityEditor;

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
				public static string Separator { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
				public static string Project { get { return System.IO.Path.GetFullPath( "." ) + Separator; } }
				public static string XcodeProjectPlist { get { return BuildTargetPathIOS + FileName.XcodeProjectPlist; } }
			}

			public class FileName
			{
				private const string _APK = "build.apk";
				private const string _PLIST = "Info.plist";

				public static string Apk { get { return Path.Separator + _APK; } }
				public static string XcodeProjectPlist { get { return Path.Separator + _PLIST; } }
			}

			public class Menu
			{
				private const string _BUILD = "/Build";
				private const string _PLIST = "/Plist";

				private const string _ALL = "/All";

				private const string _ADHOC = "/Ad Hoc";
				private const string _APPSTORE = "/App Store";
				private const string _ENTERPRISE = "/Enterprise";
				private const string _DEVELOPMENT = "/Development";

				private const string _IOS = "/iOS";
				private const string _ANDROID = "/Android";

				public const string _PLIST_ALL = _LOFLE + _PLIST + _ALL;

				public const string _PLIST_ADHOC = _LOFLE + _PLIST + _ADHOC;
				public const string _PLIST_APPSTORE = _LOFLE + _PLIST + _APPSTORE;
				public const string _PLIST_ENTERPRISE = _LOFLE + _PLIST + _ENTERPRISE;
				public const string _PLIST_DEVELOPMENT = _LOFLE + _PLIST + _DEVELOPMENT;

				public const string _BUILD_IOS = _LOFLE + _BUILD + _IOS;
				public const string _BUILD_ANDROID = _LOFLE + _BUILD + _ANDROID;
			}

			private const string _DIRECTORY_NAME_ANDROID = "apk";
			private const string _DIRECTORY_NAME_IOS = "ios";

			public class Plist
			{
				public const string APPSTORE = "app-store";
				public const string ENTERPRISE = "enterprise";
				public const string ADHOC = "ad-hoc";
				public const string DEVELOPMENT = "development";
			}

			public static string BuildTargetPathAndroid { get { return Path.Project + _DIRECTORY_NAME_ANDROID + FileName.Apk; } }
			public static string BuildTargetPathIOS { get { return Path.Project + _DIRECTORY_NAME_IOS; } }
		}

		private enum ePlistMethod
		{
			App_store,
			Enterprise,
			Ad_hoc,
			Development
		}

		private static readonly System.Collections.Generic.Dictionary<ePlistMethod, string> _plistMethodStrings = new System.Collections.Generic.Dictionary<ePlistMethod, string>()
				{
					{ ePlistMethod.App_store, Constant.Plist.APPSTORE },
					{ ePlistMethod.Enterprise, Constant.Plist.ENTERPRISE },
					{ ePlistMethod.Ad_hoc, Constant.Plist.ADHOC },
					{ ePlistMethod.Development, Constant.Plist.DEVELOPMENT },
				};

		private static string[] FindEnabledEditorScenes()
		{
			var editorScenes = new System.Collections.Generic.List<string>();
			foreach( EditorBuildSettingsScene scene in EditorBuildSettings.scenes )
			{
				if( !scene.enabled )
					continue;
				editorScenes.Add( scene.path );
			}

			return editorScenes.ToArray();
		}

		private static void InvokeBuild( string[] scenes, string targetPath, BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, BuildOptions build_options )
		{
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
			if( null != buildReport )
			{
				throw new System.Exception( buildReport );
			}
#endif
		}

		private static void CheckCommandLine( BuildTargetGroup buildTargetGroup )
		{
			var args = System.Environment.GetCommandLineArgs();
			if( null != args )
			{
				for( int i = 0; i < args.Length; i++ )
				{
					string arg = args[i];
					if( i + 1 < args.Length )
					{
						string argValue = args[i + 1];
						switch( arg )
						{
							case "-bundleIdentifier":
								{
									PlayerSettings.applicationIdentifier = argValue;
									PlayerSettings.SetApplicationIdentifier( buildTargetGroup, argValue );
								}
								break;

							case "-buildNumber":
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

							case "-appleDeveloperTeamID":
								{
									PlayerSettings.iOS.appleEnableAutomaticSigning = true;
									PlayerSettings.iOS.appleDeveloperTeamID = argValue;
								}
								break;

							case "-iOSManualProvisioningProfileID":
								{
									PlayerSettings.iOS.iOSManualProvisioningProfileID = argValue;
								}
								break;

							case "-keystorePass":
								{
									PlayerSettings.Android.keystorePass = argValue;
								}
								break;

							case "-keyaliasPass":
								{
									PlayerSettings.Android.keyaliasPass = argValue;
								}
								break;
						}
					}
					else
					{
						switch( arg )
						{
							case "-createPlists":
								{
									CreatePlists();
								}
								break;

							case "-mono":
								{
									PlayerSettings.SetScriptingBackend( buildTargetGroup, ScriptingImplementation.Mono2x );
								}
								break;

							case "-il2cpp":
								{
									PlayerSettings.SetScriptingBackend( buildTargetGroup, ScriptingImplementation.IL2CPP );
								}
								break;
						}
					}
				}
			}
		}

		private static void CreateXcodeProjectPlist()
		{
			var plist = new UnityEditor.iOS.Xcode.PlistDocument();

			if( System.IO.File.Exists( Constant.Path.XcodeProjectPlist ) )
			{
				plist.ReadFromFile( Constant.Path.XcodeProjectPlist );
			}
			else
			{
				plist.WriteToFile( Constant.Path.XcodeProjectPlist );
			}

			// '수출 규정 관련 문서가 누락됨' 방지 코드
			plist.root.SetBoolean( "ITSAppUsesNonExemptEncryption", false );

			plist.WriteToFile( Constant.Path.XcodeProjectPlist );
		}

		private static void CreateExportPlist( ePlistMethod type )
		{
			CreateExportPlist( _plistMethodStrings[type] );
		}

		private static void CreateExportPlist( string method )
		{
			string path = string.Format( "{0}{1}.plist", Constant.Path.Project, method );
			var plist = new UnityEditor.iOS.Xcode.PlistDocument();

			if( System.IO.File.Exists( path ) )
			{
				System.IO.File.Delete( path );
			}

			plist.root.SetString( "method", method );
			plist.root.SetString( "teamID", PlayerSettings.iOS.appleDeveloperTeamID );
			plist.root.SetBoolean( "compileBitcode", false );

			plist.WriteToFile( path );
		}

		[MenuItem( Constant.Menu._PLIST_ALL )]
		private static void CreatePlists()
		{
			foreach( ePlistMethod plistMethod in System.Enum.GetValues( typeof( ePlistMethod ) ) )
			{
				CreateExportPlist( plistMethod );
			}
		}

		[MenuItem( Constant.Menu._PLIST_ADHOC )]
		private static void CreateAdHocPlist()
		{
			CreateExportPlist( ePlistMethod.Ad_hoc );
		}

		[MenuItem( Constant.Menu._PLIST_APPSTORE )]
		private static void CreateAppStorePlist()
		{
			CreateExportPlist( ePlistMethod.App_store );
		}

		[MenuItem( Constant.Menu._PLIST_ENTERPRISE )]
		private static void CreateEnterprisePlist()
		{
			CreateExportPlist( ePlistMethod.Enterprise );
		}

		[MenuItem( Constant.Menu._PLIST_ENTERPRISE )]
		private static void CreateDevelopmentPlist()
		{
			CreateExportPlist( ePlistMethod.Development );
		}

		/// <summary>
		/// iOS 빌드용 기능
		/// </summary>
		[MenuItem( Constant.Menu._BUILD_IOS )]
		private static void InvokeBuildIOS()
		{
			CheckCommandLine( BuildTargetGroup.iOS );

			BuildOptions option = BuildOptions.None;

			PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

			// https://docs.unity3d.com/550/Documentation/ScriptReference/PlayerSettings.iOS-targetOSVersionString.html
			// 5.5b 부터 targetOSVersionString으로 변경됨. 비워두면 지원하는 최소 OS 버전으로 빌드
			// PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
			PlayerSettings.statusBarHidden = true;

			System.IO.Directory.CreateDirectory( Constant.BuildTargetPathIOS );

			InvokeBuild( FindEnabledEditorScenes(), Constant.BuildTargetPathIOS, BuildTargetGroup.iOS, BuildTarget.iOS, option );

			CreateXcodeProjectPlist();
		}

		/// <summary>
		/// Android 빌드용 기능
		/// </summary>
		[MenuItem( Constant.Menu._BUILD_ANDROID )]
		private static void InvokeBuildAndroid()
		{
			CheckCommandLine( BuildTargetGroup.Android );

			BuildOptions option = BuildOptions.None;

			InvokeBuild( FindEnabledEditorScenes(), Constant.BuildTargetPathAndroid, BuildTargetGroup.Android, BuildTarget.Android, option );
		}
	}
}