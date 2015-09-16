
. .\LicenseVars.ps1

# Cleans the old Header
function Clean-Header()
{
    Param(
        [string] $source
    )

    $usingStart = $source.IndexOf("using")    
    $namespaceStart = $source.IndexOf("namespace")
    $realstart = -1

    # Check for the Start-Point of the Code
    if($usingStart -ne -1 -and $namespaceStart -eq -1)
    {
        $realstart = $usingStart
    }
    elseif($namespaceStart -ne -1 -and $usingStart -eq -1)
    {
        $realstart = $namespaceStart
    }
    elseif($usingStart -ne -1 -and $namespaceStart -ne -1)
    {        
        if($usingStart -lt $namespaceStart)
        {
            $realstart = $usingStart
        }
        else
        {
            $realstart = $namespaceStart
        }
    }
    

    # Remove old Header
    if($realstart -ne -1)
    {
        $source = $source.Substring($realstart)
    }

    return $source
}

# Replace an Token in the License File
function Replace-Token()
{
    Param(
        [string] $token,
        [string] $replaceWith,
        [string] $source
    )

    return $source.Replace($token, $replaceWith)
}

# Combine Source File with the License File
function Set-Header()
{
    Param(
        [string] $license,
        [string] $source
    )
    
    return $license + [System.Environment]::NewLine + [System.Environment]::NewLine + $source 
}

cls

$items = Get-ChildItem -Path $sourcePath -Filter $sourceFilter -Recurse -Exclude $excludeFiles | ?{ $_.fullname -notmatch "\\obj\\*" } | ?{ $_.fullname -notmatch "\\bin\\*" } | ?{ $_.fullname -notmatch "\\gtk-gui\\*" }
$licenseText = Get-Content -Path $licenseFile -Raw

foreach($item in $items)
{
    Write-Host "Process" $item.Name "..."
    Write-Host ""

    # Read contet from the File
    $content = Get-Content $item.FullName -Raw
    
    # Should we take an backup
    if($backup -eq $true)
    {
        if($whatIf -eq $false)
        {
            Copy-Item $item.FullName -Destination "$item.bak"
        }
    }
    
    # Clean current Header
    $content = Clean-Header $content

    # Replace Tokens in the License Text (feel free to add another Tokens)
    $newLicenseText = Replace-Token -token '${FileName}$' -replaceWith $item.Name -source $licenseText

    # Combine the License with the Source File
    $content = Set-Header -license $newLicenseText -source $content
    
    # Write back the Sourcefile
    if($whatIf -eq $false)
    {
        Set-Content -Path $item.FullName -Value $content -Encoding UTF8
    }
    else
    {
        Write-Host $content
    }    
}

Write-Host "All done. Have an nice Day"