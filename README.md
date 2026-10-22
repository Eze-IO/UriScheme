# UriScheme

[![build](https://github.com/Eze-IO/UriScheme/actions/workflows/build.yml/badge.svg)](https://github.com/Eze-IO/UriScheme/actions/workflows/build.yml)

**Register, update and unregister custom uri schemes (protocol handlers) in Windows, with COM support.**

Once `myapp` is registered, opening a link such as `myapp://open?id=42` (from a browser, the Run dialog or `start myapp://...`) launches your application with the uri as its argument.

Targets .NET Framework 4.6.2+ and .NET 8 (`net8.0-windows`).

## Usage

Add the library to your project, then:

```c#
using Eze.IO.Application;
```

### Register a scheme

```c#
// Per user, no admin rights needed
UriScheme.RegisterScheme("myapp", @"C:\Program Files\MyApp\myapp.exe", "My App");

// With an icon, for every user on the machine (needs admin rights)
UriScheme.RegisterScheme("myapp", @"C:\Program Files\MyApp\myapp.exe", "My App",
    icon: @"C:\Program Files\MyApp\myapp.exe", iconNumber: 0, register: RecordScheme.OnMachine);

// With custom arguments, "%1" is replaced by the uri
UriScheme.RegisterScheme("myapp", @"C:\Program Files\MyApp\myapp.exe", "My App",
    parameter: "--open \"%1\"");
```

The protocol can be given as `myapp`, `myapp:` or `myapp://`. The first call writes:

```
HKEY_CURRENT_USER\Software\Classes\myapp
    (Default)      = "URL:My App Protocol"
    URL Protocol   = ""
    shell\open\command
        (Default)  = "C:\Program Files\MyApp\myapp.exe" "%1"
```

`RecordScheme.OnCurrentUser` (the default) writes under `HKEY_CURRENT_USER`; `RecordScheme.OnMachine` writes under `HKEY_LOCAL_MACHINE` and needs the process to run as administrator.

### Check, update and unregister

```c#
if (UriScheme.Exists("myapp"))
{
    UriScheme.UpdateScheme("myapp", @"D:\MyApp\myapp.exe", "My App", "\"%1\"");
    UriScheme.UnregisterScheme("myapp", "My App");
}
```

If an update fails part way (for example the new path doesn't exist), the previous registration is restored.

### Read the uri in your application

```c#
[STAThread]
static int Main(string[] args)
{
    foreach (UriBuilder uri in UriSchemeParser.GetUriBuilders(args))
    {
        // uri.Scheme == "myapp", uri.Host == "open", uri.Query == "?id=42"
    }
    return 0;
}
```

### COM

`UriSchemeManager` (ProgId `Eze.IO.Application.UriSchemeManager`) exposes the same operations to COM clients, such as VBScript or VBA, once the .NET Framework build is registered with `regasm /codebase UriScheme.dll`. Set `ThrowError` to `false` to have failures ignored instead of raised.

```vb
Set manager = CreateObject("Eze.IO.Application.UriSchemeManager")
manager.RegisterScheme "myapp", "C:\MyApp\myapp.exe", "My App", """%1"""
```

## Errors

| Exception | When |
| --- | --- |
| `FileNotFoundException` | The application or icon file doesn't exist |
| `FormatException` | The protocol isn't a valid scheme name |
| `ApplicationException` | The scheme is already registered, or is a built-in one such as `http` |
| `InvalidOperationException` | Updating or unregistering a scheme that isn't registered |
| `UnauthorizedAccessException` | No permission to write the registry, usually `OnMachine` without admin rights |

## Upgrading from 1.0

- Schemes are now stored under the protocol (`Software\Classes\myapp`) instead of under the `name` argument, which is now only the display name. Registrations made with a `name` different from the protocol never worked, and need to be removed by hand.
- `DoesItExist(protocol, name)` is now `Exists(protocol)`. The old method still works but is marked obsolete.
- The overloads that take a parameter count (`int parameters`) and `UriSchemeBuilder.Parameters` are obsolete. Windows only passes the uri as `"%1"`, so use the overloads without a count, or pass custom arguments as a string.
- `UriSchemeBuilder` no longer needs disposing; `Dispose()` does nothing and is kept for compatibility.

## Building

```
dotnet build UriScheme.sln
dotnet test UriScheme.sln
```

The tests register temporary schemes under `HKEY_CURRENT_USER` and remove them afterwards, so they run on Windows only.
