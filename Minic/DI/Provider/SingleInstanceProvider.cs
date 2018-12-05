// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Minic.DI.Provider
{
    public class SingleInstanceProvider : IInstanceProvider
    {
        //  MEMBERS
        public Type InstanceType { get{ return _Instance.GetType(); } }
        private object _Instance;
        private bool _IsNew;


        //  CONSTRUCTOR
        public SingleInstanceProvider( object instance)
        {
            _Instance = instance;
            _IsNew = true;
        }


        //  METHODS
        public void GetInstance(out object instance, out bool isNew)
        {
            instance = _Instance;
            isNew = _IsNew;
            _IsNew = false;
        }
    }
}