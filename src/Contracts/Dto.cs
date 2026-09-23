using ProtoBuf;

namespace DogAppBlazor.Contracts;

/// <summary>
/// Obalka pre prenos jednej primitivnej hodnoty. gRPC code-first vyzaduje,
/// aby kazda metoda fasady mala prave jednu poziadavkovu spravu.
/// </summary>
[ProtoContract]
public class Dto<TValue>
{
	[ProtoMember(1)]
	public TValue Value { get; set; }

	public Dto()
	{
		// NOOP
	}

	public Dto(TValue value)
	{
		Value = value;
	}
}

public static class Dto
{
	public static Dto<TValue> FromValue<TValue>(TValue value)
	{
		return new Dto<TValue>(value);
	}
}
