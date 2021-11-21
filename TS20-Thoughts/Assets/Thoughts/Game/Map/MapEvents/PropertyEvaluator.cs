using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.MapEvents;
using Thoughts.Game.Map.Properties;
using UnityEngine;
using UnityEngine.Serialization;

public class PropertyEvaluator //todo: change for PropertyReferencer
{
    /// <summary>
    /// Property that will be required for the execution of the MapEvent
    /// </summary>
    [Tooltip("Property that will be required for the execution of the MapEvent")]
    [OdinSerialize]
    protected PropertyType propertyType;
        
    /// <summary>
    /// The required property
    /// </summary>
    [FormerlySerializedAs("property")]
    [Tooltip("The required property")]
    [AssetsOnly]
    [ShowIf("propertyType", PropertyType.Other)]
    [OdinSerialize]
    protected Property property;
    
    /// <summary>
    /// Returns the Property that is being evaluated by this consequence/requirement/etc
    /// </summary>
    /// <param name="propertyHolder">The property in which lives the MapEvent where this consequence/requirement/etc lives in</param>
    /// <returns></returns>
    public Property GetProperty(Property propertyHolder)
    {
        switch (propertyType)
        {
            case PropertyType.Self:
                return propertyHolder;
            case PropertyType.Other:
                if (property == null)
                    Debug.LogWarning($"Found a PropertyEvaluator (consequence/requirement, ...) with no Property set but with the type set to PropertyType.Other (which requires a Property ot be set).\n{this.ToString()}");
                return property;
            case PropertyType.None:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsPropertyTypeNone() // Used in inspector for warnings
    {
        return propertyType == PropertyType.None;
    }

}
