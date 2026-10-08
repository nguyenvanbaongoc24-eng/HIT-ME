param([ValidateSet(0,1)][int]$Phase=0,[string]$Editor='D:\App\UNITY\6000.6.4f1\Editor\Unity.exe')
$ErrorActionPreference='Stop'
$workspacePath=Split-Path -Parent $PSScriptRoot
$projectPath=Join-Path $workspacePath 'unity-client'
if(!(Test-Path -LiteralPath $Editor)){throw 'Unity Editor missing.'}
if(!(Test-Path -LiteralPath (Join-Path (Split-Path -Parent $Editor) 'Data/PlaybackEngines/WebGLSupport'))){throw 'Web Build Support missing.'}
$lockPath=Join-Path $projectPath 'Temp/UnityLockfile'
if(Test-Path -LiteralPath $lockPath){try{$handle=[IO.File]::Open($lockPath,'Open','ReadWrite','None');$handle.Dispose()}catch{throw 'Project locked by Unity Editor.'}}
foreach($platform in @('EditMode','PlayMode')){
 & (Join-Path $PSScriptRoot 'unity.ps1') -Action $platform -Editor $Editor
 Copy-Item -LiteralPath (Join-Path $workspacePath "docs/unity-$platform-results.xml") -Destination (Join-Path $workspacePath "docs/Phase$Phase-$platform-results.xml")
}
$logPath=Join-Path $workspacePath "docs/phase$Phase-WebBuild.log"
$startedUtc=[DateTime]::UtcNow
$argsList=@('-batchmode','-nographics','-quit','-projectPath',('"'+$projectPath+'"'),'-buildTarget','WebGL','-executeMethod',"HitMe.Editor.HitMeWebBuild.BuildMasterPhase$Phase",'-logFile',('"'+$logPath+'"'))
$process=Start-Process -FilePath $Editor -ArgumentList $argsList -WindowStyle Hidden -PassThru
Write-Output "Phase$Phase build PID $($process.Id)"
$process.WaitForExit()
if($process.ExitCode -ne 0){throw "Build exit $($process.ExitCode)"}
$reportPath=Join-Path $workspacePath "docs/PHASE${Phase}_WEB_BUILD.json"
if(!(Test-Path -LiteralPath $reportPath) -or (Get-Item -LiteralPath $reportPath).LastWriteTimeUtc -lt $startedUtc){throw 'Fresh build report missing.'}
$report=Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
if($report.result -ne 'Succeeded' -or $report.errors -ne 0){throw 'Build failed.'}
Write-Output "Phase$Phase tests/build complete. $reportPath"
