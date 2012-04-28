# Protocol Buffers C# Code Generator

https://silentorbit.com/protobuf/

Basic features of [Googles Protocol Buffers](http://code.google.com/apis/protocolbuffers/docs/overview.html) in C#.

Parses a .proto file and generates a single C# source file
with classes for every message as well as code for
reading and writing them to the Protocol Buffers binary format.

## Features

 * Binary that transform a .proto specification directly into complete c# code.
 * Generated code is relatively easy to debug(only hope you wont have too)
 * Generated code does not use reflection, works after code obfuscation.
 * Build-in serialization of DateTime and TimeSpan
## Example

This is a part of the Test/Example.proto:

	package ExampleNamespace;
	
	message Person {
	  option namespace = "Personal";
	  
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

	public partial class Person
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
		public List<Personal.Person.PhoneNumber> Phone { get; set; }
	
	
		public partial class PhoneNumber
		{
			public string Number { get; set; }
			public Personal.Person.PhoneType Type { get; set; }
		}
	}

Writing this to a stream:

	Person person = new Person();
	...
	Person.Serialize(stream, person);

Reading from a stream:

	Person person2 = Person.Deserialize(stream);

## Usage

    CodeGenerator.exe Example.proto [output.cs]

If the optional output.cs parameter is omitted it will default to the basename of the .proto file.
In this example it would be Example.cs

The output is three files.

 * Example.cs - Basic class declaration(based on .proto).
 * Example.Serializer.cs - Code for reading/writing the message.
 * ProtocolParser.cs - Functions for reading and writing the protobuf wire format, static, not related to the contents of your .proto.

If you generate code from multiple .proto files you must only include ProtocolParser.cs once in your project.

## Direct Contact, FeedBack, Bugs

You can contact me using phq@silentorbit.com .

Public issues can also be submitted to the GitHub project page.

# Licence, Apache License version 2.0

All source code and generated code is licensed under the Apache License Version 2.0.

