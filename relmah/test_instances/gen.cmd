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

set config=/XF *.config
if "%1"=="config" set config=

rem backend
call :gen_server 9000 %config%

rem rem servers
call :gen_server 9001 %config%
call :gen_server 9002 %config%
call :gen_server 9003 %config%

rem rem dashboards
call :gen_dashboard 8001 %config%
call :gen_dashboard 8002 %config%
call :gen_dashboard 8003 %config%

rem rem error sources
call :gen_source 7001 %config%
call :gen_source 7002 %config%
call :gen_source 7003 %config%

goto :end

:gen
set local
robocopy ../src/Samples/RElmah.%2 %3%1 *.* /MIR %4
echo "%programfiles(x86)%\IIS Express\iisexpress.exe" /path:%cd%\%3%1 /port:%1 > run_%1.cmd
goto :eof

:gen_server
set local
call :gen %1 Server s %2
goto :eof

:gen_dashboard
set local
call :gen %1 Dashboard.Basic s %2
goto :eof

:gen_source
set local
call :gen %1 Source s %2
goto :eof

:end
