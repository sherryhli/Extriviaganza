# Note: run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy RemoteSigned` before executing

Param ( $inputFile )

# Specific to my machine, replace with your own path
$pdf2txtPath = "C:\Python27\Scripts\pdf2txt.py"

# Input can be either a directory of PDFs, or a single PDF file
if ( Test-Path -Path ${inputFile} -PathType Container ) {
    Write-Host "Input is a directory, creating output directory..."
    $baseName = (Get-Item ${inputFile}).Basename
    # Remove output if it already exists
    if (Test-Path ..\${baseName}_output) {
        Remove-Item ..\${baseName}_output -recurse
    }
    New-Item -ItemType directory -Path ..\${baseName}_output
    Write-Host "Successfully created ..\${baseName}_output"
    Write-Host "Converting PDFs in input directory to plaintext files now..."
    $files = Get-ChildItem ${inputFile}
    foreach ($file in $files) {
        $outputFile = "..\${baseName}_output\" + ${file}.BaseName + ".txt"
        # python2 refers to Python 2.7 on my machine since I have both versions 2.7 and 3.7 installed
        # PDFMiner, which provides the pdf2txt.py script can only be used with Python 2.*
        python2 ${pdf2txtPath} -o ${outputFile} ${file}.FullName
    }

}
else {
    Write-Host "Input is a file, skipping output directory creation..."
    $baseName = (Get-Item ${inputFile}).Basename
    $outputFile = "..\${baseName}_output" + ".txt"
    if (Test-Path ${outputFile}) {
        Remove-Item ${outputFile}
    }
    python2 ${pdf2txtPath} -o ${outputFile} ${inputFile}
}