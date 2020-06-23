using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC_Back.Models.Login
{
    public class Login
    {
        [Display(Name = "使用者帳號")]
        [Required(ErrorMessage = "{0}欄位為必填")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0}欄位長度需介於{2} ~ {1}")]
        public string UserID { set; get; }
        [Display(Name = "密碼")]
        [Required(ErrorMessage = "{0}欄位為必填")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0}欄位長度需介於{2} ~ {1}")]
        public string Password { set; get; }
        [Display(Name = "保持登入")]
        public bool KeepLogin { set; get; }
        [ScaffoldColumn(false)]
        public string ReturnUrl { set; get; }
    }
}