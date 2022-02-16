# csDelaunay

A .NET library providing Delaunay triangulation and Lloyd relaxation.

This is a port and interpretation of ActionScript library [as3delaunay](http://nodename.github.io/as3delaunay/).
- @PouletFrit ported the library from AS3 and added a Lloyd relaxation function.
- @frabert made significant optimizations.
- @charlieturndorf provided a cross-platform build (.NET Standard 2) that will also work on .NET Framework.

### Maintenance Note
For a maintained fork of csDelaunay, consider using [charlieturndorf/csDelaunay](https://github.com/charlieturndorf/csDelaunay).

## Setup
1. Clone the repository (you might wish to create a [submodule](https://github.blog/2016-02-01-working-with-submodules/) under another project)
2. Download and install the [.NET SDK](https://dotnet.microsoft.com/en-us/download), if you don't have it
3. On the command line, navigate to the root folder of your csDelaunay clone
4. Run init.cmd to bootstrap the dependency manager

## Build
1. On the command line, navigate to the root folder of your csDelaunay clone
2. Run build.cmd

OR
1. Build with Visual Studio
