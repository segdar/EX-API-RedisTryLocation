# EX-API-RedisTryLocation

This project is a base ASP.NET Core API that shows a pattern for:
- SQL Server geospatial-style location queries using latitude/longitude
- Entity Framework Core with a SQL Server database context
- Redis caching for faster responses
- A stored procedure that returns nearby stores by radius in meters

## Database table example

Create the table and procedure in SQL Server using the script in `Database/LocationsSchema.sql`.

The query uses `GEOGRAPHY::Point` instead of a non-standard `GEOGRAPHYPoint` helper so it works in SQL Server.

## EF Core model

The project includes:
- `Models/Location` for the stored data
- `Services/ContextDBServices` for the EF Core database context
- `Services/LocationCacheService` to cache nearby-store responses in Redis
- `Controllers/NearLocationController` to call the stored procedure

## Cached request example

POST `/api/NearLocation/nearby`

```json
{
  "latitude": 19.4326,
  "longitude": -99.1332,
  "radiusMeters": 5000
}
```

## Redis behaviour

The cache key is built from the coordinates and radius, so repeated requests with the same input are served from Redis instead of querying SQL Server again.

## Example SQL script

```sql
CREATE TABLE dbo.Locations (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Latitude DECIMAL(10,7) NOT NULL,
    Longitude DECIMAL(10,7) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GetNearbyStores]
    @Latitude DECIMAL(10,7),
    @Longitude DECIMAL(10,7),
    @RadiusMeters FLOAT
AS
BEGIN
    DECLARE @userLocation GEOGRAPHY;
    SET @userLocation = GEOGRAPHY::Point(@Latitude, @Longitude, 4326);

    SELECT
        l.Id,
        l.Name,
        @userLocation.STDistance(GEOGRAPHY::Point(l.Latitude, l.Longitude, 4326)) / 1000.0 AS DistanceKm
    FROM dbo.Locations AS l
    WHERE l.IsActive = 1
      AND @userLocation.STDistance(GEOGRAPHY::Point(l.Latitude, l.Longitude, 4326)) <= @RadiusMeters
    ORDER BY DistanceKm ASC;
END;
GO
```
