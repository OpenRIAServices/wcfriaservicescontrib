using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;
using RiaServicesContrib.DataValidation;
using RiaServicesContrib.DomainServices.Client;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace EntityGraphTests.Tests
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
        public Owner Owner { get; set; }
    }
    class Owner
    {
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
        private Trailer _trailer;
        public Trailer Trailer
        {
            get { return _trailer; }
            set
            {
                if(_trailer != value)
                {
                    _trailer = value;
                    RaisePropertyChanged("Trailer");
                }
            }
        }
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
        public ObservableCollection<Car> Cars { get; set; }

        private static EntityGraphShape _shape =
            new EntityGraphShape()
                .Edge<CarPark, Car>(CarPark => CarPark.Cars)
                .Edge<Car, Wheel>(Car => Car.Wheels)
                .Edge<Car, Door>(Car => Car.Doors)
                .Edge<Car, Engine>(Car => Car.Engine)
                .Edge<Truck, Trailer>(Truck => Truck.Trailer);

        public static EntityGraphShape Shape { get { return _shape; } }
    }

    public class UniqIds : ValidationRule
    {
        public UniqIds()
            : base(
                InputOutput<Car, string>(Car1 => Car1.Id),
                InputOutput<Car, string>(Car2 => Car2.Id)
            )
        { }

        public ValidationResult Validate(string carId1, string carId2)
        {
            if(carId1 == carId2)
            {
                return new ValidationResult("Car ids should be uniqe");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    public class TruckDoorsValidator : ValidationRule
    {
        public TruckDoorsValidator() :
            base(InputOutput<Truck, IEnumerable<Door>>(Truck => Truck.Doors)) 
        { }

        public ValidationResult Validate(IEnumerable<Door> doors)
        {
            if(doors.Count() > 2)
            {
                return new ValidationResult("Truck has max 2 doors.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    public class TruckEngineValidator : ValidationRule
    {
        public TruckEngineValidator() :
            base(
             InputOutput<Truck, Engine>(Truck => Truck.Engine),
             InputOnly<Truck, EngineType>(Truck => Truck.Engine.EngineType)
            ) 
        { }

        public ValidationResult Validate(Engine engine, EngineType engineType)
        {
            if(engineType != EngineType.Diesel)
            {
                return new ValidationResult("Truck should have a diesel engine."); 
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    public class TruckWheelsValidator : ValidationRule
    {
        public TruckWheelsValidator() :
            base(InputOutput<Truck, IEnumerable<Wheel>>(Truck => Truck.Wheels)
            ) 
        { }

        public ValidationResult Validate(IEnumerable<Wheel> wheels)
        {
            if(wheels.Count() <= 4)
            {
                return new ValidationResult("Truck should have at least 4 wheels.");
            }
            return ValidationResult.Success;
        }
    }
    public class TruckTrailerValidator : ValidationRule
    {
        public TruckTrailerValidator() :
            base(
            InputOutput<Truck, Trailer>(Truck1 => Truck1.Trailer),
            InputOutput<Truck, Trailer>(Truck2 => Truck2.Trailer)
            )
        { }
        public ValidationResult Validate(Trailer trailer1, Trailer trailer2)
        {
            if(trailer1 != null && trailer1 == trailer2)
            {
                return new ValidationResult("A trailer can be attached to a single truck only");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    [TestClass]
    public class CarsExampleTests
    {
        [TestMethod]
        public void UniqIdsTest()
        {
            var truck = new Truck { Id = "1" };
            var personCar = new PersonCar { Id = "2" };
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck, personCar }
            };
            MEFValidationRules.RegisterType(typeof(UniqIds));
            var gr = carPark.EntityGraph(CarPark.Shape);
            Assert.IsFalse(truck.HasValidationErrors);
            Assert.IsFalse(personCar.HasValidationErrors);
            truck.Id = "2";
            Assert.IsTrue(truck.HasValidationErrors);
            Assert.IsTrue(personCar.HasValidationErrors);
            personCar.Id = "1";
            Assert.IsFalse(truck.HasValidationErrors);
            Assert.IsFalse(personCar.HasValidationErrors);

            MEFValidationRules.UnregisterType(typeof(UniqIds));
        }
        [TestMethod]
        public void TruckEquipmentTest()
        {
            var truck = new Truck { Id = "1" };
            var personCar = new PersonCar { Id = "2" };
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck, personCar }
            };
            MEFValidationRules.RegisterType(typeof(TruckDoorsValidator));
            var gr = carPark.EntityGraph(CarPark.Shape);
            Assert.IsFalse(truck.HasValidationErrors);
            truck.Doors.Add(new Door());
            Assert.IsTrue(truck.HasValidationErrors);

            MEFValidationRules.UnregisterType(typeof(TruckDoorsValidator));
        }
        [TestMethod]
        public void TruckEngineTest()
        {
            var truck = new Truck { Id = "1" };
            var personCar = new PersonCar { Id = "2" };
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck, personCar }
            };
            MEFValidationRules.RegisterType(typeof(TruckEngineValidator));
            var gr = carPark.EntityGraph(CarPark.Shape);

            Assert.IsFalse(truck.HasValidationErrors);
            Assert.IsFalse(truck.Engine.HasValidationErrors);
            
            truck.Engine.EngineType = EngineType.Benzin;
            
            Assert.IsTrue(truck.HasValidationErrors);
            Assert.IsFalse(truck.Engine.HasValidationErrors);
            
            truck.Engine.EngineType = EngineType.Diesel;
            Assert.IsFalse(truck.HasValidationErrors);
            Assert.IsFalse(truck.Engine.HasValidationErrors);
            MEFValidationRules.UnregisterType(typeof(TruckEngineValidator));
        }
        [TestMethod]
        public void TruckTrailerTest()
        {
            var truck1 = new Truck { Id = "1" };
            var truck2 = new Truck { Id = "2" };
            var trailer = new Trailer();
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> { truck1, truck2 }
            };
            MEFValidationRules.RegisterType(typeof(TruckTrailerValidator));
            var gr = carPark.EntityGraph(CarPark.Shape);

            truck1.Trailer = trailer;
            Assert.IsFalse(truck1.HasValidationErrors);
            Assert.IsFalse(truck2.HasValidationErrors);

            truck2.Trailer = trailer;
            Assert.IsTrue(truck1.HasValidationErrors);
            Assert.IsTrue(truck2.HasValidationErrors);
            MEFValidationRules.UnregisterType(typeof(TruckTrailerValidator));
        }
    }
}
