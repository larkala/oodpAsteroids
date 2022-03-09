using System;
using System.Collections.Generic;
using System.Reflection;

namespace AsteroidsApp
{
    class Mediator
    {
        Dictionary<Type, List<MethodAndReciever>> _registered = new Dictionary<Type, List<MethodAndReciever>>();

        #region Properties Singleton
        private static Mediator _instance;
        public static Mediator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Mediator();
                return _instance;
            }
        }
        #endregion

        #region public methods

        /// <summary>
        /// Kan ta alla sorters typer att skicka som message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void Send<TMessage>(TMessage message)
        {
            Type type = typeof(TMessage);

            //finns det någon som registrerat sig för att lyssna på detta meddelande? Are there any subscribers?
            if (!_registered.ContainsKey(type))
                return;

            //plocka ut den typen av meddelande från listan av potentiella mottagare. 
            var list = _registered[type];

            //Kör metoden i ett annat objekt där den angett (det är denna som ska köras).
            foreach (var methodAndReciever in list)
                methodAndReciever.MethodInfo.Invoke(methodAndReciever.Reciever, new object[] { message });
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="reciever">Skickar med oss själva som mottagaren.</param>
        /// <param name="action">Metoden som vi ska kopplas tillbaka med</param>
        public void Register<TMessage>(object reciever, Action<TMessage> action)
        {
            Type type = typeof(TMessage);

            //om den redan finns, plockar vi ut listan för om typen redan vart registrerad, annars skapas en ny methodAndReciever lista
            List<MethodAndReciever> list = (_registered.ContainsKey(type)) ? _registered[type] : new List<MethodAndReciever>();

            MethodAndReciever rm = new MethodAndReciever()
            {
                MethodInfo = action.Method,
                Reciever = reciever
            };

            list.Add(rm);

            if (!_registered.ContainsKey(type))
                _registered.Add(type, list);
        }
        #endregion

        #region Private struct
        //lagringsklass som binder ihop en mottagare och en metod som ska köras.
        private struct MethodAndReciever{
            public MethodInfo MethodInfo { get; set; }
            public object Reciever { get; set; }
        }

        #endregion
    }
}
