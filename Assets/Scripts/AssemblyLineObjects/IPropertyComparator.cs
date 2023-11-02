using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPropertyComparator
{
    bool CompareColor(Properties other);
    bool CompareRotation(Properties other);

    bool CompareProperties(Properties other, Properties reference, List<PropertyType> propertiesToCompare);
}
