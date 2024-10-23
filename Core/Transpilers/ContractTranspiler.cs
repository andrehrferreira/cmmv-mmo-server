using System.Reflection;

public class ContractTraspiler: AbstractTranspiler
{
    public static void Generate()
    {
        var contracts = GetContracts();
        
        string projectDirectory = GetProjectDirectory();
        string baseDirectoryPath = Path.Combine(projectDirectory, "Core", "Packets");
        string networkDirectoryPath = Path.Combine(projectDirectory, "Core", "Network");

        List<string> serverPackets = new List<string>();
        List<string> clientPackets = new List<string>();
        List<string> multiplexPackets = new List<string>();

        if (!Directory.Exists(baseDirectoryPath))
            Directory.CreateDirectory(baseDirectoryPath);

        if (!Directory.Exists(networkDirectoryPath))
            Directory.CreateDirectory(networkDirectoryPath);

        foreach (var contract in contracts)
        {
            var fields = contract.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var attribute = contract.GetCustomAttribute<ContractAttribute>();
            var contractName = contract.Name;

            string filePath = Path.Combine(baseDirectoryPath, $"{contractName}Packet.cs");

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// This file was generated automatically, please do not change it.");
                writer.WriteLine();
                writer.WriteLine("using System.Runtime.CompilerServices;");
                writer.WriteLine();

                writer.WriteLine($"public struct {contractName}Packet");
                writer.WriteLine("{");                

                switch (attribute?.Type)
                {
                    case PacketType.Client: 
                        writer.WriteLine($"    public ClientPacket Type = ClientPacket.{contractName};");
                        clientPackets.Add(contract.Name);
                        break;
                    case PacketType.Server: 
                        writer.WriteLine($"    public ServerPacket Type = ServerPacket.{contractName};");
                        serverPackets.Add(contract.Name);
                        break;
                    case PacketType.Multiplex: //Client / Server
                        writer.WriteLine($"    public MultiplexPacket Type = MultiplexPacket.{contractName};");
                        multiplexPackets.Add(contract.Name);
                        break;
                }

                writer.WriteLine("");
                writer.WriteLine($"    public {contractName}Packet() {{ }}");
                writer.WriteLine("");

                GenerateWriteMethod(writer, contract, fields);
                GenerateReadMethod(writer, contract, fields);
               
                writer.WriteLine("}");
                writer.WriteLine();

                if(attribute?.Type == PacketType.Client || attribute?.Type == PacketType.Multiplex)
                    GenerateEvent(writer, contract);
            }
        }

        GenerateEnum("ClientPacket", clientPackets, networkDirectoryPath);
        GenerateEnum("ServerPacket", serverPackets, networkDirectoryPath);
        GenerateEnum("MultiplexPacket", multiplexPackets, networkDirectoryPath);
    }

    private static void GenerateEnum(string enumName, List<string> values, string directoryPath)
    {
        string filePath = Path.Combine(directoryPath, $"{enumName}.cs");

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// This file was generated automatically, please do not change it.");
            writer.WriteLine();
            writer.WriteLine($"public enum {enumName}");
            writer.WriteLine("{");

            for (int i = 0; i < values.Count; i++)
            {
                string value = values[i];
                
                if (i == values.Count - 1)
                    writer.WriteLine($"    {value} = {i}");
                else
                    writer.WriteLine($"    {value} = {i},");
            }

            writer.WriteLine("}");
        }
    }

    private static void GenerateWriteMethod(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        
        writer.WriteLine($"    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        writer.WriteLine($"    public static ByteBuffer Serialize({contract.Name} data)");
        writer.WriteLine("    {");
        writer.WriteLine("        var buffer = ByteBuffer.CreateEmptyBuffer();");

        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ContractFieldAttribute>();
            if (attribute != null)
            {
                var fieldType = attribute.Type;
                var fieldName = field.Name;

                switch (fieldType)
                {
                    case "integer":
                    case "int":
                    case "int32":
                    case "str":
                    case "string":
                    case "byte":
                    case "float":
                    case "bool":
                    case "boolean":
                        writer.WriteLine($"        buffer.Write(data.{fieldName});");
                        break;
                    case "decimal":
                        writer.WriteLine($"        buffer.Write((float)data.{fieldName});");
                        break;
                    default:
                        writer.WriteLine($"    // Tipo não suportado: {fieldType}");
                        break;
                }
            }
        }

        writer.WriteLine("        return buffer;");
        writer.WriteLine("    }");
        writer.WriteLine();
    }

    private static void GenerateReadMethod(StreamWriter writer, Type contract, FieldInfo[] fields)
    {
        writer.WriteLine($"    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        writer.WriteLine($"    public static {contract.Name} Deserialize(ByteBuffer buffer)");
        writer.WriteLine("    {");
        writer.WriteLine($"        var data = new {contract.Name}();");

        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ContractFieldAttribute>();
            if (attribute != null)
            {
                var fieldType = attribute.Type;
                var fieldName = field.Name;

                switch (fieldType)
                {
                    case "integer":
                    case "int":
                    case "int32":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadInt();");
                        break;
                    case "str":
                    case "string":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadString();");
                        break;
                    case "byte":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadByte();");
                        break;
                    case "float":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadFloat();");
                        break;
                    case "bool":
                    case "boolean":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadBool();");
                        break;
                    case "decimal":
                        writer.WriteLine($"        data.{fieldName} = (decimal)buffer.GetFloat();");
                        break;
                    default:
                        writer.WriteLine($"        // Tipo não suportado: {fieldType}");
                        break;
                }
            }
        }

        writer.WriteLine("        return data;");
        writer.WriteLine("    }");
    }

    private static void GenerateEvent(StreamWriter writer, Type contract)
    {
        writer.WriteLine("public partial class Server");
        writer.WriteLine("{");
        writer.WriteLine($"    public static NetworkEvents<{contract.Name}> On{contract.Name} = new NetworkEvents<{contract.Name}>();");
        writer.WriteLine("}");
    }
}