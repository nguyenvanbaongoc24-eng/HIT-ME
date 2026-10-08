param(
    [ValidateSet('Configure','EditMode','PlayMode','BuildWeb','Open')][string]$Action = 'Configure',
    [string]$Editor = 'D:\App\UNITY\6000.6.4f1\Editor\Unity.exe'
)
$workspacePath = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $workspacePath 'unity-client'
if (!(Test-Path -LiteralPath $Editor)) { throw "Unity Editor missing: $Editor" }
$arguments = @('-projectPath', ('"' + $projectPath + '"'))
if ($Action -eq 'Open') {
    Start-Process -FilePath $Editor -ArgumentList $arguments -WindowStyle Hidden
    exit
}
$logPath = Join-Path $workspacePath ('docs/unity-' + $Action + '.log')
$arguments += @('-batchmode', '-logFile', ('"' + $logPath + '"'))
switch ($Action) {
    'Configure' { $arguments += @('-nographics','-quit','-executeMethod','HitMe.Editor.FoundationSetup.Configure') }
    'BuildWeb' { $arguments += @('-nographics','-quit','-buildTarget','WebGL','-executeMethod','HitMe.Editor.FoundationSetup.BuildWeb') }
    default {
        $resultPath = Join-Path $workspacePath ('docs/unity-' + $Action + '-results.xml')
        $arguments += @('-runTests','-testPlatform',$Action,'-testResults',('"' + $resultPath + '"'))
        if ($Action -eq 'EditMode') { $arguments += '-nographics' }
    }
}
$startedUtc = (Get-Date).ToUniversalTime()
$process = Start-Process -FilePath $Editor -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Output "Unity PID $($process.Id). Log: $logPath"
$process.WaitForExit()
Write-Output "Unity exit code: $($process.ExitCode)"
if ($process.ExitCode -ne 0) { throw "Unity failed. Read $logPath" }
if ($Action -in @('EditMode','PlayMode')) {
    if (!(Test-Path -LiteralPath $resultPath) -or (Get-Item -LiteralPath $resultPath).LastWriteTimeUtc -lt $startedUtc) {
        throw 'Unity did not produce a fresh test result XML.'
    }
    [xml]$testResult = Get-Content -LiteralPath $resultPath -Raw
    $run = $testResult.'test-run'
    if ($run.result -ne 'Passed' -or [int]$run.total -eq 0) { throw "Unity tests did not pass: $($run.result)" }
    Write-Output "Tests: $($run.passed)/$($run.total) passed. Results: $resultPath"
}
