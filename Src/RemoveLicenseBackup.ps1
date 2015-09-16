
. .\LicenseVars.ps1

$items = Get-ChildItem -Path $sourcePath -Filter *.bak -Recurse -Exclude $excludeFiles | ?{ $_.fullname -notmatch "\\obj\\*" } | ?{ $_.fullname -notmatch "\\bin\\*" } | ?{ $_.fullname -notmatch "\\gtk-gui\\*" }

foreach($item in $items)
{
    Write-Host "Remove" $item.Name "..."
    Remove-Item -Path $item.FullName    
}

Write-Host "All done. Have an nice Day"