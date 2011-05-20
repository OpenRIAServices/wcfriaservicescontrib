using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;

namespace DataValidationTests
{
    public class A : Entity
    {
        private B _b;
        public B B
        {
            get
            {
                return _b;
            }
            set
            {
                if(_b != value)
                {
                    _b = value;
                    RaisePropertyChanged("B");
                }                    
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }
        private string _lastName;
        public string lastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if(_lastName != value)
                {
                    _lastName = value;
                    RaisePropertyChanged("lastName");
                }
            }
        }
    }
    public class B : Entity
    {
        private C _c;
        public C C
        {
            get
            {
                return _c;
            }
            set
            {
                if(_c != value)
                {
                    _c = value;
                    RaisePropertyChanged("C");
                }
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }
    }
    public class C : Entity
    {
        private D _d;
        public D D
        {
            get
            {
                return _d;
            }
            set
            {
                if(_d != value)
                {
                    _d = value;
                    RaisePropertyChanged("D");
                }
            }
        }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }
    }
    public class D : Entity
    {
        private A _a;
        public A A
        {
            get
            {
                return _a;
            }
            set
            {
                if(_a != value)
                {
                    _a = value;
                    RaisePropertyChanged("A");
                }
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
                }
            }
        }
    }
}
