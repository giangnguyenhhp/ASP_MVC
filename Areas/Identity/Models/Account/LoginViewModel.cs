﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ASP_MVC.Areas.Identity.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Phải nhập {0}")]
        [Display(Name = "Địa chỉ email hoặc tên tài khoản",Prompt = "Sử dụng UserName hoặc Email")]
        public string UserNameOrEmail { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu",Prompt = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Nhớ thông tin đăng nhập?")]
        public bool RememberMe { get; set; }
    }
}
