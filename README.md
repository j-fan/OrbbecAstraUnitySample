# Orbbec Unity Sample
## Prerequisites
- Windows machine
- Orbbec Astra
- Unity 2019+ (could work for others, but only tested with this)
- Visual Studio 2015+ (Orbbec SDKs seem fine in VS 2017)

## About
* Contains examples of depth and colour streams and body tracking (joints only)
* If you stand far back enough, spheres will appear representing joints
* Body tracking will only work until March 21 2020, refer to instructions on how to add your own license if you have one

## Instructions

To update Orbbec Astra SDK this project depends on

1. Download the Orbbec Astra SDK

2. Open the project in OrbbecSDK/samples/vs2015
Update with your license key in main.cpp, in the variable `const char* licenseString = "<INSERT LICENSE KEY HERE>";`
3. compile the project with the same target platform as your machine (either amd64 or x86 Windows)

4.  Copy binaries compiled in OrbbecSDK/samples/vs2015/bin/Release to the /Assets/AstraSDK/Astra/{your platform} 
    * the platform folder is either "amd64" or "x86". Create one if not present and delete the other. Only one set of binaries can be present as Unity gets confused.

## Credits
Astra SDK dotnet wrapper

https://github.com/bibigone/AstraDotNetDemo
