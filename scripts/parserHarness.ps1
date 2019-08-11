Param (
    [Parameter (Mandatory=$True,Position=0)]
    [string]$inputFile,
    [Parameter (Mandatory=$True,Position=1)]
    [string]$parserName,
    [Parameter (Mandatory=$True,Position=2)]
    [string]$level,
    [Parameter (Mandatory=$True,Position=3)]
    [string]$tournament,
    [Parameter (Mandatory=$True,Position=4)]
    [string]$year
 )

# Requirement: build the console app first
$parser = "..\QbPackParser\bin\Release\netcoreapp2.2\win10-x64\QbPackParser.exe"

# Input can be either a directory of .txt files, or a single file
if ( Test-Path -Path $inputFile -PathType Container ) {
    Write-Host "Parsing files now..."
    $files = Get-ChildItem $inputFile
    foreach ($file in $files) {
        $params = $parserName, $file.FullName, $level, $tournament, $year
        # Printing params in case problematic files need to be traced
        Write-Host $params -ForegroundColor Yellow
        & "$parser" $params
   }
}
else {
    $params = $parserName, $inputFile.FullName, $level, $tournament, $year
    Write-Host $params -ForegroundColor Yellow
    & "$parser" $params
}