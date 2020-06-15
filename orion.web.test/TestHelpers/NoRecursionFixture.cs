using AutoFixture;
using System.Linq;

namespace orion.web.test.TestHelpers
{
    public class NoRecursionFixture : Fixture
    {
        public NoRecursionFixture()
        {
            Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Behaviors.Remove(b));
            Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
