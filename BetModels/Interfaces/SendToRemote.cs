﻿using BetModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetModels.Interfaces;
public interface ISendToRemote
{
    public void SendToRemote(Form form);
}
