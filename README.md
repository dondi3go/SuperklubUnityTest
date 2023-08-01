# SuperklubUnityTest

A basic test of Superklub in Unity

Some Unity specifics are not in the Superklub project, this projet provides them.

As described in the Superklub repository, Superklub needs :
* An HTTPClient object implementing Superklub.IHttpClient interface
* Code to manages nodes according to the data provided by Superklub :
  * Spawn new nodes in a scene
  * Update nodes
  * Destroy nodes
* Superklub already supports Unity Serialization (and it is the ugliest part of the code)

## Installation

This project uses **Superklub** as a submodule

Do not forget to run `git submodule update --init --recursive` after cloning.

## How to run

To illustrate how Superklub behaves in a Unity environement :
* Run the **supersynk** server (in supersynk repo)
* Run **SuperklubSpinningBlueBox** project (in the SuperklubMSVSTests repo)
* Run this project

SuperklubSpinningBlueBox will send data to the supersynk server.

The Unity app will get these data from the server and display a rotating blue box.

It's not much, but wait to see the coming **SuperklubXR** App.

## Semantics

For the time being, the semantics of the exchanged data is naive :

* node.Shape is ["box", "ball", "pill"], it could be the id of some more complex mesh
* node.Color is ["red", "green", "blue"], again something more interesting can be imagined

## Todo list
* The Unity App is an observer for the time being. It could contribute
  to the scene by moving some local nodes. And then a third app could connect
  and see the 3D objects from the first two clients.
