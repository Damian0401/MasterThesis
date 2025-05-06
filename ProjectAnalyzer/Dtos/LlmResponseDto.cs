namespace ProjectAnalizer.Dtos;

internal record LlmResponseDto
{
    public required IEnumerable<ResultDto> Results { get; set; }
    public record ResultDto
    {
        public required string RuleId { get; set; }
        public required string RuleDescription { get; set; }
        public required string Message { get; set; }
        public required SarifDto.SeverityLevel Level { get; set; }
        public required string Path { get; set; }
        public required int StartLine { get; set; }
        public required int EndLine { get; set; }
        public required int StartColumn { get; set; }
        public required int EndColumn { get; set; }
    }
}