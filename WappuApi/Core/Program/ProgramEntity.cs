namespace WappuApi.Core.Program;

public class ProgramEntity : EntityBase
{
    public ProgramEntity() { }

    public string Title { get; set; } = "";

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }
}
