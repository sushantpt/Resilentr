# Resilentr NuGet Package Documentation

## Overview

Resilentr is a .NET library that enhances exception handling within ASP.NET Core applications. It provides a set of attributes and middleware to customize how exceptions are managed within endpoint methods coupled with logging.

## Attributes

### `AttributeProperty` Enum

Defines types of attributes used by the Resilentr library.

- `Silent`: A stealthy attribute that silences any exception, resulting in an HTTP status of 200 OK.
- `Loud`: Generates a loud error if any exception is thrown.
- `Default`: Default behavior, does nothing special.
- `Faulted`: Throws an exception, regardless of success or failure, with an HTTP status of 500 InternalServerError.

# Resilentr NuGet Package Usage

## Apply Resilentr Attribute

Apply the `ResilentrAttribute` to your endpoint methods by specifying the desired behavior for exception handling.

Example:

## Controller (MVC)
```csharp
[Resilentr(AttributeProperty.Loud, "Custom error message", HttpStatusCode.BadRequest)]
public IActionResult MyEndpoint()
{
    // Your endpoint logic
    return Ok();
}
```
## Minimal API
```csharp
app.MapGet("/GetSomething", [Resilentr(attribute:AttributeProperty.Faulted, customErrorMessage:"This endpoint has been disabled.")] () => "Hello World!");
```

## Configuring Resilentr Middleware

To integrate the Resilentr middleware into your ASP.NET Core application:
```csharp
app.UseMiddleware<ResilentrMiddleware>();
```

