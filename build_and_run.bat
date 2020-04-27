set SOLUTION_NAME=FlightSimulator
set EXE_NAME=FlightSimulatorApp

REM Load Visual Studio Build Tools into the environment. 
REM You may change the location of the Visual Studio installation or edition
REM The grader will use Visual Studio 2019. 
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat"

REM Unzip the submitted zip file into the current directory
tar -xf %SOLUTION_NAME%.zip

REM The contents of the zip may be the entire solution directory (and not the contents of that directory)
REM so cd into the solution directory first

if EXIST %SOLUTION_NAME%\ (
	if NOT EXIST %EXE_NAME%\ (
		cd %SOLUTION_NAME%
	) 
)

REM Clean and Build the solution
msbuild %SOLUTION_NAME%.sln -t:clean

REM Verify the executable is no longer present in the output folder.
IF EXIST ./out/%EXE_NAME%.exe (
	echo Output exists after clean but before build, exiting
	EXIT /B -999
)

REM All warnings are considered as errors
msbuild %SOLUTION_NAME%.sln -t:build -warnaserror

REM run the Executable
START ./out/%EXE_NAME%.exe