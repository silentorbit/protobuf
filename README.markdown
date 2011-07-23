# Protocol Buffers C# Code Generator BETA

http://silentorbit.com/protobuf-csharpgen/

Implementation of [Googles Protocol Buffers](http://code.google.com/apis/protocolbuffers/docs/overview.html) in C#.

Parses a .proto file and generates a single C# source file
with classes for every message as well as code for
reading and writing them to the Protocol Buffers binary format.

## ALPHA

This is ALPHA, untested code.

Correctness of the written binary data or handling of messages has not been tested yet.

## Usage

    CodeGenerator.exe Example.proto ExampleNamespace Output.cs

Example.proto is the path to the .proto file.

ExampleNamespace is the namespace of the generated classes.

Output.cs is the path to where we write the generated C# code.

## Direct Contact, FeedBack, Bugs

You can contact me using phq@silentorbit.com.

Public issues can also be submitted to the GitHub project page.

# Licence

All source code and generated code is licensed under AGPLv3, see COPYING.AGPLv3 for details.
(Note that much of the generated code is a copy of the AGPLv3 licenced code)

