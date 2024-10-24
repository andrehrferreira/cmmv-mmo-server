# CMMV MMO Server

This is an Multiplayer server based on the CMMV framework, which automatically generates RPC communication packages and .cpp files for use with Unreal Engine, supporting both C++ and Blueprints. Follow the steps below to set up and start the project.

## Prerequisites

* **Windows 10 or higher**
* **Unreal Engine 5.3 or higher**
* **.NET SDK 6.0 or higher**
* **Visual Studio 2022**

Before setting up the project, ensure you have the following NuGet packages installed:

1. **DotNetEnv**: Used for loading environment variables from a `.env` file.  
   [Download DotNetEnv](https://www.nuget.org/packages/DotNetEnv)

2. **Microsoft.VisualStudio.Interop**: Required for integration with Visual Studio and automation features.  
   [Download Microsoft.VisualStudio.Interop](https://www.nuget.org/packages/Microsoft.VisualStudio.Interop)


## Step 1: Create a Virtual Link with the Client Directory

Before getting started, you need to create a virtual link that points to your Unreal client directory. This client can be any Unreal Engine project you have.

1. Navigate to the directory where you want to create the virtual link.

```bash
$ .\ClientLink.ps1 -ClientDir "C:\Client" -ServerDir "C:\Server"
```

## Step 2: Configure the .env File

In the root of the server project, create a file called .env and fill it with your Unreal client and Unreal editor paths. This file is required for the system to locate and generate the packages and .cpp files correctly.

Example ``.env``:

```
CLIENT_PROJECTNAME=Client
CLIENT_PATH=C:\Client
UNREAL_EDITOR_PATH=C:\UE_5.5\Engine
```

* **CLIENT_PROJECTNAME:** The name of your Unreal client project.
* **CLIENT_PATH:** The path to your Unreal client project directory.
* **UNREAL_EDITOR_PATH:** The path to your Unreal Engine editor.

## Step 3: Build and Generate Communication Packages

Once the virtual link and .env file are set up:

1. Build the project using Visual Studio or your preferred method.
2. When building, the system will generate RPC communication packages and .cpp files that can be used directly in Unreal Engine for C++ or Blueprint integration.

## Features

* **Automatic RPC Generation:** The server generates RPC communication packages automatically based on the contracts you define.
* **Unreal Integration:** Generates .cpp files for use in Unreal Engine, accessible in both C++ and Blueprints.
* **Flexible Setup:** Easily link any Unreal project and configure paths using the .env file.
* **UDP Communication:** Supports UDP communication for high-performance tasks such as movement replication, combat, and collision handling, ensuring fast and efficient updates.
* **WebSocket Communication:** Uses WebSocket for non-immediate data such as chat, marketplace, and other systems that do not require real-time performance, avoiding interference with UDP packets.
* **Automated Testing Tools:** Includes a suite of automated testing tools that ensure the integrity of the server before startup, improving reliability and reducing the risk of runtime errors.
* **High-Performance Binary Communication:** Utilizes ByteBuffer and QueueBuffer for high-performance binary communication, optimizing data transfer between the server and clients.
* **Packet Encoding:** Provides simple XOR encoding with individual keys per client for basic protection, and elliptic curve cryptography (ECC) combined with AES for secure communication when necessary, ensuring data security where it matters.

## Checklist

| Feature                             | Status            | Notes                                                       |
|------------------------------------|-------------------|-------------------------------------------------------------|
| RPC                                | 🛠 In Progress     | RPC system is currently under development.                  |
| C# Packet Creation                 | ✅ Working         | Packet creation in C# is functional and tested.             |
| C++ Interface for Unreal           | 🛠 In Progress     | Interface creation for Unreal Engine is in progress.        |
| Testing System                     | ✅ Working         | Automated testing system is functional and operational.     |
| ByteBuffer and QueueBuffer         | ✅ Working         | High-performance binary communication buffers are working.  |
| WebSocket                          | ⏳ Not Implemented | WebSocket support has yet to be implemented.                |
| UDP                                | ⏳ Not Implemented | UDP communication setup is yet to be implemented.           |
| XOR Encoding                       | ⏳ Not Implemented | XOR encoding system is yet to be implemented.               |
| ECC and AES256 Encryption          | ⏳ Not Implemented | Encryption using ECC and AES256 needs to be implemented.    |
| Base Replication                   | ⏳ Not Implemented | Replication system for core game elements is not implemented yet. |
| JWT Authentication                 | ⏳ Not Implemented | JWT-based authentication needs to be implemented.           |
| Reactive System                    | ✅ Working         | Reactive system is fully functional and operational.        |
| Network Event System               | ✅ Working         | Reactive network event system is implemented and functional.|
| Reactive Request Handlers          | 🛠 In Progress     | Reactive request handlers with decorators are being developed. |

## Benchmark 

### Websocket

* [https://github.com/andrehrferreira/bench-ws](https://github.com/andrehrferreira/bench-ws)
* **Machine:** linux x64 | 32 vCPUs | 256.6GB Mem
* **Node:** v20.17.0
Run: Thu Oct 23 2024 21:19:12 GMT+0000 (Coordinated Universal Time)

| (index) | Server           | Avg Messages/sec | % Difference |
|---------|------------------|------------------|--------------|
| 0       | Rust             | 1,232,041.4      | 541.60%      |
| 1       | Java             | 1,175,892        | 516.41%      |
| 2       | C#               | 1,132,847.8      | 496.48%      |
| 3       | C++ (Crow + TBB) | 504,620.8        | 157.50%      |
| 4       | PHP / Swoole     | 485,236.6        | 149.58%      |
| 5       | Erlang / Elixir  | 296,681.2        | 66.95%       |
| 6       | Bun              | 266,875.2        | 53.20%       |
| 7       | Go               | 263,391.2        | 51.51%       |
| 8       | Python3          | 191,937          | 16.15%       |
| 9       | Node             | 154,831.2        | -4.33%       |
| 10      | uWebsocket.js    | 100,140.4        | -33.47%      |
| 11      | Dart             | 6,936.8          | -95.12%      |
| 12      | Ruby             | 3,456.4          | -97.51%      |