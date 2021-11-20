# VectorExpressionEngine

[![.NET](https://github.com/cbergemann/VectorExpressionEngine/actions/workflows/dotnet.yml/badge.svg)](https://github.com/cbergemann/VectorExpressionEngine/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/VectorExpressionEngine)](https://www.nuget.org/packages/VectorExpressionEngine/)

This library can be used to formulate mathematical expressions and evaluate them. The library can handle abitrary types, and it supports working with arrays/vectors in an elementwise maner (similar to MATLAB). This makes working with array data, like time series, very fast.

The library is extendable, custom types can be handled and new functions and constants can be crated easily.

## Usage

To use the library, first a context is required. This context allows the resolving of functions and variables, as well as storage of new variables. The library comes with two context implementations - ReflectionContext and ScopedContext.

```C#
public class MathLibrary
{
  // This provides a function called "sin" that simply calls the Math.Sin function
  [Expression("sin")]
  public double Sin(double x) => Math.Sin(x);
  
  // This provides a constant called "pi"
  [Expression("pi")]
  public double Pi => Math.PI;
}

...

// this is the expression to evaluate
var expression = "sin(45 * pi/180)";

// this will parse the given expression, returning a node object
var node = Parser.ParseSingle(expression);

// setup a library
var library = new MathLibrary();

// setup a context with the custom library
var reflectionContext = new ReflectionContext(library);

// evaluate the node to return a result
var result = node.Eval(reflectionContext); // result will be a double with value 1/sqrt(2) = 0.7071
```

The ReflectionContext is a read-only context. If variables are to be stored, a different context is required - e.g. the ScopedContext:

```C#

// setup a ScopedContext using the previously defined ReflectionContext:
var scopedContext = new ScopedContext(reflectionContext);

// this time multiple expressions are chained, separated by ';'
var expressionChain = "x=2; y=x*4; y+x";

// this will parse the expressions into an array of nodes
var nodes = Parser.Parse(expressionChain);

// this will evaluate all nodes, and return an array of results
var results = nodes.Select(node => node.Eval(scopedContext)).ToArray();

var finalResult = results.Last(); // this will contain the result of the calculation 2*4+2 = 10

```

The library comes with a library containing basic operations. It can be found in the BasicOperations class. The functions declared within can work with scalar double values, as well as with double-Arrays and any combination of the two.
It also contains basic logical operations, operating on bool and bool-Arrays.

## Example Application

The library comes with an example in form of a simple calculator. It can work with equations, store results in variables and call a few functions. Results can be plotted like with a graphical calculator.

You can find this example in the "SimpleCalculator" directory.

![_simpleCalculator_sshot](https://user-images.githubusercontent.com/19253536/142732271-c17473a5-bc1c-4dcc-9ebd-060a6e0d02e5.png)
