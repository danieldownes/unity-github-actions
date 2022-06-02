$mypath = $MyInvocation.MyCommand.Path
Set-Location (Split-Path -Path $mypath)
$Logfile = (Get-Location).Path + "\Logs\wait.log"

function WriteLog
{
    Param ([string]$LogString)
    $Stamp = (Get-Date).toString("yyyy/MM/dd HH:mm:ss")
    $LogMessage = "$Stamp $LogString"
    Add-content -Path $LogFile -value $LogMessage
}
New-Item -Path Logs/ -Name "from_script.txt" -ItemType "file" -Value "This is a text string."
WriteLog "STARTing..."
Start-Sleep 10
WriteLog "Time is UP!!!"
New-Item -Path Logs/ -Name "after_delay.txt" -ItemType "file" -Value "This is a text string."

