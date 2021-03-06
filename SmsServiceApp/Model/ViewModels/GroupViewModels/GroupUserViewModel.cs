﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModels.GroupViewModels
{
    /// <summary>
    /// ViewModel of ApplicationUser for inviting to Group
    /// </summary>
    public class GroupUserViewModel
    {
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
