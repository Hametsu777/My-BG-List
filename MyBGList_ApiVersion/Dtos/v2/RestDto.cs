using MyBGList.Dtos.v1;

namespace MyBGList.Dtos.v2
{
    public class RestDto<T>
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
        public T Items { get; set; } = default!;
    }
}
