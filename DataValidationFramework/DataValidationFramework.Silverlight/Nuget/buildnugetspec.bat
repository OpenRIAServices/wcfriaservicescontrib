set PKG=DataValidationFramework
mkdir lib\sl4
mkdir lib\net40

msbuild ..\..\%PKG%.Net\%PKG%.Net.csproj /p:Configuration=Release
msbuild ..\..\%PKG%.Silverlight\%PKG%.Silverlight.csproj /p:Configuration=Release

copy ..\..\%PKG%.Net\Bin\Release\%PKG%.Net.dll lib\net40
copy ..\..\%PKG%.Silverlight\Bin\Release\%PKG%.Silverlight.dll lib\sl4

subwcrev .. %PKG%.Silverlight.nuspec.src %PKG%.nuspec
