
Capture Pipeline used to record and merge ground truth capacitive and OptiTrack data.

### Instructions how the different programs act together
1. Set up Optitrack and store ttp configuration in Optitrack folder
2. Synchronize clocks on the PC and Android device
3. Start Android app
4. Establish adb reverse tunnel on PC to connect to Android app server: adb reverse tcp:9999 tcp:9999 (tunnel is used to synchronize data capture when an object is placed, log files are stored locally on each device)
5. Start this program to initiate data capture of Android capacitive raw data and concurrent optitrack data capture on PC
6. Use merge tool to match both log files by nearest timestamp
