The program monitors the power outputs of Sharp JH-1600E inverter.

It runs a daily task that starts from 4am and determines when to begin and end logging activities based on sunrise and sunset time obtained from Yahoo Weather RSS feed. (Note: the program would begin logging immediately if it starts after the sunrise).

The program requests data from inverter every 60 seconds. It stores data locally in CSV format and send request remotely to PVOutput.org using the Status API on every 10th minute of the hour.

For information about how to setup data logging from Sharp JH-1600E inverter, see the link here: http://power.vacated.net/jh1600e.php