@echo off
setlocal

set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

call %msBuildDir%\msbuild.exe  ..\src\RElmah\RElmah.csproj "/p:Configuration=Debug" /verbosity:m
