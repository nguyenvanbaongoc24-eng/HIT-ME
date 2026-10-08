$workspacePath = Split-Path -Parent $PSScriptRoot
$editorPath = 'D:\App\UNITY\6000.6.4f1\Editor\Unity.exe'
$projectPath = Join-Path $workspacePath 'unity-client'
$logPath = Join-Path $workspacePath 'docs/unity-CharacterAssets.log'
$arguments = @('-batchmode','-nographics','-quit','-projectPath',('"'+$projectPath+'"'),'-logFile',('"'+$logPath+'"'),'-executeMethod','HitMe.Editor.CharacterAssetPipeline.BuildLibrary')
$process = Start-Process -FilePath $editorPath -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Output "Unity character asset import PID $($process.Id)"
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Character pipeline failed: $logPath" }
Write-Output 'Character asset pipeline completed.'
