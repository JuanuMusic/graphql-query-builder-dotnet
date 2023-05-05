using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using GraphQL.Query.Builder.UnitTests.Models;
using Xunit;

namespace GraphQL.Query.Builder.UnitTests;

public class QueryOfTTests
{
    [Fact]
    public void Query_name()
    {
        // Arrange
        const string name = "user";

        Query<DynamicObject> query = new(name);

        // Assert
        Assert.Equal(name, query.Name);
    }

    [Fact]
    public void Query_name_required()
    {
        Assert.Throws<ArgumentNullException>(() => new Query<DynamicObject>(null));
    }

    [Fact]
    public void AddField_list()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        List<string> selectList = new()
        {
            "id",
            "name"
        };

        // Act
        foreach (string field in selectList)
        {
            query.AddField(field);
        }

        // Assert
        Assert.Equal(selectList, query.SelectList);
    }

    [Fact]
    public void AddField_string()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        const string select = "id";

        // Act
        query.AddField(select);

        // Assert
        Assert.Equal(select, query.SelectList.First());
    }

    [Fact]
    public void AddField_chained()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        // Act
        query.AddField("some").AddField("thing").AddField("else");

        // Assert
        List<string> shouldEqual = new()
        {
            "some",
            "thing",
            "else"
        };
        Assert.Equal(shouldEqual, query.SelectList);
    }

    [Fact]
    public void AddField_array()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        string[] selects =
        {
                "id",
                "name"
            };

        // Act
        foreach (string field in selects)
        {
            query.AddField(field);
        }

        // Assert
        List<string> shouldEqual = new()
        {
            "id",
            "name"
        };
        Assert.Equal(shouldEqual, query.SelectList);
    }


    [Fact]
    public void AddArgument_string_number()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        // Act
        query.AddArgument("id", 1);

        // Assert
        Assert.Equal(1, query.Arguments["id"]);
    }

    [Fact]
    public void AddArgument_string_string()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        // Act
        query.AddArgument("name", "danny");

        // Assert
        Assert.Equal("danny", query.Arguments["name"]);
    }

    [Fact]
    public void AddArgument_string_dictionary()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        Dictionary<string, int> dict = new()
        {
            { "from", 1 },
            { "to", 100 }
        };

        // Act
        query.AddArgument("price", dict.ToGraphQLObject());

        // Assert
        GraphQLObject queryWhere = (GraphQLObject)query.Arguments["price"];
        Assert.Equal(1, queryWhere["from"]);
        Assert.Equal(100, queryWhere["to"]);
    }

    [Fact]
    public void AddArguments_object()
    {
        // Arrange
        Query<GraphQLObject> query = new("car");

        Car car = new()
        {
            Name = "Bee",
            Price = 10000
        };

        // Act
        query.AddArguments(car);

        // Assert
        Assert.Equal(2, query.Arguments.Count);
        Assert.Equal("Bee", query.Arguments[nameof(Car.Name)]);
        Assert.Equal(10000m, query.Arguments[nameof(Car.Price)]);
    }

    [Fact]
    public void AddArguments_anonymous()
    {
        // Arrange
        Query<GraphQLObject> query = new("something");

        var obj = new
        {
            from = 1,
            to = 100
        };

        // Act
        query.AddArguments(obj);

        // Assert
        Assert.Equal(2, query.Arguments.Count);
        Assert.Equal(1, query.Arguments["from"]);
        Assert.Equal(100, query.Arguments["to"]);
    }

    [Fact]
    public void AddArguments_dictionary()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        Dictionary<string, object> dictionary = new()
        {
            { "from", 1 },
            { "to", 100 }
        };

        // Act
        query.AddArguments(dictionary);

        // Assert
        Assert.Equal(2, query.Arguments.Count);
        Assert.Equal(1, query.Arguments["from"]);
        Assert.Equal(100, query.Arguments["to"]);
    }

    [Fact]
    public void AddArgument_chained()
    {
        // Arrange
        Query<DynamicObject> query = new("something");

        Dictionary<string, int> dict = new()
        {
            { "from", 1 },
            { "to", 100 }
        };

        // Act
        query
            .AddArgument("id", 123)
            .AddArgument("name", "danny")
            .AddArgument("price", dict);

        // Assert
        Dictionary<string, object> shouldPass = new()
        {
            { "id", 123 },
            { "name", "danny" },
            { "price", dict }
        };
        Assert.Equal(shouldPass, query.Arguments);
    }

    [Fact]
    public void TestAddField()
    {
        Query<Car> query = new("car");
        query.AddField(c => c.Name);

        Assert.Equal(new List<string> { nameof(Car.Name) }, query.SelectList);
    }

    [Fact]
    public void TestAddField_subQuery()
    {
        Query<Car> query = new("car");
        query.AddField(c => c.Color, sq => sq);

        Assert.Equal(nameof(Car.Color), (query.SelectList[0] as IQuery<Color>).Name);
    }

    [Fact]
    public void TestAddField_customFormatter()
    {
        Query<Car> query = new("car", options: new QueryOptions
        {
            Formatter = property => $"__{property.Name.ToLower()}"
        });
        query.AddField(c => c.Name);

        Assert.Equal(new List<string> { "__name" }, query.SelectList);
    }

    [Fact]
    public void TestAddField_subQuery_customFormatter()
    {
        Query<Car> query = new("car", options: new QueryOptions
        {
            Formatter = property => $"__{property.Name.ToLower()}"
        });
        query.AddField(c => c.Color, sq => sq);

        Assert.Equal("__color", (query.SelectList[0] as IQuery<Color>).Name);
    }

    [Fact]
    public void TestQuery()
    {
        IQuery<Car>? query = new Query<Car>(nameof(Car))
            .AddField(car => car.Name)
            .AddField(car => car.Price)
            .AddField(
                car => car.Color,
                sq => sq
                    .AddField(color => color.Red)
                    .AddField(color => color.Green)
                    .AddField(color => color.Blue));

        Assert.Equal(nameof(Car), query.Name);
        Assert.Equal(3, query.SelectList.Count);
        Assert.Equal(nameof(Car.Name), query.SelectList[0]);
        Assert.Equal(nameof(Car.Price), query.SelectList[1]);

        Assert.Equal(nameof(Car.Color), (query.SelectList[2] as IQuery<Color>).Name);
        List<string> expectedSubSelectList = new()
        {
            nameof(Color.Red),
            nameof(Color.Green),
            nameof(Color.Blue)
        };
        Assert.Equal(expectedSubSelectList, (query.SelectList[2] as IQuery<Color>).SelectList);
    }

    [Fact]
    public void TestQuery_build()
    {
        IQuery<Car>? query = new Query<Car>("car")
            .AddArguments(new { id = "yk8h4vn0", km = 2100, imported = true, page = new { from = 1, to = 100 } }.ToGraphQLObject())
            .AddField(car => car.Name)
            .AddField(car => car.Price)
            .AddField(
                car => car.Color,
                sq => sq
                    .AddField(color => color.Red)
                    .AddField(color => color.Green)
                    .AddField(color => color.Blue));

        string result = query.Build();

        Assert.Equal("car(id:\"yk8h4vn0\",km:2100,imported:true,page:{from:1,to:100}){Name Price Color{Red Green Blue}}", result);
    }

    [Fact]
    public void TestSubSelectWithList()
    {
        IQuery<ObjectWithList>? query = new Query<ObjectWithList>("object")
            .AddField<SubObject>(c => c.IEnumerable, sq => sq)
            .AddField<SubObject>(c => c.List, sq => sq)
            .AddField<SubObject>(c => c.IQueryable, sq => sq)
            .AddField<SubObject>(c => c.Array, sq => sq);

        Assert.Equal(typeof(Query<SubObject>), query.SelectList[0].GetType());
        Assert.Equal(typeof(Query<SubObject>), query.SelectList[1].GetType());
        Assert.Equal(typeof(Query<SubObject>), query.SelectList[2].GetType());
        Assert.Equal(typeof(Query<SubObject>), query.SelectList[3].GetType());
    }

    [Fact]
    public void AddPossibleType_array()
    {
        // Arrange
        Query<GraphQLObject> query = new("something");

        string[] types =
        {
                "UnionA",
                "UnionB"
            };

        // Act
        foreach (string type in types)
        {
            query.AddPossibleType(type);
        }

        // Assert
        List<string> shouldEqual = new()
        {
            "UnionA",
            "UnionB"
        };
        Assert.Equal(shouldEqual, query.PossibleTypesList);
    }

    [Fact]
    public void AddPossibleType_chained()
    {
        // Arrange
        Query<GraphQLObject> query = new("something");

        // Act
        query.AddPossibleType("UnionA").AddPossibleType("UnionB").AddPossibleType("UnionC");

        // Assert
        List<string> shouldEqual = new()
        {
            "UnionA",
            "UnionB",
            "UnionC"
        };
        Assert.Equal(shouldEqual, query.PossibleTypesList);
    }

    [Fact]
    public void AddPossibleType_selector()
    {
        Query<Order> query = new("order");

        // Act
        query.AddPossibleType<Car>(q => q.AddField(pt => pt.Color,
                                    pc => pc.AddField(c => c.Blue)))
             .AddField("anotherField");

        // Assert
        List<string> shouldEqual = new()
        {
            "Product",
        };
        Assert.Equal(1, query.PossibleTypesList.Count);
        Assert.IsAssignableFrom(typeof(IQuery<Car>), query.PossibleTypesList[0]);
        Assert.Equal(1, ((IQuery<Car>)query.PossibleTypesList[0]).SelectList.Count);

    }

    class ObjectWithList : GraphQLObject
    {
        public IEnumerable<SubObject>? IEnumerable { get; set; }
        public List<SubObject>? List { get; set; }
        public IQueryable<SubObject>? IQueryable { get; set; }
        public SubObject[]? Array { get; set; }
    }

    class SubObject : GraphQLObject
    {
        public byte Id { get; set; }
    }
}
