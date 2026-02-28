using System.Collections.Generic;
using UnityEngine;

public interface IDescribableItem
{
    string GetDescriptionTemplate();
    Dictionary<string, string> GetDescriptionParams();
}
