using Microsoft.AspNetCore.Mvc.Formatters;

namespace Cukcuk.Core.DTOs
{
    public class DocumentExport
    {
        public int STT { get; set; }
        public string TenFile { get; set; } = string.Empty;
        public string LinhVuc { get; set; } = string.Empty;
        public string NgayTao { get; set; } = string.Empty;
        public string TenTaiLieu { get; set; } = string.Empty;
        public string SoTaiLieu { get; set; } = string.Empty;
        public string CoQuanBanHanh { get; set; } = string.Empty;
        public string NgayBanHanh { get; set; } = string.Empty;
        public string NguoiKy { get; set; } = string.Empty;
        public string NgayCoHieuLuc { get; set; } = string.Empty;   
    }
}
