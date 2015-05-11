<Query Kind="Program">
  <NuGetReference>StaticReflection</NuGetReference>
  <Namespace>StaticReflection</Namespace>
  <Namespace>StaticReflection.CodeGeneration</Namespace>
</Query>

void Main()
{
	/*
	This example will compare the performance of an inline redact method call, a StaticReflection generated delegate, and reflection.
	*/
	
	// first we'll run each of them once untimed to jit, cache, etc.
	
	// So as to not be disingenuous, we'll call out that this is the biggest performance hit with StaticReflection right here
	// while we aren't timing it. However it will only need to be incurred once per application start as the delegate should be cached.
	var generatedRedact = GetGeneratedRedactDelegate();
	
	generatedRedact(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	InlineRedactor(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	ReflectiveRedactor(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	
	int iterations = 10000;
	Stopwatch watch = new Stopwatch();
	watch.Start();
	for(int i = 0; i < iterations; i++)
	{
		// calling the same cached delegate each time.
		generatedRedact(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	}
	watch.Stop();
	watch.Dump("Generated Delegate Time");
	
	watch.Reset();
	watch.Start();
	for(int i = 0; i < iterations; i++)
	{
		InlineRedactor(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	}
	watch.Stop();
	watch.Dump("Inline Time (Typically the Best)");
	
	watch.Reset();
	watch.Start();
	for(int i = 0; i < iterations; i++)
	{
		ReflectiveRedactor(new Person() { Name = "test", Age = 30, SSN = "123-45-6789"});
	}
	watch.Stop();
	watch.Dump("Reflection Time");
}

// Generates a delegate identical to the Redaction example
Action<Person> GetGeneratedRedactDelegate()
{
	var redactMethodInfo = (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<UserQuery>(u => u.Redact(""));
	var methodBlock = MutatorMethodBlock<Person>.Create();
	var personType = typeof(Person);
	foreach (var memberInfo in personType.GetMembers())
	{
		if (memberInfo.GetCustomAttribute<RedactAttribute>() != null)
		{
			methodBlock.AddReAssignmentMethodCallForMember(memberInfo, redactMethodInfo, this); // this is the instance of the object for the .Redact method, in this case the current UserQuery
		}
	}
	return methodBlock.Compile();
}
// this is equivalent to the code that we generate dynamically
void InlineRedactor(Person value)
{
	// doesn't check the attribute!
	value.SSN = this.Redact(value.SSN);
}

readonly Type personType = typeof(Person);
void ReflectiveRedactor(Person personTest)
{
	foreach (var memberInfo in personType.GetMembers())
	{
		if (memberInfo.GetCustomAttribute<RedactAttribute>() != null)
		{
			switch(memberInfo.MemberType)
			{
				case MemberTypes.Field:
					var fieldInfo = (FieldInfo)memberInfo;
					var value = fieldInfo.GetValue(personTest) as string;
					value = Redact(value);
					fieldInfo.SetValue(personTest, value);
					break;
				case MemberTypes.Property:
					var propertyInfo = (PropertyInfo)memberInfo;
					var val = propertyInfo.GetValue(personTest) as string;
					val = Redact(val);
					propertyInfo.SetValue(personTest, val);
					break;
			}
		}
	}
}

// static method that will perform out mutation, in this case redaction
public string Redact(string input)
{
	// ignore the input and return and static redacted string
	return "[ Redacted ]";
}

class Person
{
	public string Name {get;set;}
	public int Age {get;set;}
	// ssn is sensitive so we'll redact that. Only the generated and reflective readaction routines use this
	[Redact]
	public string SSN {get;set;}
}

// define a simple attribute indicating that an item needs redaction.
class RedactAttribute : Attribute { }


