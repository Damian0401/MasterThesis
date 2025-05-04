namespace ProjectAnalizer.Dtos;

internal record LlmResponseDto
{
    public required List<SarifDto.RuleDto> Rules { get; set; }
    public required List<SarifDto.ResultDto> Results { get; set; }
}