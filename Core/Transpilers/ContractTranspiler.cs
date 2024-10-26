/*
 * ContractTraspiler
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

using DotNetEnv;
using System;
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

            string filePath = Path.Combine(baseDirectoryPath, $"{contractName.Replace("DTO", "")}Packet.cs");

            using (var writer = new StreamWriter(filePath))
            {
                switch (attribute?.Type)
                {
                    case PacketType.Client: clientPackets.Add(contract.Name); break;
                    case PacketType.Server: serverPackets.Add(contract.Name); break;
                    case PacketType.Multiplex: multiplexPackets.Add(contract.Name); break;
                }

                writer.WriteLine("// This file was generated automatically, please do not change it.");
                writer.WriteLine();
                writer.WriteLine("using System.Runtime.CompilerServices;");
                writer.WriteLine();
                writer.WriteLine($"public struct {contractName.Replace("DTO", "")}Packet");
                writer.WriteLine("{");                

                GenerateWriteMethod(writer, contract, fields);
                GenerateReadMethod(writer, contract, fields);

                if(attribute.Type == PacketType.Server || attribute.Type == PacketType.Multiplex)
                    GenerateSendFunction(writer, contract, fields, attribute);

                writer.WriteLine("}");
                writer.WriteLine();

                if(attribute?.Type == PacketType.Client || attribute?.Type == PacketType.Multiplex)
                    GenerateEvent(writer, contract, attribute);
            }
        }

        GenerateEnum("ClientPacket", clientPackets, networkDirectoryPath, multiplexPackets);
        GenerateEnum("ServerPacket", serverPackets, networkDirectoryPath, multiplexPackets);
    }

    private static void GenerateEnum(string enumName, List<string> values, string directoryPath, List<string> valuesMultiplex)
    {
        string filePath = Path.Combine(directoryPath, $"{enumName}.cs");

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// This file was generated automatically, please do not change it.");
            writer.WriteLine();
            writer.WriteLine($"public enum {enumName}");
            writer.WriteLine("{");

            int pointer = 0;

            if (enumName == "ServerPacket")
            {
                pointer = 1;
                writer.WriteLine($"    Queue = 0,");
            }
            
            for (int i = 0; i < values.Count; i++)
            {
                string value = values[i];
                writer.WriteLine($"    {value.Replace("DTO", "")} = {pointer},");
                pointer++;
            }

            for (int i = 0; i < valuesMultiplex.Count; i++)
            {
                string value = valuesMultiplex[i];
                writer.WriteLine($"    {value.Replace("DTO", "")} = {pointer},");
                pointer++;
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
                    case "vector3":
                        writer.WriteLine($"        buffer.Write(data.{fieldName});");
                        break;
                    case "id":
                        writer.WriteLine($"        buffer.Write(Base36.ToInt(data.{fieldName}));");
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
                    case "vector3":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadVector3();");
                        break;
                    case "id":
                        writer.WriteLine($"        data.{fieldName} = buffer.ReadId();");
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

    private static void GenerateEvent(StreamWriter writer, Type contract, ContractAttribute attribute)
    {
        writer.WriteLine("public partial class Server");
        writer.WriteLine("{");
        writer.WriteLine($"    public static NetworkEvents<{contract.Name}> On{contract.Name.Replace("DTO", "")} = new NetworkEvents<{contract.Name}>();");

        if (attribute.Action == PacketAction.AreaOfInterest && attribute.Type == PacketType.Multiplex)
            GenerateAreaOfInterestReply(writer, contract, attribute);

        writer.WriteLine("}");
    }

    private static void GenerateAreaOfInterestReply(StreamWriter writer, Type contract, ContractAttribute attribute)
    {
        writer.WriteLine();
        writer.WriteLine($"    [Subscribe(ClientPacket.{contract.Name.Replace("DTO", "")})]");
        writer.WriteLine($"    public static void On{contract.Name.Replace("DTO", "")}Handler({contract} data, Connection conn)");
        writer.WriteLine("    {");
        writer.WriteLine($"        var packet = {contract.Name.Replace("DTO", "")}Packet.Serialize(data);");
        writer.WriteLine($"        conn.Entity.Reply(ServerPacket.{contract.Name.Replace("DTO", "")}, packet, {attribute.Queue.ToString().ToLower()});");
        writer.WriteLine("    }");
    }

    private static void GenerateSendFunction(StreamWriter writer, Type contract, FieldInfo[] fields, ContractAttribute attribute)
    {
        var contractName = contract.Name;

        writer.WriteLine();
        writer.WriteLine($"    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        writer.WriteLine($"    public static void Send(Entity owner, {contractName} data{(attribute.SendType == PacketSendType.ToEntity ? ", Entity entity" : "")})");
        writer.WriteLine("    {");
        writer.WriteLine("        var buffer = Serialize(data);");

        if (attribute.Action == PacketAction.AreaOfInterest)
        {
            writer.WriteLine($"        owner.Reply(ServerPacket.{contractName.Replace("DTO", "")}, buffer, {attribute.Queue.ToString().ToLower()}, {attribute.EncryptedData.ToString().ToLower()});");
        }
        else
        {
            if (attribute.Queue)
            {
                if (attribute.SendType == PacketSendType.Self)
                {
                    writer.WriteLine($"        QueueBuffer.AddBuffer(ServerPacket.{contractName.Replace("DTO", "")}, owner.Conn.Id, buffer);");
                }
                else if (attribute.SendType == PacketSendType.ToEntity)
                {
                    writer.WriteLine($"        QueueBuffer.AddBuffer(ServerPacket.{contractName.Replace("DTO", "")}, entity.Conn.Id, buffer);");
                }
            }
            else
            {
                if (attribute.SendType == PacketSendType.Self)
                {
                    writer.WriteLine($"        owner.Conn.Send(ServerPacket.{contractName.Replace("DTO", "")}, buffer, {attribute.EncryptedData.ToString().ToLower()});");
                }
                else if (attribute.SendType == PacketSendType.ToEntity)
                {
                    writer.WriteLine($"        entity.Conn.Send(ServerPacket.{contractName.Replace("DTO", "")}, buffer, {attribute.EncryptedData.ToString().ToLower()});");
                }
            }
        }

        writer.WriteLine("    }");
        writer.WriteLine();
    }

}