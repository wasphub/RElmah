::===================================
::
:: generate test apps
::
:: usage:
::      gen           creates/syncs all the folders, skipping web.config
::      gen config    creates/syncs all the folders, including web.config
::
::===================================

@echo off
set local
cls

set config=*.config
if "%1"=="config" set config=

rem backend
call :gen_server 9000 b %config%

rem servers
call :gen_server 9001 f %config%
call :gen_server 9002 f %config%

rem dashboards
call :gen_dashboard 7001 d %config%
call :gen_dashboard 7002 d %config%

rem error sources
call :gen_source 8001 e %config%
call :gen_source 8002 e %config%


goto :end

:gen
set local
robocopy ../src/Samples/RElmah.%2 %3%1 *.* /MIR /XF %4
echo "%programfiles(x86)%\IIS Express\iisexpress.exe" /path:%cd%\%3%1 /port:%1 ^& exit > run_%3_%1.cmd
goto :eof

:gen_server
set local
call :gen %1 Server %2 %3
goto :eof

:gen_dashboard
set local
call :gen %1 Dashboard.Basic %2 %3
goto :eof

:gen_source
set local
call :gen %1 Source %2 %3
goto :eof

:end
