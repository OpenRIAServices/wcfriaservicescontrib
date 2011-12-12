set PKG=DataValidationFramework
mkdir lib\net40

msbuild ..\..\%PKG%.Net\%PKG%.Net.csproj /p:Configuration=Release
msbuild ..\..\%PKG%.EF\%PKG%.EF.csproj /p:Configuration=Release

copy ..\..\%PKG%.Net\Bin\Release\%PKG%.Net.dll lib\net40
copy ..\..\%PKG%.EF\Bin\Release\%PKG%.EF.dll lib\net40

subwcrev .. %PKG%.EF.nuspec.src %PKG%.nuspec
