# Protocol Buffers C# Code Generator BETA

http://silentorbit.com/protobuf-csharpgen/

Implementation of [Googles Protocol Buffers](http://code.google.com/apis/protocolbuffers/docs/overview.html) in C#.

Parses a .proto file and generates a single C# source file
with classes for every message as well as code for
reading and writing them to the Protocol Buffers binary format.

## Example

This is a part of the Person.proto found in the Examples directory.

	message Person {
	  required string name = 1;
	  required int32 id = 2;
	  optional string email = 3;
	
	  repeated PhoneNumber phone = 4;
	}

When compiled it you will have the following class to work with.

	public class Person : IPerson
	{
		public string Name { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }
		public IList<IPhoneNumber> Phone { get; set; }
		..

Writing this to a stream:

	Person.Write(stream, person1);

Reading from a stream:

	IPerson person2 = Person.Read(stream);

## ALPHA

This is ALPHA, untested code.

Correctness of the written binary data or handling of messages has not been tested yet.

Check Test/Example.proto for the currently implemented features.
For example the current version support nested messages but not nested message specifications in the .proto file.

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

