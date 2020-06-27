namespace CNW_WebBanQuanAo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PHANHOI")]
    public partial class PHANHOI
    {
        [Key]
        public int MaPH { get; set; }

        [StringLength(50)]
        public string MaKH { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tiêu đề")]
        public string TieuDe { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập nội dung")]
        public string NoiDung { get; set; }

        public DateTime? NgayGui { get; set; }

        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
