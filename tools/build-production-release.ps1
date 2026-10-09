param([string]$Editor='D:\App\UNITY\6000.6.4f1\Editor\Unity.exe')
$ErrorActionPreference='Stop'
$workspacePath=Split-Path -Parent $PSScriptRoot
$projectPath=Join-Path $workspacePath 'unity-client'
if(!(Test-Path -LiteralPath $Editor)){throw 'Unity Editor missing.'}
if(!(Test-Path -LiteralPath (Join-Path (Split-Path -Parent $Editor) 'Data/PlaybackEngines/WebGLSupport'))){throw 'Web Build Support missing.'}
$active=Get-CimInstance Win32_Process | Where-Object {$_.Name -eq 'Unity.exe' -and $_.CommandLine -and $_.CommandLine.Contains($projectPath)}
if($active){throw 'Close the Unity Editor before running this batch build.'}
$startedUtc=[DateTime]::UtcNow
$logPath=Join-Path $workspacePath 'docs/production-release-WebBuild.log'
$arguments=@('-batchmode','-nographics','-quit','-projectPath',('"'+$projectPath+'"'),'-buildTarget','WebGL','-executeMethod','HitMe.Editor.HitMeWebBuild.BuildProductionRelease','-logFile',('"'+$logPath+'"'))
$process=Start-Process -FilePath $Editor -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Output "Unity PID $($process.Id). Log: $logPath"
$process.WaitForExit()
if($process.ExitCode -ne 0){throw "Unity build failed: $($process.ExitCode)."}
$reportPath=Join-Path $workspacePath 'docs/PRODUCTION_RELEASE_WEB_BUILD.json'
if(!(Test-Path -LiteralPath $reportPath) -or (Get-Item -LiteralPath $reportPath).LastWriteTimeUtc -lt $startedUtc){throw 'Fresh build report missing.'}
$summary=Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
if($summary.result -ne 'Succeeded' -or $summary.errors -ne 0){throw 'Unity reported a failed build.'}
$outputPath=Join-Path $projectPath 'Builds/Web'
if(!(Test-Path -LiteralPath (Join-Path $outputPath 'index.html'))){throw 'Build index missing.'}
foreach($pattern in @('*.wasm*','*.data*','*.framework.js*','*.loader.js')){if(!(Get-ChildItem -LiteralPath (Join-Path $outputPath 'Build') -Filter $pattern | Where-Object Length -gt 0)){throw "Missing output: $pattern"}}
Write-Output "Web build succeeded: $outputPath"
