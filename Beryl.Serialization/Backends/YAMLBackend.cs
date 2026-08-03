// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Text;

namespace Beryl.Serialization.Backends;

[SerializerBackend(SerializationType.YAML)]
internal sealed class YAMLBackend : SerializerBackend
{
	private const int IndentSize = 4;

	/// <inheritdoc/>
	public override InstanceRepresentation Deserialize(byte[] data)
	{
		var fields = new List<FieldRepresentation>();
		var lines = new List<(int Indent, string Line)>();

		var reader = new StringReader(Encoding.UTF8.GetString(data));
		while (reader.Peek() != -1)
		{
			string line = reader.ReadLine()!;

			if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
				continue;

			int indent = line.Length - line.TrimStart().Length;
			lines.Add((indent, line.Trim()));
		}

		// Stack of open containers, each entry is (indentation, dotted prefix).
		var stack = new Stack<(int Indent, string Prefix)>();
		(int Indent, string Name)? pending = null;

		for (int i = 0; i < lines.Count; i++)
		{
			(int indent, string line) = lines[i];
			string[] parts = line.Split(':', 2);
			string name = parts[0].Trim();
			bool hasValue = parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]);

			while (stack.Count > 0 && stack.Peek().Indent >= indent)
				stack.Pop();

			// A key without a value is either a container (confirmed by the next line)
			// or a null-valued field; decide once the next line is known.
			if (pending != null)
			{
				if (indent > pending.Value.Indent)
					stack.Push((pending.Value.Indent, pending.Value.Name + "."));
				else
					fields.Add(new FieldRepresentation(stack.Count > 0 ? stack.Peek().Prefix + pending.Value.Name : pending.Value.Name, string.Empty));

				pending = null;
			}

			string fullName = stack.Count > 0 ? stack.Peek().Prefix + name : name;

			if (hasValue)
				fields.Add(new FieldRepresentation(fullName, parts[1].Trim()));
			else
				pending = (indent, name);
		}

		if (pending != null)
			fields.Add(new FieldRepresentation(stack.Count > 0 ? stack.Peek().Prefix + pending.Value.Name : pending.Value.Name, string.Empty));

		return new InstanceRepresentation() { Fields = fields.ToArray() };
	}

	/// <inheritdoc/>
	public override byte[] Serialize(InstanceRepresentation obj)
	{
		var sb = new StringBuilder();

		foreach (var root in BuildTree(obj.Fields))
			WriteNode(sb, root, 0);

		return Encoding.UTF8.GetBytes(sb.ToString());
	}

	/// <summary> Builds a tree of nodes from the flat dotted field names. </summary>
	private static Node[] BuildTree(FieldRepresentation[] fields)
	{
		var roots = new List<Node>();

		foreach (var field in fields)
		{
			string[] parts = field.Name.Split('.');
			Node node = FindOrCreate(roots, parts[0]);

			for (int i = 1; i < parts.Length; i++)
				node = FindOrCreate(node.Children, parts[i]);

			node.Value = field.Value;
		}

		return roots.ToArray();
	}

	private static Node FindOrCreate(List<Node> nodes, string name)
	{
		foreach (var node in nodes)
		{
			if (node.Name == name)
				return node;
		}

		var created = new Node() { Name = name };
		nodes.Add(created);
		return created;
	}

	private static void WriteNode(StringBuilder sb, Node node, int depth)
	{
		string indent = new string(' ', depth * IndentSize);

		if (node.Value != null)
		{
			sb.AppendLine($"{indent}{node.Name}: {node.Value}");
		}
		else
		{
			sb.AppendLine($"{indent}{node.Name}:");
			foreach (var child in node.Children)
				WriteNode(sb, child, depth + 1);
		}
	}

	private sealed class Node
	{
		public string Name { get; init; } = string.Empty;

		public string? Value { get; set; }

		public List<Node> Children { get; } = new();
	}
}
