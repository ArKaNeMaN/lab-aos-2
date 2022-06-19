namespace SupplierAndConsumer;

public class Supplier : AbstractEntity
{
    protected override void Do()
    {
        Buff!.Store();
        CounterInc();
    }
}