// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Beryl.Common.Utility;

/// <summary>
/// Utility for outputting to the engine console.
/// </summary>
/// <remarks> This class is asynchronous and provides faster, non-thread blocking <see cref="Console"/> equivalent operations. </remarks>
public static class BerylConsole
{
	// Allie 15/07/26:
	// TODO: Someone else should have a look over the task shenanigans of this class, im not too familiar with them so there might be some dumb decisions.

	private readonly record struct LogMessage(IMemoryOwner<char> Owner, int Length, ConsoleColor Color, TextWriter Writer);

	private static readonly Channel<LogMessage> logChannel = Channel.CreateUnbounded<LogMessage>(new UnboundedChannelOptions
	{
		SingleReader = true
	});

	static BerylConsole()
	{
		Task.Factory.StartNew(async () =>
		{
			await foreach (var message in logChannel.Reader.ReadAllAsync())
			{
				var prevColor = Console.ForegroundColor;
				Console.ForegroundColor = message.Color;

				ReadOnlySpan<char> span = message.Owner.Memory.Span[..message.Length];
				message.Writer.WriteLine(span);

				Console.ForegroundColor = prevColor;

				message.Owner.Dispose();
			}

		}, TaskCreationOptions.LongRunning);
	}

	/// <summary> Low-Level method for logging a <see cref="ReadOnlySpan{T}"/> where <c>T</c> is <see cref="char"/> to the <see cref="BerylConsole"/> with no heap allocations. </summary>
	/// <param name="message"> The message to log. </param>
	/// <param name="color"> The color to set the console to. </param>
	/// <param name="writer"> The writer to write into. </param>
	/// <remarks> This method queues the given message to be logged by an async <see cref="Task"/>. As such, messages submitted via this method will be logged when possible rather than immediately, causing a possible desync. </remarks>
	public static void StackLog(ReadOnlySpan<char> message, ConsoleColor color, TextWriter writer)
	{
		IMemoryOwner<char> owner = MemoryPool<char>.Shared.Rent(message.Length);
		message.CopyTo(owner.Memory.Span);
		logChannel.Writer.TryWrite(new LogMessage(owner, message.Length, color, writer));
	}

	/// <summary> Output an exception. </summary>
	/// <remarks> Wrapper for <see cref="StackLog(ReadOnlySpan{char}, ConsoleColor, TextWriter)"/>. </remarks>
	public static void Exception(Exception ex, [CallerFilePath] ReadOnlySpan<char> origin = "Exception", [CallerMemberName] ReadOnlySpan<char> member = "Unknown")
	{
		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin);
		ReadOnlySpan<char> output = $"[{parsedOrigin}.{member}] {ex}";
		StackLog(output, ConsoleColor.Red, Console.Error);
	}

	/// <summary> Output a message. </summary>
	/// <remarks> Wrapper for <see cref="StackLog(ReadOnlySpan{char}, ConsoleColor, TextWriter)"/>. </remarks>
	public static void Log(ReadOnlySpan<char> message, [CallerFilePath] ReadOnlySpan<char> origin = "Log")
	{
		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin);
		ReadOnlySpan<char> output = $"[{parsedOrigin}] {message}";
		StackLog(output, ConsoleColor.White, Console.Out);
	}

	/// <summary> Output a warning. </summary>
	/// <remarks> Wrapper for <see cref="StackLog(ReadOnlySpan{char}, ConsoleColor, TextWriter)"/>. </remarks>
	public static void Warning(ReadOnlySpan<char> message, [CallerFilePath] ReadOnlySpan<char> origin = "Warning", [CallerMemberName] ReadOnlySpan<char> member = "Unknown")
	{
		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin);
		ReadOnlySpan<char> output = $"[{parsedOrigin}.{member}] {message}";
		StackLog(output, ConsoleColor.Yellow, Console.Out);
	}
}
