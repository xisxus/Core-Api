namespace MvcCoreApiEmployee.Models.DTOs
{
    public class CommonDTO
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }
        public string ImgName { get; set; }
        public IFormFile ImgFile { get; set; }
        public string Experiences { get; set; }
    }
}
