# kps-counter
External 7-segment display driver for Wing Commander

![4 digit 7-segment display attached to an Arduino](https://drive.google.com/uc?export=view&id=1GX572G5cx3QwbIqyHrWrSDvsIwfwBXdj "4-digit 7-segment display")

Tested on Windows 10 with GOG release of Wing Commander 1.

##### Hardware used for external display:
- Elegoo Arduino UNO R3
- 830 tie-points breadboard
- 4 Digit 7-segment display (common cathode)
- 4 330-ohm resistors
- a bunch of M-M jumper wires

##### Notable files:
- kps_counter.ino - sketch to upload to the Arduino to receive data via serial connection and display
- WingCommanderMemoryReader.cs - interface to pull relevant data from DOSBox process
- Program.cs - CLI program that bridges the memory interface to the Arduino serial connection
- kps-counter.zip - Compiled version of the C# project, sketch, and memory.dll

*Additional information can be found in [my blog](http://gallagherdesign.net)*
