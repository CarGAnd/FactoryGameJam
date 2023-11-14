using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ModulePlacementTests
{    
    
    // This test should place a module and assert that the module placed is the correct type.
    [Test]
    public void ModulePlacementTest()
    {
        ModulesManager modulesManager = ModulesManager.Instance;

        // Place a turn module
        modulesManager.PlaceModule(ModuleTypes.TurnModule);

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ModulePlacementTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
