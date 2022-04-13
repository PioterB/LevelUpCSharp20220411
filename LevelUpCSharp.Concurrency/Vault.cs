using System;
using System.Threading;

namespace LevelUpCSharp.Concurrency
{
    public class Vault<TSecret>
    {
	    private SemaphoreSlim _in = new SemaphoreSlim(1);
	    private SemaphoreSlim _out = new SemaphoreSlim(0);

        private TSecret _secret;

        public void Put(TSecret secret)
        {
	        _in.Wait();
            _secret = secret;
            _out.Release();
        }

        public TSecret Get()
        {
	        _out.Wait();
	        var result = _secret;
            _secret = default(TSecret);
            _in.Release();
            return result;
        }
    }
}