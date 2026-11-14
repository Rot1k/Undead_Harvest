using System;
public class PassiveItemInstance
{
    public PassiveItemSO PassiveItemSO { get; }
    public Guid InstanceId { get; }

    public PassiveItemInstance(PassiveItemSO passiveItemSO)
    {
        PassiveItemSO = passiveItemSO;
        InstanceId = Guid.NewGuid();
    }
}
