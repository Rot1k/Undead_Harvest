using System;
public class PassiveItemInstance
{
    public PassiveItemSO PassiveItemSO { get; }
    public Guid InstanceId { get; }

    public object RuntimeData;

    public PassiveItemInstance(PassiveItemSO passiveItemSO)
    {
        PassiveItemSO = passiveItemSO;
        InstanceId = Guid.NewGuid();
    }
}
