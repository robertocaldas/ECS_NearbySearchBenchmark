public class InjectableAttribute : System.Attribute
{
    public bool Required { get; set; }
    public InjectableAttribute(bool required = false)
    {
        Required = required;
    }
}
