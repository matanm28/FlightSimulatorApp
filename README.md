
# FlightSimulatorApp

"Flight Simulator App"- third Mile-Stone main achievements:

    1) Using the .NET Framework to create a GUI App for FlightGear.
    2) Using MVVM architecture in a multi-threaded enviorment.
    3) Implementing a TCP Client to send/receive and parse data from FlightGear.

## More on the implementation process

This **App** has three main parts that runs it, each part with it's own designated responsabilities. The **Model** interacts with FlightGear via TCP connection, contiunasly send and read data request and notifies the relevant **ViewModel** when it's data changed. Next the **ViewModels** proccess the changed data and notifies the **Views** about the changed data. the **Views** then react to the changed data accordingly, the data flows both from and to the **Model**.
In the end the aircraft inside FlightGear reacts to the joystick and handles being moved in our app.

![FlightGearApp GUI](https://github.com/matanmkl/FlightSimulatorApp/blob/master/FlightGearApp.JPG)

## Compiling and Running
We provide a _batch script_ to compile this project, you should use it!
The batch file is called _build_and_run_ and is located at the reposetory base folder.
To properly use this _batch script_ you should download the repository content without changing any files location and have Visual Studio 2019 installed, just double click the _batch script_ and you're good to go :)
(note that _build_and_run_ should reside in the same folder as the FlightSimulatorApp.zip you downloaded).
To properly use the app you should open FlightGear with the following settings:
![FlightGear settings](https://github.com/matanmkl/FlightSimulatorApp/blob/master/FlightGearSettings.JPG)

## Python Script
We provide a python script called_dummyServer_ who acts as a FlightGear server. We used this _dummyServer_ while building the project in order to speed up devlopment. This script works with a default port number 5403 so you should run the main program with the same port number.
