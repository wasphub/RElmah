::===================================
::
:: run all apps
::
::===================================

@echo off
set local
cls

pushd Deploy

::backend
start cmd /k call run_b_9000
timeout 3

::frontends
start cmd /k call run_f_9001
timeout 1
start cmd /k call run_f_9002
timeout 1

::sources
start cmd /k call run_e_8001
timeout 1
start cmd /k call run_e_8002
timeout 1

::dashboards
start cmd /k call run_d_7001
timeout 1
start cmd /k call run_d_7002
timeout 1

popd