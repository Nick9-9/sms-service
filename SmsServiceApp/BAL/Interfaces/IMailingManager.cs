﻿using System;
using System.Collections.Generic;
using System.Text;
using Model.DTOs;
using System.Threading.Tasks;
using WebApp.Models;

namespace BAL.Managers
{
    public interface IMailingManager
    {
        IEnumerable<MessageDTO> GetUnsentMessages();

        void MarkAs(IEnumerable<MessageDTO> messages, MessageState messageState);
        void MarkAs(MessageDTO message, MessageState messageState);
    }
}
