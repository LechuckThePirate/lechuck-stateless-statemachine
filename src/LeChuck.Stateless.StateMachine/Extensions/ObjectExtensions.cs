#region

using System.Text.Json;

#endregion

namespace LeChuck.Stateless.StateMachine.Extensions
{
    public static class ObjectExtensions
    {
        public static T JsonChangeType<T>(this object obj)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
        }
    }
}