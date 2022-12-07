![Xnapper-2022-12-07-12 06 51](https://user-images.githubusercontent.com/4278331/206150936-5fdfb740-6365-4fe0-b47a-4c5965b2317a.png)

# SubstrateNET.UnityDemo

Implementation of a Ajuna.NetWallet in a Unity app where the following is possible:

- Create Wallet and store in the local file storage
- Restore wallet from file storage
- Transfer Balance from Alice to the newly created wallet account
- Information about the state of the executed extrinsic.
- See wallet account balance
- We can see how this works in this video.

# Requirements
To run this demo, you must have Unity installed on your computer. You can download it from here. Please select Unity version 2021.3.7f1.

## Run
- Run Substrate Node: [monthly-2022-11](https://github.com/paritytech/substrate/releases/tag/monthly-2022-11) in `--dev` mode
- Open project
- Open SubstrateDemo scene
- Play project

Walkthrough of the impelementation can also be found in the following [video](https://www.loom.com/share/09c93e0bc1c1452b94b9367f15da9cbe). 


## Getting started with Unity and Ajuna.SDK

If you want to get started connecting to a Unity node from withing Unity, we have created a step-by-step video guide that walks you through the .NET projects generation and integration in a Unity project. 

The series consists of three parts:
- First Video: .NET projects generation based on our node using the Ajuna.SDK.
- Second Video: Generation of the necessary DLLs for our Unity Project
- Third Video: Walkthrough of the Unity implementation that uses the above DLLs.
