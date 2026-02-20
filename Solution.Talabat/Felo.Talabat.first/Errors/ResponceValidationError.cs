namespace Felo.Talabat.Api.Errors
{
    // كلاس بعمله علشان اهندل جزء الفالديشن ايرورز وخليه استاندرد
    public class ResponceValidationError : ResponseApi
    {
        public IEnumerable<string>? Errors { get; set; }
        
        public ResponceValidationError()
            : base(400) // 400?? validation Error --> BadRequest
        {

        }
    }
}
