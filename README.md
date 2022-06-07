# Networking
![Build](https://github.com/ArturMarekNowak/Networking/actions/workflows/build.yml/badge.svg)
[![CodeFactor](https://www.codefactor.io/repository/github/arturmareknowak/networking/badge/master)](https://www.codefactor.io/repository/github/arturmareknowak/networking/overview/master)

This repository contains implementations of simple networking examples.

## Table of contents
* [General info](#general-info)
* [Screenshots](#screenshots)
* [Technologies](#technologies)
* [Setup](#setup)
* [Features](#features)
* [Status](#status)
* [Inspiration](#inspiration)


## General info
Thus far there are two projects implemented:

1. TCP - implementation of simple chat in client-server-client architecture. This implementation has two major assumptions. One is usage of multithreading and the second one is implementation of some sort of graceful shutdown on client or server disconnection. TCP protocol is utlized in order to provide connection-oriented communication between sockets. This repository was review by [@peter-csala](https://github.com/peter-csala) on CodeReview stack exchange: [Multithreaded tcp server accepting two clients with task factory and graceful shutdown](https://codereview.stackexchange.com/questions/276755/multithreaded-tcp-server-accepting-two-clients-with-task-factory-and-graceful-sh)
2. UDP - implementation of simple chat in client-server-client architecture. This implementation has two major assumptions. One is usage of multithreading and the second one is implementation of some sort of graceful shutdown on client or server disconnection. UDP protocol is utilized in order to provide connection-less communication between sockets.  
3. Netstat - console application which run is an equivalent to "nestat -a" command on windows OS. 

## Screenshots
There will be more here, stay tuned!


## Technologies
* .NET 6
* Jetbrains Rider 


## Setup
I am making sure that `dotnet build` is all that you need to make everything work. 

## Code Examples
There will be more here, stay tuned!


## Features
There will be more here, stay tuned!


## To-do list:
* SMTP
* FTP
* Netstat
* Ping
* NTP
* RFC3091

## Status
Project is: _in progress_

## Inspiration
Network programming classes at AGH University of Science and Technology
