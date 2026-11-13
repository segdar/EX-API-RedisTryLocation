namespace EX_API_RedisTryLocation.Models
{
    public sealed record NearLocation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }

    public sealed record NearLocationRequest
    {
        public decimal Latitude { get; set; }   
        public decimal Longitude { get; set; }
        public float Radius { get; set; }
    }
}
