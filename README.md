[![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FSoundSetter)](https://github.com/karashiiro/SoundSetter)

# SoundSetter
A Dalamud plugin that allows volume control from anywhere, including gpose and cutscenes.

## Usage
`/soundsetterconfig` - Opens/closes the plugin configuration.

`/ssconfig` - Alias for `/soundsetterconfig`

`Ctrl+K` - Default keybind for opening the plugin configuration. This can be changed in the plugin configuration.

## Screenshots
![Screenshot](https://raw.githubusercontent.com/karashiiro/SoundSetter/master/Assets/0.gif)
![Screenshot](https://raw.githubusercontent.com/karashiiro/SoundSetter/master/Assets/1.png)

## Remarks

It's disabled on the title screen due to laziness. The function I use to hook the configuration options takes a pointer to the base of the object
as its first argument, and I don't know how to get that pointer without waiting for the function to be called. Luckily, it's called automatically
when the player logs in, for one reason or another. So, I just take the address down once it's called on login, and disable it on the title screen.
