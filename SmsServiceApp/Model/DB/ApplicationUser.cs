﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    /// <summary>
    /// Application user, inherited from IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public int ApplicationGroupId { get; set; }
        public ApplicationGroup ApplicationGroup { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<CampaignNotification> CampaignNotifications { get; set; }
        public ICollection<EmailCampaignNotification> EmailCampaignNotifications { get; set; }

        /// <summary>
        /// Invite ID
        /// </summary>
        /// <value>
        /// Filled up when user has invite to group
        /// </value>
        public int InviteId { get; set; } = 0;

        public bool SmsNotificationsEnabled { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
    }
}
