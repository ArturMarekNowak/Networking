# Networking
![Build](https://github.com/ArturMarekNowak/Networking/actions/workflows/build.yml/badge.svg)
[![CodeFactor](https://www.codefactor.io/repository/github/arturmareknowak/networking/badge/master)](https://www.codefactor.io/repository/github/arturmareknowak/networking/overview/master)

This repository contains implementations of simple networking examples.

## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)
* [Status](#status)
* [Inspiration](#inspiration)


## General info
Thus far there are two projects implemented:

1. TCP - implementation of simple chat in client-server-client architecture. This implementation has two major assumptions. One is usage of multithreading and the second one is implementation of some sort of graceful shutdown on client or server disconnection. TCP protocol is utlized in order to provide connection-oriented communication between sockets. This repository was review by [@peter-csala](https://github.com/peter-csala) on CodeReview stack exchange: [Multithreaded tcp server accepting two clients with task factory and graceful shutdown](https://codereview.stackexchange.com/questions/276755/multithreaded-tcp-server-accepting-two-clients-with-task-factory-and-graceful-sh)
2. UDP - implementation of simple chat in client-server-client architecture. This implementation has two major assumptions. One is usage of multithreading and the second one is implementation of some sort of graceful shutdown on client or server disconnection. UDP protocol is utilized in order to provide connection-less communication between sockets.  
3. Netstat - console application which run is an equivalent to "nestat -a" command on windows OS. 
4. Ping - console application which run is an equivalent to "ping <address>" command on windows OS. 
5. NTP - implementation of simple NTP client. I was curious about the differences between the four times defined in [RFC 2030](https://datatracker.ietf.org/doc/html/rfc2030) which are reference, originate, receive and transmit timestamps and immediate call of *DateTime.Now*. This repository was review by [@peter-csala](https://github.com/peter-csala) (again :D) on CodeReview stack exchange: [NTP client displaying reference, originate, receive and transmit timestamps periodically with graceful shutdown in C#](https://codereview.stackexchange.com/questions/277266/ntp-client-displaying-reference-originate-receive-and-transmit-timestamps-peri)
6. SMTP - implementation of simple SMTP client allowing to add multiple attachments. Based on: [The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.5.1 Authentication Required?](https://stackoverflow.com/questions/18503333/the-smtp-server-requires-a-secure-connection-or-the-client-was-not-authenticated/25215834#25215834) 
7. RFC3091 - implementation of PI generator described in [RFC 3091](https://www.rfc-editor.org/rfc/rfc3091.html) document, but without the multicast service. 


## Technologies
* .NET 6
* Jetbrains Rider 


## Setup
I am making sure that `dotnet build` is all that you need to make everything work. 


## Status
Project is: _finished_


## Inspiration
Network programming classes at AGH University of Science and Technology
