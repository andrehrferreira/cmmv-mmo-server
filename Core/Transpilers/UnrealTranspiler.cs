using System.Reflection;
using System.Text;

public class UnrealTraspiler : AbstractTranspiler
{
    private static List<string> clientPackets = new List<string>();
    private static List<string> serverPackets = new List<string>();
    private static List<string> multiplexPackets = new List<string>();

    public static void Generate(string projectName)
    {
        var contracts = GetContracts();

        string projectDirectory = GetProjectDirectory();
        string sharedDirectoryPath = Path.Combine(projectDirectory, "Shared");

        foreach (var contract in contracts)
        {
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attribute = contract.GetCustomAttribute<ContractAttribute>();
            var contractName = contract.Name.Replace("DTO", "");

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

        GenerateEnumHeader("ClientPacket", clientPackets, projectName, sharedDirectoryPath);
        GenerateEnumHeader("ServerPacket", serverPackets, projectName, sharedDirectoryPath);
        GenerateEnumHeader("MultiplexPacket", multiplexPackets, projectName, sharedDirectoryPath);
        CopySubsystem(projectName, sharedDirectoryPath);
    }

    private static List<string> GetClientPackets()
    {
        return clientPackets;
    }

    private static List<string> GetServerPackets()
    {
        return serverPackets;
    }

    private static List<string> GetMultiplexPackets()
    {
        return multiplexPackets;
    }

    private static Type GetContractByName(string name)
    {
        var contracts = GetContracts();
        return contracts.FirstOrDefault(c => c.Name == name);
    }

    private static void GenerateHeaderFile(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        if (fields.Length > 0)
        {
            var contractName = contract.Name.Replace("DTO", "");

            writer.WriteLine("// This file was generated automatically, please do not change it.");
            writer.WriteLine();
            writer.WriteLine("#pragma once");
            writer.WriteLine();
            writer.WriteLine("#include \"CoreMinimal.h\"");
            writer.WriteLine("#include \"ByteBuffer.h\"");
            writer.WriteLine($"#include \"{contractName}Packet.generated.h\"");
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
            writer.WriteLine("// Function class to serialize and deserialize struct F" + contractName);
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
    }

    private static void GenerateCppFile(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        var contractName = contract.Name.Replace("DTO", "");

        if(fields.Length > 0)
        {
            writer.WriteLine("// This file was generated automatically, please do not change it.");
            writer.WriteLine();
            writer.WriteLine($"#include \"Packets/{contractName}Packet.h\"");
            writer.WriteLine("#include \"ByteBuffer.h\"");
            writer.WriteLine();
            writer.WriteLine($"F{contractName} U{contractName}Library::{contractName}Deserialize(UByteBuffer* Buffer)");
            writer.WriteLine("{");
            writer.WriteLine($"    F{contractName} Data = F{contractName}();");
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
                        case "int":
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
                        case "id":
                            writer.WriteLine($"    Data.{fieldName} = Buffer->GetId();");
                            break;
                        case "vector3":
                            writer.WriteLine($"    Data.{fieldName} = Buffer->GetVector();");
                            break;
                        case "rotator":
                            writer.WriteLine($"    Data.{fieldName} = Buffer->GetRotator();");
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
                        case "int":
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
                        case "id":
                            writer.WriteLine($"    Buffer->PutId(Data.{fieldName});");
                            break;
                        case "vector3":
                            writer.WriteLine($"    Buffer->PutVector(Data.{fieldName});");
                            break;
                        case "rotator":
                            writer.WriteLine($"    Buffer->PutRotator(Data.{fieldName});");
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
    }

    private static void GenerateEnumHeader(string enumName, List<string> values, string projectName, string sharedDirectoryPath)
    {
        string directoryPath = Path.Combine(sharedDirectoryPath, projectName, "Public", "Enums");
        string filePath = Path.Combine(directoryPath, $"{enumName.Replace("DTO", "")}.h");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        if (values.Count > 0)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("#pragma once");
                writer.WriteLine();
                writer.WriteLine($"UENUM(BlueprintType)");
                writer.WriteLine($"enum class E{enumName} : uint8");
                writer.WriteLine("{");

                for (int i = 0; i < values.Count; i++)
                {
                    string value = values[i];

                    if (i == values.Count - 1)
                        writer.WriteLine($"    {value.Replace("DTO", "")} UMETA(DisplayName = \"{value.Replace("DTO", "")}\")");
                    else
                        writer.WriteLine($"    {value.Replace("DTO", "")} UMETA(DisplayName = \"{value.Replace("DTO", "")}\"),");
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
        return fieldType.ToLower() switch
        {
            "int32" => "int32",
            "int" => "int32",
            "integer" => "int32",
            "float" => "float",
            "str" => "FString",
            "string" => "FString",
            "id" => "FString",
            "byte" => "uint8",
            "boolean" => "bool",
            "bool" => "bool",
            "vector3" => "FVector",
            "rotator" => "FRotator",
            "buffer" => "TArray<uint8>&",
            _ => "UnsupportedType"
        };
    }

    private static string GetParamCountName(int count)
    {
        return count switch
        {
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five"
        };
    }

    private static void CopySubsystem(string projectName, string sharedDirectoryPath)
    {
        string projectDirectory = GetProjectDirectory();             
        string[] files = ["Base36", "ByteBuffer", "Encryption", "QueueBuffer", "ServerSubsystem", "Websocket"];

        for(int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            string headerFilePath = Path.Combine(projectDirectory, "Unreal", file + ".h");
            string cppFilePath = Path.Combine(projectDirectory, "Unreal", file +  ".cpp");
            string headerFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Public", file + ".h");
            string cppFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Private", file +  ".cpp");

            if (File.Exists(headerFilePath))
            {
                string headerContent = File.ReadAllText(headerFilePath);
                headerContent = headerContent.Replace("CLIENT_API", $"{projectName.ToUpper()}_API");

                string headerDirectory = Path.GetDirectoryName(headerFilePathClient);

                if (!Directory.Exists(headerDirectory))
                    Directory.CreateDirectory(headerDirectory);

                if(file == "ServerSubsystem")
                {
                    headerContent = headerContent.Replace("%INCLUDES%", GenerateIncludes());
                    headerContent = headerContent.Replace("%DELEGATES%", GenerateDelegates());
                    headerContent = headerContent.Replace("%EVENTS%", GenerateEvents());
                }

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

    private static string GenerateIncludes()
    {
        var serverPackets = GetServerPackets();
        var multiplexPackets = GetMultiplexPackets(); 
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets.Concat(multiplexPackets))
        {
            result.AppendLine($"#include \"Packets/{packet.Replace("DTO", "")}Packet.h\"");
        }

        return result.ToString();
    }

    private static string GenerateDelegates()
    {
        var serverPackets = GetServerPackets();
        var multiplexPackets = GetMultiplexPackets();
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets.Concat(multiplexPackets))
        {
            var contract = GetContractByName(packet);
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length < 6 && fields.Length > 1)
            {
                string paramCountName = GetParamCountName(fields.Length);
                result.Append($"    DECLARE_DYNAMIC_MULTICAST_DELEGATE_{paramCountName}Params(F{packet.Replace("DTO", "")}Handler, ");

                for (int i = 0; i < fields.Length; i++)
                {
                    var fieldType = ConvertToUnrealType(fields[i].FieldType.Name);
                    result.Append($"{fieldType}, {fields[i].Name}");

                    if (i < fields.Length - 1)
                        result.Append(", ");
                }

                result.AppendLine(");");
            }
            else if(fields.Length == 1)
            {
                var fieldType = ConvertToUnrealType(fields[0].FieldType.Name);
                result.AppendLine($"    DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(F{packet.Replace("DTO", "")}Handler, {fieldType}, {fields[0].Name});");
            }
            else if(fields.Length == 0)
            {
                result.AppendLine($"    DECLARE_DYNAMIC_MULTICAST_DELEGATE(F{packet.Replace("DTO", "")}Handler);");
            }
            else
            {
                result.AppendLine($"    DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(F{packet.Replace("DTO", "")}Handler, F{packet.Replace("DTO", "")}, Data);");
            }
        }

        return result.ToString();
    }

    public static string GenerateEvents()
    {
        var serverPackets = GetServerPackets();
        var multiplexPackets = GetMultiplexPackets(); 
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets.Concat(multiplexPackets))
        {
            result.AppendLine($"    UPROPERTY(BlueprintAssignable, meta = (DisplayName = \"On{packet.Replace("DTO", "")}\", Keywords = \"Server Events\"), Category = \"ServerSubsystem\")");
            result.AppendLine($"    F{packet.Replace("DTO", "")}Handler On{packet.Replace("DTO", "")};");
            result.AppendLine();
        }

        return result.ToString();
    }
}