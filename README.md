# Amazon Dash Button for Philips Hue lights 

Windows Application/Service to discover Amazon Dash buttons and use them to control Philips Hue Lights

## Reuirements

* [WinpCap](https://www.winpcap.org/install/default.htm)


## Discover Amazon buttons

Run command:
```
Wikiled.DashButton.App.exe DiscoverButtons
```

Register button in Amazon program (except the last step). 
During process, our app will automatically register new button and add record to service.json
During registration, the button is assigned its Mac address as the name. It can be renamed manually.

## Register with Philips Hue Bridge

Run command:
```
Wikiled.DashButton.App.exe SetupBridge
```

And press button on Philips Hue bridge.


## Install as Windows service
```
Wikiled.DashButton.App.exe install
```

## Configuration Sample

```
{
  "Buttons": {
    "Office": {
      "Mac": "50-F5-DA-61-A9-2F",
      "Actions": [
        {
          "Groups": [ "Office" ],
          "Type": "Simple"
        }
      ]
    },
    "HallUp": {
      "Mac": "AC-63-BE-4C-BA-9D",
      "Actions": [
        {
          "Groups": [ "Corridor" ],
          "Type": "Simple"
        }
      ]
    },
    "BedroomMain": {
      "Mac": "0C-47-C9-CC-50-C2",
      "Actions": [
        {
          "Groups": [ "Main Bedroom", "Hall" ],
          "Type": "Simple"
        }
      ]
    },
    "AC-63-BE-45-2A-B4": {
      "Mac": "AC-63-BE-45-2A-B4",
      "Actions": []
    }
  },
  "Bridges": {
    "xxxx": {
      "Id": "xxxx",
      "AppKey": "xxxx"
    }
  }
}
```

-  **Actions** - specify actions for the button
-  **Groups** - for which Hue group action should be performed
-  **Type** - **Simple** - Switch On/Off lights