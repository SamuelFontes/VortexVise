<p align="center">
	<img src="/Resources/Common/vortex-vise-logo.png" alt="VortexViseLogo" />
</p>
<h1>Vortex Vise</h1>
VortexVise is an Open Source old school style deathmatch game made with Raylib in C#.  
Can be played with bots or up to 4 players over local split screen multiplayer, to join press either A or Enter on the character selection screen.  
It has a nice portability layer it can be compiled both for Desktop and Web so it's easialy portable to other platforms.  

[Play on your Browser](https://samuelfontes.github.io/VortexViseDemo/)  
[Download](https://github.com/SamuelFontes/VortexVise/releases/download/release/VortexVise.zip)  
[Itch.io](https://samuelfontes.itch.io/vortexvise)
<h2>Controls - MouseAndKeyboard/Gamepad  </h2>
[Menu]  

Confirm - Enter/A  

Back - Esc/B  

[Gameplay]  

Jump - SpaceBar/A   

HookShot - RightMouseClick/X  

PickUpItem - E/B  

UseWeapon - LeftMouseClick/RightTrigger  

UseJetPack - LeftShift/LeftTrigger  

<h2>Screenshots</h2>
<img src="/Screenshots/screenshot000.png" />
<img src="/Screenshots/screenshot0001.png" />
<img src="/Screenshots/screenshot0002.png" />
<img src="/Screenshots/screenshot001.png" />
<img src="/Screenshots/screenshot0012.png" />
<img src="/Screenshots/screenshot002.png" />
<img src="/Screenshots/screenshot003.png" />
<img src="/Screenshots/screenshot004.png" />
<img src="/Screenshots/screenshot005.png" />

<h2>License</h2>
The game is licensed under GPL 2, so you can do wathever you want with the code as long as you give the credits and publish the source code if you made any changes.  
If you want to get this code with a more permissive license just message me, if you want to use this code as base for something else I can change the license to MIT or something. 

<h2>Compiling</h2>
Both Versions require dotnet 8 installed on your machine.  
<h3>Desktop</h3>
Open the root folder on the terminal and just run like normal(should work)  

```
cd src\VortexVise.Desktop
dotnet run
```

<h3>Web</h3>
Credits to these guys on figuring out how to do this shit: (https://github.com/Kiriller12/RaylibWasm)  
Open the root folder on the terminal then nstall wasm things, publish and then dotnet-serve to run:  

```
cd src\VortexVise.Web
dotnet workload install wasm-tools
dotnet tool install --global dotnet-serve
dotnet publish -c Release
dotnet serve -d bin\Release\net8.0\browser-wasm\AppBundle
```

