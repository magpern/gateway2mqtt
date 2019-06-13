[![Build Status](https://dev.azure.com/magperndev/Gateway2mqtt/_apis/build/status/Gateway2mqtt?branchName=Development)]
# gateway2mqtt
This application will act as the bridge between a device gateway and a MQTT broker.

device -> gateway -> MQTT -> Home Assistant/OpenHab <- MQTT <- Gateway <- device

#Primary goal
Make it work with RFLink Gateway

Initial features:
- support Home Assistant
- support MQTT (user/password)
- support automatic discovery
- support RFLink Gateway

Future features:
- support OpenHab
- support MySensors Gatewate (serial)

Distant future features:
- support MySensors Network Gateway
- 
