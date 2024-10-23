using System.Reflection;

public class UnrealTraspiler : AbstractTranspiler
{
    public static void Generate(string projectName)
    {
        var contracts = GetContracts();

        string projectDirectory = GetProjectDirectory();
        string sharedDirectoryPath = Path.Combine(projectDirectory, "Shared");

        List<string> serverPackets = new List<string>();
        List<string> clientPackets = new List<string>();
        List<string> multiplexPackets = new List<string>();

        foreach (var contract in contracts)
        {
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attribute = contract.GetCustomAttribute<ContractAttribute>();
            var contractName = contract.Name;

            string headerPath = Path.Combine(sharedDirectoryPath, projectName, "Public", "Packets");
            string cppPath = Path.Combine(sharedDirectoryPath, projectName, "Private", "Packets");

            if (!Directory.Exists(headerPath))
                Directory.CreateDirectory(headerPath);

            if (!Directory.Exists(cppPath))
                Directory.CreateDirectory(cppPath);

            string headerFilePath = Path.Combine(sharedDirectoryPath, projectName, "Public", "Packets", $"{contractName}Packet.h");
            string cppFilePath = Path.Combine(sharedDirectoryPath, projectName, "Private", "Packets", $"{contractName}Packet.cpp");

            using (var writer = new StreamWriter(headerFilePath))
            {
                GenerateHeaderFile(writer, contract, fields);
            }

            using (var writer = new StreamWriter(cppFilePath))
            {
                GenerateCppFile(writer, contract, fields);
            }

            switch (attribute?.Type)
            {
                case PacketType.Client: clientPackets.Add(contract.Name); break;
                case PacketType.Server: serverPackets.Add(contract.Name); break;
                case PacketType.Multiplex: multiplexPackets.Add(contract.Name); break;
            }
        }

        GenerateEnumHeader("EClientPacket", clientPackets, projectName, sharedDirectoryPath);
        GenerateEnumHeader("EServerPacket", serverPackets, projectName, sharedDirectoryPath);
        GenerateEnumHeader("EMultiplexPacket", multiplexPackets, projectName, sharedDirectoryPath);
        CopySubsystem(projectName, sharedDirectoryPath);
    }

    private static void GenerateHeaderFile(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        var contractName = contract.Name;

        writer.WriteLine("#pragma once");
        writer.WriteLine();
        writer.WriteLine("#include \"CoreMinimal.h\"");
        writer.WriteLine("#include \"ByteBuffer.h\"");
        writer.WriteLine($"#include \"Packets/{contractName}.generated.h\"");
        writer.WriteLine();
        writer.WriteLine("USTRUCT(BlueprintType)");
        writer.WriteLine($"struct F{contractName}");
        writer.WriteLine("{");
        writer.WriteLine("    GENERATED_USTRUCT_BODY();");
        writer.WriteLine();

        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ContractFieldAttribute>();
            if (attribute != null)
            {
                var fieldType = ConvertToUnrealType(attribute.Type);
                writer.WriteLine($"    UPROPERTY(EditAnywhere, BlueprintReadWrite)");
                writer.WriteLine($"    {fieldType} {field.Name};");
                writer.WriteLine();
            }
        }

        writer.WriteLine("};");
        writer.WriteLine();
        writer.WriteLine("// Classe de função para serializar e desserializar a struct F" + contractName);
        writer.WriteLine("UCLASS()");
        writer.WriteLine($"class U{contractName}Library : public UBlueprintFunctionLibrary");
        writer.WriteLine("{");
        writer.WriteLine("    GENERATED_BODY()");
        writer.WriteLine();
        writer.WriteLine("public:");

        writer.WriteLine($"    UFUNCTION(BlueprintCallable, Category = \"{contractName}Serialization\")");
        writer.WriteLine($"    static F{contractName} {contractName}Deserialize(UByteBuffer* Buffer);");
        writer.WriteLine();
        writer.WriteLine($"    UFUNCTION(BlueprintCallable, Category = \"{contractName}Serialization\")");
        writer.WriteLine($"    static UByteBuffer* {contractName}Serialize(const F{contractName}& Data);");

        writer.WriteLine("};");
    }

    private static void GenerateCppFile(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        var contractName = contract.Name;

        writer.WriteLine("#include \"ByteBuffer.h\"");
        writer.WriteLine($"#include \"Packets/{contractName}.h\"");
        writer.WriteLine();
        writer.WriteLine($"F{contractName} U{contractName}Library::{contractName}Deserialize(UByteBuffer* Buffer)");
        writer.WriteLine("{");
        writer.WriteLine($"    F{contractName} Data;");
        writer.WriteLine("    if (!Buffer) return Data;");
        writer.WriteLine();

        // Geração da leitura dos campos do buffer
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ContractFieldAttribute>();
            if (attribute != null)
            {
                var fieldType = attribute.Type;
                var fieldName = field.Name;

                switch (fieldType)
                {
                    case "int32":
                    case "integer":
                        writer.WriteLine($"    Data.{fieldName} = Buffer->GetInt32();");
                        break;
                    case "float":
                        writer.WriteLine($"    Data.{fieldName} = Buffer->GetFloat();");
                        break;
                    case "string":
                    case "str":
                        writer.WriteLine($"    Data.{fieldName} = Buffer->GetString();");
                        break;
                    case "bool":
                        writer.WriteLine($"    Data.{fieldName} = Buffer->GetBool();");
                        break;
                    case "byte":
                        writer.WriteLine($"    Data.{fieldName} = Buffer->GetByte();");
                        break;
                    default:
                        writer.WriteLine($"    // Unsupported type: {fieldType}");
                        break;
                }
            }
        }

        writer.WriteLine("    return Data;");
        writer.WriteLine("}");
        writer.WriteLine();
        writer.WriteLine($"UByteBuffer* U{contractName}Library::{contractName}Serialize(const F{contractName}& Data)");
        writer.WriteLine("{");
        writer.WriteLine("    UByteBuffer* Buffer = UByteBuffer::CreateEmptyByteBuffer();");
        writer.WriteLine("    if (!Buffer) return nullptr;");
        writer.WriteLine();

        // Geração da escrita dos campos no buffer
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ContractFieldAttribute>();
            if (attribute != null)
            {
                var fieldType = attribute.Type;
                var fieldName = field.Name;

                switch (fieldType)
                {
                    case "int32":
                    case "integer":
                        writer.WriteLine($"    Buffer->PutInt32(Data.{fieldName});");
                        break;
                    case "float":
                        writer.WriteLine($"    Buffer->PutFloat(Data.{fieldName});");
                        break;
                    case "string":
                    case "str":
                        writer.WriteLine($"    Buffer->PutString(Data.{fieldName});");
                        break;
                    case "bool":
                        writer.WriteLine($"    Buffer->PutBool(Data.{fieldName});");
                        break;
                    case "byte":
                        writer.WriteLine($"    Buffer->PutByte(Data.{fieldName});");
                        break;
                    default:
                        writer.WriteLine($"    // Unsupported type: {fieldType}");
                        break;
                }
            }
        }

        writer.WriteLine("    return Buffer;");
        writer.WriteLine("}");
    }

    private static void GenerateEnumHeader(string enumName, List<string> values, string projectName, string sharedDirectoryPath)
    {
        string directoryPath = Path.Combine(sharedDirectoryPath, projectName, "Public", "Enums");
        string filePath = Path.Combine(directoryPath, $"{enumName}.h");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        if (values.Count > 0)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("#pragma once");
                writer.WriteLine();
                writer.WriteLine($"UENUM(BlueprintType)");
                writer.WriteLine($"enum class {enumName} : uint8");
                writer.WriteLine("{");

                for (int i = 0; i < values.Count; i++)
                {
                    string value = values[i];

                    if (i == values.Count - 1)
                        writer.WriteLine($"    {value} UMETA(DisplayName = \"{value}\")");
                    else
                        writer.WriteLine($"    {value} UMETA(DisplayName = \"{value}\"),");
                }

                writer.WriteLine("};");
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private static string ConvertToUnrealType(string fieldType)
    {
        return fieldType switch
        {
            "int32" => "int32",
            "int" => "int32",
            "integer" => "int32",
            "float" => "float",
            "str" => "FString",
            "string" => "FString",
            "byte" => "byte",
            "bool" => "bool",
            _ => "UnsupportedType"
        };
    }

    private static void CopySubsystem(string projectName, string sharedDirectoryPath)
    {
        string projectDirectory = GetProjectDirectory();

     
        string headerFilePath = Path.Combine(projectDirectory, "Core", "Network", "ServerSubsystem.h");
        string cppFilePath = Path.Combine(projectDirectory, "Core", "Network", "ServerSubsystem.cpp");

        string headerFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Public", "ServerSubsystem.h");
        string cppFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Private", "ServerSubsystem.cpp");

        if (File.Exists(headerFilePath))
        {
            string headerContent = File.ReadAllText(headerFilePath);
            headerContent = headerContent.Replace("CLIENT_API", $"{projectName}_API");

            string headerDirectory = Path.GetDirectoryName(headerFilePathClient);

            if (!Directory.Exists(headerDirectory))            
                Directory.CreateDirectory(headerDirectory);
            
            File.WriteAllText(headerFilePathClient, headerContent);
        }
        else
        {
            Console.WriteLine($"Header file not found: {headerFilePath}");
        }

        if (File.Exists(cppFilePath))
        {
            string cppContent = File.ReadAllText(cppFilePath);
            string cppDirectory = Path.GetDirectoryName(cppFilePathClient);

            if (!Directory.Exists(cppDirectory))            
                Directory.CreateDirectory(cppDirectory);
            
            File.WriteAllText(cppFilePathClient, cppContent);
        }
        else
        {
            Console.WriteLine($"CPP file not found: {cppFilePath}");
        }
    }
}