

# TestProgram, Custom Commands

These commands are executed when compiling TestProgram.exe

## Debug

    ../bin/CodeGenerator/Debug/CodeGenerator.exe --fix-nameclash ProtoSpec/ImportAll.proto --output Generated/Generated.cs --ctor --utc

    ../bin/CodeGenerator/Debug/CodeGenerator.exe --fix-nameclash ProtoSpec/ProtoFeatures.nullable.proto --output Generated/Nullables.cs --ctor --nullable --utc

## Release

    ../bin/CodeGenerator/Release/CodeGenerator.exe --fix-nameclash ProtoSpec/ImportAll.proto --output Generated/Generated.cs --ctor

# CodeGenerator, Run

These are the arguments to CodeGenerator when run from within the IDE

--fix-nameclash ../../../TestProgram/ProtoSpec/ImportAll.proto --output ../../../TestProgram/Generated/Generated.cs --ctor --utc
