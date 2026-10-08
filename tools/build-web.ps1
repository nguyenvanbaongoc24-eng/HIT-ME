param([string]$Editor='D:\App\UNITY\6000.6.4f1\Editor\Unity.exe', [ValidateSet(4,5)][int]$Sprint=5)
$ErrorActionPreference='Stop'
$workspacePath=Split-Path -Parent $PSScriptRoot
$projectPath=[IO.Path]::GetFullPath((Join-Path $workspacePath 'unity-client'))
if(!(Test-Path -LiteralPath $Editor)){throw "Unity Editor missing: $Editor"}
if(!(Test-Path -LiteralPath (Join-Path $projectPath 'ProjectSettings/ProjectVersion.txt'))){throw "Unity Project missing: $projectPath"}
$modulePath=Join-Path (Split-Path -Parent $Editor) 'Data/PlaybackEngines/WebGLSupport'
if(!(Test-Path -LiteralPath $modulePath)){throw 'Web Build Support missing.'}
$lockPath=Join-Path $projectPath 'Temp/UnityLockfile'
if(Test-Path -LiteralPath $lockPath){try{$handle=[IO.File]::Open($lockPath,'Open','ReadWrite','None');$handle.Dispose()}catch{throw 'Unity project is locked. Close its Editor before batch build.'}}
$activeEditor=Get-CimInstance Win32_Process | Where-Object {$_.Name -eq 'Unity.exe' -and $_.CommandLine -like '*-projectPath*' -and $_.CommandLine.Contains($projectPath)}
if($activeEditor){throw 'Unity Editor is already using this project.'}
$logPath=Join-Path $workspacePath ('docs/sprint'+$Sprint+'-WebBuild.log')
$startedUtc=[DateTime]::UtcNow
$argsList=@('-batchmode','-nographics','-quit','-projectPath',('"'+$projectPath+'"'),'-buildTarget','WebGL','-executeMethod',('HitMe.Editor.HitMeWebBuild.BuildSprint'+$Sprint),'-logFile',('"'+$logPath+'"'))
$process=Start-Process -FilePath $Editor -ArgumentList $argsList -WindowStyle Hidden -PassThru
Write-Output "Unity PID $($process.Id), log $logPath"
$process.WaitForExit()
if($process.ExitCode -ne 0){throw "Unity build failed with exit $($process.ExitCode). Read $logPath"}
$outputPath=Join-Path $projectPath 'Builds/Web'
$indexPath=Join-Path $outputPath 'index.html'
# Unity incremental builds preserve timestamps of unchanged template files.
# Require a newly written successful BuildPipeline report, then validate output files.
$reportPath=Join-Path $workspacePath ('docs/SPRINT'+$Sprint+'_WEB_BUILD.json')
if(!(Test-Path -LiteralPath $reportPath) -or (Get-Item -LiteralPath $reportPath).LastWriteTimeUtc -lt $startedUtc){throw 'Build did not produce a fresh report.'}
$buildSummary=Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
if($buildSummary.result -ne 'Succeeded' -or $buildSummary.errors -ne 0 -or $buildSummary.bytes -le 0){throw 'Unity reported build failure.'}
if(!(Test-Path -LiteralPath $indexPath)){throw 'Build index.html missing.'}
foreach($pattern in @('*.wasm*','*.data*','*.loader.js')){if(!(Get-ChildItem -LiteralPath (Join-Path $outputPath 'Build') -Filter $pattern | Where-Object Length -gt 0)){throw "Missing build output: $pattern"}}
Write-Output "Web build succeeded: $outputPath"
