# Define mapping query

```http request
PUT accommodations
{
  "settings": {
    "index.max_ngram_diff": 20,
    "analysis": {
      "analyzer": {
        "destination_analyzer": {
          "tokenizer": "standard",
          "filter": ["lowercase", "trim"]
        },
        "ngram_analyzer": {
          "tokenizer": "ngram_tokenizer",
          "filter": ["lowercase", "trim"]
        }
      },
      "tokenizer": {
        "ngram_tokenizer": {
          "type": "ngram",
          "min_gram": 3,
          "max_gram": 20,
          "token_chars": ["letter", "digit"]
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "name": {
        "type": "text",
        "fields": {
          "keyword": { "type": "keyword" },
          "ngram": { "type": "text", "analyzer": "ngram_analyzer", "search_analyzer": "standard" }
        }
      },
      "name_suggest": {
        "type": "completion",
        "analyzer": "simple",
        "preserve_separators": true,
        "preserve_position_increments": true,
        "max_input_length": 50
      },
      "destinationId": { "type": "keyword" },
      "destinationName": {
        "type": "text",
        "fields": {
          "ngram": { "type": "text", "analyzer": "ngram_analyzer", "search_analyzer": "standard" }
        }
      },
      "fullDestination": { "type": "text", "analyzer": "destination_analyzer" },
      "accommodationType": { "type": "keyword" },
      "coordinate": { "type": "geo_point" }
    }
  }
}
```
```http request
PUT destinations
{
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "name": { "type": "text" },
      "name_suggest": {
        "type": "completion",
        "analyzer": "simple",
        "preserve_separators": true,
        "preserve_position_increments": true,
        "max_input_length": 100
      },
      "fullName": { "type": "text" },
      "centerCoordinate": { "type": "geo_point" },
      "accommodationCount": { "type": "integer" }
    }
  }
}
```

# Add data

```http request
POST accommodations/_bulk
{ "index": { "_index": "accommodations" } }
{ "id": "a1f3c9d0-1e2b-4a5c-b678-0a1b2c3d4e5f", "name": "Royal Jakarta Hotel", "name_suggest": { "input": ["Royal","Jakarta","Hotel","Royal Jakarta","Royal Hotel","Jakarta Hotel","Royal Jakarta Hotel"] }, "destinationId": "d1", "destinationName": "Jakarta", "fullDestination": "Jakarta, DKI Jakarta, Indonesia", "accommodationType": "Hotel", "coordinate": { "lat": -6.2088, "lon": 106.8456 } }
{ "index": { "_index": "accommodations" } }
{ "id": "b2e4d0f1-2f3c-4b5d-c789-1b2c3d4e5f6a", "name": "Bandung Mountain Resort", "name_suggest": { "input": ["Bandung","Mountain","Resort","Bandung Mountain","Bandung Resort","Mountain Resort","Bandung Mountain Resort"] }, "destinationId": "d2", "destinationName": "Bandung", "fullDestination": "Bandung, West Java, Indonesia", "accommodationType": "Resort", "coordinate": { "lat": -6.9175, "lon": 107.6191 } }
{ "index": { "_index": "accommodations" } }
{ "id": "d4a6b2c3-4b5c-4d7e-e901-3d4e5f6a7b8c", "name": "Yogyakarta Boutique Villa", "name_suggest": { "input": ["Yogyakarta","Boutique","Villa","Yogyakarta Boutique","Yogyakarta Villa","Boutique Villa","Yogyakarta Boutique Villa"] }, "destinationId": "d4", "destinationName": "Yogyakarta", "fullDestination": "Yogyakarta, Special Region of Yogyakarta, Indonesia", "accommodationType": "Villa", "coordinate": { "lat": -7.7956, "lon": 110.3695 } }
{ "index": { "_index": "accommodations" } }
{ "id": "d0a2b8c9-0b1c-4d3e-4567-9d0e1f2a3b4c", "name": "Yogyakarta Heritage Villa", "name_suggest": { "input": ["Yogyakarta","Heritage","Villa","Yogyakarta Heritage","Yogyakarta Villa","Heritage Villa","Yogyakarta Heritage Villa"] }, "destinationId": "d4", "destinationName": "Yogyakarta", "fullDestination": "Yogyakarta, Special Region of Yogyakarta, Indonesia", "accommodationType": "Villa", "coordinate": { "lat": -7.7972, "lon": 110.3700 } }
```

```http request
POST destinations/_bulk
{ "index": { "_index": "destinations" } }
{ "id": "d1", "name": "Jakarta", "name_suggest": { "input": ["Jakarta"] }, "fullName": "Jakarta, DKI Jakarta, Indonesia", "centerCoordinate": { "lat": -6.2088, "lon": 106.8456 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d2", "name": "Bandung", "name_suggest": { "input": ["Bandung"] }, "fullName": "Bandung, West Java, Indonesia", "centerCoordinate": { "lat": -6.9175, "lon": 107.6191 }, "accommodationCount": 2 }
{ "index": { "_index": "destinations" } }
{ "id": "d3", "name": "Surabaya", "name_suggest": { "input": ["Surabaya"] }, "fullName": "Surabaya, East Java, Indonesia", "centerCoordinate": { "lat": -7.2575, "lon": 112.7521 }, "accommodationCount": 2 }
{ "index": { "_index": "destinations" } }
{ "id": "d4", "name": "Yogyakarta", "name_suggest": { "input": ["Yogyakarta"] }, "fullName": "Yogyakarta, Special Region of Yogyakarta, Indonesia", "centerCoordinate": { "lat": -7.7956, "lon": 110.3695 }, "accommodationCount": 2 }
{ "index": { "_index": "destinations" } }
{ "id": "d5", "name": "Malang", "name_suggest": { "input": ["Malang"] }, "fullName": "Malang, East Java, Indonesia", "centerCoordinate": { "lat": -7.9825, "lon": 112.6303 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d6", "name": "Semarang", "name_suggest": { "input": ["Semarang"] }, "fullName": "Semarang, Central Java, Indonesia", "centerCoordinate": { "lat": -6.9667, "lon": 110.4167 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d7", "name": "Bogor", "name_suggest": { "input": ["Bogor"] }, "fullName": "Bogor, West Java, Indonesia", "centerCoordinate": { "lat": -6.5950, "lon": 106.8167 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d8", "name": "Depok", "name_suggest": { "input": ["Depok"] }, "fullName": "Depok, West Java, Indonesia", "centerCoordinate": { "lat": -6.4025, "lon": 106.7949 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d9", "name": "Tangerang", "name_suggest": { "input": ["Tangerang"] }, "fullName": "Tangerang, Banten, Indonesia", "centerCoordinate": { "lat": -6.1783, "lon": 106.6319 }, "accommodationCount": 1 }
{ "index": { "_index": "destinations" } }
{ "id": "d10", "name": "Solo", "name_suggest": { "input": ["Solo"] }, "fullName": "Solo, Central Java, Indonesia", "centerCoordinate": { "lat": -7.5754, "lon": 110.8240 }, "accommodationCount": 1 }
```