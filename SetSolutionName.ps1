param (
        [Parameter(
        Mandatory=$true,
        ValueFromPipeline=$true,
        HelpMessage = "New solution name (e.g. DogAppBlazor)")]
		[string]$NewSolutionName
)

[string]$SolutionFolder = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path);

Get-ChildItem -recurse $SolutionFolder -include *.cs,*.csproj,*.config,*.ps1,*.json,*.tsx,*.cshtml,*.props,*.razor,*.json,*.html,*.js,*.slnx | where { $_ -is [System.IO.FileInfo] } | where { !$_.FullName.Contains("\packages\") } | where { !$_.FullName.Contains("\obj\") } | where { !$_.FullName.Contains("package.json") } | where { !$_.Name.Equals("_SetApplicationName.ps1") } |
Foreach-Object {
    Set-ItemProperty $_.FullName -name IsReadOnly -value $false
    [string]$Content = [IO.File]::ReadAllText($_.FullName)
    $Content = $Content.Replace('DogAppBlazor', $NewSolutionName)
    [IO.File]::WriteAllText($_.FullName, $Content, [System.Text.Encoding]::UTF8)
}

Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'Havit.Blazor.SimpleBlazorWebAppTemplate.slnx')) -newName ($NewSolutionName + '.slnx')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'src\DogAppBlazor\DogAppBlazor.csproj')) -newName ($NewSolutionName + '.csproj')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'src\DogAppBlazor.Client\DogAppBlazor.Client.csproj')) -newName ($NewSolutionName + '.Client.csproj')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'src\DogAppBlazor.Client')) -newName ($NewSolutionName + '.Client')
Rename-Item -path ([System.IO.Path]::Combine($SolutionFolder, 'src\DogAppBlazor')) -newName ($NewSolutionName)
