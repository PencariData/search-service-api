migrate:
	dotnet ef migrations add $(name) \
	  --project SearchService.Infrastructure \
	  --startup-project SearchService.API

update:
	dotnet ef database update \
	  --project SearchService.Infrastructure \
	  --startup-project SearchService.API
