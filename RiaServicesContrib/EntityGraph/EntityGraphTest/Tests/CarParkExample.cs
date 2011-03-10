using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraph;
using EntityGraph.RIA;
using EntityGraph.RIA.Validation;
using EntityGraph.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    public class Wheel : Entity
    {
    }
    public enum EngineType
    {
        Diesel,
        Benzin,
        Gaz
    }
    public class Engine : Entity
    {
        private EngineType _engineType;
        public EngineType EngineType
        {
            get { return _engineType; }
            set
            {
                if(_engineType != value)
                {
                    _engineType = value;
                    RaisePropertyChanged("EngineType");
                }
            }
        }
    }
    public class Door : Entity
    {
    }
    public abstract class Car : Entity
    {
        public string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                if(_id != value)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }
        public ObservableCollection<Wheel> Wheels { get; set; }
        public ObservableCollection<Door> Doors { get; set; }
        public Engine Engine { get; set; }
    }
    public class Trailer : Entity
    {
    }
    public class Truck : Car
    {
        public Truck()
        {
            Wheels = new ObservableCollection<Wheel>{
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel()};
            Doors = new ObservableCollection<Door>{
                new Door(),
                new Door()};
            Engine = new Engine { EngineType = EngineType.Diesel };
        }
        public Trailer Trailer { get; set; }
    }
    public class PersonCar : Car
    {
        public PersonCar()
        {
            Wheels = new ObservableCollection<Wheel>{
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel()};
            Doors = new ObservableCollection<Door>{
                new Door(),
                new Door(),
                new Door(),
                new Door()};
            Engine = new Engine { EngineType = EngineType.Benzin };
        }
    }
    public class CarPark : Entity
    {
        public static EntityGraphShape Shape { get; private set; }
        public CarPark()
        {
            Shape = new EntityGraphShape()
            .Edge<CarPark, Car>(CarPark => CarPark.Cars)
            .Edge<Car, Wheel>(Car => Car.Wheels)
            .Edge<Car, Door>(Car => Car.Doors)
            .Edge<Car, Engine>(Car => Car.Engine)
            .Edge<Truck, Trailer>(Truck => Truck.Trailer);
        }
        public ObservableCollection<Car> Cars { get; set; }
    }

    public class UniqIds : ValidationRule
    {
        public UniqIds()
            : base(
                InputOutput<Car, string>(C1 => C1.Id),
                InputOutput<Car, string>(C2 => C2.Id)
            ) { }

        public void Validate(string c1, string c2)
        {
            if(c1 == c2)
            {
                Result = new ValidationResult("Car  ids should be uniqe");
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
    public class TruckDoorsValidator : ValidationRule
    {
        public TruckDoorsValidator() :
            base(InputOutput<Truck, IEnumerable<Door>>(Truck => Truck.Doors)) 
        { }

        public void Validate(IEnumerable<Door> doors)
        {
            if(doors.Count() > 2)
            {
                Result = new ValidationResult("Truck has max 2 doors."); return;
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
    public class TruckEngineValidator : ValidationRule
    {
        public TruckEngineValidator() :
            base(
             InputOutput<Truck, Engine>(Truck => Truck.Engine),
             InputOutput<Truck, EngineType>(Truck => Truck.Engine.EngineType)
            ) 
        { }

        public void Validate(Engine engine, EngineType engineType)
        {
            if(engineType != EngineType.Diesel)
            {
                Result = new ValidationResult("Truck should have a diesel engine."); return;
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
    public class TruckWheelsValidator : ValidationRule
    {
        public TruckWheelsValidator() :
            base(InputOutput<Truck, IEnumerable<Wheel>>(Truck => Truck.Wheels)
            ) 
        { }

        public void Validate(IEnumerable<Wheel> wheels)
        {
            if(wheels.Count() <= 4)
            {
                Result = new ValidationResult("Truck should have at least 4 wheels."); return;
            }
            Result = ValidationResult.Success;
        }
    }
    [TestClass]
    public class CarsExampleTests
    {
        [TestMethod]
        public void UniqIdsTest()
        {
            var truck = new Truck { Id = "1" };
            var personCar = new PersonCar { Id = "1" };
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck, personCar }
            };
            MEFValidationRules.RegisterType(typeof(UniqIds));
            var gr = carPark.EntityGraph(CarPark.Shape);
            Assert.IsTrue(truck.HasValidationErrors);
            Assert.IsTrue(personCar.HasValidationErrors);
            truck.Id = "2";
            Assert.IsFalse(truck.HasValidationErrors);
            Assert.IsFalse(personCar.HasValidationErrors);
            MEFValidationRules.UnregisterType(typeof(UniqIds));
        }
        [TestMethod]
        public void TruckEquipmenttest()
        {
            var truck = new Truck { Id = "1" };
            var personCar = new PersonCar { Id = "2" };
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck, personCar }
            };
            MEFValidationRules.RegisterAssembly(typeof(CarPark).Assembly);
            var gr = carPark.EntityGraph(CarPark.Shape);
            Assert.IsFalse(truck.HasValidationErrors);
            truck.Engine.EngineType = EngineType.Benzin;
            Assert.IsTrue(truck.HasValidationErrors);

            MEFValidationRules.UnregisterAssembly(typeof(CarPark).Assembly);
        }
    }
}
