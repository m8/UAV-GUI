# Tele-Touch #
The project has a three main controllers.

-GUI

-Raspberry Pi

-Microcontroller (atmega328)


## GUI ##
The GUI was written at C#.net.

## Raspberry Pi ##
We have python script that runs on the Raspi 3. First of all you should install python 2.7 to your raspi.

```
#!python
sudo apt-get install python-2.7
```
After this process, you have to install OpenCV. You can complete this process from that link: 
[
http://www.pyimagesearch.com/2016/04/18/install-guide-raspberry-pi-3-raspbian-jessie-opencv-3/](Link URL)


Ok, now let's put the python script to your Desktop. When we open our Raspi we have to change our source.

```
#!python

source ~/.profile
```

After that part we should change our directory.

```
#!python

cd Desktop/
```
Now we can start our script.


```
#!python

python proje.py
```
## Microcontroller

We use Arduino cart for controlling our circuit. It's programmed in C with ArduinoIDE. Also you can download the PCB files.