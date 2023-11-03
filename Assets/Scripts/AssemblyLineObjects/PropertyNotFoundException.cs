using System;
using System.Collections;
using System.Collections.Generic;

public class PropertyNotFoundException : Exception
{
    public PropertyNotFoundException(string message) : base(message) { }
}
