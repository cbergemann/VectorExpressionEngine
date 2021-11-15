using System;
using System.Collections.Generic;
using System.Linq;

namespace VectorExpressionEngine
{
    public class NodeObjectArray : Node
    {
        public NodeObjectArray(IEnumerable<Node> elements)
        {
            Elements = elements.ToArray();
        }

        public Node[] Elements { get; }

        public override object Eval(Context ctx)
        {
            if (Elements.Length == 0)
            {
                throw new SyntaxException("invalid array definition - array cannot be empty");
            }

            var elementObjects = Elements.Select(e => e.Eval(ctx)).ToArray();
            var argumentValues = Array.CreateInstance(elementObjects[0].GetType(), elementObjects.Length);

            try
            {
                elementObjects.CopyTo(argumentValues, 0);
            }
            catch (InvalidCastException)
            {
                throw new SyntaxException("invalid array definition - element types not compatible");
            }

            return argumentValues;
        }
    }
}
