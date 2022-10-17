# TankRobot
A remote-controlled robot tank programmed in C#.net

The C#.net project includes client and server projects. The client is to be installed on a windows laptop, and the server program is to be installed on the robot's windows machine. The windows machine on board the robot will control the robot, including a pan/tilt webcam, robot arm, and GPS. 
The client program on a laptop is used to remotely control the robot via WIFI using any joystick connected to the USB. Life video and audio are streamed to the robot, and live audio is streamed back to the robot from the laptop.
The client's application can control the followings:
1-	The robot arm with 4 servo motors.
2-	Control webcam pan/tilt.
3-	The audio volume on the robot.
4-	Shut down the robot remotely.
5-	Steering and speed control of the robot.
