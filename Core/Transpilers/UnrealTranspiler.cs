/*
 * UnrealTraspiler
 * 
 * Author: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *    
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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

        clientPackets.AddRange(multiplexPackets);
        serverPackets.AddRange(multiplexPackets);

        GenerateEnumHeader("ClientPacket", clientPackets, projectName, sharedDirectoryPath);
        GenerateEnumHeader("ServerPacket", serverPackets, projectName, sharedDirectoryPath);
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
            writer.WriteLine($"struct F{contractName}Recive");
            writer.WriteLine("{");
            writer.WriteLine("    GENERATED_USTRUCT_BODY();");
            writer.WriteLine();

            foreach (var field in fields)
            {
                var attributeField = field.GetCustomAttribute<ContractFieldAttribute>();
                if (attributeField != null && (attributeField.ReplyType == FieldReplyType.ServerOnly || attributeField.ReplyType == FieldReplyType.Mutiplex))
                {
                    var fieldType = ConvertToUnrealType(attributeField.Type);
                    writer.WriteLine($"    UPROPERTY(EditAnywhere, BlueprintReadWrite)");
                    writer.WriteLine($"    {fieldType} {field.Name};");
                    writer.WriteLine();
                }
            }

            writer.WriteLine("};");

            writer.WriteLine();
            writer.WriteLine("USTRUCT(BlueprintType)");
            writer.WriteLine($"struct F{contractName}Send");
            writer.WriteLine("{");
            writer.WriteLine("    GENERATED_USTRUCT_BODY();");
            writer.WriteLine();

            foreach (var field in fields)
            {
                var attributeField = field.GetCustomAttribute<ContractFieldAttribute>();
                if (attributeField != null && (attributeField.ReplyType == FieldReplyType.ClientOnly || attributeField.ReplyType == FieldReplyType.Mutiplex))
                {
                    var fieldType = ConvertToUnrealType(attributeField.Type);
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
            writer.WriteLine();
            writer.WriteLine($"    UFUNCTION(BlueprintCallable, Category = \"{contractName}Serialization\")");
            writer.WriteLine($"    static F{contractName}Recive {contractName}Deserialize(UByteBuffer* Buffer);");
            writer.WriteLine();
            writer.WriteLine($"    UFUNCTION(BlueprintCallable, Category = \"{contractName}Serialization\")");
            writer.WriteLine($"    static UByteBuffer* {contractName}Serialize(const F{contractName}Send& Data);");

            /*var attribute = contract.GetCustomAttribute<ContractAttribute>();
            if (attribute.Type == PacketType.Client || attribute.Type == PacketType.Multiplex)
            {
                writer.WriteLine();
                writer.WriteLine($"    UFUNCTION(BlueprintCallable, Category = \"{contractName}Communication\")");
                writer.WriteLine($"    void Send{contractName}(const F{contractName}& Data);");
            }*/

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
            writer.WriteLine($"    F{contractName}Recive Data = F{contractName}Recive();");
            writer.WriteLine("    if (!Buffer) return Data;");
            writer.WriteLine();

            foreach (var field in fields)
            {
                var attributeField = field.GetCustomAttribute<ContractFieldAttribute>();
                if (attributeField != null && (attributeField.ReplyType == FieldReplyType.ServerOnly || attributeField.ReplyType == FieldReplyType.Mutiplex))
                {
                    var fieldType = attributeField.Type;
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
            writer.WriteLine($"UByteBuffer* U{contractName}Library::{contractName}Serialize(const F{contractName}Send& Data)");
            writer.WriteLine("{");
            writer.WriteLine("    UByteBuffer* Buffer = UByteBuffer::CreateEmptyByteBuffer();");
            writer.WriteLine("    if (!Buffer) return nullptr;");
            writer.WriteLine();

            foreach (var field in fields)
            {
                var attributeField = field.GetCustomAttribute<ContractFieldAttribute>();
                if (attributeField != null && (attributeField.ReplyType == FieldReplyType.ClientOnly || attributeField.ReplyType == FieldReplyType.Mutiplex))
                {
                    var fieldType = attributeField.Type;
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

            /*var attribute = contract.GetCustomAttribute<ContractAttribute>();
            if (attribute.Type == PacketType.Client || attribute.Type == PacketType.Multiplex)
            {
                writer.WriteLine();
                writer.WriteLine($"void U{contractName}Library::Send{contractName}(const F{contractName}& Data)");
                writer.WriteLine("{");
                writer.WriteLine("    if (!UServerSubsystem::GetServerSubsystem(nullptr) || !UServerSubsystem::GetServerSubsystem(nullptr)->UDPInstance) return;");
                writer.WriteLine();
                writer.WriteLine($"    UByteBuffer* Buffer = U{contractName}Library::{contractName}Serialize(Data);");
                writer.WriteLine("    if (!Buffer) return;");
                writer.WriteLine();
                writer.WriteLine($"    UServerSubsystem::GetServerSubsystem(nullptr)->UDPInstance->SendMessage(static_cast<uint8>(PacketType::{contractName}), Buffer);");
                writer.WriteLine("}");
            }*/
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
        string[] files = { "Base36", "ByteBuffer", "Encryption", "ServerSubsystem", "Websocket", "UDP" };

        CopyFilesFromDirectories(files, projectDirectory, sharedDirectoryPath, projectName);

        string enumsDirectory = Path.Combine(projectDirectory, "Unreal", "Enums");
        string flagsDirectory = Path.Combine(projectDirectory, "Unreal", "Flags");

        string enumsPublicDirectory = Path.Combine(sharedDirectoryPath, projectName, "Public", "Enums");
        string enumsPrivateDirectory = Path.Combine(sharedDirectoryPath, projectName, "Private", "Enums");
        string flagsPublicDirectory = Path.Combine(sharedDirectoryPath, projectName, "Public", "Flags");
        string flagsPrivateDirectory = Path.Combine(sharedDirectoryPath, projectName, "Private", "Flags");

        CopyFilesFromSubDirectory(enumsDirectory, enumsPublicDirectory, enumsPrivateDirectory, "Enums");
        CopyFilesFromSubDirectory(flagsDirectory, flagsPublicDirectory, flagsPrivateDirectory, "Flags");
    }

    private static void CopyFilesFromDirectories(string[] files, string projectDirectory, string sharedDirectoryPath, string projectName)
    {
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            string headerFilePath = Path.Combine(projectDirectory, "Unreal", file + ".h");
            string cppFilePath = Path.Combine(projectDirectory, "Unreal", file + ".cpp");
            string headerFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Public", file + ".h");
            string cppFilePathClient = Path.Combine(sharedDirectoryPath, projectName, "Private", file + ".cpp");

            if (File.Exists(headerFilePath))
            {
                string headerContent = File.ReadAllText(headerFilePath);
                headerContent = headerContent.Replace("CLIENT_API", $"{projectName.ToUpper()}_API");

                string headerDirectory = Path.GetDirectoryName(headerFilePathClient);

                if (!Directory.Exists(headerDirectory))
                    Directory.CreateDirectory(headerDirectory);

                if (file == "ServerSubsystem")
                {
                    headerContent = headerContent.Replace("//%INCLUDES%", GenerateIncludes());
                    headerContent = headerContent.Replace("//%DELEGATES%", GenerateDelegates());
                    headerContent = headerContent.Replace("//%EVENTS%", GenerateEvents());
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

                if (file == "ServerSubsystem")
                {
                    cppContent = cppContent.Replace("//%INCLUDES%", GenerateIncludes());
                    cppContent = cppContent.Replace("//%FUNCTIONS%", GenerateSendFunctions());
                    cppContent = cppContent.Replace("//%PARSEDDATASWITCH%", GenerateParsedDataSwitch());
                }

                File.WriteAllText(cppFilePathClient, cppContent);
            }
            else
            {
                Console.WriteLine($"CPP file not found: {cppFilePath}");
            }
        }
    }

    private static void CopyFilesFromSubDirectory(string sourceDirectory, string targetPublicDirectory, string targetPrivateDirectory, string diretoryName)
    {
        if (Directory.Exists(sourceDirectory))
        {
            var headerFiles = Directory.GetFiles(sourceDirectory, "*.h", SearchOption.AllDirectories);

            foreach (var headerFile in headerFiles)
            {
                string relativePath = Path.GetRelativePath(sourceDirectory, headerFile);
                string destinationPath = Path.Combine(targetPublicDirectory, relativePath);

                string destinationDirectory = Path.GetDirectoryName(destinationPath);

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                string content = File.ReadAllText(headerFile);
                File.WriteAllText(destinationPath, content);

                Console.WriteLine($"Header file copied: {headerFile} -> {destinationPath}");
            }

            var cppFiles = Directory.GetFiles(sourceDirectory, "*.cpp", SearchOption.AllDirectories);

            foreach (var cppFile in cppFiles)
            {
                string relativePath = Path.GetRelativePath(sourceDirectory, cppFile);
                string destinationPath = Path.Combine(targetPrivateDirectory, relativePath);

                string destinationDirectory = Path.GetDirectoryName(destinationPath);

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                string content = File.ReadAllText(cppFile);
                string includeDirective = Path.GetFileNameWithoutExtension(cppFile) + ".h";
                string includePath = Path.Combine(diretoryName, relativePath.Replace(".cpp", ".h")).Replace("\\", "/");

                content = content.Replace($"#include \"{includeDirective}\"", $"#include \"{includePath}\"");

                File.WriteAllText(destinationPath, content);

                Console.WriteLine($"CPP file copied: {cppFile} -> {destinationPath}");
            }
        }
        else
        {
            Console.WriteLine($"Directory not found: {sourceDirectory}");
        }
    }

    private static string GenerateSendFunctions()
    {
        var clientPackets = GetClientPackets();
        StringBuilder result = new StringBuilder();

        foreach (var packetName in clientPackets)
        {
            var contract = GetContractByName(packetName);
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length > 0)
            {
                result.AppendLine($"void UServerSubsystem::Send{packetName.Replace("DTO", "")}(const F{packetName.Replace("DTO", "")}& Data)");
                result.AppendLine("{");
                result.AppendLine($"    UByteBuffer* Buffer = U{packetName.Replace("DTO", "")}Library::{packetName.Replace("DTO", "")}Serialize(Data);");
                result.AppendLine();
                result.AppendLine("    if (UDPInstance) {");
                result.AppendLine($"        uint8 PacketId = static_cast<uint8>(EClientPacket::{packetName.Replace("DTO", "")});");
                result.AppendLine($"        UDPInstance->SendEncryptedMessage(PacketId, Buffer, ECCPublicKey);");
                result.AppendLine("    }");
                result.AppendLine("}");
                result.AppendLine();
            }
            else
            {
                result.AppendLine($"void UServerSubsystem::Send{packetName.Replace("DTO", "")}()");
                result.AppendLine("{");
                result.AppendLine($"    UByteBuffer* Buffer = UByteBuffer::CreateEmptyByteBuffer();");
                result.AppendLine();
                result.AppendLine("    if (UDPInstance) {");
                result.AppendLine($"        uint8 PacketId = static_cast<uint8>(EClientPacket::{packetName.Replace("DTO", "")});");
                result.AppendLine($"        UDPInstance->SendEncryptedMessage(PacketId, Buffer, ECCPublicKey);");
                result.AppendLine("    }");
                result.AppendLine("}");
                result.AppendLine();
            }
        }

        return result.ToString();
    }

    private static string GenerateIncludes()
    {
        var serverPackets = GetServerPackets();
        var clientPackets = GetClientPackets();
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets.Concat(clientPackets))
        {
            result.AppendLine($"#include \"Packets/{packet.Replace("DTO", "")}Packet.h\"");
        }

        return result.ToString();
    }

    private static string GenerateDelegates()
    {
        var serverPackets = GetServerPackets();
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets)
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
        StringBuilder result = new StringBuilder();

        foreach (var packet in serverPackets)
        {
            result.AppendLine($"    UPROPERTY(BlueprintAssignable, meta = (DisplayName = \"On{packet.Replace("DTO", "")}\", Keywords = \"Server Events\"), Category = \"ServerSubsystem\")");
            result.AppendLine($"    F{packet.Replace("DTO", "")}Handler On{packet.Replace("DTO", "")};");
            result.AppendLine();
        }

        foreach (var packet in clientPackets)
        {
            var contract = GetContractByName(packet);
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length > 0)
            {
                result.AppendLine($"    UFUNCTION(BlueprintCallable, Category = \"ServerSubsystem\")");
                result.AppendLine($"    void Send{packet.Replace("DTO", "")}(const F{packet.Replace("DTO", "")}& Data);");
                result.AppendLine();
            }
            else
            {
                result.AppendLine($"    UFUNCTION(BlueprintCallable, Category = \"ServerSubsystem\")");
                result.AppendLine($"    void Send{packet.Replace("DTO", "")}();");
                result.AppendLine();
            }
        }

        return result.ToString();
    }

    private static string GenerateParsedDataSwitch()
    {
        StringBuilder switchBuilder = new StringBuilder();
        var serverPackets = GetServerPackets();

        foreach (var packet in serverPackets)
        {
            var contract = GetContractByName(packet);
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);

            switchBuilder.AppendLine($"            case EServerPacket::{packet.Replace("DTO", "")}:");
            switchBuilder.AppendLine("            {");

            if (fields.Length == 1)
            {
                var fieldType = ConvertToUnrealType(fields[0].FieldType.Name);
                var fieldName = fields[0].Name;

                switchBuilder.AppendLine($"                auto Data = U{packet.Replace("DTO", "")}Library::{packet.Replace("DTO", "")}Deserialize(ReceivedBuffer);");
                switchBuilder.AppendLine($"                On{packet.Replace("DTO", "")}.Broadcast(Data.{fieldName});");
            }
            else if (fields.Length == 0)
            {
                switchBuilder.AppendLine($"                On{packet.Replace("DTO", "")}.Broadcast();");
            }
            else if (fields.Length < 6)
            {
                switchBuilder.AppendLine($"                auto Data = U{packet.Replace("DTO", "")}Library::{packet.Replace("DTO", "")}Deserialize(ReceivedBuffer);");
                var parameters = new List<string>();

                foreach (var field in fields)
                {
                    var fieldType = ConvertToUnrealType(field.FieldType.Name);
                    var fieldName = field.Name;
                    parameters.Add($"Data.{fieldName}");
                }

                switchBuilder.AppendLine($"                On{packet.Replace("DTO", "")}.Broadcast({string.Join(", ", parameters)});");
            }
            else
            {
                switchBuilder.AppendLine($"                auto Data = U{packet.Replace("DTO", "")}Library::{packet.Replace("DTO", "")}Deserialize(ReceivedBuffer);");
                switchBuilder.AppendLine($"                On{packet.Replace("DTO", "")}.Broadcast(Data);");
            }

            switchBuilder.AppendLine("                break;");
            switchBuilder.AppendLine("            }");
        }

        return switchBuilder.ToString();
    }
}