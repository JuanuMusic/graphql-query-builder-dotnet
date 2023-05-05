namespace GraphQL.Query.Builder.UnitTests.Models;

public class Order : GraphQLObject
{
    public Car? Product { get; set; }
}
