using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Marks a property to be supplied with values from the query string.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SupplyComplexFromQueryAttribute : CascadingParameterAttributeBase;