namespace SonarQubeToSarif.Dtos;

internal record HotspotsDto
{
    public required PagingDto Paging { get; set; }
    public required List<HotspotDto> Hotspots { get; set; }
    public required List<ComponentDto> Components { get; set; }

    public record ComponentDto
    {
        public required string Key { get; set; }
        public required string Qualifier { get; set; }
        public required string Name { get; set; }
        public required string LongName { get; set; }
        public string? Path { get; set; }
    }

    public record HotspotDto
    {
        public required string Key { get; set; }
        public required string Component { get; set; }
        public required string Project { get; set; }
        public required string SecurityCategory { get; set; }
        public required string VulnerabilityProbability { get; set; }
        public required string Status { get; set; }
        public required int Line { get; set; }
        public required string Message { get; set; }
        public required string Author { get; set; }
        public required string CreationDate { get; set; }
        public required string UpdateDate { get; set; }
        public required TextRangeDto TextRange { get; set; }
        public required List<object> Flows { get; set; }
        public required string RuleKey { get; set; }
        public required List<object> MessageFormattings { get; set; }
    }

    public record PagingDto
    {
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
        public required int Total { get; set; }
    }

    public record TextRangeDto
    {
        public required int StartLine { get; set; }
        public required int EndLine { get; set; }
        public required int StartOffset { get; set; }
        public required int EndOffset { get; set; }
    }
}
