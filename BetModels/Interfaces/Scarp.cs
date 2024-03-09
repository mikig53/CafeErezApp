using BetModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetModels.Interfaces;
public interface IScarp
{
    public Task<List<TelesportCategory>> ScarpAsync();
}
