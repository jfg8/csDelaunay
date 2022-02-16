# csDelaunay

A .NET library providing Delaunay triangulation and Lloyd relaxation.

This is a port and interpretation of ActionScript library [as3delaunay](http://nodename.github.io/as3delaunay/).
- @PouletFrit ported the library from AS3 and added a Lloyd relaxation function.
- @frabert made significant optimizations.
- @charlieturndorf provided a cross-platform build (.NET Standard 2) that will also work on .NET Framework.

### Cross-Platform
csDelaunay will run [anywhere .NET Standard 2.0 will run](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0).
You can build and develop on Windows, Mac, or Linux (requires .NET SDK 2.1.5 or higher).

**Mac/Linux Dev Note:**
The scripts `init.cmd` and `build.cmd` are for Windows, but they're very simple and you can do the same thing in the terminal. Please feel free to submit a PR adding bash scripts.

## Setup
1. Clone the repository (you might wish to create a [submodule](https://github.blog/2016-02-01-working-with-submodules/) under another project)
2. Download and install the [.NET SDK](https://dotnet.microsoft.com/en-us/download), if you don't have it
3. On the command line, navigate to the root folder of your csDelaunay clone
4. Run `init.cmd` to bootstrap the dependency manager

## Build
1. On the command line, navigate to the root folder of your csDelaunay clone
2. Run `build.cmd`

OR
1. Build with Visual Studio
