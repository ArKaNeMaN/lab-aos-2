using System.Threading;
using System.Windows;

namespace SupplierAndConsumer;

// public delegate void BufferCallback(InfBuffer buffer);
public delegate void BufferActionCallback(InfBuffer buffer, InfBufferActionType type, int? index);

public class InfBuffer
{
    private Mutex _mainMutex;
    // private Mutex _availableMutex;
    
    private readonly int _initialTotal;
    private int _total;
    private int _taken;

    private readonly BufferActionCallback? _callback;

    public InfBuffer(BufferActionCallback? callback, int initialTotal = 0)
    {
        _callback = callback;
        
        _initialTotal = initialTotal;
        _total = initialTotal;
        
        _mainMutex = new Mutex();
        // _availableMutex = new Mutex();
        
        Event(InfBufferActionType.Init);
    }

    private void WaitForItem()
    {
        // if (GetRest() < 1)
        // {
        //     _availableMutex.WaitOne();
        // }
        
        // Не придумал как на мьютексте сделать.
        while (GetRest() < 1) {}
        
        Wait();
    }

    public int GetRest()
    {
        return GetTotal() - GetTaken();
    }

    public int GetTotal()
    {
        return _total;
    }

    public int GetTaken()
    {
        return _taken;
    }

    private void Wait()
    {
        _mainMutex.WaitOne();
    }

    public void Take()
    {
        WaitForItem();

        _taken++;
        Event(InfBufferActionType.Take, _taken - 1);

        // if (GetRest() < 1)
        // {
        //     _availableMutex.WaitOne();
        // }
        
        _mainMutex.ReleaseMutex();
    }

    public void Store()
    {
        Wait();

        _total++;
        Event(InfBufferActionType.Store, _total - 1);
        
        _mainMutex.ReleaseMutex();

        // if (GetRest() == 1)
        // {
        //     _availableMutex.ReleaseMutex();
        // }
    }

    public void Reset()
    {
        _total = _initialTotal;
        _taken = 0;
        
        _mainMutex = new Mutex();
        // _availableMutex = new Mutex();
        
        Event(InfBufferActionType.Init);
    }
    
    private void Event(InfBufferActionType type, int? index = null)
    {
        Application.Current?.Dispatcher?.Invoke(() => 
        {
            _callback?.Invoke(this, type, index);
        });
    }
}