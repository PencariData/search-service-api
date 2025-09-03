# Search Service Project

![.NET](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet&logoColor=white)
![Elasticsearch](https://img.shields.io/badge/Elasticsearch-8.14.3-blue?logo=elasticsearch&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-Learning-yellow)

A **search service API** for accommodation data, built as a learning project to explore **search API design** using **Elasticsearch**. The project demonstrates efficient and flexible search capabilities suitable for real-world applications.

## Features

- Full-text search across accommodation listings
- Filtering and sorting by various criteria (e.g., price, location, rating)
- Utilizes Elasticsearch’s advanced search functionalities
- Designed with scalability and performance in mind

## Prerequisites

- **Elasticsearch** v8.14.3
- **.NET SDK** v8

## Getting Started

### Configuration

1. Copy `appsettings.json` to `appsettings.Local.json` (optional) and update with your local settings.
2. Ensure Elasticsearch is running on your machine or configured endpoint.

### Build & Run

```bash
dotnet build
dotnet run
```

The API will be available at http://localhost:5125 by default.

You could check the API documentation on http://localhost:5125/swagger/index.html

## Notes

- This project is intended for **learning purposes** and is **not production-ready**.
- Elasticsearch must be running locally for the API to function properly.
- Designed to demonstrate **search optimization, query design, and API structuring**.
- To properly fetch data, you should manually add data to your elasticsearch index.
  For convenience, an example query is provided to seed Elasticsearch data. Please check seed-data.md file

## Future Improvements

- Implement caching for frequently searched queries
- Enhance filtering options and search relevance scoring
- Implement data seeding for accommodation and destinations
