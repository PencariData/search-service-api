using SearchService.Shared.Constants;

namespace SearchService.Infrastructure.Helpers.QueryBuilding;

public class ElasticsearchQueryBuilder
{
    private readonly Dictionary<string, object> _queryBody = new();
    private int _from;
    private int _size;
    private Dictionary<string, object>? _suggest;

    public ElasticsearchQueryBuilder WithPagination(int page, int limit)
    {
        _from = page * limit;
        _size = limit;
        return this;
    }

    public ElasticsearchQueryBuilder WithSize(int size)
    {
        _size = size;
        return this;
    }

    public ElasticsearchQueryBuilder WithMatch(string field, string query, string fuzziness = DataConstants.AutoFuzziness)
    {
        _queryBody["query"] = new
        {
            match = new Dictionary<string, object>
            {
                [field] = new { query = query, fuzziness = fuzziness }
            }
        };
        return this;
    }

    public ElasticsearchQueryBuilder WithMultiMatch(List<string> fields, string query, string fuzziness = DataConstants.AutoFuzziness)
    {
        _queryBody["query"] = new
        {
            multi_match = new
            {
                query,
                fields,
                fuzziness = fuzziness
            }
        };
        return this;
    }

    public ElasticsearchQueryBuilder WithBoolQuery(Action<BoolQueryBuilder> configure)
    {
        var boolBuilder = new BoolQueryBuilder();
        configure(boolBuilder);
        _queryBody["query"] = new { @bool = boolBuilder.Build() };
        return this;
    }

    public ElasticsearchQueryBuilder WithRangeQuery(string field, object? gte = null, object? lte = null)
    {
        var rangeQuery = new Dictionary<string, object>();
        if (gte != null) rangeQuery["gte"] = gte;
        if (lte != null) rangeQuery["lte"] = lte;

        _queryBody["query"] = new
        {
            range = new Dictionary<string, object> { [field] = rangeQuery }
        };
        return this;
    }

    public ElasticsearchQueryBuilder WithTermQuery(string field, object value)
    {
        _queryBody["query"] = new
        {
            term = new Dictionary<string, object> { [field] = value }
        };
        return this;
    }

    public ElasticsearchQueryBuilder WithSuggestion(string suggestionName, string prefix, string field, int size, int fuzziness = DataConstants.DefaultFuzziness)
    {
        _suggest ??= new Dictionary<string, object>();
        _suggest[suggestionName] = new
        {
            prefix,
            completion = new
            {
                field,
                size,
                fuzzy = new { fuzziness }
            }
        };
        return this;
    }

    public ElasticsearchQueryBuilder WithSort(string field, string order = "asc")
    {
        _queryBody["sort"] = new[] { new Dictionary<string, object> { [field] = new { order } } };
        return this;
    }

    public ElasticsearchQueryBuilder WithSourceFields(params string[] fields)
    {
        _queryBody["_source"] = fields;
        return this;
    }

    public object Build()
    {
        var result = new Dictionary<string, object>
        {
            ["from"] = _from,
            ["size"] = _size
        };

        foreach (var kvp in _queryBody)
        {
            result[kvp.Key] = kvp.Value;
        }

        if (_suggest != null)
        {
            result["suggest"] = _suggest;
        }

        return result;
    }
}

public class BoolQueryBuilder
{
    private readonly List<object> _must = new();
    private readonly List<object> _should = new();
    private readonly List<object> _mustNot = new();
    private readonly List<object> _filter = new();

    public BoolQueryBuilder Must(object query)
    {
        _must.Add(query);
        return this;
    }

    public BoolQueryBuilder Should(object query)
    {
        _should.Add(query);
        return this;
    }

    public BoolQueryBuilder MustNot(object query)
    {
        _mustNot.Add(query);
        return this;
    }

    public BoolQueryBuilder Filter(object query)
    {
        _filter.Add(query);
        return this;
    }

    public object Build()
    {
        var result = new Dictionary<string, object>();
        
        if (_must.Count > 0) result["must"] = _must;
        if (_should.Count > 0) result["should"] = _should;
        if (_mustNot.Count > 0) result["must_not"] = _mustNot;
        if (_filter.Count > 0) result["filter"] = _filter;

        return result;
    }
}
