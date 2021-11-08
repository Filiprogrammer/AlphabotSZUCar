using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Core.Handler
{
    public class ResponseHandler
    {
        public delegate void Response(IAlphabotResponse response);
        public List<Response> Listeners { get; private set; }

        public ResponseHandler()
        {
            Listeners = new List<Response>();
        }

        public void AddResponseListener(Response listener)
        {
            Listeners.Add(listener);
        }

        public void AddResponseListener(List<Response> listeners)
        {
            foreach (Response listener in listeners)
            {
                Listeners.Add(listener);
            }
        }

        public bool RemoveResponseListener(Response listener)
        {
            return Listeners.Remove(listener);
        }

        public IAlphabotResponse WaitForResponse(int responseTimeout = 5000)
        {
            throw new NotImplementedException();
        }

    }
}
