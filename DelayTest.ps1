$Logfile = (Get-Location).Path + "\wait.log"
function WriteLog
{
    Param ([string]$LogString)
    $Stamp = (Get-Date).toString("yyyy/MM/dd HH:mm:ss")
    $LogMessage = "$Stamp $LogString"
    Add-content -Path $LogFile -value $LogMessage
}
WriteLog "STARTing..."
Start-Sleep 10
WriteLog "Time is UP!!!"
