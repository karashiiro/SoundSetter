[![Download count](https://img.shields.io/endpoint?url=https://vz32sgcoal.execute-api.us-east-1.amazonaws.com/SoundSetter)](https://github.com/karashiiro/SoundSetter)

# SoundSetter
A [Dalamud](https://github.com/goatcorp/Dalamud) plugin that allows volume control from anywhere, including gpose and cutscenes.

## Usage
`/soundsetterconfig` - Opens/closes the plugin configuration.

`/ssconfig` - Alias for `/soundsetterconfig`

`/ssmv +/-amount`: Adjust the game's master volume by the specified quantity.

`/ssbgm +/-amount`: Adjust the game's BGM volume by the specified quantity.

`/sssfx +/-amount`: Adjust the game's SFX volume by the specified quantity.

`/ssv +/-amount`: Adjust the game's voice volume by the specified quantity.

`/sssys +/-amount`: Adjust the game's system sound volume by the specified quantity.

`/ssas +/-amount`: Adjust the game's ambient sound volume by the specified quantity.

`/ssp +/-amount`: Adjust the game's performance volume by the specified quantity.

`Ctrl+K` - Default keybind for opening the plugin configuration. This can be changed in the plugin configuration.

## Screenshots
![Screenshot](https://raw.githubusercontent.com/karashiiro/SoundSetter/master/Assets/0.gif)
![Screenshot](https://raw.githubusercontent.com/karashiiro/SoundSetter/master/Assets/1.png)

## Remarks

It's disabled on the title screen due to laziness. The function I use to hook the configuration options takes a pointer to the base of the object
as its first argument, and I don't know how to get that pointer without waiting for the function to be called. Luckily, it's called automatically
when the player logs in, for one reason or another. So, I just take the address down once it's called on login, and disable it on the title screen.
