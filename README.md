[![Build Status](https://dev.azure.com/magperndev/Gateway2mqtt/_apis/build/status/Gateway2mqtt?branchName=Development)](https://dev.azure.com/magperndev/Gateway2mqtt/_build/latest?definitionId=1&branchName=Development)
![Build Status](https://img.shields.io/azure-devops/build/magperndev/bf848090-f403-4315-a1e6-5d077aaef8d3/1/Development.svg) ![Build Status](https://img.shields.io/azure-devops/coverage/magperndev/gateway2mqtt/1/Development.svg) ![Build Status](https://img.shields.io/azure-devops/tests/magperndev/gateway2mqtt/1/Development.svg)

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
