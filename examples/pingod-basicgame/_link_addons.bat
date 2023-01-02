SET cDir=%cd%
cd ../../addons
SET addDir=%cd%
cd %cDir%
mklink /D addons "%addDir%"