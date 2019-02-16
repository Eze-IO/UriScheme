# UriScheme Register
**Un/Register uri schemes in Windows, with COM support**
**Add library to project then put 'using Eze.IO.Application;'**

### Example
``` c#
[STAThread]
static int Main(string[] args)
{
  UriScheme.Register("test://", "C:/Test/myapp.exe", "Test", "C:/Test/myapp.exe", 0, RecordScheme.OnMachine);
  return 0;
}
```
