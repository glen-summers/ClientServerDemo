@echo off
setlocal
cls

set MYSQL_VER=MySQL Server 5.7
set MYSQL_WORKBENCH_VER=MySQL Workbench 6.3 CE
set DBUSER=DemoUser
set DBPASS=DemoUser

set MYSQL_WORKBENCH="c:\Program Files\MySQL\%MYSQL_WORKBENCH_VER%\MySQLWorkbench.exe"
set SQL="c:\Program Files\MySQL\%MYSQL_VER%\bin\mysql.exe"

set CRED=--user=root --password
set USERCRED=--user=%DBUSER% --password=%DBPASS%
set OPT=--show-warnings
set NOWARNOPT=

if NOT EXIST %SQL% echo please install mysql %SQL% & goto :eof

rem check db running? issue sc start etc.

%SQL% %USERCRED% %OPT% -e ""
if %ERRORLEVEL% equ 0 goto :userOk

set cmd="CREATE USER '%DBUSER%'@'localhost' IDENTIFIED BY '%DBPASS%';GRANT ALL PRIVILEGES ON *.* TO '%DBUSER%'@'localhost';FLUSH PRIVILEGES;"
echo Enter creds for root@localhost for dev db setup
%SQL% %CRED% %OPT% -e %cmd%
if %ERRORLEVEL% neq 0 echo Setup failed & goto :eof

:userOk
%SQL% %USERCRED% %OPT% -e "status;"
if %ERRORLEVEL% neq 0 echo status failed & goto :eof

%SQL% %USERCRED% %NOWARNOPT%  --default-character-set=utf8 < .\Schema.sql
if %ERRORLEVEL% neq 0 echo schema deploy failed & goto :eof

echo done.