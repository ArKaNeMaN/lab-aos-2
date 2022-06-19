namespace SupplierAndConsumer;

public class Consumer : AbstractEntity
{
    protected override void Do()
    {
        Buff!.Take();
        CounterInc();
    }
}