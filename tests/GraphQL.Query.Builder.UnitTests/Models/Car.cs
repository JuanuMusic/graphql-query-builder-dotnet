using System.Dynamic;
using System.Xml.Linq;

namespace GraphQL.Query.Builder.UnitTests.Models;

public class Car : GraphQLObject
{
    public string? Name
    {
        get => GetProperty<string>(nameof(Name));
        set => SetProperty(nameof(Name), value);
    }

    public decimal? Price
    {
        get => GetProperty<decimal?>(nameof(Price));
        set => SetProperty(nameof(Price), value);
    }

    public Color? Color {
        get => GetProperty<Color?>(nameof(Color));
        set => SetProperty(nameof(Color), value);
    }
}
