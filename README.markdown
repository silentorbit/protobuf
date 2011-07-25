# Protocol Buffers C# Code Generator BETA

http://silentorbit.com/protobuf-csharpgen/

Implementation of [Googles Protocol Buffers](http://code.google.com/apis/protocolbuffers/docs/overview.html) in C#.

Parses a .proto file and generates a single C# source file
with classes for every message as well as code for
reading and writing them to the Protocol Buffers binary format.

## Example

This is a part of the Test/Example.proto:

	message Person {
	  required string name = 1;
	  required int32 id = 2;
	  optional string email = 3;

	  enum PhoneType {
	    MOBILE = 0;
	    HOME = 1;
	    WORK = 2;
	  }

	  message PhoneNumber {
	    required string number = 1;
	    optional PhoneType type = 2 [default = HOME];
	  }

	  repeated PhoneNumber phone = 4;
	}

When compiled it you will have the following class to work with.

	public class Person : IPerson
	{
		public enum PhoneType
		{
			MOBILE = 0,
			HOME = 1,
			WORK = 2,
		}
	
		public string Name { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }
		public List<Person.IPhoneNumber> Phone { get; set; }
		
		...
		
		public class PhoneNumber : IPhoneNumber
		{
		
			public string Number { get; set; }
			public Person.PhoneType Type { get; set; }
			...

Writing this to a stream:

	Person.Write(stream, person1);

Person can be either of class Person or your own class implementing the interface IPerson.

Reading from a stream:

	IPerson person2 = Person.Read(stream);

## ALPHA

This is ALPHA, untested code.

Correctness of the written binary data or handling of messages has not been tested yet.

Check Test/Example.proto for the currently implemented features.

## Usage

    CodeGenerator.exe Example.proto ExampleNamespace Example.cs

The output is tree files.

 * Example.cs - Basic class declaration(based on .proto).
 * Example.Backend.cs - Enum and interface declarations. Code for reading/writing the message 
 * ProtocolParser.cs - Helper functions for the backend, constant, not related to the .proto specification.

If you generate code from multiple .proto files you only need to include one ProtocolParser.cs.

## Direct Contact, FeedBack, Bugs

You can contact me using phq@silentorbit.com.

Public issues can also be submitted to the GitHub project page.

# Licence

All source code and generated code is licensed under GPLv3, see COPYING.GPLv3 for details.
(Note that much of the generated code is a copy of the GPLv3 licenced code)

