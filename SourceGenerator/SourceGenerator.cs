using Microsoft.CodeAnalysis;

namespace SilentOrbit.ProtocolBuffers;

[Generator]
public class SourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUGx
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
        Debug.WriteLine("Initalize code generator");
    }

    public delegate void SaveSource(string filename, string source);

    public void Execute(GeneratorExecutionContext context)
    {
        Debug.WriteLine("Looking for files...");
        var protoFiles = context.AdditionalFiles.Where(a => a.Path.EndsWith(".proto")).Select(f => f.Path).ToList();

        foreach (var protoFile in protoFiles)
            Debug.WriteLine("Found " + protoFile);

        var parser = new FileParser();
        var collection = parser.Import(protoFiles);

        Console.WriteLine(collection);

        //Interpret and reformat
        try
        {
            var pp = new ProtoPrepare();
            pp.Prepare(collection);
        }
        catch (ProtoFormatException pfe)
        {
            Console.WriteLine();
            Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
            throw;
        }

        //Generate code
        var code = new ProtoCode(context);
        code.Save(collection);
    }

}
