using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetModels.Interfaces;
public interface IPreferencesService
{
    public T Get<T>(string key, T defaultValue);

    public void Set<T>(string key, T value);
    
}
