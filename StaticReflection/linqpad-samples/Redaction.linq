<Query Kind="Program">
  <NuGetReference>StaticReflection</NuGetReference>
  <Namespace>StaticReflection</Namespace>
  <Namespace>StaticReflection.CodeGeneration</Namespace>
</Query>

void Main()
{
	/*
	This example will show how reflection can be used to build a mutator method that will be compiled
	and provided as a delegate. This is useful to emit methods that will not incur a reflection hit
	everytime they are executed. The reflection hit can be incured only once and the delegate used subsequently.
	
	In this example a person object contains a sensetive field, the SSN. Suppose we are logging this object but
	don't want to log the sensitive value. This example will use a redact attribute to generate a method that
	will redact the person object with near static code performance. It will then compare the performance with
	a standard method that performs the same operation.
	*/


	// we'll need a Method info object for our mutator method. Lambdas will help us get it in a typesafe way.
	// The Redact method here won't be executed, instead the lambda is used to get the MemberInfo for the redact method.
	var redactMethodInfo = (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<UserQuery>(u => u.Redact("")); // The empty string isn't relevant as this will never be executed.
	
	// create the mutator methodblock
	var methodBlock = MutatorMethodBlock<Person>.Create();
	
	// lets now find all members that have a redact attribute
	var personType = typeof(Person);
	foreach (var memberInfo in personType.GetMembers())
	{
		if (memberInfo.GetCustomAttribute<RedactAttribute>() != null)
		{
			// we've found the attribute, dynamically run the field through the mutator. In a real situation parameter type and return type checking would be needed.
			methodBlock.AddReAssignmentMethodCallForMember(memberInfo, redactMethodInfo, this); // this is the instance of the object for the .Redact method, in this case the current UserQuery
		}
	}
	
	// we've dynamically built our mutator method expression. Now lets get a delegate.
	Action<Person> personRedactDelegate = methodBlock.Compile();
	
	// we now have a static method that does no reflection. It will peform as if you'd written the mutator code inline directly.
	var personTest = new Person() { Name = "test", Age = 30, SSN = "123-45-6789"};
	Stopwatch watch = new Stopwatch();
	watch.Start();
	personRedactDelegate(personTest);
	watch.Stop();
	personTest.Dump("Redacted Person");
	watch.Dump("Elapsed Time for Generated Code");
	
	// for comparison, we'll call a standard method that does the exact same operation
	personTest = new Person() { Name = "test", Age = 30, SSN = "123-45-6789"};
	watch.Start();
	this.InlineRedactor(personTest);
	watch.Stop();
	watch.Dump("Elapsed Time for Static Code");
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
	// ssn is sensitive so we'll redact that
	[Redact]
	public string SSN {get;set;}
}

// define a simple attribute indicating that an item needs redaction
class RedactAttribute : Attribute { }

// this is equivalent to the code that we generate dynamically
void InlineRedactor(Person value)
{
	value.SSN = this.Redact(value.SSN);
}