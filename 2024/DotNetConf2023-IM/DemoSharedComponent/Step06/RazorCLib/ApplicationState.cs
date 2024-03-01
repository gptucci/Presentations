using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorCLib;

public class ApplicationState
{

    int _currentCount = 0;
    public int CurrentCount { get { 
            return _currentCount;
        }
        set {
            _currentCount=value;
            NotifyStateChanged();
        } 
    }
    public event Action? OnChanged;
    

    private void NotifyStateChanged()
    {
        OnChanged?.Invoke();
    }
}
