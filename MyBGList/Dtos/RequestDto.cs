using MyBGList.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyBGList.Dtos
{
    public class RequestDto
    {
        [DefaultValue(0)]
        public int PageIndex { get; set; }

        [DefaultValue(10)]
        [Range(1, 100)]
        public int PageSize { get; set; }

        [DefaultValue("Name")]
        [SortColumnValidator(typeof(BoardGameDto))]
        public string? SortColumn { get; set; } = "Name";

        [DefaultValue("ASC")]
        [SortOrderValidator]
        public string? SortOrder { get; set; } = "ASC";

        [DefaultValue(null)]
        public string? FilterQuery { get; set; } = null;
    }
}
