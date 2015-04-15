::===================================
::
:: run all apps
::
::===================================

@echo off
set local
cls

::frontend
start cmd /k call run_f_9001
timeout 1

::source
start cmd /k call run_e_8001
timeout 1

::dashboard
start cmd /k call run_d_7001
timeout 1