set PKG=DataValidationFramework
mkdir lib\sl4
mkdir lib\net40

msbuild ..\%PKG%.Net\%PKG%.Net.csproj
msbuild ..\%PKG%.Silverlight\%PKG%.Silverlight.csproj

copy ..\%PKG%.Net\Bin\Debug\%PKG%.Net.dll lib\net40
copy ..\%PKG%.Silverlight\Bin\Debug\%PKG%.Silverlight.dll lib\sl4

subwcrev . %PKG%.nuspec.src %PKG%.nuspec
