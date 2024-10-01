﻿using HeaterSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Gherkin.Quick;

namespace thermostaat.AcceptanceTests.StepDefinitions;

[FeatureFile("./Features/Thermostat.Feature")] // Annotation that links feature file
public sealed class ThermostatSteps : Feature  // Must inherit from Feature
{
    // Setup

    private const double Setpoint = 20.0;
    private const double Offset = 2.0;
    private const double Difference = 0.5;
    private const int MaxFailures = 2;

    private const string UrlMockoon = "http://localhost:3000/data/2.5/weather";
    private const string UrlMockoonException = "http://localhost:3000/data/2.5/weather/exception";

    private readonly IHeatingElement heatingElement = null;
    private readonly ITemperatureSensor temperatureSensor = null;
    private readonly Thermostat thermostat;

    public ThermostatSteps()
    {
        temperatureSensor = new TemperatureSensorOpenWeather();
        heatingElement = new HeatingElementStub();
        thermostat = new Thermostat(temperatureSensor, heatingElement)
        {
            Setpoint = Setpoint,
            Offset = Offset,
            MaxFailures = MaxFailures
        };
    }

    // Step implementations, possible attributes are Given, When, Then, And

    [Given(@"the heater is off")]
    [When(@"the temperature exceeds upper boundary")]
    public void SetHeaterOff()
    {
        string queryParam = "?temp=" + (Setpoint + Offset + Difference).ToString(CultureInfo.InvariantCulture);
        temperatureSensor.Url = $"{UrlMockoon}{queryParam}";
        thermostat.Work();
    }

    [Given(@"the heater is on")]
    [When(@"the temperature is less than lower boundary")]
    public void SetHeaterOn()
    {
        string queryParam = "?temp=" + (Setpoint - Offset - Difference).ToString(CultureInfo.InvariantCulture);
        temperatureSensor.Url = $"{UrlMockoon}{queryParam}";
        thermostat.Work();
    }

    [When(@"the temperature is between boundaries")]
    public void SetTemperatureBetweenBoundaries()
    {
        string queryParam = "?temp=" + (Setpoint).ToString(CultureInfo.InvariantCulture);
        temperatureSensor.Url = $"{UrlMockoon}{queryParam}";
    }

    [When(@"the temperature equals lower boundary")]
    public void SetTemperatureToLowerBoundary()
    {
        string queryParam = "?temp=" + (Setpoint - Offset).ToString(CultureInfo.InvariantCulture);
        temperatureSensor.Url = $"{UrlMockoon}{queryParam}";
    }

    [When(@"the temperature equals upper boundary")]
    public void SetTemperatureToUpperBoundary()
    {
        string queryParam = "?temp=" + (Setpoint + Offset).ToString(CultureInfo.InvariantCulture);
        temperatureSensor.Url = $"{UrlMockoon}{queryParam}";
    }

    [Then(@"turn heater off")]
    [Then(@"do nothing - heater is off")]
    public void CheckHeaterOff()
    {
        thermostat.Work();
        Assert.False(heatingElement.IsEnabled);
    }
    [Then(@"turn heater on")]
    [Then(@"do nothing - heater is on")]
    public void CheckHeaterOn()
    {
        thermostat.Work();
        Assert.True(heatingElement.IsEnabled);
    }
}

