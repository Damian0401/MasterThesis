namespace SonarQubeToSarif.Dtos;

internal record IssuesDto
{
    public required int Total { get; set; }
    public required int P { get; set; }
    public required int Ps { get; set; }
    public required PagingDto Paging { get; set; }
    public required int EffortTotal { get; set; }
    public required List<IssueDto> Issues { get; set; }
    public required List<ComponentDto> Components { get; set; }
    public required List<object> Facets { get; set; }

    public record ComponentDto
    {
        public required string Key { get; set; }
        public required bool Enabled { get; set; }
        public required string Qualifier { get; set; }
        public required string Name { get; set; }
        public required string LongName { get; set; }
        public string? Path { get; set; }
    }

    public record ImpactDto
    {
        public required string SoftwareQuality { get; set; }
        public required string Severity { get; set; }
    }

    public record IssueDto
    {
        public required string Key { get; set; }
        public required string Rule { get; set; }
        public required string Severity { get; set; }
        public required string Component { get; set; }
        public required string Project { get; set; }
        public required int Line { get; set; }
        public required string Hash { get; set; }
        public required TextRangeDto TextRange { get; set; }
        public required List<object> Flows { get; set; }
        public required string Status { get; set; }
        public required string Message { get; set; }
        public required string Effort { get; set; }
        public required string Debt { get; set; }
        public required string Author { get; set; }
        public required List<string> Tags { get; set; }
        public required string CreationDate { get; set; }
        public required string UpdateDate { get; set; }
        public required string Type { get; set; }
        public required string Scope { get; set; }
        public required bool QuickFixAvailable { get; set; }
        public required List<object> MessageFormattings { get; set; }
        public required List<object> CodeVariants { get; set; }
        public required string CleanCodeAttribute { get; set; }
        public required string CleanCodeAttributeCategory { get; set; }
        public required List<ImpactDto> Impacts { get; set; }
        public required string IssueStatus { get; set; }
        public required bool PrioritizedRule { get; set; }
        public string? ExternalRuleEngine { get; set; }
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

