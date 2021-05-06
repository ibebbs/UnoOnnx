using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UnoOnnx.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new UnoOnnx.App(), args);
		host.Run();
	}
}
}
