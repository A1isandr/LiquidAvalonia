<div align=center>
  <h1>LiquidAvalonia</h1>
</div>

An attempt at replicating Apple's **Liquid Glass** design in [Avalonia](https://avaloniaui.net). This is still **Work In Progress**, so there is some issues.

The library comes with several new controls:
- Glass: creates lens effect, by bending content underneath it. Has properties to adjust tint, smoothness and depth;
- Blur: simply blurs background with ability to control blur radius and tint;
- Squircle: as the name suggests, approximates squircle's form. This is from my other library [Squircle.Avalonia](https://github.com/A1isandr/Squircle.Avalonia)

## Getting Started
This library can by installed from NuGet. From your project's folder you can execute this command in terminal:

``` bash
dotnet add package LiquidAvalonia
```
Then reference it in application's App.axaml file:

``` xml
<Application
    ...
    xmlns:liquid="https://github.com/A1isandr/LiquidAvalonia">
    
    <Application.Styles>
        ...
        <liquid:LiquidAvaloniaTheme/>
    </Application.Styles>
</Application>
```

## Gallery

<div align=center>
  
  ![1](https://github.com/user-attachments/assets/66c2b0a1-f08c-4235-9d35-9463169c985f)

  Glass over another material.
  
  ![2](https://github.com/user-attachments/assets/0965442a-a7e5-43cb-9ed3-151e43f5e328)

  Glass over text.

  ![3](https://github.com/user-attachments/assets/8aeffbe8-5af9-477b-9ba4-927d75155c1f)

  Animation of Tint, TintColor, Smoothness and Depth properties
</div>

## Known Issues
- Glass: At the moment, "frost" effect is not presented;
- Glass: Refracted image has some aliasing;
- Glass: RotationTransform may not work properly;
- Glass: other rendering issues may occur.
