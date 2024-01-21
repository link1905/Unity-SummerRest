namespace SummerRest.Editor.Models
{
    /// <summary>
    /// A service can contain services and requests for building api structure <br/>
    /// Service is not callable (only <see cref="Request"/>)
    /// </summary>
    public class Service : EndpointContainer
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Services.Remove(this);
            base.Delete(fromParent);
        }
        public override string TypeName => nameof(Service);
    }
}