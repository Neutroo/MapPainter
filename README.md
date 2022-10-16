# MapPainter
An application for creating a route map that the robot takes over the network and then moves along a given trajectory on C# with WPF.

## Getting Started
These instructions will get you a copy of the project, it will make it up and running on your local machine for development or usage.

### Robot Construction
The robot contains of:
* Arduino Mega 2560 R3
* Bluetooth-module HC-06
* Accelerometer and Gyroscope Sensor MPU-6050
* DC Motor Driver Module L298N
* Gearmotors (x4)
* DC-DC Converter XL6009
* YwRobot Breadboard Power Supply MB-V2

Wiring schema:
![Wiring Schema](https://raw.githubusercontent.com/Neutroo/Neutroo/main/Images/MapPainter/WiringSchema.png)

### Prerequisites
1. Install sketch for the robot - [click](https://github.com/Neutroo/MapPainter/releases/download/1.0/4WheelRobot.zip).
2. Upload a sketch to an Arduino.\
If you don't know how - read the [instruction](https://www.dummies.com/article/technology/computers/hardware/arduino/how-to-upload-a-sketch-to-an-arduino-164738).

### Installing
To download a copy of the project [click here](https://github.com/neutroo/MapPainter/archive/refs/heads/master.zip).

### Running the project
1. Build the project.

2. Right click on the "MapPainter" in the "Solution explorer" and click on "Open folder in the explorer".

3. Open bin/Debug or bin/Release and run MapPainter.exe.

![ConnectPage](https://raw.githubusercontent.com/Neutroo/Neutroo/main/Images/MapPainter/Screenshot%202022-03-28%20222436.png)

4. Click to the connect button and start paint the route.

## Results
<div>
  <a>
    <img width="49%" src="https://raw.githubusercontent.com/Neutroo/Neutroo/main/Images/MapPainter/hexagon_program.gif"/>
    <img width="49%" src="https://raw.githubusercontent.com/Neutroo/Neutroo/main/Images/MapPainter/hexagon_robot.gif"/>
  </a>
</div>

## Built With
* [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) - The framework used
* [WPF](https://docs.microsoft.com/ru-ru/visualstudio/designers/getting-started-with-wpf?view=vs-2022) - Used for creating client layer

## Authors
* Rodion Kushnarenko - [Neutro](https://github.com/Neutroo)
* Ivan Timofeev - [Raikarus](https://github.com/Raikarus)

See also the list of [contributors](https://github.com/Neutroo/MapPainter/graphs/contributors) who participated in this project.

## License
This project is licensed under the MIT License - see the [LICENSE](https://github.com/neutroo/MapPainter/blob/master/LICENSE) file for details.
