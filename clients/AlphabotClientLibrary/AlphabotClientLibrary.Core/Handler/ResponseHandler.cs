using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Core.Handler
{
    public class ResponseHandler
    {
        public delegate void Response(IAlphabotResponse response);

        private List<Response> _listeners = new List<Response>();

        public IReadOnlyCollection<Response> Listeners {
            get { return _listeners.AsReadOnly(); }
        }

        public void AddResponseListener(Response listener)
        {
            _listeners.Add(listener);
        }

        public void AddResponseListener(List<Response> listeners)
        {
            foreach (Response listener in listeners)
                _listeners.Add(listener);
        }

        public bool RemoveResponseListener(Response listener)
        {
            return _listeners.Remove(listener);
        }

        public IAlphabotResponse WaitForResponse(int responseTimeout = 5000)
        {
            throw new NotImplementedException();
        }
    }
}
