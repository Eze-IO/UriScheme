# UriScheme Register
**Un/Register uri schemes in Windows, with COM support**
## Add library to project then put 'using Eze.IO.Application;'

### Example
``` c#
[STAThread]
static int Main(string[] args)
{
  UriScheme.RegisterScheme("test://", "C:/Test/myapp.exe", "Test", "C:/Test/myapp.exe", 0, RecordScheme.OnMachine);
  return 0;
}
```

This writes `Software\Classes\test` (under HKCU or HKLM), so opening `test://anything` launches `"C:/Test/myapp.exe" "%1"`. The `name` argument ("Test") is the display name stored as `URL:Test Protocol`. Registering with `RecordScheme.OnMachine` needs administrator rights.
