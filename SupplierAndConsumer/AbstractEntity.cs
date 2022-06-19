using System.Threading;

namespace SupplierAndConsumer;

abstract public class AbstractEntity
{
    private bool _stopFlag = false;
    private Thread? _thread = null;
    private int _count = 0;
    public int Speed = 200;

    protected InfBuffer? Buff;

    public void SetBuffer(InfBuffer buff)
    {
        Buff = buff;
    }

    protected void CounterInc()
    {
        _count++;
    }

    public void Start()
    {
        if (_thread != null || Buff == null)
        {
            return;
        }

        _stopFlag = false;
        _thread = new Thread(Worker);
        _thread.Start();
    }

    private void Worker()
    {
        while (true)
        {
            Do();
            
            Thread.Sleep(Speed);

            if (_stopFlag)
            {
                break;
            }
        }
    }

    protected abstract void Do();

    public void Stop()
    {
        if (_thread != null)
        {
            _stopFlag = true;
            _thread.Join();
            _thread = null;
        }
        
        _stopFlag = false;
    }

    public int GetCount()
    {
        return _count;
    }

    public void Reset()
    {
        Stop();
        
        _stopFlag = false;
        _count = 0;
    }
}