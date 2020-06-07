# Scripts

Note: before executing the scripts here, you may need to run:

```
Set-ExecutionPolicy -Scope Process -ExecutionPolicy RemoteSigned
```

To call `parserHarness.ps1`:
```
.\parserHarness.ps1 -inputFile [path-to-question-file] -parserName [pace | scop] -level [MS | HS | C | T] -tournament [tournament-name] -year [year]
```