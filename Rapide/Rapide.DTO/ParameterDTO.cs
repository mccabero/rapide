namespace Rapide.DTO
{
    public class ParameterDTO : BaseDTO
    {
        public string? Name { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public int NumericData { get; set; }

        public string? OtherData { get; set; }

        public int ParameterGroupId { get; set; }

        public ParameterGroupDTO ParameterGroup { get; set; }
    }
}